using System;
using System.Collections.Generic;
using System.Linq;
using FairyGUI;
using GameFramework.ObjectPool;
using GameFramework.Resource;
using UnityEngine;

namespace GameFramework.UI
{
    internal sealed partial class UIManager : GameFrameworkModule, IUIManager
    {
        public float InstanceAutoReleaseInterval
        {
            get => m_InstancePool.AutoReleaseInterval;
            set => m_InstancePool.AutoReleaseInterval = value;
        }
        public int InstanceCapacity
        {
            get => m_InstancePool.Capacity;
            set => m_InstancePool.Capacity = value;
        }
        public float InstanceExpireTime
        {
            get => m_InstancePool.ExpireTime;
            set => m_InstancePool.ExpireTime = value;
        }
        public int InstancePriority
        {
            get => m_InstancePool.Priority;
            set => m_InstancePool.Priority = value;
        }
        public int UIGroupCount => m_UIGroups.Count;
        public event EventHandler<LoadFormSuccessEventArgs> LoadFormSuccess;
        public event EventHandler<LoadFormFailureEventArgs> LoadFormFailure;
        public event EventHandler<LoadFormUpdateEventArgs> LoadFormUpdate;
        public event EventHandler<LoadFormDependencyEventArgs> LoadFormDependency;
        public event EventHandler<CloseFormCompleteEventArgs> CloseFormComplete;

        private readonly LoadAssetCallbacks m_LoadAssetCallbacks;
        private readonly Dictionary<UIGroupEnum, IUIGroup> m_UIGroups;

        private Dictionary<string, List<UIFormBase>> m_ToBeLoadFormInst;
        private Dictionary<string, int> m_PackageRefCount;
        private List<string> m_LoadingPkgNames;
        private Queue<UIFormBase> m_NeedCloseForms;

        private bool m_IsShutdown;
        private IObjectPoolManager m_ObjectPoolManager;
        private IResourceManager m_ResourceManager;
        private IObjectPool<UIFormInstanceObject> m_InstancePool;
        private IUIFitHelper m_UIFitHelper;
        private IUIJumpHelper m_UIJumpHelper;
        private IUICameraHelper m_UICameraHelper;
        private EventHandler<LoadFormSuccessEventArgs> m_LoadUIFormSuccessEventHandler;
        private EventHandler<LoadFormFailureEventArgs> m_LoadUIFormFailureEventHandler;
        private EventHandler<LoadFormUpdateEventArgs> m_LoadUIFormUpdateEventHandler;
        private EventHandler<LoadFormDependencyEventArgs> m_LoadUIFormDependencyEventHandler;
        private EventHandler<CloseFormCompleteEventArgs> m_CloseUIFormCompleteEventHandler;
        private OnFormInstanceReleaseCall m_OnFormInstanceReleaseCall;

        public UIManager()
        {
            m_LoadAssetCallbacks = new LoadAssetCallbacks(LoadFormSuccessCallback, LoadFormFailureCallback, LoadFormUpdateCallback, LoadFormDependencyCallback);
            m_ObjectPoolManager = null;
            m_ResourceManager = null;
            m_InstancePool = null;
            m_UIFitHelper = null;
            m_UIJumpHelper = null;
            m_UICameraHelper = null;
            m_UIGroups = new Dictionary<UIGroupEnum, IUIGroup>();
            m_ToBeLoadFormInst = new Dictionary<string, List<UIFormBase>>();
            m_PackageRefCount = new Dictionary<string, int>();
            m_LoadingPkgNames = new List<string>();
            m_NeedCloseForms = new Queue<UIFormBase>();
            m_LoadUIFormSuccessEventHandler = null;
            m_LoadUIFormFailureEventHandler = null;
            m_LoadUIFormUpdateEventHandler = null;
            m_LoadUIFormDependencyEventHandler = null;
            m_CloseUIFormCompleteEventHandler = null;
            m_IsShutdown = false;
            m_OnFormInstanceReleaseCall = OnInstanceReleaseCall;
        }

        public void SetObjectPoolManager(IObjectPoolManager objectPoolManager)
        {
            if (objectPoolManager == null) throw new GameFrameworkException("ObjectPoolManager is invalid");
            m_ObjectPoolManager = objectPoolManager;
            m_InstancePool = m_ObjectPoolManager.CreateSingleSpawnObjectPool<UIFormInstanceObject>("UI Instance Pool");
        }

        public void SetResourceManager(IResourceManager resourceManager)
        {
            if (resourceManager == null) throw new GameFrameworkException("ResourceManager is invalid");
            m_ResourceManager = resourceManager;
        }

        public void SetUIFitHelper(IUIFitHelper uiFitHelper)
        {
            if (uiFitHelper == null) throw new GameFrameworkException("UIFitHelper is invalid");
            m_UIFitHelper = uiFitHelper;
        }

        public void SetUIJumpHelper(IUIJumpHelper iuiJumpHelper)
        {
            if (iuiJumpHelper == null) throw new GameFrameworkException("UIJumpHelper is invalid");
            m_UIJumpHelper = iuiJumpHelper;
        }

        public void SetUICameraHelper(IUICameraHelper uiCameraHelper)
        {
            if (uiCameraHelper == null) throw new GameFrameworkException("UICameraHelper is invalid");
            m_UICameraHelper = uiCameraHelper;
        }

        public UIFormBase OpenForm(Type formType, bool closeOther, object userData)
        {
            if (formType == null) throw new GameFrameworkException("FormType is invalid");
            if (m_ResourceManager == null) throw new GameFrameworkException("You must set ResourceManager first");
            UIFormBase uiForm = (UIFormBase)ReferencePool.Acquire(formType);
            uiForm.UserData = userData;
            GetUIGroup(uiForm.Config.GroupEnum).CloseOthers = closeOther;

            if (m_LoadingPkgNames.Contains(uiForm.Config.PkgName))
            {
                if (m_ToBeLoadFormInst.TryGetValue(uiForm.Config.PkgName, out List<UIFormBase> uiForms))
                {
                    uiForms.Add(uiForm);
                }
                else
                {
                    uiForms = new List<UIFormBase> { uiForm };
                    m_ToBeLoadFormInst.Add(uiForm.Config.PkgName, uiForms);
                }
                return uiForm;
            }

            if (UIPackage.GetByName(uiForm.Config.PkgName) != null)
            {
                InternalOpenUIForm(uiForm, 0);
            }
            else
            {
                m_LoadingPkgNames.Add(uiForm.Config.PkgName);
                m_ResourceManager.LoadUIPackagesAsync(uiForm.Config.PkgName, m_LoadAssetCallbacks, uiForm);
            }

            return uiForm;
        }

        public UIFormBase OpenForm<T>(bool closeOther, object userData) where T : UIFormBase
        {
            return OpenForm(typeof(T), closeOther, userData);
        }

        public UIFormBase GetForm<T>() where T : UIFormBase
        {
            return GetForm(typeof(T));
        }

        public UIFormBase GetForm(Type formType)
        {
            UIFormBase uiFormBase = null;
            foreach (var kv in m_UIGroups)
            {
                uiFormBase = kv.Value.GetForm(formType);
                if (uiFormBase != null) break;
            }
            return uiFormBase;
        }

        public UIFormBase GetFormByName(string formName)
        {
            UIFormBase uiFormBase = null;
            foreach (var kv in m_UIGroups)
            {
                uiFormBase = kv.Value.GetForm(formName);
                if (uiFormBase != null) break;
            }
            return uiFormBase;
        }

        public UIFormBase GetTopForm()
        {
            IUIGroup topGroup = null;
            foreach (var uiGroup in m_UIGroups)
            {
                if (uiGroup.Value.UIForms.Count <= 0) continue;
                if (topGroup == null || uiGroup.Value.GroupEnum > topGroup.GroupEnum) topGroup = uiGroup.Value;
            }
            return topGroup?.GetTopForm();
        }

        public UIFormBase GetTopForm(UIGroupEnum groupEnum)
        {
            m_UIGroups.TryGetValue(groupEnum, out IUIGroup uiGroup);
            return uiGroup?.GetTopForm();
        }

        public void CloseForm<T>() where T : UIFormBase
        {
            UIFormBase uiForm = GetForm<T>();
            if (uiForm == null) return;
            CloseForm(uiForm);
        }

        public void CloseForm(Type uiFormType)
        {
            UIFormBase uiForm = GetForm(uiFormType);
            if (uiForm == null) return;
            CloseForm(uiForm);
        }

        public void CloseForm(UIFormBase uiForm)
        {
            if (uiForm == null) return;
            if (uiForm.Instance != null) m_InstancePool.Unspawn(uiForm.Instance);
            GetUIGroup(uiForm.Config.GroupEnum).CloseForm(uiForm);
            if (m_CloseUIFormCompleteEventHandler != null)
            {
                var args = CloseFormCompleteEventArgs.Create(uiForm.Config.ResName, uiForm.Config.GroupEnum);
                m_CloseUIFormCompleteEventHandler(this, args);
                ReferencePool.Release(args);
            }
        }

        public void CloseAllForm()
        {
            foreach (var groupInfo in m_UIGroups)
            {
                groupInfo.Value.CloseAllForm();
            }
        }

        public void CloseFormImmediately<T>() where T : UIFormBase
        {
            UIFormBase uiForm = GetForm<T>();
            if (uiForm == null) return;
            CloseFormImmediately(uiForm);
        }

        public void CloseFormImmediately(UIFormBase uiForm)
        {
            if (uiForm == null) return;
            GetUIGroup(uiForm.Config.GroupEnum).CloseFormImmediately(uiForm);
        }

        public void CloseFormByGroup(UIGroupEnum groupEnum)
        {
            IUIGroup uiGroup = GetUIGroup(groupEnum);
            uiGroup.CloseAllForm();
        }

        public bool HasForm<T>() where T : UIFormBase
        {
            return HasForm(typeof(T));
        }

        public bool HasForm(Type formType)
        {
            bool result = false;
            foreach (var kv in m_UIGroups)
            {
                if (kv.Value.UIForms.Exists(x => x.GetType() == formType)) return true;
            }
            return result;
        }

        public IUIGroup GetUIGroup(UIGroupEnum groupEnum)
        {
            m_UIGroups.TryGetValue(groupEnum, out IUIGroup uiGroup);
            uiGroup ??= AddUIGroup(groupEnum);
            return uiGroup;
        }

        public IUIGroup AddUIGroup(UIGroupEnum groupEnum)
        {
            m_UIGroups.TryGetValue(groupEnum, out IUIGroup uiGroup);
            if (uiGroup != null) return uiGroup;
            uiGroup = new UIGroupDefault(groupEnum);
            m_UIGroups.Add(groupEnum, uiGroup);
            return uiGroup;
        }

        public void AddToStage(UIFormBase uiForm)
        {
            if (uiForm == null) return;
            uiForm.AddFormBg();
            m_UIFitHelper.FitForm(uiForm);
            GetUIGroup(uiForm.Config.GroupEnum).AddToStage();
        }

        public void RemoveFromStage(UIFormBase uiForm)
        {
            if (uiForm == null) return;
            GetUIGroup(uiForm.Config.GroupEnum).RemoveFromStage(uiForm);
        }

        public void SetUIFormInstanceLocked(object uiFormInstance, bool locked)
        {
            if (uiFormInstance == null) throw new GameFrameworkException("UIFormInstance is invalid");
            m_InstancePool.SetLocked(uiFormInstance, locked);
        }

        public void SetUIFormInstancePriority(object uiFormInstance, int priority)
        {
            if (uiFormInstance == null) throw new GameFrameworkException("UIFormInstance is invalid");
            m_InstancePool.SetPriority(uiFormInstance, priority);
        }

        public void Back()
        {
            m_UIJumpHelper.Back();
        }

        public void GoHome()
        {
            m_UIJumpHelper.GoHome();
        }

        public void UICameraAttach(Camera targetCamera)
        {
            m_UICameraHelper.UICameraAttach(targetCamera);
        }

        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
            foreach (var groupInfo in m_UIGroups)
            {
                groupInfo.Value.Update(elapseSeconds, realElapseSeconds);
            }

            while (m_NeedCloseForms.Count > 0)
            {
                UIFormBase uiForm = m_NeedCloseForms.Dequeue();
                CloseForm(uiForm);
            }
        }

        internal override void Shutdown()
        {
            m_IsShutdown = false;
            foreach (var groupInfo in m_UIGroups)
            {
                groupInfo.Value.Shutdown();
            }
            m_UIGroups.Clear();
            m_ToBeLoadFormInst.Clear();
            m_LoadingPkgNames.Clear();
            m_NeedCloseForms.Clear();
            m_PackageRefCount.Clear();
        }

        private void InternalOpenUIForm(UIFormBase uiForm, float duration)
        {
            if (uiForm == null) throw new GameFrameworkException("Cannot create UIForm in UIFormHelper");
            try
            {
                var formInst = m_InstancePool.Spawn(uiForm.Config.ResName);
                if (formInst != null) uiForm.Instance = formInst.Target as GComponent;

                // 记录跳转
                if (uiForm.Config.InBackList) m_UIJumpHelper.Record(uiForm.GetType());
                GetUIGroup(uiForm.Config.GroupEnum).OpenForm(uiForm);
                if (m_LoadUIFormSuccessEventHandler != null)
                {
                    var eventArgs = LoadFormSuccessEventArgs.Create(uiForm, duration);
                    m_LoadUIFormSuccessEventHandler(this, eventArgs);
                    ReferencePool.Release(eventArgs);
                }
                if (formInst == null)
                {
                    m_InstancePool.Register(
                        UIFormInstanceObject.Create(uiForm, m_OnFormInstanceReleaseCall), true);
                }

                if (uiForm.Config.AutoRelease)
                {
                    // ui主包引用
                    OnPackageRefIncrease(uiForm.Config.PkgName);
                    // ui自定义依赖引用
                    OnPackageRefIncrease(uiForm.Config.Depends);
                    // ui依赖包引用
                    OnPackageRefIncrease(GetPackageDependencies(uiForm.Config.PkgName, uiForm.Config.Depends));
                }

            }
            catch (Exception e)
            {
                if (m_LoadUIFormFailureEventHandler != null)
                {
                    var eventArgs = LoadFormFailureEventArgs.Create(uiForm.Config.ResName, uiForm.Config.GroupEnum, e.Message);
                    m_LoadUIFormFailureEventHandler(this, eventArgs);
                    ReferencePool.Release(eventArgs);
                }
                throw;
            }
        }

        private void LoadFormDependencyCallback(string pkgName, string dependencyAssetName, int loadedCount, int totalCount, object userdata)
        {
            UIFormBase uiForm = userdata as UIFormBase;
            if (uiForm == null) throw new GameFrameworkException("LoadFormDependencyCallback form is invalid");
            if (m_LoadUIFormDependencyEventHandler != null)
            {
                var args = LoadFormDependencyEventArgs.Create(uiForm.Config.ResName,
                    uiForm.Config.GroupEnum, dependencyAssetName, loadedCount,
                    totalCount);
                m_LoadUIFormDependencyEventHandler(this, args);
                ReferencePool.Release(args);
            }
        }

        private void LoadFormUpdateCallback(string pkgName, float progress, object userdata)
        {
            UIFormBase uiForm = userdata as UIFormBase;
            if (uiForm == null) throw new GameFrameworkException("LoadFormUpdateCallback form is invalid");
            if (m_LoadUIFormUpdateEventHandler != null)
            {
                var args = LoadFormUpdateEventArgs.Create(uiForm.Config.ResName,
                    uiForm.Config.GroupEnum, progress);
                m_LoadUIFormUpdateEventHandler(this, args);
                ReferencePool.Release(args);
            }
        }

        private void LoadFormFailureCallback(string pkgName, LoadResourceStatus status, string errorMessage, object userdata)
        {
            UIFormBase uiForm = userdata as UIFormBase;
            if (uiForm == null) throw new GameFrameworkException("LoadFormFailureCallback form is invalid");
            m_LoadingPkgNames.Remove(pkgName);
            var resName = uiForm.Config.ResName;
            var message = Utility.Text.Format("LoadUIFormFailure, asset name={0},state={1},errorMsg={2}", resName, status.ToString(), errorMessage);
            if (m_LoadUIFormFailureEventHandler != null)
            {
                var args = LoadFormFailureEventArgs.Create(resName,
                    uiForm.Config.GroupEnum, message);
                m_LoadUIFormFailureEventHandler(this, args);
                ReferencePool.Release(args);
            }
            DisposeToBeLoadForm(pkgName);
            throw new GameFrameworkException(message);
        }

        private void LoadFormSuccessCallback(string pkgName, object asset, float duration, object userdata)
        {
            UIFormBase uiForm = userdata as UIFormBase;
            if (uiForm == null) throw new GameFrameworkException("LoadFormSuccessCallback form is invalid");
            //依赖包加载完成并不能直接打开，需要等待主包加载完成
            if (!pkgName.Equals(uiForm.Config.PkgName)) return;
            m_LoadingPkgNames.Remove(pkgName);
            InternalOpenUIForm(uiForm, duration);
            OpenToBeLoadForm(pkgName, duration);
        }

        private void OpenToBeLoadForm(string pkgName, float duration)
        {
            if (m_ToBeLoadFormInst.Count <= 0) return;
            if (!m_ToBeLoadFormInst.TryGetValue(pkgName, out List<UIFormBase> uiFormList)) return;
            foreach (var uiForm in uiFormList)
            {
                InternalOpenUIForm(uiForm, duration);
            }
            uiFormList.Clear();
            m_ToBeLoadFormInst.Remove(pkgName);
        }

        private void DisposeToBeLoadForm(string pkgName)
        {
            if (m_ToBeLoadFormInst.Count <= 0) return;
            if (!m_ToBeLoadFormInst.TryGetValue(pkgName, out List<UIFormBase> uiFormList)) return;
            foreach (var uiForm in uiFormList)
            {
                uiForm.Dispose();
            }
            uiFormList.Clear();
            m_ToBeLoadFormInst.Remove(pkgName);
        }

        private void OnInstanceReleaseCall(UIFormBase uiForm)
        {
            if (uiForm == null) return;
            GameFrameworkLog.Warning("UIFormHelper OnInstanceReleaseCall type = {0}", uiForm.GetType());
            if (uiForm.Config.AutoRelease)
            {
                OnPackageRefReduce(GetPackageDependencies(uiForm.Config.PkgName, uiForm.Config.Depends));
                OnPackageRefReduce(uiForm.Config.Depends);
                OnPackageRefReduce(uiForm.Config.PkgName);
            }
            ReferencePool.Release(uiForm);
        }

        private void OnPackageRefIncrease(string pkgName)
        {
            if (m_PackageRefCount.ContainsKey(pkgName)) m_PackageRefCount[pkgName]++;
            else m_PackageRefCount[pkgName] = 1;
        }

        private void OnPackageRefIncrease(string[] pkgNames)
        {
            if (pkgNames == null || pkgNames.Length == 0) return;
            foreach (string pkgName in pkgNames)
            {
                OnPackageRefIncrease(pkgName);
            }
        }

        private void OnPackageRefIncrease(List<string> pkgNames)
        {
            if (pkgNames == null || pkgNames.Count == 0) return;
            foreach (string pkgName in pkgNames)
            {
                OnPackageRefIncrease(pkgName);
            }
        }

        private void OnPackageRefReduce(string pkgName)
        {
            m_PackageRefCount[pkgName]--;
            if (m_PackageRefCount[pkgName] <= 0)
            {
                m_ResourceManager.UnloadUIAsset(pkgName);
                UIPackage.GetByName(pkgName)?.UnloadAssets();
                UIPackage.RemovePackage(pkgName);
                m_PackageRefCount.Remove(pkgName);
                GameFrameworkLog.Warning("Remove UIPackage {0}", pkgName);
            }
        }

        private void OnPackageRefReduce(string[] pkgNames)
        {
            if (pkgNames == null || pkgNames.Length == 0) return;
            foreach (string pkgName in pkgNames)
            {
                OnPackageRefReduce(pkgName);
            }
        }

        private void OnPackageRefReduce(List<string> pkgNames)
        {
            if (pkgNames == null || pkgNames.Count == 0) return;
            foreach (string pkgName in pkgNames)
            {
                OnPackageRefReduce(pkgName);
            }
        }

        private List<string> GetPackageDependencies(string pkgName, string[] selfDepends)
        {
            List<string> depends = null;
            UIPackage mainPkg = UIPackage.GetByName(pkgName);
            if (mainPkg != null && mainPkg.dependencies != null && mainPkg.dependencies.Length > 0)
            {
                depends = mainPkg.dependencies.Select(depend => depend["name"]).ToList();
                if (selfDepends != null && selfDepends.Length > 0)
                {
                    foreach (var dName in selfDepends)
                    {
                        depends.Remove(dName);
                    }
                }
            }
            return depends;
        }
    }
}