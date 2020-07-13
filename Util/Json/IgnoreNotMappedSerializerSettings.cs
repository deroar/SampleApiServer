using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace SampleApiServer.Util.Json
{
    /// <summary>
    /// Entity Framework と一本化するために、JsonIgnore アトリビュートに加え NotMapped アトリビュートでもプロパティを無視するようにする設定
    /// </summary>
    public class IgnoreNotMappedSerializerSettings : JsonSerializerSettings
    {
        /// <summary>
        /// IgnoreNotMappedSerializerSettingsのコンストラクタ
        /// </summary>
        public IgnoreNotMappedSerializerSettings()
        {
            ContractResolver = new IgnoreNotMappedContractResolver();
        }

        internal class IgnoreNotMappedContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var prop = base.CreateProperty(member, memberSerialization);
                if (Attribute.IsDefined(member, typeof(NotMappedAttribute)))
                {
                    prop.Ignored = true;
                }
                return prop;
            }
        }
    }
}
