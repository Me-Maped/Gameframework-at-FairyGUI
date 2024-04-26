using System;
using GameFramework;
using GameFramework.ObjectPool;
using GameFramework.Resource;
using GameFramework.UI;
using FairyGUI;
using UnityEngine;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    /// 界面组件。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/UI")]
    public sealed partial class UIComponent : GameFrameworkComponent
    {
        private IUIManager m_UIManager = null;
        private EventComponent m_EventComponent = null;
        [SerializeField]
        private bool m_EnableOpenUIFormSuccessEvent = true;
        [SerializeField]
        private bool m_EnableOpenUIFormFailureEvent = true;
        [SerializeField]
        private bool m_EnableOpenUIFormUpdateEvent = false;
        [SerializeField]
        private bool m_EnableOpenUIFormDependencyAssetEvent = false;
        [SerializeField]
        private bool m_EnableCloseUIFormCompleteEvent = true;
        [SerializeField]
        private float m_InstanceAutoReleaseInterval = 60f;
        [SerializeField]
        private int m_InstanceCapacity = 16;
        [SerializeField]
        private float m_InstanceExpireTime = 60f;
        [SerializeField]
        private int m_InstancePriority = 0;
        [SerializeField]
        private Transform m_InstanceRoot = null;
        
        [SerializeField]
        private string m_UIFitHelperTypeName = "UnityGameFramework.Runtime.DefaultUIFitHelper";
        [SerializeField]
        private UIFitHelperBase m_CustomUIFitHelper = null;
        [SerializeField]
        private string m_UIJumpHelperTypeName = "UnityGameFramework.Runtime.DefaultUIJumpHelper";
        [SerializeField]
        private UIJumpHelperBase m_CustomUIJumpHelper = null;
        
        [SerializeField]
        private string m_UICameraHelperTypeName = "UnityGameFramework.Runtime.DefaultUICameraHelper";
        [SerializeField]
        private UICameraHelperBase m_CustomUICameraHelper = null;
        
        [SerializeField]
        private string m_LaunchTipsSettingName = "Settings/LaunchTipsSettings";
        
        [SerializeField]
        private string[] m_LaunchPkgNames = {"UI/Launch"};
        
        private Vector2 m_SafeNotch=Vector2.zero;
        private UIFitHelperBase m_FitHelperBase;
        private UIJumpHelperBase m_JumpHelperBase;
        private UICameraHelperBase m_CameraHelperBase;

        /// <summary>
        /// 异形屏顶部的安全区域
        /// </summary>
        public Vector2 SafeNotch => m_SafeNotch;

        /// <summary>
        /// 获取界面组数量。
        /// </summary>
        public int UIGroupCount => m_UIManager.UIGroupCount;
        
        /// <summary>
        /// 启动提示资源名称（Resources目录下）
        /// </summary>
        public string LaunchTipsSettingName => m_LaunchTipsSettingName;
        
        /// <summary>
        /// 启动界面包名（Resources目录下）
        /// </summary>
        public string[] LaunchPkgNames => m_LaunchPkgNames;

        /// <summary>
        /// 获取或设置界面实例对象池自动释放可释放对象的间隔秒数。
        /// </summary>
        public float InstanceAutoReleaseInterval
        {
            get => m_UIManager.InstanceAutoReleaseInterval;
            set => m_UIManager.InstanceAutoReleaseInterval = m_InstanceAutoReleaseInterval = value;
        }

        /// <summary>
        /// 获取或设置界面实例对象池的容量。
        /// </summary>
        public int InstanceCapacity
        {
            get => m_UIManager.InstanceCapacity;
            set => m_UIManager.InstanceCapacity = m_InstanceCapacity = value;
        }

        /// <summary>
        /// 获取或设置界面实例对象池对象过期秒数。
        /// </summary>
        public float InstanceExpireTime
        {
            get => m_UIManager.InstanceExpireTime;
            set => m_UIManager.InstanceExpireTime = m_InstanceExpireTime = value;
        }

        /// <summary>
        /// 获取或设置界面实例对象池的优先级。
        /// </summary>
        public int InstancePriority
        {
            get => m_UIManager.InstancePriority;
            set => m_UIManager.InstancePriority = m_InstancePriority = value;
        }

        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            m_UIManager = GameFrameworkEntry.GetModule<IUIManager>();
            if (m_UIManager == null)
            {
                Log.Fatal("UI manager is invalid.");
                return;
            }

            if (m_EnableOpenUIFormSuccessEvent) m_UIManager.LoadFormSuccess += OnOpenUIFormSuccess;
            if(m_EnableOpenUIFormFailureEvent) m_UIManager.LoadFormFailure += OnOpenUIFormFailure;
            if (m_EnableOpenUIFormUpdateEvent) m_UIManager.LoadFormUpdate += OnOpenUIFormUpdate;
            if (m_EnableOpenUIFormDependencyAssetEvent) m_UIManager.LoadFormDependency += OnOpenUIFormDependencyAsset;
            if (m_EnableCloseUIFormCompleteEvent) m_UIManager.CloseFormComplete += OnCloseUIFormComplete;
        }

        private void Start()
        {
            m_EventComponent = GameEntry.GetComponent<EventComponent>();
            if (m_EventComponent == null)
            {
                Log.Fatal("Event component is invalid.");
                return;
            }

            m_UIManager.SetResourceManager(GameFrameworkEntry.GetModule<IResourceManager>());
            m_UIManager.SetObjectPoolManager(GameFrameworkEntry.GetModule<IObjectPoolManager>());
            
            m_UIManager.InstanceAutoReleaseInterval = m_InstanceAutoReleaseInterval;
            m_UIManager.InstanceCapacity = m_InstanceCapacity;
            m_UIManager.InstanceExpireTime = m_InstanceExpireTime;
            m_UIManager.InstancePriority = m_InstancePriority;
            CreateHelper();
            InitSettings();
            if (m_InstanceRoot == null)
            {
                m_InstanceRoot = new GameObject("UI Form Instances").transform;
                m_InstanceRoot.SetParent(gameObject.transform);
                m_InstanceRoot.localScale = Vector3.one;
            }
        }

        /// <summary>
        /// 创建界面相关的管理器
        /// </summary>
        private void CreateHelper()
        {
            m_FitHelperBase = Helper.CreateHelper(m_UIFitHelperTypeName, m_CustomUIFitHelper);
            if (m_FitHelperBase != null)
            {
                m_UIManager.SetUIFitHelper(m_FitHelperBase);
                m_FitHelperBase.name = "UI Fit Helper";
                Transform formHelperTrans = m_FitHelperBase.transform;
                formHelperTrans.SetParent(this.transform);
                formHelperTrans.localScale = Vector3.one;
            }
            else Log.Error("Can not create UI fit helper.");
            
            m_JumpHelperBase = Helper.CreateHelper(m_UIJumpHelperTypeName, m_CustomUIJumpHelper, UIGroupCount);
            if (m_JumpHelperBase != null)
            {
                m_UIManager.SetUIJumpHelper(m_JumpHelperBase);
                m_JumpHelperBase.name = "UI Jump Helper";
                Transform groupHelperTrans = m_JumpHelperBase.transform;
                groupHelperTrans.SetParent(this.transform);
                groupHelperTrans.localScale = Vector3.one;
            }
            else Log.Error("Can not create UI jump helper.");
            
            m_CameraHelperBase = Helper.CreateHelper(m_UICameraHelperTypeName, m_CustomUICameraHelper, UIGroupCount);
            if (m_CameraHelperBase != null)
            {
                m_UIManager.SetUICameraHelper(m_CameraHelperBase);
                m_CameraHelperBase.name = "UI Camera Helper";
                Transform groupHelperTrans = m_CameraHelperBase.transform;
                groupHelperTrans.SetParent(this.transform);
                groupHelperTrans.localScale = Vector3.one;
            }
            else Log.Error("Can not create UI camera helper.");
        }

        /// <summary>
        /// 初始化FairyGUI相关设置
        /// </summary>
        private void InitSettings()
        {
            //TODO 获取屏幕安全区域
            m_SafeNotch = new Vector2(Screen.safeArea.x, Screen.safeArea.y);
            // 设置屏幕适配
            GRoot.inst.ApplyContentScaleFactor();
            GRoot.inst.SetContentScaleFactor(SettingsUtils.FrameworkGlobalSettings.UIWidth,
                SettingsUtils.FrameworkGlobalSettings.UIHeight, UIContentScaler.ScreenMatchMode.MatchWidthOrHeight);
            //手动管理资源
            UIPackage.unloadBundleByFGUI = false;
            // UI根名字重命名
            GRoot.inst.displayObject.parent.gameObject.name = "FairyGUI";
            GRoot.inst.displayObject.gameObject.name = "UIRoot";
            //字体包
            UIConfig.defaultFont = "FZJUZXFJW";
            //UI相机设置
            m_CameraHelperBase.InitCamera();
        }
        #region 接口层
        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="formType"></param>
        /// <param name="closeOther"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        public UIFormBase OpenForm(System.Type formType, bool closeOther = false, object userData = null)
        {
            return m_UIManager.OpenForm(formType, closeOther, userData);
        }

        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="closeOther"></param>
        /// <param name="userData"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T OpenForm<T>(bool closeOther = false,object userData = null) where T : UIFormBase
        {
            return (T)m_UIManager.OpenForm<T>(closeOther,userData);
        }
        
        /// <summary>
        /// 根据类型获取界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetForm<T>() where T : UIFormBase
        {
            return (T)m_UIManager.GetForm<T>();
        }
        
        /// <summary>
        /// 根据类型获取界面
        /// </summary>
        /// <param name="uiFormType"></param>
        /// <returns></returns>
        public UIFormBase GetForm(System.Type uiFormType)
        {
            return m_UIManager.GetForm(uiFormType);
        }
        
        /// <summary>
        /// 根据名称获取界面
        /// </summary>
        /// <param name="uiFormName"></param>
        /// <returns></returns>
        public UIFormBase GetFormByName(string uiFormName)
        {
            return m_UIManager.GetFormByName(uiFormName);
        }

        /// <summary>
        /// 获取顶层界面
        /// </summary>
        /// <returns></returns>
        public UIFormBase GetTopForm()
        {
            return m_UIManager.GetTopForm();
        }

        /// <summary>
        /// 获取顶层界面
        /// </summary>
        /// <param name="groupEnum"></param>
        /// <returns></returns>
        public UIFormBase GetTopForm(UIGroupEnum groupEnum)
        {
            return m_UIManager.GetTopForm(groupEnum);
        }

        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <param name="uiForm"></param>
        public void CloseForm(UIFormBase uiForm)
        {
            m_UIManager.CloseForm(uiForm);
        }

        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void CloseForm<T>() where T : UIFormBase
        {
            m_UIManager.CloseForm<T>();
        }
        
        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <param name="uiForm"></param>
        public void CloseForm(Type uiForm)
        {
            m_UIManager.CloseForm(uiForm);
        }

        /// <summary>
        /// 关闭所有界面
        /// </summary>
        public void CloseAllForm()
        {
            m_UIManager.CloseAllForm();
        }
        
        /// <summary>
        /// 立即关闭界面
        /// </summary>
        /// <param name="uiForm"></param>
        public void CloseFormImmediately(UIFormBase uiForm)
        {
            m_UIManager.CloseFormImmediately(uiForm);
        }
        
        /// <summary>
        /// 立即关闭界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void CloseFormImmediately<T>() where T : UIFormBase
        {
            m_UIManager.CloseFormImmediately<T>();
        }
        
        /// <summary>
        /// 关闭某一组的界面
        /// </summary>
        /// <param name="groupEnum"></param>
        public void CloseFormByGroup(UIGroupEnum groupEnum)
        {
            m_UIManager.CloseFormByGroup(groupEnum);
        }
        
        /// <summary>
        /// 存在某个界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool HasForm<T>() where T : UIFormBase
        {
            return m_UIManager.HasForm<T>();
        }
        
        /// <summary>
        /// 存在某个界面
        /// </summary>
        /// <param name="uiFormType"></param>
        /// <returns></returns>
        public bool HasForm(System.Type uiFormType)
        {
            return m_UIManager.HasForm(uiFormType);
        }
        
        /// <summary>
        /// 获取界面组
        /// </summary>
        /// <param name="groupEnum"></param>
        /// <returns></returns>
        public IUIGroup GetUIGroup(UIGroupEnum groupEnum)
        {
            return m_UIManager.GetUIGroup(groupEnum);
        }
        
        /// <summary>
        /// 增加界面组
        /// </summary>
        /// <param name="groupEnum"></param>
        /// <returns></returns>
        public IUIGroup AddUIGroup(UIGroupEnum groupEnum)
        {
            return m_UIManager.AddUIGroup(groupEnum);
        }
        
        /// <summary>
        /// 增加界面到舞台
        /// </summary>
        /// <param name="uiForm"></param>
        public void AddToStage(UIFormBase uiForm)
        {
            m_UIManager.AddToStage(uiForm);
        }
        
        /// <summary>
        /// 从舞台移除界面
        /// </summary>
        /// <param name="uiForm"></param>
        public void RemoveFromStage(UIFormBase uiForm)
        {
            m_UIManager.RemoveFromStage(uiForm);
        }

        /// <summary>
        /// 设置界面实例是否加锁
        /// </summary>
        /// <param name="uiFormInstance"></param>
        /// <param name="locked"></param>
        public void SetUIFormInstanceLocked(object uiFormInstance, bool locked)
        {
            m_UIManager.SetUIFormInstanceLocked(uiFormInstance,locked);
        }

        /// <summary>
        /// 设置界面实例的优先级
        /// </summary>
        /// <param name="uiFormInstance"></param>
        /// <param name="priority"></param>
        public void SetUIFormInstancePriority(object uiFormInstance, int priority)
        {
            m_UIManager.SetUIFormInstancePriority(uiFormInstance, priority);
        }

        /// <summary>
        /// 返回
        /// </summary>
        public void Back()
        {
            m_UIManager.Back();
        }

        /// <summary>
        /// 回到主页
        /// </summary>
        public void GoHome()
        {
            m_UIManager.GoHome();
        }

        /// <summary>
        /// 将UI相机附加到某个摄像机
        /// </summary>
        /// <param name="targetCamera"></param>
        public void UICameraAttach(Camera targetCamera)
        {
            m_UIManager.UICameraAttach(targetCamera);
        }

        #endregion
        #region Event
        private void OnOpenUIFormSuccess(object sender, LoadFormSuccessEventArgs e)
        {
            m_EventComponent.Fire(this, OpenUIFormSuccessEventArgs.Create(e));
        }

        private void OnOpenUIFormFailure(object sender, LoadFormFailureEventArgs e)
        {
            Log.Warning("Open UI form failure, asset name '{0}', UI group name '{1}', error message '{2}'.", e.UIFormAssetName, e.GroupEnum, e.ErrorMessage);
            if (m_EnableOpenUIFormFailureEvent)
            {
                m_EventComponent.Fire(this, OpenUIFormFailureEventArgs.Create(e));
            }
        }

        private void OnOpenUIFormUpdate(object sender, LoadFormUpdateEventArgs e)
        {
            m_EventComponent.Fire(this, OpenUIFormUpdateEventArgs.Create(e));
        }

        private void OnOpenUIFormDependencyAsset(object sender, LoadFormDependencyEventArgs e)
        {
            m_EventComponent.Fire(this, OpenUIFormDependencyAssetEventArgs.Create(e));
        }

        private void OnCloseUIFormComplete(object sender, CloseFormCompleteEventArgs e)
        {
            m_EventComponent.Fire(this, CloseUIFormCompleteEventArgs.Create(e));
        }
        #endregion
    }
}