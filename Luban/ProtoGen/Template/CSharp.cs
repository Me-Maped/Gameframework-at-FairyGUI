// 修改后的CSharp.txt模板
// 该文件自动生成，请勿手动修改!!!
// 生成时间: {{generate_time}}
// - by Maped

using System;
using System.Collections.Generic;
namespace {{pb_namespace}}
{
    // 协议号定义
    public class CMD
    {
{{for info in infos}}
		public const int {{info.protocol_name}} = {{info.protocol_id}};{{end}}
    }

    // 协议类型映射
    public static class PBHelper
    {
        public static Dictionary<Type, int> ProtoTypeDic = new()
        {
        {{for info in infos}}
			{ typeof({{info.protocol_name}}),CMD.{{info.protocol_name}} },{{end}}
        };

        public static int Get<T>()
		{
			if (!ProtoTypeDic.ContainsKey(typeof(T))) return -1;
			return (int)ProtoTypeDic[typeof(T)];
		}

		public static int Get(Type type)
		{
			if (!ProtoTypeDic.ContainsKey(type)) return -1;
			return (int)ProtoTypeDic[type];
		}

		public static Type GetType(int cmd)
		{
			foreach(var kv in ProtoTypeDic)
			{
				if(kv.Value == cmd) return kv.Key;
			}
			return null;
		}

		public static bool Contain(int cmd)
		{
			return ProtoTypeDic.ContainsValue(cmd);
		}
    }
}