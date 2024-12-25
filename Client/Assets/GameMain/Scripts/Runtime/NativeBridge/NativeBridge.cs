using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameMain
{
    public class NativeBridge : MonoSingleton<NativeBridge>
    {
#if UNITY_EDITOR
#elif UNITY_ANDROID
        private const string JAVA_CLASS = "com.unity.bridge.UnityBridge";
        private const string JAVA_INTERFACE = "com.unity.bridge.IUnityCaller";
        private const string JAVA_INSTANCE = "getInstance";
        private const string JAVA_SET_CALLBACK = "setBridgeCallback";
        private const string CALL_NATIVE = "onUnityCall";
        private const string CALL_NATIVE_ASYNC = "onUnityCallAsync";
        
        #region Native Call Unity
         private static AndroidJavaClass _javaClass = null;
         private static AndroidJavaObject _javaObject = null;
         private class AndroidCallbackImpl : AndroidJavaProxy
         {
             public AndroidCallbackImpl(string javaInterface) : base(javaInterface) { }
             public string callUnitySync(int callType, string strData)
             {
                 return Instance.OnNativeCallSync(callType, strData);
             }
             public void callUnityWait(int callType, string timeStamp, string strData)
             {
                 Instance.OnNativeCall(callType, timeStamp, strData);
             }
         }
         #endregion
#elif UNITY_IOS
        #region Unity Call Native
        [DllImport("__Internal")]
        private static extern void Call_Native(int callType, string strData);

        [DllImport("__Internal")]
        private static extern void Call_Native_Async(string timeStamp, string strData);
        #endregion

        #region Native Call Unity
        private delegate string NativeCallSyncDelegate(int callType, [MarshalAs(UnmanagedType.LPStr)] string jsonContent);
        private delegate void NativeCallDelegate(int callType,[MarshalAs(UnmanagedType.LPStr)] string timeStamp, [MarshalAs(UnmanagedType.LPStr)] string jsonContent);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, EntryPoint = "reg_call_unity_sync_func")]
        private static extern void RegisterNativeCallSyncFunc(NativeCallSyncDelegate func);
        
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, EntryPoint = "reg_call_unity_wait_func")]
        private static extern void RegisterNativeCallFunc(NativeCallDelegate func);
        
        [AOT.MonoPInvokeCallback(typeof(NativeCallSyncDelegate))]
        public static string OnNativeCallSyncFunc(int callType, string jsonContent)
        {
            return Instance.OnNativeCallSync(callType,jsonContent);
        }

        [AOT.MonoPInvokeCallback(typeof(NativeCallDelegate))]
        public static void OnNativeCallFunc(int callType,string timeStamp, string jsonContent)
        {
            Instance.OnNativeCall(callType, timeStamp, jsonContent);
        }
        #endregion
#endif

        /// <summary>
        /// 通用回调
        /// </summary>
        public readonly Dictionary<int, OnNativeCallbackSync> CommonSyncCallDic =
            new Dictionary<int, OnNativeCallbackSync>();
        /// <summary>
        /// 通用回调，异步
        /// </summary>
        public readonly Dictionary<int, OnNativeCallbackAsync> CommonAsyncCallDic =
            new Dictionary<int, OnNativeCallbackAsync>();

        protected override void Init()
        {
            Log.Info("NativeBridge - Init");
#if UNITY_EDITOR
#elif UNITY_ANDROID
            _javaClass = new AndroidJavaClass(JAVA_CLASS);
            var proxy = new AndroidJavaObject(JAVA_INTERFACE, new AndroidCallbackImpl(JAVA_INTERFACE));
            _javaObject = _javaClass.CallStatic<AndroidJavaObject>(JAVA_INSTANCE);
            _javaObject.Call(JAVA_SET_CALLBACK, proxy);
#elif UNITY_IOS
            RegisterNativeCallSyncFunc(OnNativeCallSyncFunc);
            RegisterNativeCallFunc(OnNativeCallFunc);
#endif
        }

        /// <summary>
        /// 调用oc/java
        /// </summary>
        /// <param name="callType"></param>
        /// <param name="strData"></param>
        public void CallNative(int callType, string strData)
        {
            Log.Info("NativeBridge - CallNativeCommon: " + callType + ", strData:" + strData);
#if UNITY_EDITOR
#elif UNITY_ANDROID
            _javaObject.CallStatic(CALL_NATIVE, callType, strData);
#elif UNITY_IOS
            Call_Native(callType, strData);
#endif
        }

        /// <summary>
        /// Native通用调用，同步，带有返回值
        /// </summary>
        /// <param name="callType"></param>
        /// <param name="strData"></param>
        /// <returns></returns>
        public string OnNativeCallSync(int callType, string strData)
        {
            if (CommonSyncCallDic.TryGetValue(callType, out OnNativeCallbackSync action))
            {
                return action?.Invoke(strData);
            }
            return string.Empty;
        }

        /// <summary>
        /// Native通用调用，异步，作为回调处理
        /// </summary>
        /// <param name="callType"></param>
        /// <param name="timeStamp"></param>
        /// <param name="strData"></param>
        public void OnNativeCall(int callType, string timeStamp, string strData)
        {
            if (CommonAsyncCallDic.TryGetValue(callType, out OnNativeCallbackAsync action))
            {
                action?.Invoke(strData, json =>
                {
#if UNITY_EDITOR
#elif UNITY_IOS
                    Call_Native_Async(timeStamp, json);
#elif UNITY_ANDROID
                    _javaObject.CallStatic(CALL_NATIVE_ASYNC, timeStamp, json);
#endif
                });
            }
        }
    }
}