using System;
using System.Collections.Generic;
using FairyGUI;

namespace GameFramework.UI
{
    public abstract class UIFormBase : UIFormCore
    {
        public bool InHideMode => m_State == UIFormState.HIDE_START || m_State == UIFormState.HIDE_OVER;
        public bool IsWaitingForData { get; set; }
        public object UserData { get; set; }
        public override GComponent Instance { get; set; }
        
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

        public override UIFormConfig Config => m_Config ??= CreateConfig();

        public UIFormBase()
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
        }

        protected abstract UIFormConfig CreateConfig();
        protected abstract void SerializeChild();

        /// <summary>
        /// 打开界面
        /// </summary>
        /// <exception cref="GameFrameworkException"></exception>
        internal void Open()
        {
            if (Config == null) throw new GameFrameworkException("Config is null,you must override Config property");
            m_State = UIFormState.SHOW_START;
            CreateInstance();
            IsWaitingForData = true;
            OnRequestData();
        }

        /// <summary>
        /// 设置界面是否可见
        /// </summary>
        /// <param name="visible"></param>
        internal void SetVisible(bool visible)
        {
            if (Instance == null)
                throw new GameFrameworkException("Instance is null,you must override Instance property");
            Instance.visible = visible;
            m_FormBg?.SetVisible(visible);
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
        internal void Close()
        {
            // 不可重复关闭
            if (InHideMode) return;
            m_State = UIFormState.HIDE_START;
            try
            {
                OnClose();
                OnPartClose();
            }
            catch (Exception e)
            {
                throw new GameFrameworkException(e.Message);
            }
            OnPlayCloseAnimation();
        }

        /// <summary>
        /// 立即关闭界面
        /// </summary>
        internal void CloseImmediately()
        {
            m_InAnimation?.Stop();
            m_InAnimation = null;
            m_FormBg?.CloseImmediately();
            if (InHideMode) return;
            m_State = UIFormState.HIDE_START;
            try
            {
                OnClose();
                OnPartClose();
            }
            catch (Exception e)
            {
                throw new GameFrameworkException(e.Message);
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
                GRoot.inst.AddChild(m_FormBg.Loader);
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
        /// 创建实例
        /// </summary>
        /// <exception cref="GameFrameworkException"></exception>
        private void CreateInstance()
        {
            Instance ??= UIPackage.CreateObject(Config.PkgName, Config.ResName).asCom;
            if (Instance==null||Instance.isDisposed) throw new GameFrameworkException("Can not create Instance, maybe the pkg has been removed");
            SerializeChild();
            Instance.fairyBatching = true;
            Instance.name = Config.InstName;
            Instance.displayObject.name = Config.InstName;
            RegisterCompEvents(Instance);
            OnInit();
            InitPart();
        }

        /// <summary>
        /// 注册组件事件
        /// </summary>
        /// <param name="comp"></param>
        private void RegisterCompEvents(GComponent comp)
        {
            GObject[] children = comp.GetChildren();
            foreach (var child in children)
            {
                if (child.data == null)
                {
                    if (child is GComponent childComp)
                    {
                        RegisterCompEvents(childComp);
                    }
                    continue;
                }

                var infos = child.data.ToString().Split('|');
                var action = infos[0];
                switch (action)
                {
                    case UIFormActionConst.RETURN:
                        child.onClick.Add(GoReturn);
                        break;
                    case UIFormActionConst.HOME:
                        child.onClick.Add(GoHome);
                        break;
                    case UIFormActionConst.CLOSE:
                        child.onClick.Add(CloseForm);
                        break;
                }
            }
        }

        /// <summary>
        /// 返回上一层
        /// </summary>
        protected void GoReturn()
        {
            GameFrameworkEntry.GetModule<IUIManager>().Back();
        }

        /// <summary>
        /// 回到主界面
        /// </summary>
        protected void GoHome()
        {
            GameFrameworkEntry.GetModule<IUIManager>().GoHome();
        }

        /// <summary>
        /// 关闭当前界面
        /// </summary>
        private void CloseForm()
        {
            GameFrameworkEntry.GetModule<IUIManager>().CloseForm(this);
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
            Instance.visible = true;
            Instance.touchable = true;
            GameFrameworkEntry.GetModule<IUIManager>().AddToStage(this);
            try
            {
                OnOpen();
                OnPartOpen();
            }
            catch (Exception e)
            {
                throw new GameFrameworkException(e.Message);
            }
            OnPlayOpenAnimation();
        }

        /// <summary>
        /// 调用所有部件的OnOpen方法
        /// </summary>
        private void OnPartOpen()
        {
            if(m_FormParts == null || m_FormParts.Count <= 0) return;
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
            if(m_FormParts == null || m_FormParts.Count <= 0) return;
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
            if(m_FormParts == null || m_FormParts.Count <= 0) return;
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
            if(m_FormParts == null || m_FormParts.Count <= 0) return;
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
            if(m_FormParts == null || m_FormParts.Count <= 0) return;
            foreach (var part in m_FormParts)
            {
                part.Destroy();
            }
            m_FormParts.Clear();
            m_FormParts = null;
        }

        /// <summary>
        /// In动效播放完成,开始执行打开逻辑
        /// </summary>
        protected void OpenComplete()
        {
            if(Instance.fairyBatching) Instance.InvalidateBatchingState();
            m_InAnimation?.Stop();
            m_FormBg?.Open(-1);
            m_OpenCompleteCallback?.Invoke();
            m_OpenCompleteCallback = null;
            m_State = UIFormState.SHOW_OVER;
            Instance.touchable = true;
            OnOpenComplete();
            OnPartOpenComplete();
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
            Instance.visible = false;
            Instance.touchable = false;
            m_CloseCompleteCallback?.Invoke();
            m_CloseCompleteCallback = null;
            m_FormFinishCallback?.Invoke();
            m_FormFinishCallback = null;
            OnCloseComplete();
            OnPartCloseComplete();
        }

        internal void Destroy()
        {
            Dispose();
            DestroyParts();
            m_FormBg?.Dispose();
            m_FormBg = null;
            Instance = null;
            m_IsDataReady = false;
            ReferencePool.Release(m_Config);
            m_Config = null;
        }

        public override void Wait(Action callback)
        {
            m_FormFinishCallback -= callback;
            m_FormFinishCallback += callback;
        }

        public override void Shutdown()
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
        /// 重新打开界面, UserData数据变化
        /// </summary>
        public virtual void Reopen() { }

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
            m_OutAnimation.Play(()=>{CloseComplete(null);});
            //加一层保护，避免Out动画异常
            float constTime = (m_OutAnimation.totalDuration + 0.1f) * 1.5f;
            Timers.inst.Remove(CloseComplete);
            Timers.inst.Add(constTime, 1, CloseComplete);
        }
        #endregion
    }
}