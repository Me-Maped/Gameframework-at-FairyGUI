using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using FairyGUI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityGameFramework.Runtime;
using YooAsset;
using Object = UnityEngine.Object;

namespace GameFramework.Resource
{
    /// <summary>
    /// 资源管理器。
    /// </summary>
    internal sealed partial class ResourceManager : GameFrameworkModule, IResourceManager
    {
        #region Propreties
        /// <summary>
        /// 资源包名称。
        /// </summary>
        public string PackageName { get; set; } = SettingsUtils.FrameworkGlobalSettings.DefaultPkgName;
        
        /// <summary>
        /// 资源包版本信息
        /// </summary>
        public string PackageVersion { set; get; }

        /// <summary>
        /// 资源系统运行模式。
        /// </summary>
        public EPlayMode PlayMode { get; set; }

        /// <summary>
        /// 下载文件校验等级。
        /// </summary>
        public EVerifyLevel VerifyLevel { get; set; }

        /// <summary>
        /// 设置异步系统参数，每帧执行消耗的最大时间切片（单位：毫秒）
        /// </summary>
        public long Milliseconds { get; set; }

        /// <summary>
        /// 获取游戏框架模块优先级。
        /// </summary>
        /// <remarks>优先级较高的模块会优先轮询，并且关闭操作会后进行。</remarks>
        internal override int Priority => 4;

        /// <summary>
        /// 实例化的根节点。
        /// </summary>
        public Transform InstanceRoot { get; set; }

        /// <summary>
        /// 资源服务器地址。
        /// </summary>
        public string HostServerURL { get; set; }

        /// <summary>
        /// The total number of frames since the start of the game (Read Only).
        /// </summary>
        private static int _lastUpdateFrame = 0;

        private string m_ReadOnlyPath;
        private string m_ReadWritePath;

        /// <summary>
        /// 获取资源只读区路径。
        /// </summary>
        public string ReadOnlyPath
        {
            get
            {
                return m_ReadOnlyPath;
            }
        }

        /// <summary>
        /// 获取资源读写区路径。
        /// </summary>
        public string ReadWritePath
        {
            get
            {
                return m_ReadWritePath;
            }
        }

        public int DownloadingMaxNum { get; set; }
        public int FailedTryAgain { get; set; }
        
        /// <summary>
        /// 是否重新检查AssetInfo，并更新配置文件
        /// </summary>
        public bool RecheckAssetInfo { get; set; }


        /// <summary>
        /// 加载的资源句柄
        /// </summary>
        private Dictionary<(string key,int hash), OperationHandleBase> m_AssetHandlers;

        /// <summary>
        /// 因未更新Manifest导致找不到路径时需要重试的资源
        /// </summary>
        private List<string> m_LoadFailBeforeUpdate;
        
        /// <summary>
        /// 加载的UI资源
        /// </summary>
        private Dictionary<string, List<string>> m_UIAssetHandlers;

        #endregion

        /// <summary>
        /// 初始化资源管理器的新实例。
        /// </summary>
        public ResourceManager()
        {
            m_AssetHandlers = new Dictionary<ValueTuple<string,int>, OperationHandleBase>();
            m_LoadFailBeforeUpdate = new List<string>();
            m_UIAssetHandlers = new Dictionary<string, List<string>>();
        }

        public void Initialize()
        {
            // 初始化资源系统
            YooAssets.Initialize(new YooAssetsLogger(), InstanceRoot);
            YooAssets.SetOperationSystemMaxTimeSlice(Milliseconds);
            YooAssets.SetCacheSystemCachedFileVerifyLevel(VerifyLevel);

            // 创建默认的资源包
            string packageName = PackageName;
            var defaultPackage = YooAssets.TryGetPackage(packageName);
            if (defaultPackage == null)
            {
                defaultPackage = YooAssets.CreatePackage(packageName);
                YooAssets.SetDefaultPackage(defaultPackage);
            }
        }

        #region 设置接口
        /// <summary>
        /// 设置资源只读区路径。
        /// </summary>
        /// <param name="readOnlyPath">资源只读区路径。</param>
        public void SetReadOnlyPath(string readOnlyPath)
        {
            if (string.IsNullOrEmpty(readOnlyPath))
            {
                throw new GameFrameworkException("Read-only path is invalid.");
            }

            m_ReadOnlyPath = readOnlyPath;
        }

        /// <summary>
        /// 设置资源读写区路径。
        /// </summary>
        /// <param name="readWritePath">资源读写区路径。</param>
        public void SetReadWritePath(string readWritePath)
        {
            if (string.IsNullOrEmpty(readWritePath))
            {
                throw new GameFrameworkException("Read-write path is invalid.");
            }

            m_ReadWritePath = readWritePath;
        }


        #endregion

        public InitializationOperation InitPackage()
        {
            // 创建默认的资源包
            string packageName = PackageName;
            var package = YooAssets.TryGetPackage(packageName);
            if (package == null)
            {
                package = YooAssets.CreatePackage(packageName);
                YooAssets.SetDefaultPackage(package);
            }

            // 编辑器下的模拟模式
            InitializationOperation initializationOperation = null;
            if (PlayMode == EPlayMode.EditorSimulateMode)
            {
                var createParameters = new EditorSimulateModeParameters();
                createParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(packageName);
                initializationOperation = package.InitializeAsync(createParameters);
            }

            // 单机运行模式
            if (PlayMode == EPlayMode.OfflinePlayMode)
            {
                var createParameters = new OfflinePlayModeParameters();
                createParameters.DecryptionServices = new GameDecryptionServices();
                initializationOperation = package.InitializeAsync(createParameters);
            }

            // 联机运行模式
            if (PlayMode == EPlayMode.HostPlayMode)
            {
                var createParameters = new HostPlayModeParameters();
                createParameters.DecryptionServices = new GameDecryptionServices();
                createParameters.BuildinQueryServices = new GameBuildinQueryServices();
                createParameters.DeliveryQueryServices = new GameDeliveryQueryServices();
                createParameters.RemoteServices = new GameRemoteServices(HostServerURL,HostServerURL);
                initializationOperation = package.InitializeAsync(createParameters);
            }

            return initializationOperation;
        }

        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
            DebugCheckDuplicateDriver();
            YooAssets.Update();
        }

        internal override void Shutdown()
        {
            m_AssetHandlers.Clear();
            m_LoadFailBeforeUpdate.Clear();
            m_UIAssetHandlers.Clear();
            m_AssetHandlers = null;
            m_LoadFailBeforeUpdate = null;
            m_UIAssetHandlers = null;
            YooAssets.Destroy();
        }

        [Conditional("DEBUG")]
        private void DebugCheckDuplicateDriver()
        {
            if (_lastUpdateFrame > 0)
            {
                if (_lastUpdateFrame == Time.frameCount)
                    YooLogger.Warning($"There are two {nameof(YooAssetsDriver)} in the scene. Please ensure there is always exactly one driver in the scene.");
            }

            _lastUpdateFrame = Time.frameCount;
        }

        #region Public Methods
        /// <summary>
        /// 异步资源加载
        /// </summary>
        /// <param name="location">资源地址</param>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <returns></returns>
        /// <exception cref="GameFrameworkException"></exception>
        public async UniTask<TObject> LoadAssetAsync<TObject>(string location) where TObject : UnityEngine.Object
        {
            if (typeof(TObject) == typeof(GameObject))
            {
                GameFrameworkLog.Warning("If you need instance a GameObject, you should call InstantiateAsync().");
            }
            AssetOperationHandle handle = await InternalLoadAssetHandleAsync(location);
            return handle.AssetObject as TObject;
        }

        /// <summary>
        /// 同步资源加载
        /// </summary>
        /// <param name="location"></param>
        /// <typeparam name="TObject"></typeparam>
        /// <returns></returns>
        public TObject LoadAssetSync<TObject>(string location) where TObject : UnityEngine.Object
        {
            if (typeof(TObject) == typeof(GameObject))
            {
                GameFrameworkLog.Warning("If you need instance a GameObject, you should call InstantiateSync().");
            }
            return InternalLoadAssetHandle(location).AssetObject as TObject;
        }

        /// <summary>
        /// 异步实例化预制体
        /// </summary>
        /// <param name="prefabPath"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="parent"></param>
        /// <param name="worldPositionStays"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<GameObject> InstantiateTaskAsync(string prefabPath,Vector3 position, Quaternion rotation, Transform parent,bool worldPositionStays)
        {
            AssetOperationHandle handle = await InternalLoadAssetHandleAsync(prefabPath, false);
            InstantiateOperation instHandle;
            if (worldPositionStays)
            {
                instHandle = handle.InstantiateAsync(parent, true);
            }
            else
            {
                instHandle = handle.InstantiateAsync(position, rotation, parent);
            }
            await instHandle.Task;
            GameObject result = instHandle.Result;
            m_AssetHandlers[(prefabPath, result.GetHashCode())] = handle;
            return result;
        }

        /// <summary>
        /// 同步实例化预制体
        /// </summary>
        /// <param name="prefabPath"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="parent"></param>
        /// <param name="worldPositionStays"></param>
        /// <returns></returns>
        public GameObject InstantiateSync(string prefabPath, Vector3 position, Quaternion rotation, Transform parent,bool worldPositionStays)
        {
            if (position == default) position = Vector3.zero;
            if (rotation == default) rotation = Quaternion.identity;
            AssetOperationHandle handle = InternalLoadAssetHandle(prefabPath, false);
            GameObject result;
            if (worldPositionStays)
            {
                result = handle.InstantiateSync(parent, true);
            }
            else
            {
                result = handle.InstantiateSync(position, rotation, parent);
            }
            m_AssetHandlers[(prefabPath, result.GetHashCode())] = handle;
            return result;
        }

        /// <summary>
        /// 异步加载原始文件资源
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        /// <exception cref="GameFrameworkException"></exception>
        public async UniTask<byte[]> LoadRawFileAsync(string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                throw new GameFrameworkException("Asset location is invalid.");
            }
            ResourcePackage assetPackage = YooAssets.TryGetPackage(PackageName);
            AssetInfo assetInfo = assetPackage.GetAssetInfo(location);
            if (assetInfo.IsInvalid)
            {
                if(RecheckAssetInfo) return await RecheckLoadFailAssets(location);
                throw new GameFrameworkException(Utility.Text.Format("Can not load asset '{0}'.", location));
            }
            RawFileOperationHandle handle = assetPackage.LoadRawFileAsync(assetInfo);
            await handle.Task;
            if (handle == null || handle.Status == EOperationStatus.Failed)
            {
                string errorMessage = Utility.Text.Format("Can not load asset '{0}'.", location);
                throw new GameFrameworkException(errorMessage);
            }
            byte[] result = handle.GetRawFileData();
            m_AssetHandlers[(location, -1)] = handle;
            return result;
        }

        public byte[] LoadRawFileSync(string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                throw new GameFrameworkException("Asset location is invalid.");
            }
            ResourcePackage assetPackage = YooAssets.TryGetPackage(PackageName);
            AssetInfo assetInfo = assetPackage.GetAssetInfo(location);
            if (assetInfo.IsInvalid)
            {
                throw new GameFrameworkException($"Asset info '{location}' is invalid.");
            }
            RawFileOperationHandle handle = assetPackage.LoadRawFileSync(assetInfo);
            byte[] result = handle.GetRawFileData();
            m_AssetHandlers[(location, -1)] = handle;
            return result;
        }

        /// <summary>
        /// 异步加载原始文件资源
        /// </summary>
        /// <param name="location"></param>
        /// <param name="resultCallback"></param>
        public async UniTask LoadRawFileAsync(string location, Action<byte[]> resultCallback)
        {
            byte[] result = await LoadRawFileAsync(location);
            resultCallback?.Invoke(result);
        }

        public async System.Threading.Tasks.Task<TObject> LoadAssetTaskAsync<TObject>(string location) where TObject : UnityEngine.Object
        {
            AssetOperationHandle handle = await InternalLoadAssetHandleAsync(location);
            return handle.AssetObject as TObject;
        }

        public async UniTask<GameObject> InstantiateAsync(string prefabPath,Vector3 position, Quaternion rotation, Transform parent,bool worldPositionStays)
        {
            AssetOperationHandle handle = await InternalLoadAssetHandleAsync(prefabPath,false);
            InstantiateOperation instHandle;
            if (worldPositionStays)
            {
                instHandle = handle.InstantiateAsync(parent, true);
            }
            else
            {
                instHandle = handle.InstantiateAsync(position, rotation, parent);
            }
            await instHandle.Task;
            GameObject result = instHandle.Result;
            m_AssetHandlers[(prefabPath, result.GetHashCode())] = handle;
            return result;
        }

        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public async void LoadAssetAsync(string assetName, Type assetType, LoadAssetCallbacks loadAssetCallbacks, object userData)
        {
            float duration = Time.time;

            if (loadAssetCallbacks == null)
            {
                throw new GameFrameworkException("Load asset callbacks is invalid.");
            }
            
            AssetOperationHandle handle = await InternalLoadAssetHandleAsync(assetName);
            if (handle == null || handle.AssetObject == null || handle.Status == EOperationStatus.Failed)
            {
                string errorMessage = Utility.Text.Format("Can not load asset '{0}'.", assetName);
                loadAssetCallbacks.LoadAssetFailureCallback?.Invoke(assetName, LoadResourceStatus.NotReady, errorMessage, userData);
                throw new GameFrameworkException(errorMessage);
            }
            if (loadAssetCallbacks.LoadAssetSuccessCallback != null)
            {
                duration = Time.time - duration;
                loadAssetCallbacks.LoadAssetSuccessCallback(assetName, handle.AssetObject, duration, userData);
            }
        }


        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public async void LoadAssetAsync(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData)
        {
            float duration = Time.time;

            if (loadAssetCallbacks == null)
            {
                throw new GameFrameworkException("Load asset callbacks is invalid.");
            }
            AssetOperationHandle handle = await InternalLoadAssetHandleAsync(assetName);
            if (handle == null || handle.AssetObject == null || handle.Status == EOperationStatus.Failed)
            {
                string errorMessage = Utility.Text.Format("Can not load asset '{0}'.", assetName);
                loadAssetCallbacks.LoadAssetFailureCallback?.Invoke(assetName, LoadResourceStatus.NotReady, errorMessage, userData);
                throw new GameFrameworkException(errorMessage);
            }
            if (loadAssetCallbacks.LoadAssetSuccessCallback != null)
            {
                duration = Time.time - duration;
                loadAssetCallbacks.LoadAssetSuccessCallback(assetName, handle.AssetObject, duration, userData);
            }
        }
        #endregion

        public void UnloadUnusedAssets()
        {
            YooAssets.GetPackage(PackageName).UnloadUnusedAssets();
        }

        public void ForceUnloadAllAssets()
        {
            YooAssets.GetPackage(PackageName).ForceUnloadAllAssets();
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="asset"></param>
        public void UnloadAsset(object asset)
        {
            if (m_AssetHandlers == null) return;
            int hashCode = asset.GetHashCode();
            bool hasRelease = false;
            var enumerator = m_AssetHandlers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Key.hash == hashCode)
                {
                    enumerator.Current.Value.ReleaseInternal();
                    GameFrameworkLog.Info($"Unload asset {enumerator.Current.Key}");
                    m_AssetHandlers.Remove(enumerator.Current.Key);
                    hasRelease = true;
                    break;
                }
            }
            enumerator.Dispose();
            if (!hasRelease)
            {
                if (asset is Object unityObj)
                {
                    Object.DestroyImmediate(unityObj);
                }
                else
                {
                    GameFrameworkLog.Warning("UnloadAsset: {0} failed", asset);
                }
            }
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="assetPath"></param>
        public void UnloadAsset(string assetPath)
        {
            if (m_AssetHandlers == null) return;
            var enumerator = m_AssetHandlers.GetEnumerator();
            bool hasRelease = false;
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Key.key.Equals(assetPath))
                {
                    enumerator.Current.Value.ReleaseInternal();
                    GameFrameworkLog.Info($"Unload asset {enumerator.Current.Key}");
                    m_AssetHandlers.Remove(enumerator.Current.Key);
                    hasRelease = true;
                    break;
                }
            }
            enumerator.Dispose();
            if (!hasRelease)
            {
                GameFrameworkLog.Warning("UnloadAsset: {0} failed", assetPath);
            }
        }

        public void UnloadAsset(Object asset)
        {
            if (m_AssetHandlers == null) return;
            bool hasRelease = false;
            int hashCode = asset.GetHashCode();
            var enumerator = m_AssetHandlers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Key.Item2 == hashCode)
                {
                    enumerator.Current.Value.ReleaseInternal();
                    GameFrameworkLog.Info($"Unload asset {enumerator.Current.Key}");
                    m_AssetHandlers.Remove(enumerator.Current.Key);
                    hasRelease = true;
                    break;
                }
            }
            enumerator.Dispose();
            if(!hasRelease) Object.DestroyImmediate(asset);
        }

        /// <summary>
        /// 检查资源是否存在。
        /// </summary>
        /// <param name="assetName">要检查资源的名称。</param>
        /// <returns>检查资源是否存在的结果。</returns>
        public HasAssetResult HasAsset(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                throw new GameFrameworkException("Asset name is invalid.");
            }
            AssetInfo assetInfo = YooAssets.GetAssetInfo(assetName);
            if (assetInfo == null)
            {
                return HasAssetResult.NotExist;
            }
            return HasAssetResult.AssetOnDisk;
        }

        /// <summary>
        /// 异步加载场景。
        /// </summary>
        /// <param name="sceneAssetName">要加载场景资源的名称。</param>
        /// <param name="loadSceneMode">加载模式</param>
        /// <param name="priority">加载场景资源的优先级。</param>
        /// <param name="loadSceneCallbacks">加载场景回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public async void LoadScene(string sceneAssetName, LoadSceneMode loadSceneMode, int priority, LoadSceneCallbacks loadSceneCallbacks, object userData = null)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                throw new GameFrameworkException("Scene asset name is invalid.");
            }

            if (loadSceneCallbacks == null)
            {
                throw new GameFrameworkException("Load scene callbacks is invalid.");
            }

            float duration = Time.time;

            SceneOperationHandle handle = YooAssets.LoadSceneAsync(sceneAssetName, loadSceneMode, suspendLoad: false, priority: priority);

            await handle.Task;

            if (loadSceneCallbacks.LoadSceneSuccessCallback != null)
            {
                duration = Time.time - duration;

                loadSceneCallbacks.LoadSceneSuccessCallback(sceneAssetName, handle.SceneObject, duration, userData);
            }
        }

        /// <summary>
        /// 异步卸载场景。
        /// </summary>
        /// <param name="sceneAssetName">要卸载场景资源的名称</param>
        /// <param name="unloadSceneCallbacks">卸载场景回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <exception cref="GameFrameworkException">游戏框架异常。</exception>
        public void UnloadScene(string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks, object userData = null)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                throw new GameFrameworkException("Scene asset name is invalid.");
            }

            if (unloadSceneCallbacks == null)
            {
                throw new GameFrameworkException("Unload scene callbacks is invalid.");
            }

            Utility.Unity.StartCoroutine(UnloadSceneCo(sceneAssetName, unloadSceneCallbacks, userData));
        }

        /// <summary>
        /// 异步加载UI包，自动加载依赖包
        /// </summary>
        /// <param name="pkgName"></param>
        /// <param name="dependPkgs"></param>
        /// <param name="loadAssetCallbacks"></param>
        /// <param name="userData"></param>
        /// <exception cref="GameFrameworkException"></exception>
        public async void LoadUIPackagesAsync(string pkgName,string[] dependPkgs, LoadAssetCallbacks loadAssetCallbacks, object userData)
        {
            //加载包描述数据
            if (string.IsNullOrEmpty(pkgName))
            {
                throw new GameFrameworkException("Asset name is invalid.");
            }
            if (loadAssetCallbacks == null)
            {
                throw new GameFrameworkException("Load asset callbacks is invalid.");
            }
            float duration = Time.time;
            string rootPath = SettingsUtils.FrameworkGlobalSettings.UIResourceFolder;
            TextAsset pkgDesc = await InternalLoadUIAsset(pkgName);
            UIPackage uiPackage = UIPackage.AddPackage(pkgDesc.bytes, string.Empty, (name, extension, type, packageItem) =>
            {
                LoadUIExtensions(rootPath, name, extension, type, packageItem, loadAssetCallbacks, duration, userData);
            });
            List<string> depPkgNames = null;
            if (dependPkgs != null && dependPkgs.Length > 0)
            {
                depPkgNames = new List<string>();
                depPkgNames.AddRange(dependPkgs);
            }
            if (uiPackage.dependencies != null && uiPackage.dependencies.Length > 0)
            {
                depPkgNames ??= new List<string>();
                depPkgNames.AddRange(uiPackage.dependencies.Select(dependDic => dependDic["name"]).ToList());
            }
            if (depPkgNames != null && depPkgNames.Count > 0)
            {
                // 这里只取一层依赖，设计UI时尽量不要出现多层包依赖
                for (int i = 0; i < depPkgNames.Count; i++)
                {
                    string depPkgName = depPkgNames[i];
                    TextAsset depPkgDesc = await InternalLoadUIAsset(depPkgName);
                    UIPackage.AddPackage(depPkgDesc.bytes, string.Empty,
                        (name, extension, type, packageItem) =>
                        {
                            LoadUIExtensions(rootPath, name, extension, type, packageItem, loadAssetCallbacks, duration,
                                userData);
                        });
                    // 所有包加载完成后给出回调
                    if (i == depPkgNames.Count - 1)
                    {
                        loadAssetCallbacks.LoadAssetSuccessCallback?.Invoke(pkgName, pkgDesc, Time.time - duration,
                            userData);
                    }
                }
                GameFrameworkLog.Info("Main package '{0}', loading dependent packages '{1}'", pkgName,
                    depPkgNames.ToString());
            }
            else
            {
                loadAssetCallbacks.LoadAssetSuccessCallback?.Invoke(pkgName, pkgDesc, Time.time - duration, userData);
            }
        }

        /// <summary>
        /// 卸载UI资源。
        /// </summary>
        /// <param name="assetName"></param>
        public void UnloadUIAsset(string assetName)
        {
            if (m_UIAssetHandlers.TryGetValue(assetName, out var locationList))
            {
                foreach (var location in locationList)
                {
                    UnloadAsset(location);
                }
                locationList.Clear();
                m_UIAssetHandlers.Remove(assetName);
            }
        }

        private async UniTask<TextAsset> InternalLoadUIAsset(string pkgName)
        {
            //加载包描述数据
            if (string.IsNullOrEmpty(pkgName))
            {
                throw new GameFrameworkException("Asset name is invalid.");
            }
            string rootPath = SettingsUtils.FrameworkGlobalSettings.UIResourceFolder;
            string resPath = Utility.Text.Format("{0}{1}/{2}_fui.bytes", rootPath, pkgName, pkgName);
            TextAsset pkgDesc = await LoadAssetAsync<TextAsset>(resPath);
            if (pkgDesc == null)
            {
                throw new GameFrameworkException(Utility.Text.Format("Asset name '{0}' is invalid.", pkgName));
            }
            m_UIAssetHandlers.TryGetValue(pkgName, out var handleList);
            if (handleList == null)
            {
                handleList = new List<string>();
                m_UIAssetHandlers.Add(pkgName, handleList);
            }
            handleList.Add(resPath);
            return pkgDesc;
        }

        /// <summary>
        /// UI包资源加载委托
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="name"></param>
        /// <param name="extension"></param>
        /// <param name="type"></param>
        /// <param name="packageItem"></param>
        /// <param name="loadAssetCallbacks"></param>
        /// <param name="duration"></param>
        /// <param name="userData"></param>
        private async void LoadUIExtensions(string rootPath, string name, string extension, Type type, PackageItem packageItem, LoadAssetCallbacks loadAssetCallbacks, float duration, object userData)
        {
            string extPath = Utility.Text.Format("{0}{1}/{1}_{2}{3}", rootPath, packageItem.owner.name, name, extension);
            var targetObj = await LoadAssetAsync<Object>(extPath);
            packageItem.owner.SetItemAsset(packageItem, targetObj, DestroyMethod.None);
            loadAssetCallbacks.LoadAssetSuccessCallback?.Invoke(name, targetObj, Time.time - duration, userData);
        }

        private IEnumerator UnloadSceneCo(string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks, object userData)
        {
            AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(SceneComponent.GetSceneName(sceneAssetName));
            if (asyncOperation == null)
            {
                yield break;
            }

            yield return asyncOperation;

            if (asyncOperation.allowSceneActivation)
            {
                if (unloadSceneCallbacks.UnloadSceneSuccessCallback != null)
                {
                    unloadSceneCallbacks.UnloadSceneSuccessCallback(sceneAssetName, userData);
                }
            }
            else
            {
                if (unloadSceneCallbacks.UnloadSceneFailureCallback != null)
                {
                    unloadSceneCallbacks.UnloadSceneFailureCallback(sceneAssetName, userData);
                }
            }
        }

        private async UniTask<AssetOperationHandle> InternalLoadAssetHandleAsync(string location, bool recordRef = true)
        {
            if (string.IsNullOrEmpty(location))
            {
                throw new GameFrameworkException("Asset location is invalid.");
            }
            ResourcePackage assetPackage = YooAssets.TryGetPackage(PackageName);
            AssetInfo assetInfo = assetPackage.GetAssetInfo(location);
            if (assetInfo.IsInvalid)
            {
                if(RecheckAssetInfo) return await InternalReloadAssetHandleAsync(location);
                throw new GameFrameworkException(Utility.Text.Format("Can not load asset '{0}'.", location));
            }
            AssetOperationHandle handle = assetPackage.LoadAssetAsync(assetInfo);
            await handle.Task;
            if (handle.AssetObject == null || handle.Status == EOperationStatus.Failed)
            {
                string errorMessage = Utility.Text.Format("Can not load asset '{0}'.", location);
                throw new GameFrameworkException(errorMessage);
            }
            if (recordRef) m_AssetHandlers[(location, handle.AssetObject.GetHashCode())] = handle;
            return handle;
        }

        private AssetOperationHandle InternalLoadAssetHandle(string location, bool recordRef = true)
        {
            if (string.IsNullOrEmpty(location))
            {
                throw new GameFrameworkException("Asset location is invalid.");
            }
            ResourcePackage assetPackage = YooAssets.TryGetPackage(PackageName);
            AssetInfo assetInfo = assetPackage.GetAssetInfo(location);
            if (assetInfo.IsInvalid)
            {
                throw new GameFrameworkException($"Asset info {location} is invalid.");
            }
            AssetOperationHandle handle = assetPackage.LoadAssetSync(assetInfo);
            if (handle.AssetObject == null || handle.Status == EOperationStatus.Failed)
            {
                string errorMessage = Utility.Text.Format("Can not load asset '{0}'.", location);
                throw new GameFrameworkException(errorMessage);
            }
            if (recordRef) m_AssetHandlers[(location, handle.AssetObject.GetHashCode())] = handle;
            return handle;
        }

        private async UniTask<AssetOperationHandle> InternalReloadAssetHandleAsync(string location)
        {
            if (m_LoadFailBeforeUpdate.Contains(location))
            {
                m_LoadFailBeforeUpdate.Remove(location);
                string errorMessage = Utility.Text.Format("Can not load asset handle '{0}'.", location);
                throw new GameFrameworkException(errorMessage);
            }
            m_LoadFailBeforeUpdate.Add(location);
            await ReUpdatePackageManifest();
            AssetOperationHandle result = await InternalLoadAssetHandleAsync(location);
            m_LoadFailBeforeUpdate.Remove(location);
            return result;
        }

        private async UniTask<byte[]> RecheckLoadFailAssets(string location)
        {
            // 已经失败过，说明确实是资源问题，直接抛出异常
            if (m_LoadFailBeforeUpdate.Contains(location))
            {
                m_LoadFailBeforeUpdate.Remove(location);
                string errorMessage = Utility.Text.Format("Can not load asset '{0}'.", location);
                throw new GameFrameworkException(errorMessage);
            }
            // 首次加载失败，可能是服务端资源更新，需要进行Manifest同步后再加载
            m_LoadFailBeforeUpdate.Add(location);
            await ReUpdatePackageManifest();
            
            // 再去加载一次
            byte[] result = await LoadRawFileAsync(location);
            m_LoadFailBeforeUpdate.Remove(location);

            return result;
        }

        private async UniTask ReUpdatePackageManifest()
        {
            var activePackage = YooAssets.GetPackage(PackageName);
            var versionLoadOperation = activePackage.UpdatePackageVersionAsync(timeout: 10);
            await versionLoadOperation.Task;
            
            if (!versionLoadOperation.IsDone || versionLoadOperation.Status != EOperationStatus.Succeed)
            {
                throw new GameFrameworkException(versionLoadOperation.Error);
            }
            PackageVersion = versionLoadOperation.PackageVersion;

            var manifestLoadOperation = activePackage.UpdatePackageManifestAsync(PackageVersion, timeout: 10);
            await manifestLoadOperation.Task;
            if (!manifestLoadOperation.IsDone || manifestLoadOperation.Status != EOperationStatus.Succeed)
            {
                throw new GameFrameworkException(versionLoadOperation.Error);
            }
        }
    }
}