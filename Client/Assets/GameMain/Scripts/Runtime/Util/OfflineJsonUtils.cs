using System;

namespace GameMain
{
    /// <summary>
    /// 非运行时Json序列化工具
    /// </summary>
    public static class OfflineJsonUtils
    {
        private static NewtonsoftJsonHelper _jsonHelper;
        public static NewtonsoftJsonHelper JsonHelper => _jsonHelper ??= new NewtonsoftJsonHelper();
        
        public static T ToObject<T>(string json) where T : class
        {
            return JsonHelper.ToObject<T>(json);
        }

        public static object ToObject(Type objectType, string json)
        {
            return JsonHelper.ToObject(objectType, json);
        }
        
        public static string ToJson(object obj)
        {
            return JsonHelper.ToJson(obj);
        }
    }
}