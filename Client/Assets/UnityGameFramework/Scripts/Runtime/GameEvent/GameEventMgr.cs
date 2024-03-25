using System;
using System.Collections.Generic;
using GameFramework;

namespace UnityGameFramework.Runtime
{
    public class GameEventMgr : IReference
    {
        private List<int> m_listEventTypes;
        private List<Delegate> m_listHandles;
        private List<string> m_listEventNames;
        private List<Delegate> m_listHandles2;
        private bool m_isInit = false;

        public GameEventMgr()
        {
            if (m_isInit)
            {
                return;
            }

            m_isInit = true;
            m_listEventTypes = new List<int>();
            m_listHandles = new List<Delegate>();
            m_listEventNames = new List<string>();
            m_listHandles2 = new List<Delegate>();
        }

        public void Clear()
        {
            if (!m_isInit)
            {
                return;
            }

            for (int i = 0; i < m_listEventTypes.Count; ++i)
            {
                var eventType = m_listEventTypes[i];
                var handle = m_listHandles[i];
                GameEvent.RemoveEventListener(eventType, handle);
            }

            for (int i = 0; i < m_listEventNames.Count; i++)
            {
                var eventName = m_listEventNames[i];
                var handle = m_listHandles2[i];
                GameEvent.RemoveEventListener(eventName, handle);
            }

            m_listEventTypes.Clear();
            m_listHandles.Clear();
            m_listEventNames.Clear();
            m_listHandles2.Clear();
        }

        private void InternalAddEvent(int eventType, Delegate handler)
        {
            m_listEventTypes.Add(eventType);
            m_listHandles.Add(handler);
        }

        private void InternalAddEvent(string eventName, Delegate handler)
        {
            m_listEventNames.Add(eventName);
            m_listHandles2.Add(handler);
        }

        public void AddEvent(int eventType, Action handler)
        {
            if (GameEvent.AddEventListener(eventType, handler))
            {
                InternalAddEvent(eventType, handler);
            }
        }
        
        public void AddEvent(string eventName, Action handler)
        {
            if (GameEvent.AddEventListener(eventName, handler))
            {
                InternalAddEvent(eventName, handler);
            }
        }

        public void AddEvent<T>(int eventType, Action<T> handler)
        {
            if (GameEvent.AddEventListener(eventType, handler))
            {
                InternalAddEvent(eventType, handler);
            }
        }
        
        public void AddEvent<T>(string eventName, Action<T> handler)
        {
            if (GameEvent.AddEventListener(eventName, handler))
            {
                InternalAddEvent(eventName, handler);
            }
        }

        public void AddEvent<T, U>(int eventType, Action<T, U> handler)
        {
            if (GameEvent.AddEventListener(eventType, handler))
            {
                InternalAddEvent(eventType, handler);
            }
        }
        
        public void AddEvent<T, U>(string eventName, Action<T, U> handler)
        {
            if (GameEvent.AddEventListener(eventName, handler))
            {
                InternalAddEvent(eventName, handler);
            }
        }

        public void AddEvent<T, U, V>(int eventType, Action<T, U, V> handler)
        {
            if (GameEvent.AddEventListener(eventType, handler))
            {
                InternalAddEvent(eventType, handler);
            }
        }

        public void AddEvent<T, U, V>(string eventName, Action<T, U, V> handler)
        {
            if (GameEvent.AddEventListener(eventName, handler))
            {
                InternalAddEvent(eventName, handler);
            }
        }

        public void AddEvent<T, U, V, W>(int eventType, Action<T, U, V, W> handler)
        {
            if (GameEvent.AddEventListener(eventType, handler))
            {
                InternalAddEvent(eventType, handler);
            }
        }
        
        public void AddEvent<T, U, V, W>(string eventName, Action<T, U, V, W> handler)
        {
            if (GameEvent.AddEventListener(eventName, handler))
            {
                InternalAddEvent(eventName, handler);
            }
        }
    }
}