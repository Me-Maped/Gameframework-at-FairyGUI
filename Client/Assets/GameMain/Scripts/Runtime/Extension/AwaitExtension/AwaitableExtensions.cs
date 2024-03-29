using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.DataTable;
using GameFramework.Event;
using GameFramework.Resource;
using GameFramework.UI;
using GameMain;
using Google.Protobuf;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityGameFramework.Runtime;
using OpenUIFormFailureEventArgs = UnityGameFramework.Runtime.OpenUIFormFailureEventArgs;
using OpenUIFormSuccessEventArgs = UnityGameFramework.Runtime.OpenUIFormSuccessEventArgs;

namespace UGFExtensions.Await
{
    public static partial class AwaitableExtensions
    {
        private static readonly Dictionary<string, TaskCompletionSource<UIFormBase>> s_UIFormTcs =
            new Dictionary<string, TaskCompletionSource<UIFormBase>>();
        
        private static readonly Dictionary<int, TaskCompletionSource<Entity>> s_EntityTcs =
            new Dictionary<int, TaskCompletionSource<Entity>>();

        private static readonly Dictionary<int, TaskCompletionSource<EntityLogic>> s_EntityLogicTcs =
            new Dictionary<int, TaskCompletionSource<EntityLogic>>();

        private static readonly Dictionary<string, TaskCompletionSource<bool>> s_DataTableTcs =
            new Dictionary<string, TaskCompletionSource<bool>>();

        private static readonly Dictionary<string, TaskCompletionSource<bool>> s_SceneTcs =
            new Dictionary<string, TaskCompletionSource<bool>>();

        private static readonly HashSet<int> s_WebSerialIDs = new HashSet<int>();
        private static readonly List<WebResult> s_DelayReleaseWebResult = new List<WebResult>();

        private static readonly HashSet<int> s_DownloadSerialIds = new HashSet<int>();
        private static readonly List<DownLoadResult> s_DelayReleaseDownloadResult = new List<DownLoadResult>();

        private static readonly Dictionary<int,TaskCompletionSource<IMessage>> s_NetMessageTcs =
            new Dictionary<int, TaskCompletionSource<IMessage>>();
        private static readonly Dictionary<int, Type> s_NetMessageTypes = new Dictionary<int, Type>();

        private static bool s_IsSubscribeEvent = false;

        /// <summary>
        /// 注册需要的事件 (需再流程入口处调用 防止框架重启导致事件被取消问题)
        /// </summary>
        public static void SubscribeEvent()
        {
            EventComponent eventComponent = UnityGameFramework.Runtime.GameEntry.GetComponent<EventComponent>();
            eventComponent.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
            eventComponent.Subscribe(OpenUIFormFailureEventArgs.EventId, OnOpenUIFormFailure);
            
            eventComponent.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            eventComponent.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntityLogicSuccess);
            eventComponent.Subscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);
            eventComponent.Subscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityLogicFailure);

            eventComponent.Subscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
            eventComponent.Subscribe(LoadSceneFailureEventArgs.EventId, OnLoadSceneFailure);

            eventComponent.Subscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
            eventComponent.Subscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);

            eventComponent.Subscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);
            eventComponent.Subscribe(DownloadFailureEventArgs.EventId, OnDownloadFailure);
            
            eventComponent.Subscribe(GameMain.NetworkSuccessEventArgs.EventId, OnNetworkSuccess);
            s_IsSubscribeEvent = true;
        }

        private static void TipsSubscribeEvent()
        {
            if (!s_IsSubscribeEvent)
            {
                throw new Exception("Use await/async extensions must to subscribe event!");
            }
        }
        
        /// <summary>
        /// 打开界面（可等待）
        /// </summary>
        public static Task<UIFormBase> OpenUIFormAsync<T>(this UIComponent uiComponent, bool closeOther = false, object userData = null)where T : UIFormBase
        {
            TipsSubscribeEvent();
            UIFormBase uiForm = uiComponent.OpenForm<T>(closeOther,userData);
            var tcs = new TaskCompletionSource<UIFormBase>();
            s_UIFormTcs.Add(uiForm.Config.PkgName, tcs);
            return tcs.Task;
        }

        private static void OnOpenUIFormSuccess(object sender, GameEventArgs e)
        {
            OpenUIFormSuccessEventArgs ne = (OpenUIFormSuccessEventArgs)e;
            s_UIFormTcs.TryGetValue(ne.UIForm.Config.InstName, out TaskCompletionSource<UIFormBase> tcs);
            if (tcs != null)
            {
                tcs.SetResult(ne.UIForm);
                s_UIFormTcs.Remove(ne.UIForm.Config.PkgName);
            }
        }

        private static void OnOpenUIFormFailure(object sender, GameEventArgs e)
        {
            OpenUIFormFailureEventArgs ne = (OpenUIFormFailureEventArgs)e;
            s_UIFormTcs.TryGetValue(ne.UIFormAssetName, out TaskCompletionSource<UIFormBase> tcs);
            if (tcs != null)
            {
                tcs.SetException(new GameFrameworkException(ne.ErrorMessage));
                s_UIFormTcs.Remove(ne.UIFormAssetName);
            }
        }

        /// <summary>
        /// 显示实体（可等待）
        /// </summary>
        public static Task<Entity> ShowEntityAsync(this EntityComponent entityComponent, int entityId,
            Type entityLogicType, string entityAssetName, string entityGroupName, int priority, object userData)
        {
            TipsSubscribeEvent();
            var tcs = new TaskCompletionSource<Entity>();
            s_EntityTcs.Add(entityId, tcs);
            entityComponent.ShowEntity(entityId, entityLogicType, entityAssetName, entityGroupName, priority, userData);
            return tcs.Task;
        }

        /// <summary>
        /// 显示实体
        /// </summary>
        /// <param name="entityComponent"></param>
        /// <param name="entityId"></param>
        /// <param name="entityAssetName"></param>
        /// <param name="entityGroupName"></param>
        /// <param name="priority"></param>
        /// <param name="userData"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Task<EntityLogic> ShowEntityAsync<T>(this EntityComponent entityComponent, int entityId,
            string entityAssetName, string entityGroupName, int priority = 0, object userData = null) where T : EntityLogic
        {
            TipsSubscribeEvent();
            var tcs = new TaskCompletionSource<EntityLogic>();
            s_EntityLogicTcs.Add(entityId, tcs);
            entityComponent.ShowEntity<T>(entityId, entityAssetName, entityGroupName, priority, userData);
            return tcs.Task;
        }

        private static void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;
            s_EntityTcs.TryGetValue(ne.Entity.Id, out var tcs);
            if (tcs != null)
            {
                tcs.SetResult(ne.Entity);
                s_EntityTcs.Remove(ne.Entity.Id);
            }
        }

        private static void OnShowEntityFailure(object sender, GameEventArgs e)
        {
            ShowEntityFailureEventArgs ne = (ShowEntityFailureEventArgs)e;
            s_EntityTcs.TryGetValue(ne.EntityId, out var tcs);
            if (tcs != null)
            {
                tcs.SetException(new GameFrameworkException(ne.ErrorMessage));
                s_EntityTcs.Remove(ne.EntityId);
            }
        }
        
        private static void OnShowEntityLogicSuccess(object sender, GameEventArgs e)
        {
            ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;
            s_EntityLogicTcs.TryGetValue(ne.Entity.Id, out var tcs);
            if (tcs != null)
            {
                tcs.SetResult(ne.Entity.Logic);
                s_EntityLogicTcs.Remove(ne.Entity.Id);
            }
        }

        private static void OnShowEntityLogicFailure(object sender, GameEventArgs e)
        {
            ShowEntityFailureEventArgs ne = (ShowEntityFailureEventArgs)e;
            s_EntityLogicTcs.TryGetValue(ne.EntityId, out var tcs);
            if (tcs != null)
            {
                tcs.SetException(new GameFrameworkException(ne.ErrorMessage));
                s_EntityLogicTcs.Remove(ne.EntityId);
            }
        }


        /// <summary>
        /// 加载场景（可等待）
        /// </summary>
        public static Task<bool> LoadSceneAsync(this SceneComponent sceneComponent, string sceneAssetName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            TipsSubscribeEvent();
            var tcs = new TaskCompletionSource<bool>();
            s_SceneTcs.Add(sceneAssetName, tcs);
            sceneComponent.LoadScene(sceneAssetName, loadSceneMode);
            return tcs.Task;
        }

        private static void OnLoadSceneSuccess(object sender, GameEventArgs e)
        {
            LoadSceneSuccessEventArgs ne = (LoadSceneSuccessEventArgs)e;
            s_SceneTcs.TryGetValue(ne.SceneAssetName, out var tcs);
            if (tcs != null)
            {
                tcs.SetResult(true);
                s_SceneTcs.Remove(ne.SceneAssetName);
            }
            GameModule.UI.UICameraAttach(Camera.main);
        }

        private static void OnLoadSceneFailure(object sender, GameEventArgs e)
        {
            LoadSceneFailureEventArgs ne = (LoadSceneFailureEventArgs)e;
            s_SceneTcs.TryGetValue(ne.SceneAssetName, out var tcs);
            if (tcs != null)
            {
                tcs.SetException(new GameFrameworkException(ne.ErrorMessage));
                s_SceneTcs.Remove(ne.SceneAssetName);
            }
        }

        /// <summary>
        /// 异步加载资源（可等待）。
        /// </summary>
        /// <param name="resourceComponent">资源加载组件。</param>
        /// <param name="assetName">资源路径。</param>
        /// <param name="callback">回调函数。</param>
        public static async void LoadAssetAsync<T>(this ResourceComponent resourceComponent, string assetName, Action<T> callback)
            where T : UnityEngine.Object
        {
            T ret = await resourceComponent.LoadAssetAsync<T>(assetName);

            if (callback != null)
            {
                callback?.Invoke(ret);
            }
        }

        /// <summary>
        /// 加载资源（可等待）
        /// </summary>
        public static Task<T> LoadAssetAsync<T>(this ResourceComponent resourceComponent, string assetName)
            where T : UnityEngine.Object
        {
            TipsSubscribeEvent();
            TaskCompletionSource<T> loadAssetTcs = new TaskCompletionSource<T>();
            resourceComponent.LoadAssetAsync(assetName, typeof(T), new LoadAssetCallbacks(
                (tempAssetName, asset, duration, userdata) =>
                {
                    var source = loadAssetTcs;
                    loadAssetTcs = null;
                    T tAsset = asset as T;
                    if (tAsset != null)
                    {
                        source.SetResult(tAsset);
                    }
                    else
                    {
                        source.SetException(new GameFrameworkException(
                            $"Load asset failure load type is {asset.GetType()} but asset type is {typeof(T)}."));
                    }
                },
                (tempAssetName, status, errorMessage, userdata) => { loadAssetTcs.SetException(new GameFrameworkException(errorMessage)); }
            ));

            return loadAssetTcs.Task;
        }

        /// <summary>
        /// 加载多个资源（可等待）
        /// </summary>
        public static async Task<T[]> LoadAssetsAsync<T>(this ResourceComponent resourceComponent, string[] assetName) where T : UnityEngine.Object
        {
            TipsSubscribeEvent();
            if (assetName == null)
            {
                return null;
            }

            T[] assets = new T[assetName.Length];
            Task<T>[] tasks = new Task<T>[assets.Length];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = AwaitableExtensions.LoadAssetAsync<T>(resourceComponent,assetName[i]);
            }

            await Task.WhenAll(tasks);
            for (int i = 0; i < assets.Length; i++)
            {
                assets[i] = tasks[i].Result;
            }

            return assets;
        }


        /// <summary>
        /// 增加Web请求任务（可等待）
        /// </summary>
        public static Task<WebResult> AddWebRequestAsync(this WebRequestComponent webRequestComponent,
            string webRequestUri, WWWForm wwwForm = null, object userdata = null)
        {
            TipsSubscribeEvent();
            var tsc = new TaskCompletionSource<WebResult>();
            int serialId = webRequestComponent.AddWebRequest(webRequestUri, wwwForm,
                AwaitDataWrap<WebResult>.Create(userdata, tsc));
            s_WebSerialIDs.Add(serialId);
            return tsc.Task;
        }

        /// <summary>
        /// 增加Web请求任务（可等待）
        /// </summary>
        public static Task<WebResult> AddWebRequestAsync(this WebRequestComponent webRequestComponent,
            string webRequestUri, byte[] postData, object userdata = null)
        {
            TipsSubscribeEvent();
            var tsc = new TaskCompletionSource<WebResult>();
            int serialId = webRequestComponent.AddWebRequest(webRequestUri, postData,
                AwaitDataWrap<WebResult>.Create(userdata, tsc));
            s_WebSerialIDs.Add(serialId);
            return tsc.Task;
        }

        /// <summary>
        ///  增加Net请求任务（可等待）
        /// </summary>
        public static Task<IMessage> SendAsync<T>(this NetworkComponent networkComponent, int cmdId, IMessage msg) where T : IMessage
        {
            TipsSubscribeEvent();
            var tsc = new TaskCompletionSource<IMessage>();
            var packet = GameMain.CSNetPacket.Create(cmdId,msg);
            networkComponent.Send(packet);
            s_NetMessageTcs.Add(packet.UniId, tsc);
            s_NetMessageTypes[packet.UniId] = typeof(T);
            return tsc.Task;
        }

        private static void OnWebRequestSuccess(object sender, GameEventArgs e)
        {
            WebRequestSuccessEventArgs ne = (WebRequestSuccessEventArgs)e;
            if (s_WebSerialIDs.Contains(ne.SerialId))
            {
                if (ne.UserData is AwaitDataWrap<WebResult> webRequestUserdata)
                {
                    WebResult result = WebResult.Create(ne.GetWebResponseBytes(), false, string.Empty,
                        webRequestUserdata.UserData);
                    s_DelayReleaseWebResult.Add(result);
                    webRequestUserdata.Source.TrySetResult(result);
                    ReferencePool.Release(webRequestUserdata);
                }

                s_WebSerialIDs.Remove(ne.SerialId);
                if (s_WebSerialIDs.Count == 0)
                {
                    for (int i = 0; i < s_DelayReleaseWebResult.Count; i++)
                    {
                        ReferencePool.Release(s_DelayReleaseWebResult[i]);
                    }

                    s_DelayReleaseWebResult.Clear();
                }
            }
        }

        private static void OnWebRequestFailure(object sender, GameEventArgs e)
        {
            WebRequestFailureEventArgs ne = (WebRequestFailureEventArgs)e;
            if (s_WebSerialIDs.Contains(ne.SerialId))
            {
                if (ne.UserData is AwaitDataWrap<WebResult> webRequestUserdata)
                {
                    WebResult result = WebResult.Create(null, true, ne.ErrorMessage, webRequestUserdata.UserData);
                    webRequestUserdata.Source.TrySetResult(result);
                    s_DelayReleaseWebResult.Add(result);
                    ReferencePool.Release(webRequestUserdata);
                }

                s_WebSerialIDs.Remove(ne.SerialId);
                if (s_WebSerialIDs.Count == 0)
                {
                    for (int i = 0; i < s_DelayReleaseWebResult.Count; i++)
                    {
                        ReferencePool.Release(s_DelayReleaseWebResult[i]);
                    }

                    s_DelayReleaseWebResult.Clear();
                }
            }
        }

        /// <summary>
        /// 增加下载任务（可等待)
        /// </summary>
        public static Task<DownLoadResult> AddDownloadAsync(this DownloadComponent downloadComponent,
            string downloadPath,
            string downloadUri,
            object userdata = null)
        {
            TipsSubscribeEvent();
            var tcs = new TaskCompletionSource<DownLoadResult>();
            int serialId = downloadComponent.AddDownload(downloadPath, downloadUri,
                AwaitDataWrap<DownLoadResult>.Create(userdata, tcs));
            s_DownloadSerialIds.Add(serialId);
            return tcs.Task;
        }

        private static void OnDownloadSuccess(object sender, GameEventArgs e)
        {
            DownloadSuccessEventArgs ne = (DownloadSuccessEventArgs)e;
            if (s_DownloadSerialIds.Contains(ne.SerialId))
            {
                if (ne.UserData is AwaitDataWrap<DownLoadResult> awaitDataWrap)
                {
                    DownLoadResult result = DownLoadResult.Create(false, string.Empty, awaitDataWrap.UserData);
                    s_DelayReleaseDownloadResult.Add(result);
                    awaitDataWrap.Source.TrySetResult(result);
                    ReferencePool.Release(awaitDataWrap);
                }

                s_DownloadSerialIds.Remove(ne.SerialId);
                if (s_DownloadSerialIds.Count == 0)
                {
                    for (int i = 0; i < s_DelayReleaseDownloadResult.Count; i++)
                    {
                        ReferencePool.Release(s_DelayReleaseDownloadResult[i]);
                    }

                    s_DelayReleaseDownloadResult.Clear();
                }
            }
        }

        private static void OnDownloadFailure(object sender, GameEventArgs e)
        {
            DownloadFailureEventArgs ne = (DownloadFailureEventArgs)e;
            if (s_DownloadSerialIds.Contains(ne.SerialId))
            {
                if (ne.UserData is AwaitDataWrap<DownLoadResult> awaitDataWrap)
                {
                    DownLoadResult result = DownLoadResult.Create(true, ne.ErrorMessage, awaitDataWrap.UserData);
                    s_DelayReleaseDownloadResult.Add(result);
                    awaitDataWrap.Source.TrySetResult(result);
                    ReferencePool.Release(awaitDataWrap);
                }

                s_DownloadSerialIds.Remove(ne.SerialId);
                if (s_DownloadSerialIds.Count == 0)
                {
                    for (int i = 0; i < s_DelayReleaseDownloadResult.Count; i++)
                    {
                        ReferencePool.Release(s_DelayReleaseDownloadResult[i]);
                    }

                    s_DelayReleaseDownloadResult.Clear();
                }
            }
        }

        private static void OnNetworkSuccess(object sender, GameEventArgs e)
        {
            GameMain.NetworkSuccessEventArgs ne = (GameMain.NetworkSuccessEventArgs)e;
            s_NetMessageTcs.TryGetValue(ne.UniId, out TaskCompletionSource<IMessage> tcs);
            if (tcs != null)
            {
                Type msgType = s_NetMessageTypes[ne.UniId];
                tcs.SetResult(ne.Packet.Deserialize(msgType));
                s_NetMessageTcs.Remove(ne.UniId);
                s_NetMessageTypes.Remove(ne.UniId);
            }
        }
    }
}