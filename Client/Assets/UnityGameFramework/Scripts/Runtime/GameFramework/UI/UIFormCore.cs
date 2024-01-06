using System;
using System.Collections.Generic;
using FairyGUI;
using GameFramework.Event;
using UnityEngine;

namespace GameFramework.UI
{
    public abstract class UIFormCore : IWaitable, IReference
    {
        public abstract UIFormConfig Config { get; }
        public abstract GComponent Instance { get; set; }
        public abstract void Clear();

        private Dictionary<GObject, EventCallback0> m_ClickCallbacks;
        private Dictionary<GObject, EventCallback1> m_ClickCallbacks1;
        private Dictionary<int, EventHandler<GameEventArgs>> m_Events;
        private Dictionary<GObject, float> m_ProtectBtns;
        private List<GObject> m_CachedList;
        private List<GObject> m_CachedList2;

        /// <summary>
        /// 周期更新
        /// </summary>
        /// <param name="elapseSeconds"></param>
        /// <param name="realElapseSeconds"></param>
        public virtual void Update(float elapseSeconds, float realElapseSeconds) { }

        /// <summary>
        /// 关闭界面组
        /// </summary>
        public virtual void Shutdown() { }


        /// <summary>
        /// 接口方法
        /// </summary>
        /// <param name="callback"></param>
        public virtual void Wait(Action callback) { }

        /// <summary>
        /// 添加Btn点击事件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="callback"></param>
        public void AddClick(GObject obj, EventCallback0 callback)
        {
            if (m_ClickCallbacks == null)
                m_ClickCallbacks = new Dictionary<GObject, EventCallback0>();
            if (m_ClickCallbacks.ContainsKey(obj)) return;
            m_ClickCallbacks.Add(obj, callback);
            obj.onClick.Set(callback);
        }

        /// <summary>
        /// 添加Btn点击事件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="callback"></param>
        public void AddClick(GObject obj, EventCallback1 callback)
        {
            if (m_ClickCallbacks1 == null)
                m_ClickCallbacks1 = new Dictionary<GObject, EventCallback1>();
            if (m_ClickCallbacks1.ContainsKey(obj)) return;
            m_ClickCallbacks1.Add(obj, callback);
            obj.onClick.Set(callback);
        }

        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="id"></param>
        /// <param name="e"></param>
        public void AddEvent(int id, EventHandler<GameEventArgs> e)
        {
            if (m_Events == null)
                m_Events = new Dictionary<int, EventHandler<GameEventArgs>>();
            if (m_Events.ContainsKey(id)) return;
            m_Events.Add(id, e);
            GameFrameworkEntry.GetModule<IEventManager>().Subscribe(id, e);
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        /// <param name="id"></param>
        /// <param name="e"></param>
        public void RemoveEvent(int id, EventHandler<GameEventArgs> e)
        {
            if (m_Events == null) return;
            if (!m_Events.ContainsKey(id)) return;
            m_Events.Remove(id);
            GameFrameworkEntry.GetModule<IEventManager>().Unsubscribe(id, e);
        }

        /// <summary>
        /// 按钮保护
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="seconds"></param>
        public void ProtectBtn(GObject obj, float seconds = 5f)
        {
            if (m_ProtectBtns == null)
                m_ProtectBtns = new Dictionary<GObject, float>();
            if (m_ProtectBtns.ContainsKey(obj)) m_ProtectBtns.Remove(obj);
            m_ProtectBtns.Add(obj, seconds);
            obj.touchable = false;
            if (obj.displayObject != null && obj.displayObject is Container)
                ((Container)obj.displayObject).touchable = false;
            Timers.inst.Remove(RevertProtectBtn);
            Timers.inst.Add(1f, 0, RevertProtectBtn);
        }

        /// <summary>
        /// 销毁
        /// </summary>
        internal void Dispose()
        {
            DisposeEvent();
            DisposeClick();
            DisposeProtectBtn();
            m_Events?.Clear();
            m_Events = null;
            m_ClickCallbacks?.Clear();
            m_ClickCallbacks1?.Clear();
            m_ClickCallbacks = null;
            m_ClickCallbacks1 = null;
        }

        /// <summary>
        /// 按钮保护
        /// </summary>
        /// <param name="param"></param>
        internal void RevertProtectBtn(object param)
        {
            float curTime = Time.time;
            if (m_ProtectBtns == null)
            {
                Timers.inst.Remove(RevertProtectBtn);
                return;
            }

            if (m_CachedList == null) m_CachedList = new List<GObject>(m_ProtectBtns.Keys);
            else
            {
                m_CachedList.Clear();
                m_CachedList.AddRange(m_ProtectBtns.Keys);
            }
            if (m_CachedList2 == null) m_CachedList2 = new List<GObject>();
            else m_CachedList2.Clear();
            for (int i = m_CachedList.Count - 1; i >= 0; i--)
            {
                GObject key = m_CachedList[i];
                float value = m_ProtectBtns[key];
                if (value <= curTime)
                {
                    if (!key.isDisposed)
                    {
                        key.touchable = true;
                        if (key.displayObject != null && key.displayObject is Container)
                            ((Container)key.displayObject).touchable = true;
                    }
                    m_CachedList2.Add(key);
                }
            }

            foreach (var removeObj in m_CachedList2)
            {
                m_ProtectBtns.Remove(removeObj);
            }

            if (m_ProtectBtns.Count == 0)
            {
                Timers.inst.Remove(RevertProtectBtn);
            }
        }

        /// <summary>
        /// 移除点击事件
        /// </summary>
        internal void DisposeClick()
        {
            if (m_ClickCallbacks != null)
            {
                foreach (var clickInfo in m_ClickCallbacks)
                {
                    clickInfo.Key.onClick.Clear();
                }
                m_ClickCallbacks.Clear();
            }
            if (m_ClickCallbacks1 != null)
            {
                foreach (var clickInfo in m_ClickCallbacks1)
                {
                    clickInfo.Key.onClick.Clear();
                }
                m_ClickCallbacks1.Clear();
            }
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        internal void DisposeEvent()
        {
            if (m_Events == null) return;
            foreach (var eventInfo in m_Events)
            {
                GameFrameworkEntry.GetModule<IEventManager>().Unsubscribe(eventInfo.Key, eventInfo.Value);
            }
            m_Events.Clear();
        }

        /// <summary>
        /// 移除按钮保护
        /// </summary>
        internal void DisposeProtectBtn()
        {
            Timers.inst.Remove(RevertProtectBtn);
            if (m_ProtectBtns == null) return;
            foreach (var protectBtn in m_ProtectBtns)
            {
                if (protectBtn.Key.isDisposed) continue;
                protectBtn.Key.touchable = true;
                if (protectBtn.Key.displayObject != null && protectBtn.Key.displayObject is Container)
                    ((Container)protectBtn.Key.displayObject).touchable = true;
            }
            m_ProtectBtns.Clear();
        }
    }
}