using GameFramework;
using GameFramework.Event;
using GameFramework.UI;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    /// 打开界面成功事件。
    /// </summary>
    public sealed class OpenUIFormSuccessEventArgs : GameEventArgs
    {
        /// <summary>
        /// 打开界面成功事件编号。
        /// </summary>
        public static readonly int EventId = typeof(OpenUIFormSuccessEventArgs).GetHashCode();

        /// <summary>
        /// 初始化打开界面成功事件的新实例。
        /// </summary>
        public OpenUIFormSuccessEventArgs()
        {
            UIForm = null;
            Duration = 0f;
        }

        /// <summary>
        /// 获取打开界面成功事件编号。
        /// </summary>
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        /// <summary>
        /// 获取打开成功的界面。
        /// </summary>
        public UIFormBase UIForm
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取加载持续时间。
        /// </summary>
        public float Duration
        {
            get;
            private set;
        }

        /// <summary>
        /// 创建打开界面成功事件。
        /// </summary>
        /// <param name="e">内部事件。</param>
        /// <returns>创建的打开界面成功事件。</returns>
        public static OpenUIFormSuccessEventArgs Create(LoadFormSuccessEventArgs e)
        {
            OpenUIFormSuccessEventArgs openUIFormSuccessEventArgs = ReferencePool.Acquire<OpenUIFormSuccessEventArgs>();
            openUIFormSuccessEventArgs.UIForm = e.UIForm;
            openUIFormSuccessEventArgs.Duration = e.Duration;
            return openUIFormSuccessEventArgs;
        }

        /// <summary>
        /// 清理打开界面成功事件。
        /// </summary>
        public override void Clear()
        {
            UIForm = null;
            Duration = 0f;
        }
    }
}
