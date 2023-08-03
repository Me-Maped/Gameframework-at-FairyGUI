using GameFramework;
using GameFramework.Event;
using GameFramework.UI;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    /// 关闭界面完成事件。
    /// </summary>
    public sealed class CloseUIFormCompleteEventArgs : GameEventArgs
    {
        /// <summary>
        /// 关闭界面完成事件编号。
        /// </summary>
        public static readonly int EventId = typeof(CloseUIFormCompleteEventArgs).GetHashCode();

        /// <summary>
        /// 初始化关闭界面完成事件的新实例。
        /// </summary>
        public CloseUIFormCompleteEventArgs()
        {
            UIFormAssetName = null;
            GroupEnum = UIGroupEnum.NONE;
        }

        /// <summary>
        /// 获取关闭界面完成事件编号。
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
        /// 获取界面所属的界面组。
        /// </summary>
        public UIGroupEnum GroupEnum
        {
            get;
            private set;
        }

        /// <summary>
        /// 创建关闭界面完成事件。
        /// </summary>
        /// <param name="e">内部事件。</param>
        /// <returns>创建的关闭界面完成事件。</returns>
        public static CloseUIFormCompleteEventArgs Create(CloseFormCompleteEventArgs e)
        {
            CloseUIFormCompleteEventArgs closeUIFormCompleteEventArgs = ReferencePool.Acquire<CloseUIFormCompleteEventArgs>();
            closeUIFormCompleteEventArgs.UIFormAssetName = e.ConfigResName;
            closeUIFormCompleteEventArgs.GroupEnum = e.ConfigGroupEnum;
            return closeUIFormCompleteEventArgs;
        }

        /// <summary>
        /// 清理关闭界面完成事件。
        /// </summary>
        public override void Clear()
        {
            UIFormAssetName = null;
            GroupEnum = UIGroupEnum.NONE;
        }
    }
}
