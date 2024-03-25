using System;
using GameFramework;
using GameFramework.UI;

namespace UnityGameFramework.Runtime
{
    public abstract class UIModelBase<T> : IUIModel where T : UIModelBase<T>, new()
    {
        private static T _inst;
        public static T Inst => _inst ??= ReferencePool.Acquire<T>();

        private GameEventMgr _gameEventMgr;
        public GameEventMgr EventMgr => _gameEventMgr??=ReferencePool.Acquire<GameEventMgr>();
        
        public abstract void Clear();

        void IUIModel.Init()
        {
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
            if (_gameEventMgr != null)
            {
                ReferencePool.Release(_gameEventMgr);
                _gameEventMgr = null;
            }
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

        protected void Trigger<TValue>(string propertyName, TValue value)
        {
            GameEvent.Send(propertyName, value);
        }

        public void Register<TValue>(string propertyName, Action<TValue> handler)
        {
            EventMgr.AddEvent(propertyName,handler);
        }

        public void Register<TValue>(int hashCode, Action<TValue> handler)
        {
            EventMgr.AddEvent(hashCode, handler);
        }

        public void Register(string propertyName, Action handler)
        {
            EventMgr.AddEvent(propertyName, handler);
        }
        
        public void Register(int hashCode, Action handler)
        {
            EventMgr.AddEvent(hashCode, handler);
        }
    }
}