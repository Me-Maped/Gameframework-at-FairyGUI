namespace GameFramework.UI
{
    public class LoadFormDependencyEventArgs: GameFrameworkEventArgs
    {
        /// <summary>
        /// 初始化打开界面时加载依赖资源事件的新实例。
        /// </summary>
        public LoadFormDependencyEventArgs()
        {
            UIFormAssetName = null;
            GroupEnum = UIGroupEnum.NONE;
            DependencyAssetName = null;
            LoadedCount = 0;
            TotalCount = 0;
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
        /// 获取界面组名称。
        /// </summary>
        public UIGroupEnum GroupEnum
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

        public override void Clear()
        {
            UIFormAssetName = null;
            GroupEnum = UIGroupEnum.NONE;
            DependencyAssetName = null;
            LoadedCount = 0;
            TotalCount = 0;
        }

        public static LoadFormDependencyEventArgs Create(string uiFormAssetName, UIGroupEnum groupEnum, string dependencyAssetName, int loadedCount, int totalCount)
        {
            LoadFormDependencyEventArgs loadFormDepEventArgs = ReferencePool.Acquire<LoadFormDependencyEventArgs>();
            loadFormDepEventArgs.UIFormAssetName = uiFormAssetName;
            loadFormDepEventArgs.GroupEnum = groupEnum;
            loadFormDepEventArgs.DependencyAssetName = dependencyAssetName;
            loadFormDepEventArgs.LoadedCount = loadedCount;
            loadFormDepEventArgs.TotalCount = totalCount;
            return loadFormDepEventArgs;
        }
    }
}