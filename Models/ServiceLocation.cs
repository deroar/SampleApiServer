namespace SampleApiServer.Models
{
    /// <summary>
    /// サービスロケーション
    /// </summary>
    public class ServiceLocation
    {
        /// <summary>
        /// ApiServerのURL
        /// </summary>
        public string ApiServerUrl { get; set; }

        /// <summary>
        /// アセットのURL
        /// </summary>
        public string AssetUrl { get; set; }
    }
}
