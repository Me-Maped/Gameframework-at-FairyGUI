using System;
using GameFramework;

namespace GameMain
{
    public static class Sdk
    {
        /// <summary>
        /// 通用类型调用
        /// </summary>
        /// <param name="callType"></param>
        public static void CallNative(int callType)
        {
            NativeBridge.Instance.CallNative(callType, string.Empty);
        }
        
        /// <summary>
        /// 通用类型调用
        /// </summary>
        /// <param name="callType"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void CallNative<T>(int callType, T data)
        {
            string jsonContent = data == null ? string.Empty : Utility.Json.ToJson(data);
            NativeBridge.Instance.CallNative(callType, jsonContent);
        }

        /// <summary>
        /// 监听Native的调用，同步给出返回值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="commonCallback"></param>
        public static void Register(int type, OnNativeCallbackSync commonCallback)
        {
            if (NativeBridge.Instance.CommonSyncCallDic.ContainsKey(type))
            {
                NativeBridge.Instance.CommonSyncCallDic[type] += commonCallback;
            }
            else
            {
                NativeBridge.Instance.CommonSyncCallDic.Add(type, commonCallback);
            }
        }

        /// <summary>
        /// 监听native调用，异步给出返回值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="commonCallback"></param>
        public static void Register(int type, OnNativeCallbackAsync commonCallback)
        {
            if (NativeBridge.Instance.CommonAsyncCallDic.ContainsKey(type))
            {
                NativeBridge.Instance.CommonAsyncCallDic[type] += commonCallback;
            }
            else
            {
                NativeBridge.Instance.CommonAsyncCallDic.Add(type, commonCallback);
            }
        }

        /// <summary>
        /// 移除Native的调用监听
        /// </summary>
        /// <param name="type"></param>
        /// <param name="commonCallback"></param>
        public static void Unregister(int type, OnNativeCallbackSync commonCallback)
        {
            if (NativeBridge.Instance.CommonSyncCallDic.ContainsKey(type))
            {
                NativeBridge.Instance.CommonSyncCallDic[type] -= commonCallback;
            }
        }

        /// <summary>
        /// 移除Native的调用监听
        /// </summary>
        /// <param name="type"></param>
        /// <param name="commonCallback"></param>
        public static void Unregister(int type, OnNativeCallbackAsync commonCallback)
        {
            if (NativeBridge.Instance.CommonAsyncCallDic.ContainsKey(type))
            {
                NativeBridge.Instance.CommonAsyncCallDic[type] -= commonCallback;
            }
        }
    }
}