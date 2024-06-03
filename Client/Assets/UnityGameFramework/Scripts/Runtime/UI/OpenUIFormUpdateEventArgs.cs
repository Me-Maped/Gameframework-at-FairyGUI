using GameFramework;
using GameFramework.Event;
using GameFramework.UI;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    /// 打开界面更新事件。
    /// </summary>
    public sealed class OpenUIFormUpdateEventArgs : GameEventArgs
    {
        /// <summary>
        /// 打开界面更新事件编号。
        /// </summary>
        public static readonly int EventId = typeof(OpenUIFormUpdateEventArgs).GetHashCode();

        /// <summary>
        /// 初始化打开界面更新事件的新实例。
        /// </summary>
        public OpenUIFormUpdateEventArgs()
        {
            UIFormAssetName = null;
            Progress = 0f;
        }

        /// <summary>
        /// 获取打开界面更新事件编号。
        /// </summary>
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        /// <summary>
        /// 获取界面资源名称。
        /// </summary>
        public string UIFormAssetName
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取打开界面进度。
        /// </summary>
        public float Progress
        {
            get;
            private set;
        }

        /// <summary>
        /// 创建打开界面更新事件。
        /// </summary>
        /// <param name="e">内部事件。</param>
        /// <returns>创建的打开界面更新事件。</returns>
        public static OpenUIFormUpdateEventArgs Create(LoadFormUpdateEventArgs e)
        {
            OpenUIFormUpdateEventArgs openUIFormUpdateEventArgs = ReferencePool.Acquire<OpenUIFormUpdateEventArgs>();
            openUIFormUpdateEventArgs.UIFormAssetName = e.UIFormAssetName;
            openUIFormUpdateEventArgs.Progress = e.Progress;
            return openUIFormUpdateEventArgs;
        }

        /// <summary>
        /// 清理打开界面更新事件。
        /// </summary>
        public override void Clear()
        {
            UIFormAssetName = null;
            Progress = 0f;
        }
    }
}