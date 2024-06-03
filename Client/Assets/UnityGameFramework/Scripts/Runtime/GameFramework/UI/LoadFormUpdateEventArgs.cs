namespace GameFramework.UI
{
    public class LoadFormUpdateEventArgs: GameFrameworkEventArgs
    {
        /// <summary>
        /// 初始化打开界面更新事件的新实例。
        /// </summary>
        public LoadFormUpdateEventArgs()
        {
            UIFormAssetName = null;
            Progress = 0f;
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

        public override void Clear()
        {
            UIFormAssetName = null;
            Progress = 0f;
        }

        public static LoadFormUpdateEventArgs Create(string uiFormAssetName, float progress)
        {
            LoadFormUpdateEventArgs loadFormUpdateEventArgs = ReferencePool.Acquire<LoadFormUpdateEventArgs>();
            loadFormUpdateEventArgs.UIFormAssetName = uiFormAssetName;
            loadFormUpdateEventArgs.Progress = progress;
            return loadFormUpdateEventArgs;
        }
    }
}