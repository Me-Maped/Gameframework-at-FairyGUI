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
                catch (Exception e)
                {
                    throw new GameFrameworkException(e.Message);
                }
            }
        }

        void IUIModel.Open()
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

        void IUIModel.Close()
        {
            if (_gameEventMgr != null)
            {
                ReferencePool.Release(_gameEventMgr);
                _gameEventMgr = null;
            }
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

        void IUIModel.Update(float elapseSeconds, float realElapseSeconds)
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

        #region Event
        protected void Trigger<TValue>(int hashCode, TValue value)
        {
            GameEvent.Send(hashCode, value);
        }

        protected void Trigger<TValue>(string propertyName, TValue value)
        {
            GameEvent.Send(propertyName, value);
        }

        protected void Trigger<TValue>(Enum enumValue, TValue value)
        {
            Trigger(enumValue.ToString(), value);
        }

        protected void Trigger(int hashCode)
        {
            GameEvent.Send(hashCode);
        }
        
        protected void Trigger(string propertyName)
        {
            GameEvent.Send(propertyName);
        }
        
        protected  void Trigger(Enum enumValue)
        {
            Trigger(enumValue.ToString());
        }

        public void Register<TValue>(string propertyName, Action<TValue> handler)
        {
            EventMgr.AddEvent(propertyName,handler);
        }

        public void Register<TValue>(int hashCode, Action<TValue> handler)
        {
            EventMgr.AddEvent(hashCode, handler);
        }
        
        public void Register<TValue>(Enum enumValue, Action<TValue> handler)
        {
            Register(enumValue.ToString(), handler);
        }

        public void Register(string propertyName, Action handler)
        {
            EventMgr.AddEvent(propertyName, handler);
        }
        
        public void Register(int hashCode, Action handler)
        {
            EventMgr.AddEvent(hashCode, handler);
        }
        
        public void Register(Enum enumValue, Action handler)
        {
            Register(enumValue.ToString(), handler);
        }
        #endregion

    }
}