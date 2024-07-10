using FairyGUI;
using GameFramework;
using GameFramework.UI;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    /// Default
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class UIForm<T> : UIFormBase where T : GComponent
    {
        public T View { get; protected set; }
        protected abstract override UIFormConfig CreateConfig();
        protected override void SerializeChild()
        {
            View = Instance as T;
            if (View == null)
            {
                throw  new GameFrameworkException(
                    "View case fail. Is this a new package? please check 'UIBinder.cs' and click 'Export UIBinder' in GameFramework tools");
            }
        }

        #region Protected Methods
        protected void GoTo<TView>(bool closeOther = false,object userData = null) where TView : UIFormBase
        {
            GameEntry.GetComponent<UIComponent>().CloseForm(this);
            GameEntry.GetComponent<UIComponent>().OpenForm<TView>(closeOther, userData);
        }
        #endregion
    }
    
    /// <summary>
    /// MVC
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TCtrl"></typeparam>
    public abstract class UIForm<TView,TModel,TCtrl> : UIForm<TView> where TModel : UIModelBase<TModel>, new() where TCtrl : class, IUIController, new() where TView : GComponent
    {
        protected TModel Model { get; private set; }
        protected TCtrl Ctrl { get; private set; }

        protected override IUIController GetUICtrl()
        {
            Ctrl = ReferencePool.Acquire<TCtrl>();
            return Ctrl;
        }

        protected override IUIModel GetUIModel()
        {
            Model = UIModelBase<TModel>.Inst;
            return Model;
        }
    }
    
    /// <summary>
    /// Singleton
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    public abstract class UIForm<TView,TModel> : UIForm<TView> where TModel : UIModelBase<TModel>, new() where TView : GComponent
    {
        protected TModel Model { get; private set; }

        protected override IUIModel GetUIModel()
        {
            Model = UIModelBase<TModel>.Inst;
            return Model;
        }
    }
}