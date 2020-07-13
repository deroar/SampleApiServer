using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace SampleApiServer.Util.Json
{
    /// <summary>
    /// private な setter しかもたないプロパティにも書き込めるようにする JSON serializer の設定
    /// </summary>
    public class PrivatePropWritingSerializerSettings : JsonSerializerSettings
    {
        /// <summary>
        /// PrivatePropJsonSerializerSettingsのコンストラクタ
        /// </summary>
        public PrivatePropWritingSerializerSettings()
        {
            ContractResolver = new PrivatePropReadingContractResolver();
        }

        internal class PrivatePropReadingContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var prop = base.CreateProperty(member, memberSerialization);

                if (!prop.Writable)
                {
                    var property = member as PropertyInfo;
                    if (property != null)
                    {
                        var hasPrivateSetter = property.GetSetMethod(true) != null;
                        prop.Writable = hasPrivateSetter;
                    }
                }

                return prop;
            }
        }
    }
}
