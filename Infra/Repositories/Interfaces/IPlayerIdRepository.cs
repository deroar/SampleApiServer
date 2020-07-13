using System.Threading.Tasks;

namespace SampleApiServer.Infra.Repositories
{
    /// <summary>
    /// PlayerIDを扱うリポジトリインターフェース
    /// </summary>
    public interface IPlayerIdRepository
    {
        /// <summary>
        /// PlayerIDを取得する
        /// </summary>
        /// <returns>PlayerID</returns>
        Task<long> GetPlayerId();
    }
}
