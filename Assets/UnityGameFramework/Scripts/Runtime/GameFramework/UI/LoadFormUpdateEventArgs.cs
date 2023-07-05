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
            GroupEnum = UIGroupEnum.NONE;
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
        /// 获取界面组名称。
        /// </summary>
        public UIGroupEnum GroupEnum
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
            GroupEnum = UIGroupEnum.NONE;
            Progress = 0f;
        }

        public static LoadFormUpdateEventArgs Create(string uiFormAssetName, UIGroupEnum groupEnum, float progress)
        {
            LoadFormUpdateEventArgs loadFormUpdateEventArgs = ReferencePool.Acquire<LoadFormUpdateEventArgs>();
            loadFormUpdateEventArgs.UIFormAssetName = uiFormAssetName;
            loadFormUpdateEventArgs.GroupEnum = groupEnum;
            loadFormUpdateEventArgs.Progress = progress;
            return loadFormUpdateEventArgs;
        }
    }
}