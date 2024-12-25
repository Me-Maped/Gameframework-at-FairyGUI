using System;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameMain
{
    public class SdkTest : MonoBehaviour
    {
        private void OnEnable()
        {
            Sdk.Register(1, OnNativeCallTest);
        }

        private void OnDisable()
        {
            Sdk.Unregister(1, OnNativeCallTest);
        }

        private void Start()
        {
            Sdk.CallNative(2,"test no callback");
            Sdk.CallNative(3, "test callback");
            Sdk.Register(2, OnNativeCallTest);
            Sdk.Register(3,OnCallNativeBackTest);
        }

        private string OnNativeCallTest(string strData)
        {
            // do something
            Sdk.Unregister(2, OnNativeCallTest);
            return null;
        }

        private void OnCallNativeBackTest(string json,Action<string> callNative)
        {
            // do something
            Sdk.Unregister(3, OnCallNativeBackTest);
            // delay awhile
            callNative?.Invoke("call native");
        }
    }
}