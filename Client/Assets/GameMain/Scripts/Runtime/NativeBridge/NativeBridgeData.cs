using System;
using GameFramework;
using UnityEngine;

namespace GameMain
{
    [Serializable]
    public class NativeBridgeData : IReference
    {
        [SerializeField]
        public string funcName;
        
        [SerializeField]
        public string strData;

        public static NativeBridgeData Create(int callType, string strData)
        {
            return Create(callType.ToString(), strData);
        }
        
        public static NativeBridgeData Create(string funcName, string strData)
        {
            var nativeBridgeData = ReferencePool.Acquire<NativeBridgeData>();
            nativeBridgeData.funcName = funcName;
            nativeBridgeData.strData = strData;
            return nativeBridgeData;
        }
        
        public static string CreateJson(int callType, string strData)
        {
            return CreateJson(callType.ToString(), strData);
        }

        public static string CreateJson(string funcName, string strData)
        {
            var nativeBridgeData = ReferencePool.Acquire<NativeBridgeData>();
            nativeBridgeData.funcName = funcName;
            nativeBridgeData.strData = strData;
            string result = Utility.Json.ToJson(nativeBridgeData);
            ReferencePool.Release(nativeBridgeData);
            return result;
        }
        
        public void Clear()
        {
            funcName = null;
            strData = null;
        }
    }
}