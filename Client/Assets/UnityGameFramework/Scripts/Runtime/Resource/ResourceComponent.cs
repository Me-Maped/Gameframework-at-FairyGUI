using System;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Resource;
using UnityEngine;
using YooAsset;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    /// 资源组件。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/Resource")]
    public class ResourceComponent : GameFrameworkComponent
    {
        #region Propreties
        private const int DefaultPriority = 0;

        private IResourceManager m_ResourceManager;
        private bool m_ForceUnloadUnusedAssets = false;
        private bool m_PreorderUnloadUnusedAssets = false;
        private bool m_PerformGCCollect = false;
        private AsyncOperation m_AsyncOperation = null;
        private float m_LastUnloadUnusedAssetsOperationElapseSeconds = 0f;
        
        [SerializeField]
        private float m_MinUnloadUnusedAssetsInterval = 60f;

        [SerializeField]
        private float m_MaxUnloadUnusedAssetsInterval = 300f;
        
        /// <summary>
        /// 资源包名称。
        /// </summary>
        [SerializeField]
        public string PackageName = "MEngine";
        
        /// <summary>
        /// 资源系统运行模式。
        /// </summary>
        [SerializeField]
        public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

        /// <summary>
        /// 下载文件校验等级。
        /// </summary>
        [SerializeField]
        public EVerifyLevel VerifyLevel = EVerifyLevel.Middle;
        
        [SerializeField]
        private ReadWritePathType m_ReadWritePathType = ReadWritePathType.Unspecified;
        
        /// <summary>
        /// 设置异步系统参数，每帧执行消耗的最大时间切片（单位：毫秒）
        /// </summary>
        [SerializeField]
        public long Milliseconds = 30;

        [SerializeField]
        private int m_DownloadingMaxNum = 2;
        
        [SerializeField]
        private int m_FailedTryAgain = 3;
        
        
        /// <summary>
        /// 获取或设置同时最大下载数目。
        /// </summary>
        public int DownloadingMaxNum
        {
            get => m_DownloadingMaxNum;
            set => m_DownloadingMaxNum = value;
        }
        /// <summary>
        /// 获取失败重试次数
        /// </summary>
        public int FailedTryAgain
        {
            get => m_FailedTryAgain;
            set => m_FailedTryAgain = value;
        }
        
        /// <summary>
        /// 获取资源读写路径类型。
        /// </summary>
        public ReadWritePathType ReadWritePathType => m_ReadWritePathType;

        /// <summary>
        /// 获取或设置无用资源释放的最小间隔时间，以秒为单位。
        /// </summary>
        public float MinUnloadUnusedAssetsInterval
        {
            get => m_MinUnloadUnusedAssetsInterval;
            set => m_MinUnloadUnusedAssetsInterval = value;
        }

        /// <summary>
        /// 获取或设置无用资源释放的最大间隔时间，以秒为单位。
        /// </summary>
        public float MaxUnloadUnusedAssetsInterval
        {
            get => m_MaxUnloadUnusedAssetsInterval;
            set => m_MaxUnloadUnusedAssetsInterval = value;
        }
        
        /// <summary>
        /// 获取无用资源释放的等待时长，以秒为单位。
        /// </summary>
        public float LastUnloadUnusedAssetsOperationElapseSeconds => m_LastUnloadUnusedAssetsOperationElapseSeconds;

        /// <summary>
        /// 获取资源只读路径。
        /// </summary>
        public string ReadOnlyPath => m_ResourceManager.ReadOnlyPath;

        /// <summary>
        /// 获取资源读写路径。
        /// </summary>
        public string ReadWritePath => m_ResourceManager.ReadWritePath;

        /// <summary>
        /// 当前最新的包裹版本。
        /// </summary>
        public string PackageVersion 
        {
            set => m_ResourceManager.PackageVersion = value;
            get => m_ResourceManager.PackageVersion;
        }
        
        /// <summary>
        /// 资源下载器，用于下载当前资源版本所有的资源包文件。
        /// </summary>
        public ResourceDownloaderOperation Downloader { get; set; }

        #endregion
        
        private void Start()
        {
            BaseComponent baseComponent = GameEntry.GetComponent<BaseComponent>();
            if (baseComponent == null)
            {
                Log.Fatal("Base component is invalid.");
                return;
            }

            m_ResourceManager = GameFrameworkEntry.GetModule<IResourceManager>();
            if (m_ResourceManager == null)
            {
                Log.Fatal("YooAssetsManager component is invalid.");
                return;
            }

#if UNITY_EDITOR
            if (PlayMode == EPlayMode.EditorSimulateMode)
            {
                Log.Info("During this run, Game Framework will use editor resource files, which you should validate first.");
            }
#else
            PlayMode = Define.PkgArg.YooMode;
#endif

            m_ResourceManager.SetReadOnlyPath(Application.streamingAssetsPath);
            if (m_ReadWritePathType == ReadWritePathType.TemporaryCache)
            {
                m_ResourceManager.SetReadWritePath(Application.temporaryCachePath);
            }
            else
            {
                if (m_ReadWritePathType == ReadWritePathType.Unspecified)
                {
                    m_ReadWritePathType = ReadWritePathType.PersistentData;
                }

                m_ResourceManager.SetReadWritePath(Application.persistentDataPath);
            }
            
            m_ResourceManager.PackageName = PackageName;
            m_ResourceManager.PlayMode = PlayMode;
            m_ResourceManager.VerifyLevel = VerifyLevel;
            m_ResourceManager.Milliseconds = Milliseconds;
            m_ResourceManager.InstanceRoot = transform;
            m_ResourceManager.HostServerURL = SettingsUtils.GetResDownLoadPath();
            m_ResourceManager.Initialize();
            Log.Info($"AssetsComponent Run Mode：{PlayMode}");
        }

        /// <summary>
        /// 初始化操作。
        /// </summary>
        /// <returns></returns>
        public InitializationOperation InitPackage()
        {
            m_ResourceManager = GameFrameworkEntry.GetModule<IResourceManager>();
            if (m_ResourceManager == null)
            {
                Log.Fatal("YooAssetsManager component is invalid.");
                return null;
            }
            return m_ResourceManager.InitPackage();
        }

        #region 加载资源
        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <typeparam name="T">要加载的资源类型。</typeparam>
        /// <returns>UniTask资源实例。</returns>
        public async UniTask<T> LoadAssetAsync<T>(string assetName) where T : UnityEngine.Object
        {
            return await m_ResourceManager.LoadAssetAsync<T>(assetName);
        }

        /// <summary>
        /// 异步加载文件资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public async UniTask<byte[]> LoadRawFileAsync(string assetName)
        {
            return await m_ResourceManager.LoadRawFileAsync(assetName);
        }

        /// <summary>
        /// 异步加载文件资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="resultCallback"></param>
        public async UniTask LoadRawFileAsync(string assetName, Action<byte[]> resultCallback)
        {
            await m_ResourceManager.LoadRawFileAsync(assetName, resultCallback);
        }

        /// <summary>
        /// 异步实例化预制体
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="parent"></param>
        /// <param name="worldPositionStays"></param>
        /// <returns></returns>
        public async UniTask<GameObject> InstantiateAsync(string assetName, Vector3 position = default,
            Quaternion rotation = default, Transform parent = null,bool worldPositionStays = false)
        {
            return await m_ResourceManager.InstantiateAsync(assetName, position, rotation, parent, worldPositionStays);
        }

        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <param name="assetName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<T> LoadAssetTaskAsync<T>(string assetName) where T : UnityEngine.Object
        {
            return await m_ResourceManager.LoadAssetTaskAsync<T>(assetName);
        }

        /// <summary>
        /// 异步实例化预制体
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="parent"></param>
        /// <param name="worldPositionStays"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<GameObject> InstantiateTaskAsync(string assetName,
            Vector3 position = default, Quaternion rotation = default, Transform parent = null, bool worldPositionStays = false)
        {
            return await m_ResourceManager.InstantiateTaskAsync(assetName, position, rotation, parent,
                worldPositionStays);
        }

        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadAssetAsync(string assetName, Type assetType, LoadAssetCallbacks loadAssetCallbacks, object userData = null)
        {
            LoadAssetAsync(assetName, assetType, DefaultPriority, loadAssetCallbacks, userData);
        }
        
        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="priority">加载资源的优先级。</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadAssetAsync(string assetName, Type assetType, int priority, LoadAssetCallbacks loadAssetCallbacks, object userData)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                Log.Error("Asset name is invalid.");
                return;
            }
            m_ResourceManager.LoadAssetAsync(assetName, assetType, loadAssetCallbacks, userData);
        }

        /// <summary>
        /// 同步加载资源。
        /// </summary>
        /// <param name="assetName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T LoadAssetSync<T>(string assetName) where T : UnityEngine.Object
        {
            return m_ResourceManager.LoadAssetSync<T>(assetName);
        }

        /// <summary>
        /// 同步实例化资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="parent"></param>
        /// <param name="worldPositionStays"></param>
        /// <returns></returns>
        public GameObject InstantiateSync(string assetName,Vector3 position = default,Quaternion rotation = default,Transform parent = null,bool worldPositionStays = false)
        {
            return m_ResourceManager.InstantiateSync(assetName, position, rotation, parent, worldPositionStays);
        }
        
        /// <summary>
        /// 同步加载文件
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public byte[] LoadRawFileSync(string assetName)
        {
            return m_ResourceManager.LoadRawFileSync(assetName);
        }
        #endregion

        #region 卸载资源
        /// <summary>
        /// 卸载资源。
        /// </summary>
        /// <param name="asset">要卸载的资源。</param>
        public void UnloadAsset(object asset)
        {
            if (asset == null)
            {
                Log.Error("Asset is invalid.");
                return;
            }
            m_ResourceManager.UnloadAsset(asset);
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="asset"></param>
        /// <exception cref="GameFrameworkException"></exception>
        public void UnloadAsset(UnityEngine.Object asset)
        {
            if (asset == null)
            {
                Log.Error("Asset is invalid.");
                return;
            }
            m_ResourceManager.UnloadAsset(asset);
        }

        /// <summary>
        /// 卸载资源。
        /// </summary>
        /// <param name="assetPath"></param>
        /// <exception cref="GameFrameworkException"></exception>
        public void UnloadAsset(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                Log.Error("Asset path is invalid.");
                return;
            }
            m_ResourceManager.UnloadAsset(assetPath);
        }
        #endregion

        #region 检查资源
        /// <summary>
        /// 设置默认的资源包。
        /// </summary>
        public void SetDefaultPackage(string packageName)
        {
            var package = YooAssets.GetPackage(packageName);
            if (package == null)
            {
                Log.Error("Package '{0}' is not exist.");
                return;
            }
            m_ResourceManager.PackageName = packageName;
            YooAssets.SetDefaultPackage(package);
        }
        
        /// <summary>
        /// 根据单个资源路径检查是否需要从远端下载。
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool IsNeedDownload(string location)
        {
            return YooAssets.IsNeedDownloadFromRemote(location);
        }

        /// <summary>
        /// 根据资源标签检查是否需要从远端下载。
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public bool IsNeedDownloadByTag(string tagName)
        {
            AssetInfo[] assetInfos = YooAssets.GetAssetInfos(tagName);
            foreach (var assetInfo in assetInfos)
            {
                if (YooAssets.IsNeedDownloadFromRemote(assetInfo)) return true;
            }
            return false;
        }

        /// <summary>
        /// 根据资源标签检查是否需要从远端下载。
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool IsNeedDownloadByTag(string[] tags)
        {
            AssetInfo[] assetInfos = YooAssets.GetAssetInfos(tags);
            foreach (var assetInfo in assetInfos)
            {
                if (YooAssets.IsNeedDownloadFromRemote(assetInfo)) return true;
            }
            return false;
        }
        
        /// <summary>
        /// 检查资源定位地址是否有效。
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public bool CheckLocationValid(string location)
        {
            return YooAssets.CheckLocationValid(location);
        }
        #endregion

        #region 下载资源
        public UpdatePackageVersionOperation UpdatePackageVersionAsync(bool appendTimeTicks = true, int timeout = 60)
        {
            var package = YooAssets.GetPackage(PackageName);
            return package.UpdatePackageVersionAsync(appendTimeTicks,timeout);
        }

        public UpdatePackageManifestOperation UpdatePackageManifestAsync(string packageVersion,bool autoSaveManifest =true, int timeout = 60)
        {
            var package = YooAssets.GetPackage(PackageName);
            return package.UpdatePackageManifestAsync(packageVersion,autoSaveManifest,timeout);
        }
        
        /// <summary>
        /// 创建资源下载器，用于下载当前资源版本所有的资源包文件
        /// </summary>
        /// <param name="downloadingMaxNumber">同时下载的最大文件数</param>
        /// <param name="failedTryAgain">下载失败的重试次数, -1表示使用全局设置</param>
        public ResourceDownloaderOperation CreateResourceDownloader(int downloadingMaxNumber = -1, int failedTryAgain = -1)
        {
            if (downloadingMaxNumber == -1) downloadingMaxNumber = DownloadingMaxNum;
            if (failedTryAgain == -1) failedTryAgain = FailedTryAgain;
            var package = YooAssets.GetPackage(PackageName);
            Downloader = package.CreateResourceDownloader(downloadingMaxNumber,failedTryAgain);
            return Downloader;
        }

        /// <summary>
        /// 创建资源下载器，用于下载指定资源标签的资源包文件
        /// </summary>
        /// <param name="resTag"></param>
        /// <param name="downloadingMaxNumber"></param>
        /// <param name="failedTryAgain"></param>
        /// <returns></returns>
        public ResourceDownloaderOperation CreateResourceDownloader(string resTag, int downloadingMaxNumber = -1,
            int failedTryAgain = -1)
        {
            if (string.IsNullOrEmpty(resTag))
            {
                Log.Warning("CreateResourceDownloader resTag is null or empty, download full");
                return CreateResourceDownloader(downloadingMaxNumber, failedTryAgain);
            }
            if (downloadingMaxNumber == -1) downloadingMaxNumber = DownloadingMaxNum;
            if (failedTryAgain == -1) failedTryAgain = FailedTryAgain;
            var package = YooAssets.GetPackage(PackageName);
            Downloader = package.CreateResourceDownloader(resTag,downloadingMaxNumber,failedTryAgain);
            return Downloader;
        }

        /// <summary>
        /// 创建资源下载器，用于下载指定的资源包文件
        /// </summary>
        /// <param name="location"></param>
        /// <param name="downloadingMaxNumber"></param>
        /// <param name="failedTryAgain"></param>
        /// <returns></returns>
        public ResourceDownloaderOperation CreateBundleDownloader(string location, int downloadingMaxNumber = -1,
            int failedTryAgain = -1)
        {
            if (string.IsNullOrEmpty(location))
            {
                Log.Error("CreateBundleDownloader location is null");
                return null;
            }
            if (downloadingMaxNumber == -1) downloadingMaxNumber = DownloadingMaxNum;
            if (failedTryAgain == -1) failedTryAgain = FailedTryAgain;
            var package = YooAssets.GetPackage(PackageName);
            Downloader = package.CreateBundleDownloader(location,downloadingMaxNumber,failedTryAgain);
            return Downloader;
        }

        /// <summary>
        /// 检查并下载资源
        /// </summary>
        /// <param name="location"></param>
        /// <param name="progressAction"></param>
        /// <returns></returns>
        public async UniTask<bool> CheckAndDownloadResource(string location,DownloaderOperation.OnDownloadProgress progressAction = null)
        {
            if (!IsNeedDownload(location)) return true;
            var downloader = YooAssets.CreateBundleDownloader(location, DownloadingMaxNum, FailedTryAgain);
            downloader.BeginDownload();
            if(progressAction!=null) downloader.OnDownloadProgressCallback += progressAction;
            await downloader;
            if(progressAction!=null) downloader.OnDownloadProgressCallback -= progressAction;
            return downloader.Status == EOperationStatus.Succeed;
        }
        
        /// <summary>
        /// 检查并根据标签下载对应资源包
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="progressAction"></param>
        /// <returns></returns>
        public async UniTask<bool> CheckAndDownloadResourceByTag(string tagName,DownloaderOperation.OnDownloadProgress progressAction= null)
        {
            if (!IsNeedDownloadByTag(tagName)) return true;
            var downloader = YooAssets.CreateResourceDownloader(tagName, DownloadingMaxNum, FailedTryAgain);
            downloader.BeginDownload();
            if(progressAction!=null) downloader.OnDownloadProgressCallback += progressAction;
            await downloader;
            if(progressAction!=null) downloader.OnDownloadProgressCallback -= progressAction;
            return downloader.Status == EOperationStatus.Succeed;
        }

        /// <summary>
        /// 检查并根据标签下载对应资源包
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="progressAction"></param>
        /// <returns></returns>
        public async UniTask<bool> CheckAndDownloadResourceByTag(string[] tags,
            DownloaderOperation.OnDownloadProgress progressAction = null)
        {
            if (!IsNeedDownloadByTag(tags)) return true;
            var downloader = YooAssets.CreateResourceDownloader(tags, DownloadingMaxNum, FailedTryAgain);
            downloader.BeginDownload();
            if(progressAction!=null) downloader.OnDownloadProgressCallback += progressAction;
            await downloader;
            if(progressAction!=null) downloader.OnDownloadProgressCallback -= progressAction;
            return downloader.Status == EOperationStatus.Succeed;
        }
        #endregion

        #region 清理资源
        /// <summary>
        /// 清理包裹未使用的缓存文件
        /// </summary>
        public ClearUnusedCacheFilesOperation ClearUnusedCacheFilesAsync()
        {
            var package = YooAssets.GetPackage(PackageName);
            return package.ClearUnusedCacheFilesAsync();
        }
       
        /// <summary>
        /// 强制执行释放未被使用的资源。
        /// </summary>
        /// <param name="performGCCollect">是否使用垃圾回收。</param>
        public void ForceUnloadUnusedAssets(bool performGCCollect)
        {
            m_ForceUnloadUnusedAssets = true;
            if (performGCCollect)
            {
                m_PerformGCCollect = true;
            }
        }

        /// <summary>
        /// 清空包裹的沙盒目录
        /// </summary>
        public void ClearSandbox()
        {
            YooAssets.GetPackage(PackageName).ClearPackageSandbox();
        }
        #endregion

        private void Update()
        {
            m_LastUnloadUnusedAssetsOperationElapseSeconds += Time.unscaledDeltaTime;
            if (m_AsyncOperation == null && (m_ForceUnloadUnusedAssets || m_LastUnloadUnusedAssetsOperationElapseSeconds >= m_MaxUnloadUnusedAssetsInterval || m_PreorderUnloadUnusedAssets && m_LastUnloadUnusedAssetsOperationElapseSeconds >= m_MinUnloadUnusedAssetsInterval))
            {
                Log.Info("Unload unused assets...");
                m_ForceUnloadUnusedAssets = false;
                m_PreorderUnloadUnusedAssets = false;
                m_LastUnloadUnusedAssetsOperationElapseSeconds = 0f;
                m_AsyncOperation = Resources.UnloadUnusedAssets();
            }
            
            if (m_AsyncOperation is { isDone: true })
            {
                m_ResourceManager.UnloadUnusedAssets();
                m_AsyncOperation = null;
                if (m_PerformGCCollect)
                {
                    Log.Info("GC.Collect...");
                    m_PerformGCCollect = false;
                    GC.Collect();
                }
            }
        }
    }
}
