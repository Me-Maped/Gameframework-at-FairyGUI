using GameFramework;
using GameFramework.Event;
using GameFramework.UI;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    /// 打开界面时加载依赖资源事件。
    /// </summary>
    public sealed class OpenUIFormDependencyAssetEventArgs : GameEventArgs
    {
        /// <summary>
        /// 打开界面时加载依赖资源事件编号。
        /// </summary>
        public static readonly int EventId = typeof(OpenUIFormDependencyAssetEventArgs).GetHashCode();

        /// <summary>
        /// 初始化打开界面时加载依赖资源事件的新实例。
        /// </summary>
        public OpenUIFormDependencyAssetEventArgs()
        {
            UIFormAssetName = null;
            DependencyAssetName = null;
            LoadedCount = 0;
            TotalCount = 0;
        }

        /// <summary>
        /// 获取打开界面时加载依赖资源事件编号。
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
        /// 获取被加载的依赖资源名称。
        /// </summary>
        public string DependencyAssetName
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取当前已加载依赖资源数量。
        /// </summary>
        public int LoadedCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取总共加载依赖资源数量。
        /// </summary>
        public int TotalCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 创建打开界面时加载依赖资源事件。
        /// </summary>
        /// <param name="e">内部事件。</param>
        /// <returns>创建的打开界面时加载依赖资源事件。</returns>
        public static OpenUIFormDependencyAssetEventArgs Create(LoadFormDependencyEventArgs e)
        {
            OpenUIFormDependencyAssetEventArgs openUIFormDependencyAssetEventArgs = ReferencePool.Acquire<OpenUIFormDependencyAssetEventArgs>();
            openUIFormDependencyAssetEventArgs.UIFormAssetName = e.UIFormAssetName;
            openUIFormDependencyAssetEventArgs.DependencyAssetName = e.DependencyAssetName;
            openUIFormDependencyAssetEventArgs.LoadedCount = e.LoadedCount;
            openUIFormDependencyAssetEventArgs.TotalCount = e.TotalCount;
            return openUIFormDependencyAssetEventArgs;
        }

        /// <summary>
        /// 清理打开界面时加载依赖资源事件。
        /// </summary>
        public override void Clear()
        {
            UIFormAssetName = null;
            DependencyAssetName = null;
            LoadedCount = 0;
            TotalCount = 0;
        }
    }
}
