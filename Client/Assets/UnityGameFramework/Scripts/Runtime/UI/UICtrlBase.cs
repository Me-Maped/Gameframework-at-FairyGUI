using System;
using GameFramework;
using GameFramework.UI;

namespace UnityGameFramework.Runtime
{
    public abstract class UICtrlBase<TView,TModel> : IUIController where TView : UIFormBase where TModel : class, IUIModel
    {
        protected TView View;
        protected TModel Model;
        public virtual void Clear()
        {
            View = null;
            Model = null;
        }

        void IUIController.Init(UIFormBase uiForm, IUIModel uiModel)
        {
            View = uiForm as TView;
            Model = uiModel as TModel;
            if (View == null || Model == null)
            {
                throw new GameFrameworkException("Cast fail. View or Model is null");
            }
            if (Define.PkgArg.Debug)
            {
                OnInit();
            }
            else
            {
                try
                {
                    OnInit();
                }
                catch(Exception e)
                {
                    throw new GameFrameworkException(e.Message);
                }
            }
        }

        void IUIController.Open()
        {
            if (Define.PkgArg.Debug)
            {
                OnOpen();
            }
            else
            {
                try
                {
                    OnOpen();
                }
                catch (Exception e)
                {
                    throw new GameFrameworkException(e.Message);
                }
            }
        }

        void IUIController.Close()
        {
            if (Define.PkgArg.Debug)
            {
                OnClose();
            }
            else
            {
                try
                {
                    OnClose();
                }
                catch (Exception e)
                {
                    throw new GameFrameworkException(e.Message);
                }
            }
        }

        void IUIController.Update(float elapseSeconds, float realElapseSeconds)
        {
            if (Define.PkgArg.Debug)
            {
                OnUpdate(elapseSeconds, realElapseSeconds);
            }
            else
            {
                try
                {
                    OnUpdate(elapseSeconds, realElapseSeconds);
                }
                catch (Exception e)
                {
                    throw new GameFrameworkException(e.Message);
                }
            }
        }

        protected virtual void OnInit() { }

        protected virtual void OnOpen() { }
        
        protected virtual void OnClose() { }
        
        protected virtual void OnUpdate(float elapseSeconds, float realElapseSeconds) { }
    }
}