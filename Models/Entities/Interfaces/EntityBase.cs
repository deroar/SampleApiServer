using PropertyChanged;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SampleApiServer.Models.Entities
{
    /// <summary>
    /// Entityを表す抽象クラス
    /// </summary>
    public abstract class EntityBase : INotifyPropertyChanged
    {
        /// <summary>
        /// 登録日時。
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Propertyが変更されたときに呼ばれるイベント。INotifyPropertyChangedの実装。
        /// Fodyによって中間コンパイル時にインジェクトされる
        /// </summary>
#pragma warning disable 67 // Fodyによる中間コンパイルを受けるためイベント未使用の警告を消去
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 67

        /// <summary>
        /// 値が変更されているか？
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        [DoNotNotify]
        public bool HasChange { get; set; } = false;

        /// <summary>
        /// 最初のコミットか？
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public bool IsFirstCommit => internalVersion == 0 && !isInCommitProcess;

        /// <summary>
        /// Entityのバージョン
        /// </summary>
        [ConcurrencyCheck]
        [DoNotNotify]
        public long Version
        {
            get => HasChange ? internalVersion + 1 : internalVersion;

            private set
            {
                internalVersion = value;
            }
        }

        // Entityのバージョンの実体
        private long internalVersion;

        // 永続層に保存している途中か？（INSERT もしくは UPDATE を実行し、コミットを待っている状態か？）
        private bool isInCommitProcess = false;

        // Submit時に呼ばれるコールバック
        Func<Task> onSubmit;

        /// <summary>
        /// コミットを待つ状態にセットする
        /// </summary>
        public void PrepareToSubmit(Func<Task> onSubmit)
        {
            if (!HasChange)
            {
                throw new InvalidOperationException($"Cannot stage unchanged entity");
            }
            this.onSubmit = onSubmit;
            isInCommitProcess = true;
        }

        /// <summary>
        /// 状態の変更を確定する。
        /// </summary>
        public async Task SubmitChanges()
        {
            if (!HasChange)
            {
                return;
            }
            await (onSubmit?.Invoke() ?? Task.CompletedTask);
            onSubmit = null;
            ++internalVersion;
            HasChange = false;
            isInCommitProcess = false;
        }

        /// <summary>
        /// Propertyが変更されたときに呼ばれる関数の本体
        /// </summary>
        protected void OnPropertyChanged(string _)
        {
            if (!HasChange)
                HasChange = true;
        }
    }
}
