using System.ComponentModel.DataAnnotations.Schema;

namespace SampleApiServer.Models.Entities
{
    /// <summary>
    /// PlayerIdによって紐付けされるEntityを表すインターフェース
    /// </summary>
    public abstract class PlayerBoundEntityBase : EntityBase
    {
        /// <summary>
        /// PlayerのID
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long PlayerId { get; protected set; }
    }
}
