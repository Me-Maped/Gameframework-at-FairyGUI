using GameFramework;
using GameFramework.Event;
using GameFramework.UI;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    /// 打开界面失败事件。
    /// </summary>
    public sealed class OpenUIFormFailureEventArgs : GameEventArgs
    {
        /// <summary>
        /// 打开界面失败事件编号。
        /// </summary>
        public static readonly int EventId = typeof(OpenUIFormFailureEventArgs).GetHashCode();

        /// <summary>
        /// 初始化打开界面失败事件的新实例。
        /// </summary>
        public OpenUIFormFailureEventArgs()
        {
            UIFormAssetName = null;
            ErrorMessage = null;
        }

        /// <summary>
        /// 获取打开界面失败事件编号。
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
        /// 获取错误信息。
        /// </summary>
        public string ErrorMessage
        {
            get;
            private set;
        }

        /// <summary>
        /// 创建打开界面失败事件。
        /// </summary>
        /// <param name="e">内部事件。</param>
        /// <returns>创建的打开界面失败事件。</returns>
        public static OpenUIFormFailureEventArgs Create(LoadFormFailureEventArgs e)
        {
            OpenUIFormFailureEventArgs openUIFormFailureEventArgs = ReferencePool.Acquire<OpenUIFormFailureEventArgs>();
            openUIFormFailureEventArgs.UIFormAssetName = e.UIFormAssetName;
            openUIFormFailureEventArgs.ErrorMessage = e.ErrorMessage;
            return openUIFormFailureEventArgs;
        }

        /// <summary>
        /// 清理打开界面失败事件。
        /// </summary>
        public override void Clear()
        {
            UIFormAssetName = null;
            ErrorMessage = null;
        }
    }
}