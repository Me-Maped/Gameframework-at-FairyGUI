using System;
using GameFramework;
using GameFramework.UI;

namespace UnityGameFramework.Runtime
{
    public abstract class UIModelBase<TCtrl> : IUIModel where TCtrl : class, IUIController
    {
        protected TCtrl Controller;
        public abstract void Clear();

        void IUIModel.Init(IUIController controller)
        {
            Controller = controller as TCtrl;
            if (Controller == null)
            {
                throw new GameFrameworkException("Cast fail. Controller is null");
            }
            try
            {
                OnInit();
            }
            catch (Exception e)
            {
                throw new GameFrameworkException(e.Message);
            }
        }

        void IUIModel.Open()
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

        void IUIModel.Close()
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

        void IUIModel.Update(float elapseSeconds, float realElapseSeconds)
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

        protected virtual void OnInit() { }

        protected virtual void OnOpen() { }
        
        protected virtual void OnClose() { }
        
        protected virtual void OnUpdate(float elapseSeconds, float realElapseSeconds) { }
    }
}