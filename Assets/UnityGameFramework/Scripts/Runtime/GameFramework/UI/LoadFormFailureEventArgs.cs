namespace GameFramework.UI
{
    public class LoadFormFailureEventArgs: GameFrameworkEventArgs
    {
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
        /// 获取错误信息。
        /// </summary>
        public string ErrorMessage
        {
            get;
            private set;
        }

        public LoadFormFailureEventArgs()
        {
            UIFormAssetName = null;
            GroupEnum = UIGroupEnum.NONE;
            ErrorMessage = null;
        }
        
        public override void Clear()
        {
            UIFormAssetName = null;
            GroupEnum = UIGroupEnum.NONE;
            ErrorMessage = null;
        }

        public static LoadFormFailureEventArgs Create(string uiFormAssetName, UIGroupEnum groupEnum, string errorMessage)
        {
            LoadFormFailureEventArgs loadFormFailEventArgs = ReferencePool.Acquire<LoadFormFailureEventArgs>();
            loadFormFailEventArgs.UIFormAssetName = uiFormAssetName;
            loadFormFailEventArgs.GroupEnum = groupEnum;
            loadFormFailEventArgs.ErrorMessage = errorMessage;
            return loadFormFailEventArgs;
        }
    }
}