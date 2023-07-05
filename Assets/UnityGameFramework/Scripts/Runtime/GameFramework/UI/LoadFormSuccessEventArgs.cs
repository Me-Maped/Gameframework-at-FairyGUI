namespace GameFramework.UI
{
    public class LoadFormSuccessEventArgs: GameFrameworkEventArgs
    {
        /// <summary>
        /// 初始化打开界面成功事件的新实例。
        /// </summary>
        public LoadFormSuccessEventArgs()
        {
            UIForm = null;
            Duration = 0f;
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
        public override void Clear()
        {
            UIForm = null;
            Duration = 0f;
        }

        public static LoadFormSuccessEventArgs Create(UIFormBase uiForm, float duration)
        {
            LoadFormSuccessEventArgs loadFormSucEventArgs = ReferencePool.Acquire<LoadFormSuccessEventArgs>();
            loadFormSucEventArgs.UIForm = uiForm;
            loadFormSucEventArgs.Duration = duration;
            return loadFormSucEventArgs;
        }
    }
}