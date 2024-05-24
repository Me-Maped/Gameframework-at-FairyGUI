using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

namespace GameFramework.Resource
{
    /// <summary>
    /// 资源管理器接口。
    /// </summary>
    public interface IResourceManager
    {
        /// <summary>
        /// 同时下载的最大数目。
        /// </summary>
        int DownloadingMaxNum
        {
            get;
            set;
        }
        
        /// <summary>
        /// 失败重试最大数目。
        /// </summary>
        int FailedTryAgain
        {
            get;
            set;
        }
        
        /// <summary>
        /// 获取资源只读区路径。
        /// </summary>
        string ReadOnlyPath
        {
            get;
        }

        /// <summary>
        /// 获取资源读写区路径。
        /// </summary>
        string ReadWritePath
        {
            get;
        }
        
        /// <summary>
        /// 设置资源只读区路径。
        /// </summary>
        /// <param name="readOnlyPath">资源只读区路径。</param>
        void SetReadOnlyPath(string readOnlyPath);

        /// <summary>
        /// 设置资源读写区路径。
        /// </summary>
        /// <param name="readWritePath">资源读写区路径。</param>
        void SetReadWritePath(string readWritePath);

        /// <summary>
        /// 初始化接口。
        /// </summary>
        void Initialize();

        /// <summary>
        /// 初始化操作。
        /// </summary>
        /// <returns></returns>
        InitializationOperation InitPackage();
        
        /// <summary>
        /// 获取或设置资源包名称。
        /// </summary>
        string PackageName
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置资源包版本。
        /// </summary>
        string PackageVersion
        {
            get;
            set;
        }
        
        /// <summary>
        /// 获取或设置运行模式。
        /// </summary>
        EPlayMode PlayMode
        {
            get;
            set;
        }
        
        /// <summary>
        /// 获取或设置下载文件校验等级。
        /// </summary>
        EVerifyLevel VerifyLevel
        {
            get;
            set;
        }
        
        /// <summary>
        /// 获取或设置异步系统参数，每帧执行消耗的最大时间切片（单位：毫秒）。
        /// </summary>
        long Milliseconds
        {
            get;
            set;
        }
        
        Transform InstanceRoot
        {
            get;
            set;
        }
        
        /// <summary>
        /// 热更链接URL。
        /// </summary>
        string HostServerURL
        {
            get;
            set;
        }
        
        /// <summary>
        /// 卸载资源。
        /// </summary>
        /// <param name="asset">要卸载的资源。</param>
        void UnloadAsset(object asset);

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="assetPath"></param>
        void UnloadAsset(string assetPath);

        /// <summary>
        ///  卸载资源
        /// </summary>
        /// <param name="asset"></param>
        void UnloadAsset(UnityEngine.Object asset);

        /// <summary>
        /// 资源回收（卸载引用计数为零的资源）
        /// </summary>
        void UnloadUnusedAssets();
        
        /// <summary>
        /// 强制回收所有资源
        /// </summary>
        void ForceUnloadAllAssets();

        /// <summary>
        /// 同步加载资源。
        /// </summary>
        /// <param name="assetPath"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T LoadAssetSync<T>(string assetPath) where T : UnityEngine.Object;

        /// <summary>
        /// 同步实例化预制体
        /// </summary>
        /// <param name="prefabPath"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="parent"></param>
        /// <param name="worldPositionStays"></param>
        /// <returns></returns>
        GameObject InstantiateSync(string prefabPath, Vector3 position, Quaternion rotation, Transform parent,bool worldPositionStays);

        /// <summary>
        /// 同步加载原始文件
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        byte[] LoadRawFileSync(string assetPath);
        
        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <param name="assetPath"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Cysharp.Threading.Tasks.UniTask<T> LoadAssetAsync<T>(string assetPath) where T : UnityEngine.Object;
        
        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="assetType"></param>
        /// <returns></returns>
        Cysharp.Threading.Tasks.UniTask<UnityEngine.Object> LoadAssetAsync(string assetPath, System.Type assetType);

        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <param name="assetPath"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        System.Threading.Tasks.Task<T> LoadAssetTaskAsync<T>(string assetPath) where T : UnityEngine.Object;
        
        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="assetType"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<UnityEngine.Object> LoadAssetTaskAsync(string assetPath, System.Type assetType);

        /// <summary>
        /// 异步实例化资源。
        /// </summary>
        /// <param name="prefabPath"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="parent"></param>
        /// <param name="worldPositionStays"></param>
        /// <returns></returns>
        Cysharp.Threading.Tasks.UniTask<GameObject> InstantiateAsync(string prefabPath,Vector3 position, Quaternion rotation, Transform parent,bool worldPositionStays);

        /// <summary>
        /// 异步实例化资源。
        /// </summary>
        /// <param name="prefabPath"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="parent"></param>
        /// <param name="worldPositionStays"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<GameObject> InstantiateTaskAsync(string prefabPath,Vector3 position, Quaternion rotation, Transform parent,bool worldPositionStays);

        /// <summary>
        ///  异步加载文件资源。
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        Cysharp.Threading.Tasks.UniTask<byte[]> LoadRawFileAsync(string assetPath);

        /// <summary>
        /// 异步加载文件资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="resultCallback"></param>
        Cysharp.Threading.Tasks.UniTask LoadRawFileAsync(string assetName, Action<byte[]> resultCallback);

        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        void LoadAssetAsync(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData);

        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载的资源类型。</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        void LoadAssetAsync(string assetName,Type assetType, LoadAssetCallbacks loadAssetCallbacks, object userData);
        
        /// <summary>
        /// 检查资源是否存在。
        /// </summary>
        /// <param name="assetName">要检查资源的名称。</param>
        /// <returns>检查资源是否存在的结果。</returns>
        HasAssetResult HasAsset(string assetName);

        /// <summary>
        /// 异步加载场景。
        /// </summary>
        /// <param name="sceneAssetName">要加载场景资源的名称。</param>
        /// <param name="loadSceneMode">场景加载模式</param>
        /// <param name="priority">加载场景资源的优先级。</param>
        /// <param name="loadSceneCallbacks">加载场景回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        void LoadScene(string sceneAssetName,LoadSceneMode loadSceneMode, int priority, LoadSceneCallbacks loadSceneCallbacks, object userData = null);
        /// <summary>
        /// 异步卸载场景。
        /// </summary>
        /// <param name="sceneAssetName">要卸载场景资源的名称。</param>
        /// <param name="unloadSceneCallbacks">卸载场景回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        void UnloadScene(string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks, object userData = null);

        /// <summary>
        /// 异步加载UI资源包，自动加载依赖包。
        /// </summary>
        /// <param name="pkgName"></param>
        /// <param name="dependPkgs">自定义依赖包</param>
        /// <param name="loadAssetCallbacks"></param>
        /// <param name="userData"></param>
        void LoadUIPackagesAsync(string pkgName,string[] dependPkgs, LoadAssetCallbacks loadAssetCallbacks, object userData);

        /// <summary>
        /// 异步加载单个UI资源包
        /// </summary>
        /// <param name="pkgName"></param>
        Cysharp.Threading.Tasks.UniTask<FairyGUI.UIPackage> LoadUIPackageAsync(string pkgName);
    }
}