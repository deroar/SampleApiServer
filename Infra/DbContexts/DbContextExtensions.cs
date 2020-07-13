using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SampleApiServer.Infra.DbContexts
{
    /// <summary>
    /// DbContextの拡張メソッド
    /// </summary>
    public static class DbContextExtensions
    {
        /// <summary>
        /// EntityからPrimaryKeyのプロパティの値を抜き出す
        /// </summary>
        /// <typeparam name="T">対象のEntityのタイプ</typeparam>
        /// <param name="dbContext">dbContext</param>
        /// <param name="entity">Entityの実体</param>
        /// <returns>PrimaryKeyの値</returns>
        public static object[] FindPrimaryKeyValues<T>(this DbContext dbContext, T entity)
        {
            return dbContext.FindPrimaryKey<T>().Properties
                .Select(p => entity.GetPropertyValue(p.Name))
                .ToArray();
        }

        /// <summary>
        /// DbContextがLocalにキャッシュしているEntityを検索する。
        /// ローカルアクセスのみを提供する関数が無かったので移植。
        /// https://github.com/aspnet/EntityFrameworkCore/blob/049edb0d9f0592d664adf39b765c174a3c2cb831/src/EFCore/Internal/EntityFinder.cs#L253
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public static T FindLocal<T>(this DbContext dbContext, object[] keyValues) where T : class
        {
            if (keyValues == null || keyValues.Any(v => v == null))
            {
                return null;
            }

            var key = dbContext.FindPrimaryKey<T>();
            var keyProperties = key.Properties;

            if (keyProperties.Count != keyValues.Length)
            {
                if (keyProperties.Count == 1)
                {
                    throw new ArgumentException(
                        CoreStrings.FindNotCompositeKey(typeof(T).ShortDisplayName(), keyValues.Length));
                }

                throw new ArgumentException(
                    CoreStrings.FindValueCountMismatch(typeof(T).ShortDisplayName(), keyProperties.Count, keyValues.Length));
            }

            for (var i = 0; i < keyValues.Length; i++)
            {
                var valueType = keyValues[i].GetType();
                var propertyType = keyProperties[i].ClrType;
                if (valueType != propertyType.UnwrapNullableType())
                {
                    throw new ArgumentException(
                        CoreStrings.FindValueTypeMismatch(
                            i, typeof(T).ShortDisplayName(), valueType.ShortDisplayName(), propertyType.ShortDisplayName()));
                }
            }

#pragma warning disable EF1001 // Internal EF Core API usage.
            return dbContext.GetDependencies().StateManager.TryGetEntry(key, keyValues)?.Entity as T;
#pragma warning restore EF1001 // Internal EF Core API usage.
        }

        /// <summary>
        /// この Entity はリレーションのナビゲーションプロパティをもっているか？
        /// </summary>
        /// <param name="dbContext">dbContext</param>
        /// <returns>この Entity はリレーションのナビゲーションプロパティをもっているか？</returns>
        public static bool HasRelationProperty<T>(this DbContext dbContext)
        {
            return dbContext.Model.FindEntityType(typeof(T)).GetNavigations().Any();
        }

        /// <summary>
        /// リレーションの1階層目をIncluedeするクエリの基礎を組み立てる
        /// </summary>
        /// <remarks>
        /// 階層的にネストしたリレーションは取得せず、1階層しか読み込まないので注意が必要
        /// リレーションを階層的に取得したい場合、もしくはデフォルトでは読み込ませたくない場合、特殊な Repository を用意する想定
        /// </remarks>
        /// <param name="dbContext">dbContext</param>
        /// <returns>リレーションをIncluedeするクエリの基礎</returns>
        public static IQueryable<T> BuildShallowIncludingQuery<T>(this DbContext dbContext) where T : class
        {
            var query = dbContext.Set<T>().AsQueryable();
            var navProps = dbContext.Model.FindEntityType(typeof(T)).GetNavigations();
            foreach (var navProp in navProps)
            {
                query = query.Include(navProp.Name);
            }
            return query;
        }

        /// <summary>
        /// property が内部的に何番目の Primary Key として扱われているか返す
        /// </summary>
        /// <param name="dbContext">dbContext</param>
        /// <param name="propertyLambda">Entityからプロパティを得る式</param>
        /// <returns>Primary Keyの中での順番</returns>
        public static int FindPrimaryKeyIndex<T, TProperty>(this DbContext dbContext, Expression<Func<T, TProperty>> propertyLambda)
        {
            var propInfo = GetPropertyInfoByType(propertyLambda);
            var pKeys = dbContext.FindPrimaryKey<T>().Properties;
            for (int i = 0; i < pKeys.Count; ++i)
            {
                if (pKeys[i].Name == propInfo.Name)
                    return i;
            }
            throw new InvalidOperationException($"{propInfo.Name} is not primary key of Entity {typeof(T).Name}");
        }

        /// <summary>
        /// Findで条件を指定するための式を生成する
        /// </summary>
        /// <remarks>
        /// https://github.com/aspnet/EntityFrameworkCore/blob/049edb0d9f0592d664adf39b765c174a3c2cb831/src/EFCore/Internal/EntityFinder.cs#L332
        /// だいたいここから持ってきた。PrimaryKeyの値を元に式木を組み立てるのはもともと EF が FindAsync の内部でやっていること。
        /// この式を使って問い合わせを実行する Find の根本に Include を挟みたいので拡張した。
        /// </remarks>
        /// <param name="dbContext">dbContext</param>
        /// <param name="keyValues">PrimaryKeyの値</param>
        /// <returns>Findで使う式</returns>
        public static Expression<Func<TEntity, bool>> BuildPredicateToFind<TEntity>(
            this DbContext dbContext,
            object[] keyValues)
        {
            var keyProperties = dbContext.FindPrimaryKey<TEntity>().Properties;
            var bufKeyValues = new ValueBuffer(keyValues);
            var entityParameter = Expression.Parameter(typeof(TEntity), "e");
            var keyValuesConstant = Expression.Constant(bufKeyValues);

            var predicate = GenerateEqualExpression(keyProperties[0], 0);

            for (var i = 1; i < keyProperties.Count; i++)
            {
                predicate = Expression.AndAlso(predicate, GenerateEqualExpression(keyProperties[i], i));
            }

            return Expression.Lambda<Func<TEntity, bool>>(predicate, entityParameter);

            // (p.PlayerId == "xxx") のようなpropertyの等価を比較する式木を生成している
            BinaryExpression GenerateEqualExpression(IProperty property, int i) =>
                // == 演算子の部分
                Expression.Equal(
                    Expression.Call(
                        // p.PlayerId の部分
                        typeof(EF).GetTypeInfo().GetDeclaredMethod(nameof(EF.Property)).MakeGenericMethod(property.ClrType),
                        entityParameter,
                        Expression.Constant(property.Name, typeof(string))),
                    Expression.Convert(
                        // "xxx" の部分
                        Expression.Call(
                            keyValuesConstant,
                            typeof(ValueBuffer).GetRuntimeProperties().Single(p => p.GetIndexParameters().Length > 0).GetMethod,
                            Expression.Constant(i)),
                        property.ClrType));
        }

        private static IKey FindPrimaryKey<T>(this DbContext dbContext)
        {
            return dbContext.Model.FindEntityType(typeof(T)).FindPrimaryKey();
        }

        private static object GetPropertyValue<T>(this T entity, string name)
        {
            return entity.GetType().GetProperty(name).GetValue(entity, null);
        }

        private static Type UnwrapNullableType(this Type type) => Nullable.GetUnderlyingType(type) ?? type;

        private static PropertyInfo GetPropertyInfoByType<T, TProperty>(Expression<Func<T, TProperty>> propertyLambda)
        {
            Type type = typeof(T);

            var member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property");

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property");

            if (type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a property that is not from type {type}");

            return propInfo;
        }
    }
}
