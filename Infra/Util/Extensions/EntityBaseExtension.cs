using SampleApiServer.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SampleApiServer.Infra.Util.Extensions
{
    /// <summary>
    /// Entityに依存性を注入するための拡張
    /// </summary>
    public static class EntityBaseExtension
    {
        /// <summary>
        /// 依存性を注入する
        /// </summary>
        /// <param name="entity">対象のEntityBase</param>
        /// <param name="serviceProvider">サービスプロバイダ</param>
        public static T InjectDependencies<T>(this T entity, IServiceProvider serviceProvider)
            where T : EntityBase
        {
            var injectFields = entity.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => Attribute.IsDefined(field, typeof(InjectByRepositoryAttribute)));

            foreach (FieldInfo injectField in injectFields)
            {
                injectField.SetValue(entity, serviceProvider.GetService(injectField.FieldType));
            }

            foreach (EntityBase child in entity.GetChildEntities())
            {
                child.InjectDependencies(serviceProvider);
            }

            return entity;
        }

        private static IEnumerable<EntityBase> GetChildEntities<T>(this T entity)
            where T : EntityBase
        {
            var childEntities = entity.GetType().GetProperties()
                .Where(n => n.PropertyType.IsSubclassOf(typeof(EntityBase)))
                .Select(n => (EntityBase)n.GetValue(entity))
                .Where(n => n != null);
            var collections = entity.GetType().GetProperties()
                .Where(n => n.PropertyType.IsGenericType)
                .Where(n => n.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                .Where(n => n.PropertyType.GetGenericArguments()[0].IsSubclassOf(typeof(EntityBase)))
                .Select(n => (IEnumerable<EntityBase>)n.GetValue(entity))
                .Where(n => n != null);
            return childEntities.Concat(collections.SelectMany(n => n.AsEnumerable()));
        }
    }
}
