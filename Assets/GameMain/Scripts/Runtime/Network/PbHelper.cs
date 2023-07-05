using System;
using System.Collections.Generic;
using GameFramework;
using Google.Protobuf;
using UnityGameFramework.Runtime;

namespace GameMain
{
    public static class PbHelper
    {
        private static Dictionary<Type, IMessage> _cacheDic = new Dictionary<Type, IMessage>();
        
        public static T Get<T>() where T : IMessage
        {
            if (_cacheDic.TryGetValue(typeof(T), out var pb)) return (T) pb;
            return Factory<T>();
        }

        private static T Factory<T>() where T : IMessage
        {
            var inst = Activator.CreateInstance<T>();
            if (inst == null) Log.Fatal(Utility.Text.Format("T {0} is invalid", typeof(T)));
            _cacheDic.Add(typeof(T),inst);
            return inst;
        }
    }
}