using System;
using GameFramework;
using UnityGameFramework.Runtime;

namespace GameMain
{
    public static class CloneUtils
    {
        public static T DeepCopyByJson<T>(this T obj) where T : class
        {
            try
            {
                if (obj == null) return null;
                if (!UnityEngine.Application.isPlaying)
                {
                    string json = OfflineJsonUtils.ToJson(obj);
                    return OfflineJsonUtils.ToObject<T>(json);
                }
                else
                {
                    string json = Utility.Json.ToJson(obj);
                    return Utility.Json.ToObject<T>(json);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }
        
        public static object DeepCopyByJson(Type objType, object obj)
        {
            try
            {
                if (obj == null) return null;
                if (!UnityEngine.Application.isPlaying)
                {
                    string json = OfflineJsonUtils.ToJson(obj);
                    return OfflineJsonUtils.ToObject(objType, json);
                }
                else
                {
                    string json = Utility.Json.ToJson(obj);
                    return Utility.Json.ToObject(objType, json);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }
    }
}