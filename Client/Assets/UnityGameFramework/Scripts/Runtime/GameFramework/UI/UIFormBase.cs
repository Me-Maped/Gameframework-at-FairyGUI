﻿using System;
using System.Collections.Generic;
using FairyGUI;
using GameFramework.Event;
using GameFramework.Localization;

namespace GameFramework.UI
{
    public abstract class UIFormBase : UIFormCore
    {
        public bool InHideMode => m_State == UIFormState.HIDE_START || m_State == UIFormState.HIDE_OVER;
        public bool IsWaitingForData { get; set; }
        public object UserData { get; set; }
        public override GComponent Instance { get; set; }
        public bool IsOpened => m_State == UIFormState.SHOW_START || m_State == UIFormState.SHOW_OVER;

        protected List<UIFormPartBase> FormParts => m_FormParts;

        private UIFormConfig m_Config;
        private UIFormState m_State;
        private List<UIFormPartBase> m_FormParts;
        private bool m_IsDataReady;
        private UIFormBg m_FormBg;
        private Transition m_InAnimation;
        private Transition m_OutAnimation;
        private Action m_OpenCompleteCallback;
        private Action m_CloseCompleteCallback;
        private Action m_FormFinishCallback;

        private Dictionary<GTextField,UIL10NEventArgs> m_L10NEventArgs;

        public override UIFormConfig Config => m_Config ??= CreateConfig();

        protected UIFormBase()
        {
            m_Config = null;
            m_FormParts = null;
            m_IsDataReady = false;
            m_FormBg = null;
            m_InAnimation = null;
            m_OutAnimation = null;
            m_OpenCompleteCallback = null;
            m_CloseCompleteCallback = null;
            m_FormFinishCallback = null;
            m_L10NEventArgs = new Dictionary<GTextField, UIL10NEventArgs>();
        }

        protected abstract UIFormConfig CreateConfig();
        protected abstract void SerializeChild();

        /// <summary>
        /// 打开界面
        /// </summary>
        /// <exception cref="GameFrameworkException"></exception>
        internal void InternalOpen()
        {
            if (Config == null) throw new GameFrameworkException("Config is null,you must override Config property");
            if (m_State == UIFormState.SHOW_OVER)
            {
                if (Define.PkgArg.Debug)
                {
                    OnReopen();
                }
                else
                {
                    try
                    {
                        OnReopen();
                    }
                    catch (Exception e)
                    {
                        throw new GameFrameworkException(e.Message);
                    }
                }
                return;
            }
            m_State = UIFormState.SHOW_START;
            if(Instance == null) CreateInstance();
            IsWaitingForData = true;
            SerializeChild();
            OnRequestData();
        }

        /// <summary>
        /// 关闭界面
        /// </summary>
        internal void CloseOffline()
        {
            if (Instance == null) return;
            GameFrameworkEntry.GetModule<IUIManager>().RemoveFromStage(this);
            m_IsDataReady = false;
            m_State = UIFormState.HIDE_OVER;
        }

        /// <summary>
        /// 关闭界面
        /// </summary>
        internal void InternalClose()
        {
            // 不可重复关闭
            if (InHideMode) return;
            m_State = UIFormState.HIDE_START;
            if (Define.PkgArg.Debug)
            {
                OnClose();
                OnPartClose();
                OnMVCClose();
            }
            else
            {
                try
                {
                    OnClose();
                    OnPartClose();
                    OnMVCClose();
                }
                catch (Exception e)
                {
                    throw new GameFrameworkException(e.Message);
                }
            }
            OnPlayCloseAnimation();
        }

        /// <summary>
        /// 立即关闭界面
        /// </summary>
        internal void InternalCloseImmediately()
        {
            m_InAnimation?.Stop();
            m_InAnimation = null;
            m_FormBg?.CloseImmediately();
            if (InHideMode) return;
            m_State = UIFormState.HIDE_START;
            if (Define.PkgArg.Debug)
            {
                OnClose();
                OnPartClose();
                OnMVCClose();
            }
            else
            {
                try
                {
                    OnClose();
                    OnPartClose();
                    OnMVCClose();
                }
                catch (Exception e)
                {
                    throw new GameFrameworkException(e.Message);
                }
            }
            CloseComplete(null);
        }

        /// <summary>
        /// 添加一个背景
        /// </summary>
        internal void AddFormBg()
        {
            if (string.IsNullOrEmpty(Config.BgUrl)) return;
            if (m_FormBg == null)
            {
                m_FormBg = new UIFormBg
                {
                    Key = Config.InstName
                };
                Instance.AddChildAt(m_FormBg.Loader, 0);
                m_FormBg.Loader.displayObject.gameObject.transform.SetAsFirstSibling();
            }
            m_FormBg.SetVisible(true);
            m_FormBg.Load(Config.BgUrl, Config.InstName);
        }

        /// <summary>
        /// 添加打开完成的事件
        /// </summary>
        /// <param name="callback"></param>
        public void PushOpenCompleteCallback(Action callback)
        {
            m_OpenCompleteCallback -= callback;
            m_OpenCompleteCallback += callback;
        }

        /// <summary>
        /// 添加完全关闭的事件
        /// </summary>
        /// <param name="callback"></param>
        public void PushCloseCompleteCallback(Action callback)
        {
            m_CloseCompleteCallback -= callback;
            m_CloseCompleteCallback += callback;
        }

        /// <summary>
        /// 设置界面是否可见
        /// </summary>
        /// <param name="visible"></param>
        public void SetVisible(bool visible)
        {
            if (Instance == null)
                throw new GameFrameworkException("Instance is null,you must override Instance property");
            Instance.visible = visible;
            m_FormBg?.SetVisible(visible);
            OnVisibleChange(visible);
        }
        
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <exception cref="GameFrameworkException"></exception>
        private void CreateInstance()
        {
            Instance = UIPackage.CreateObject(Config.PkgName, Config.ResName).asCom;
            if (Instance == null || Instance.isDisposed) throw new GameFrameworkException("Can not create Instance, maybe the pkg has been removed");
            SerializeChild();
            Instance.fairyBatching = true;
            Instance.name = Config.InstName;
            Instance.displayObject.name = Config.InstName;
            InternalRegisterCompEvents(Instance);
            if (Define.PkgArg.Debug)
            {
                OnMVCInit();
                OnInit();
            }
            else
            {
                try
                {
                    OnMVCInit();
                    OnInit();
                }
                catch (Exception e)
                {
                    throw new GameFrameworkException(e.Message);
                }
            }
            InitPart();
        }

        /// <summary>
        /// 注册组件事件
        /// </summary>
        /// <param name="comp"></param>
        private void InternalRegisterCompEvents(GComponent comp)
        {
            GObject[] children = comp.GetChildren();
            foreach (var child in children)
            {
                if (child.data == null)
                {
                    if (child is GComponent childComp)
                    {
                        InternalRegisterCompEvents(childComp);
                    }
                    continue;
                }

                RegisterComponentEvent(child, child.data.ToString().Replace(" ", "").Split('|'));
            }
        }

        /// <summary>
        /// 注册组件事件（根据fairy中自定义数据进行绑定，自定义数据用‘|’分割，暂时没有做冲突处理）
        /// </summary>
        /// <param name="target"></param>
        /// <param name="actions"></param>
        protected virtual void RegisterComponentEvent(GObject target, string[] actions)
        {
            foreach (string action in actions)
            {
                if (action.Equals(UIFormActionConst.RETURN))
                {
                    AddClick(target, SelfGoReturn);
                }
                if (action.Equals(UIFormActionConst.HOME))
                {
                    AddClick(target, SelfGoHome);
                }
                if (action.Equals(UIFormActionConst.CLOSE))
                {
                    AddClick(target, SelfClose);
                }
                if (action.Equals(UIFormActionConst.BACKGROUND))
                {
                    target.MakeFullScreen();
                    target.Center();
                }
                if (action.Equals(UIFormActionConst.L10N) && target is GTextField textField)
                {
                    RegisterL10NEvent(textField,textField.text);
                }
            }
        }

        /// <summary>
        /// 注册多语言
        /// </summary>
        /// <param name="target">文本组件</param>
        /// <param name="l10NKey">多语言ID</param>
        protected virtual void RegisterL10NEvent(GTextField target, string l10NKey)
        {
            if (m_L10NEventArgs.ContainsKey(target))
            {
                ReferencePool.Release(m_L10NEventArgs[target]);
                m_L10NEventArgs.Remove(target);
            }
            m_L10NEventArgs.Add(target, UIL10NEventArgs.Create(target, l10NKey));
        }

        /// <summary>
        /// 返回上一层
        /// </summary>
        protected virtual void SelfGoReturn()
        {
            GameFrameworkEntry.GetModule<IUIManager>().JumpHelper.Back();
        }

        /// <summary>
        /// 回到主界面
        /// </summary>
        protected virtual void SelfGoHome()
        {
            GameFrameworkEntry.GetModule<IUIManager>().JumpHelper.GoHome();
        }

        /// <summary>
        /// 关闭当前界面
        /// </summary>
        protected virtual void SelfClose()
        {
            GameFrameworkEntry.GetModule<IUIManager>().CloseForm(this);
        }

        /// <summary>
        /// 立即关闭当前界面
        /// </summary>
        protected virtual void SelfCloseImmediately()
        {
            GameFrameworkEntry.GetModule<IUIManager>().CloseFormImmediately(this);
        }

        /// <summary>
        /// 退出游戏
        /// </summary>
        protected virtual void SelfQuitGame()
        {
            UnityGameFramework.Runtime.GameEntry.Shutdown(UnityGameFramework.Runtime.ShutdownType.Quit);
        }

        /// <summary>
        /// 初始化部件
        /// </summary>
        private void InitPart()
        {
            if (m_FormParts != null) return;
            if (Config.FormParts == null || Config.FormParts.Length <= 0) return;
            m_FormParts = new List<UIFormPartBase>();
            foreach (var partType in Config.FormParts)
            {
                var partView = (UIFormPartBase)Activator.CreateInstance(partType);
                partView.Init(this);
                m_FormParts.Add(partView);
            }
        }

        /// <summary>
        /// 检查所有数据并打开界面
        /// </summary>
        private void CheckAndOpen()
        {
            if (!m_IsDataReady) return;
            if (GameFrameworkEntry.GetModule<IUIManager>().GetForm(GetType()) == null) return;
            SetVisible(true);
            Instance.touchable = true;
            GameFrameworkEntry.GetModule<IUIManager>().AddToStage(this);
            if (Define.PkgArg.Debug)
            {
                // Debug模式下直接抛出错误，方便调试
                OnMVCOpen();
                OnOpen();
                OnPartOpen();
            }
            else
            {
                try
                {
                    OnMVCOpen();
                    OnOpen();
                    OnPartOpen();
                }
                catch (Exception e)
                {
                    // 正式环境下的异常处理，可以是关闭界面，或者不做处理
                    throw new GameFrameworkException(e.Message);
                }
            }
            OnPlayOpenAnimation();
        }

        /// <summary>
        /// 调用所有部件的OnOpen方法
        /// </summary>
        private void OnPartOpen()
        {
            if (m_FormParts == null || m_FormParts.Count <= 0) return;
            foreach (var part in m_FormParts)
            {
                part.Open();
            }
        }

        /// <summary>
        /// 调用所有部件的OnOpenComplete方法
        /// </summary>
        private void OnPartOpenComplete()
        {
            if (m_FormParts == null || m_FormParts.Count <= 0) return;
            foreach (var part in m_FormParts)
            {
                part.OpenComplete();
            }
        }

        /// <summary>
        /// 调用所有部件的OnClose方法
        /// </summary>
        private void OnPartClose()
        {
            if (m_FormParts == null || m_FormParts.Count <= 0) return;
            foreach (var part in m_FormParts)
            {
                part.Close();
            }
        }

        /// <summary>
        /// 调用所有部件的OnCloseComplete方法
        /// </summary>
        private void OnPartCloseComplete()
        {
            if (m_FormParts == null || m_FormParts.Count <= 0) return;
            foreach (var part in m_FormParts)
            {
                part.CloseComplete();
            }
        }

        /// <summary>
        /// 销毁部件
        /// </summary>
        private void DestroyParts()
        {
            if (m_FormParts == null || m_FormParts.Count <= 0) return;
            foreach (var part in m_FormParts)
            {
                part.Destroy();
            }
            m_FormParts.Clear();
            m_FormParts = null;
        }

        /// <summary>
        /// 移除多语言更新事件
        /// </summary>
        private void RemoveL10NEvent()
        {
            foreach (KeyValuePair<GTextField,UIL10NEventArgs> kv in m_L10NEventArgs)
            {
                ReferencePool.Release(kv.Value);
            }
            m_L10NEventArgs.Clear();
            m_L10NEventArgs = null;
        }
        
        /// <summary>
        /// In动效播放完成,开始执行打开逻辑
        /// </summary>
        protected void OpenComplete()
        {
            if (Instance.fairyBatching) Instance.InvalidateBatchingState();
            m_InAnimation?.Stop();
            m_FormBg?.Open(-1);
            m_OpenCompleteCallback?.Invoke();
            m_OpenCompleteCallback = null;
            m_State = UIFormState.SHOW_OVER;
            Instance.touchable = true;
            if (Define.PkgArg.Debug)
            {
                OnOpenComplete();
                OnPartOpenComplete();
            }
            else
            {
                try
                {
                    OnOpenComplete();
                    OnPartOpenComplete();
                }
                catch (Exception e)
                {
                    throw new GameFrameworkException(e.Message);
                }
            }
        }

        /// <summary>
        /// Out动效播放完成,开始执行关闭逻辑
        /// </summary>
        /// <param name="o"></param>
        protected void CloseComplete(object o)
        {
            Timers.inst.Remove(CloseComplete);
            m_OutAnimation?.Stop();
            DisposeProtectBtn();
            DisposeEvent();
            DisposeClick();
            GameFrameworkEntry.GetModule<IUIManager>().RemoveFromStage(this);
            m_FormBg?.Close(-1);
            m_IsDataReady = false;
            m_State = UIFormState.HIDE_OVER;
            SetVisible(false);
            Instance.touchable = false;
            m_CloseCompleteCallback?.Invoke();
            m_CloseCompleteCallback = null;
            m_FormFinishCallback?.Invoke();
            m_FormFinishCallback = null;
            if (Define.PkgArg.Debug)
            {
                OnCloseComplete();
                OnPartCloseComplete();
            }
            else
            {
                try
                {
                    OnCloseComplete();
                    OnPartCloseComplete();
                }
                catch (Exception e)
                {
                    throw new GameFrameworkException(e.Message);
                }
            }
        }

        /// <summary>
        /// 周期更新
        /// </summary>
        /// <param name="elapseSeconds"></param>
        /// <param name="realElapseSeconds"></param>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (Define.PkgArg.Debug)
            {
                OnMVCUpdate(elapseSeconds, realElapseSeconds);
                OnUpdate(elapseSeconds, realElapseSeconds);
            }
            else
            {
                try
                {
                    OnMVCUpdate(elapseSeconds, realElapseSeconds);
                    OnUpdate(elapseSeconds, realElapseSeconds);
                }
                catch (Exception e)
                {
                    throw new GameFrameworkException(e.Message);
                }
            }
        }

        private void Destroy()
        {
            RemoveL10NEvent();
            Dispose();
            DestroyParts();
            m_FormBg?.Dispose();
            m_FormBg = null;
            Instance = null;
            m_IsDataReady = false;
            ReferencePool.Release(m_Config);
            m_Config = null;
            OnDestroyed();
            OnMVCDestroyed();
        }

        public override void Wait(Action callback)
        {
            m_FormFinishCallback -= callback;
            m_FormFinishCallback += callback;
        }

        public override void Shutdown()
        {
            Destroy();
            m_Config = null;
            m_FormParts = null;
            m_IsDataReady = false;
            m_FormBg = null;
            m_InAnimation = null;
            m_OutAnimation = null;
            m_OpenCompleteCallback = null;
            m_CloseCompleteCallback = null;
            m_FormFinishCallback = null;
        }

        public override void Clear()
        {
            Destroy();
            m_Config = null;
            m_FormParts = null;
            m_IsDataReady = false;
            m_FormBg = null;
            m_InAnimation = null;
            m_OutAnimation = null;
            m_OpenCompleteCallback = null;
            m_CloseCompleteCallback = null;
            m_FormFinishCallback = null;
        }

        #region Virtual Methods

        /// <summary>
        /// 初始化
        /// </summary>
        protected virtual void OnInit() { }
        
        /// <summary>
        /// 打开界面
        /// </summary>
        protected virtual void OnOpen() { }
        
        /// <summary>
        /// 界面已经打开后，再次调用打开时执行
        /// </summary>
        protected virtual void OnReopen(){}

        /// <summary>
        /// 打开界面完成
        /// </summary>
        protected virtual void OnOpenComplete() { }

        /// <summary>
        /// 关闭界面
        /// </summary>
        protected virtual void OnClose() { }

        /// <summary>
        /// 关闭界面完成
        /// </summary>
        protected virtual void OnCloseComplete() { }
        
        /// <summary>
        /// 界面可见发生修改
        /// </summary>
        /// <param name="visible"></param>
        protected virtual void OnVisibleChange(bool visible) { }

        /// <summary>
        /// 轮询
        /// </summary>
        /// <param name="elapseSeconds"></param>
        /// <param name="realElapseSeconds"></param>
        protected virtual void OnUpdate(float elapseSeconds, float realElapseSeconds) { }
        
        /// <summary>
        /// 销毁
        /// </summary>
        protected  virtual void OnDestroyed() { }

        /// <summary>
        /// 请求数据
        /// </summary>
        protected virtual void OnRequestData()
        {
            OnRequestDataEnd();
        }

        /// <summary>
        /// 请求数据结束
        /// </summary>
        protected virtual void OnRequestDataEnd()
        {
            IsWaitingForData = false;
            m_IsDataReady = true;
            CheckAndOpen();
        }

        /// <summary>
        /// 播放打开动画
        /// </summary>
        protected virtual void OnPlayOpenAnimation()
        {
            m_OutAnimation?.Stop();
            m_OutAnimation = null;
            m_InAnimation = Instance.GetTransition("In");
            if (m_InAnimation == null)
            {
                OpenComplete();
                return;
            }
            m_InAnimation.Play(OpenComplete);
        }

        /// <summary>
        /// 播放关闭动画
        /// </summary>
        protected virtual void OnPlayCloseAnimation()
        {
            m_InAnimation?.Stop();
            m_InAnimation = null;
            m_OutAnimation = Instance.GetTransition("Out");
            if (m_OutAnimation == null)
            {
                CloseComplete(null);
                return;
            }
            m_OutAnimation.Play(() => { CloseComplete(null); });
            //加一层保护，避免Out动画异常
            float constTime = (m_OutAnimation.totalDuration + 0.1f) * 1.5f;
            Timers.inst.Remove(CloseComplete);
            Timers.inst.Add(constTime, 1, CloseComplete);
        }
        #endregion

        #region MVC 可选

        private IUIModel m_UIModel;
        private IUIController m_UICtrl;

        protected virtual IUIController GetUICtrl()
        {
            return null;
        }

        protected virtual IUIModel GetUIModel()
        {
            return null;
        }
        
        /// <summary>
        /// MVC初始化
        /// </summary>
        private void OnMVCInit()
        {
            m_UICtrl = GetUICtrl();
            m_UIModel = GetUIModel();
            m_UIModel?.Init();
            m_UICtrl?.Init(this, m_UIModel);
        }
        
        /// <summary>
        /// MVC打开
        /// </summary>
        private void OnMVCOpen()
        {
            m_UIModel?.Open();
            m_UICtrl?.Open();
        }

        /// <summary>
        /// MVC关闭
        /// </summary>
        private void OnMVCClose()
        {
            m_UIModel?.Close();
            m_UICtrl?.Close();
        }
        /// <summary>
        /// MVC轮询
        /// </summary>
        /// <param name="elapseSeconds"></param>
        /// <param name="realElapseSeconds"></param>
        private void OnMVCUpdate(float elapseSeconds, float realElapseSeconds)
        {
            m_UIModel?.Update(elapseSeconds, realElapseSeconds);
            m_UICtrl?.Update(elapseSeconds, realElapseSeconds);
        }
        /// <summary>
        /// 销毁MVC
        /// </summary>
        private void OnMVCDestroyed()
        {
            if (m_UICtrl != null)
            {
                ReferencePool.Release(m_UICtrl);
                m_UICtrl = null;
            }
            // model为持久化类型，不销毁，只释放引用
            m_UIModel = null;
        }
        #endregion
    }
}