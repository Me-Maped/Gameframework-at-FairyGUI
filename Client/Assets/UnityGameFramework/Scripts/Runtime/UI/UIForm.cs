using FairyGUI;
using GameFramework;
using GameFramework.UI;

namespace UnityGameFramework.Runtime
{
    public abstract class UIForm<T> : UIFormBase where T : GComponent
    {
        protected T View;
        protected abstract override UIFormConfig CreateConfig();
        protected override void SerializeChild()
        {
            View = Instance as T;
        }

        #region Public Methods
        public void Close(bool immediately = false)
        {
            if (!immediately) GameEntry.GetComponent<UIComponent>().CloseForm(this);
            else GameEntry.GetComponent<UIComponent>().CloseFormImmediately(this);
        }

        #endregion
    }

    public abstract class UIForm<TView, TModel, TCtrl> : UIFormBase where TView : GComponent
        where TModel : UIModelBase<TModel>, new()
        where TCtrl : class, IUIController, new()
    {
        public TView View { get; protected set; }
        protected TModel Model { get; private set; }
        protected abstract override UIFormConfig CreateConfig();

        protected override void SerializeChild()
        {
            View = Instance as TView;
            if (View == null)
            {
                throw new GameFrameworkException(
                    "View case fail. Is this a new package? please check 'UIBinder.cs' and click 'Export UIBinder' in GameFramework tools");
            }
        }

        protected override IUIController GetUICtrl()
        {
            return ReferencePool.Acquire<TCtrl>();
        }

        protected override IUIModel GetUIModel()
        {
            Model = UIModelBase<TModel>.Inst;
            return Model;
        }

        #region Public Methods

        public void Close(bool immediately = false)
        {
            if (!immediately) GameEntry.GetComponent<UIComponent>().CloseForm(this);
            else GameEntry.GetComponent<UIComponent>().CloseFormImmediately(this);
        }

        #endregion
    }
}