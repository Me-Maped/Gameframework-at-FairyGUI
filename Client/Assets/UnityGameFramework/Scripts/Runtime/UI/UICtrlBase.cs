using System;
using GameFramework;
using GameFramework.UI;

namespace UnityGameFramework.Runtime
{
    public abstract class UICtrlBase<TView,TModel> : IUIController where TView : UIFormBase where TModel : class, IUIModel
    {
        protected TView View;
        protected TModel Model;
        public abstract void Clear();

        void IUIController.Init(UIFormBase uiForm, IUIModel uiModel)
        {
            View = uiForm as TView;
            Model = uiModel as TModel;
            if (View == null || Model == null)
            {
                throw new GameFrameworkException("Cast fail. View or Model is null");
            }
            try
            {
                Model.Init();
                OnInit();
            }
            catch(Exception e)
            {
                throw new GameFrameworkException(e.Message);
            }
        }

        void IUIController.Open()
        {
            try
            {
                Model.Open();
                OnOpen();
            }
            catch (Exception e)
            {
                throw new GameFrameworkException(e.Message);
            }
        }

        void IUIController.Close()
        {
            try
            {
                Model.Close();
                OnClose();
            }
            catch (Exception e)
            {
                throw new GameFrameworkException(e.Message);
            }
        }

        void IUIController.Update(float elapseSeconds, float realElapseSeconds)
        {
            try
            {
                Model.Update(elapseSeconds, realElapseSeconds);
                OnUpdate(elapseSeconds, realElapseSeconds);
            }
            catch (Exception e)
            {
                throw new GameFrameworkException(e.Message);
            }
        }

        protected virtual void OnInit() { }

        protected virtual void OnOpen() { }
        
        protected virtual void OnClose() { }
        
        protected virtual void OnUpdate(float elapseSeconds, float realElapseSeconds) { }
    }
}