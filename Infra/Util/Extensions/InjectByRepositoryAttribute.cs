using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleApiServer.Infra.Util.Extensions
{
    /// <summary>
    /// リポジトリによるDIの対象であることを示すマーカAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class InjectByRepositoryAttribute : Attribute
    {
    }
}
