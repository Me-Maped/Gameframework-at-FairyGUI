using FairyGUI;

namespace GameFramework.UI
{
    public abstract class UIFormPartBase : UIFormCore
    {
        protected UIFormBase Parent;
        protected abstract string PartName { get; }
        protected abstract void SerializeChild();
        
        //Part不需要Config, 或者拿父类的Config
        public override UIFormConfig Config => Parent.Config;
        public override GComponent Instance { get; set; }

        internal void Init(UIFormBase parent)
        {
            Parent = parent;
            Instance = Parent.Instance.GetChildByPath(PartName) as GComponent;
            if (Instance == null) throw new GameFrameworkException("PartName is not exist in UIForm.");
            SerializeChild();
            OnInit();
        }
        internal void Open()
        {
            OnOpen();
        }
        internal void OpenComplete()
        {
            OnOpenComplete();
        }
        internal void Close()
        {
            OnClose();
            Parent = null;
            Dispose();
        }
        internal void CloseComplete()
        {
            OnCloseComplete();
            DisposeProtectBtn();
            DisposeEvent();
            DisposeClick();
        }
        internal void Destroy()
        {
            // Part的Instance是Base Instance的子组件，不需要手动释放
            Parent = null;
            Instance = null;
            Dispose();
        }

        #region Virtual Methods
        protected virtual void OnInit() {}
        protected virtual void OnOpen(){}
        protected virtual void OnClose(){}
        protected virtual void OnOpenComplete(){}
        protected virtual void OnCloseComplete(){}

        #endregion
    }
}