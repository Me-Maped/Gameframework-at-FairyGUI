// 修改后的CSharp.txt模板
// 该文件自动生成，请勿手动修改!!!
// 生成时间: 2025/5/5 09:34:05
// - by Maped

using System;
using System.Collections.Generic;
namespace Pb
{
    // 协议号定义
    public class CMD
    {

		public const int SyncPid = 2000;
		public const int SyncPlayers = 2001;
		public const int Player = 2002;
		public const int Position = 2003;
		public const int MovePackage = 2004;
		public const int BroadCast = 2005;
		public const int Talk = 2006;
		public const int ReqBagInfo = 3000;
		public const int ResBagInfo = 3001;
		public const int ReqComposePet = 3002;
		public const int ResComposePet = 3003;
		public const int ReqUseItem = 3004;
		public const int ReqSellItem = 3005;
		public const int ResItemChange = 3006;
		public const int TestStruct = 3007;
		public const int A = 3008;
		public const int B = 3009;
		public const int UserInfo = 3010;
		public const int ReqLogin = 3011;
		public const int ResLogin = 3012;
		public const int ResLevelUp = 3013;
		public const int HeartBeat = 3014;
		public const int ResErrorCode = 3015;
		public const int ResPrompt = 3016;
		public const int Place = 3017;
		public const int MoveMessage = 3018;
		public const int ErrorAck = 1000;
		public const int PingReq = 1001;
		public const int PingAck = 1002;
		public const int TestNtf = 1003;
    }

    // 协议类型映射
    public static class PBHelper
    {
        public static Dictionary<Type, int> ProtoTypeDic = new()
        {
        
        			{ typeof(SyncPid),CMD.SyncPid },
        			{ typeof(SyncPlayers),CMD.SyncPlayers },
        			{ typeof(Player),CMD.Player },
        			{ typeof(Position),CMD.Position },
        			{ typeof(MovePackage),CMD.MovePackage },
        			{ typeof(BroadCast),CMD.BroadCast },
        			{ typeof(Talk),CMD.Talk },
        			{ typeof(ReqBagInfo),CMD.ReqBagInfo },
        			{ typeof(ResBagInfo),CMD.ResBagInfo },
        			{ typeof(ReqComposePet),CMD.ReqComposePet },
        			{ typeof(ResComposePet),CMD.ResComposePet },
        			{ typeof(ReqUseItem),CMD.ReqUseItem },
        			{ typeof(ReqSellItem),CMD.ReqSellItem },
        			{ typeof(ResItemChange),CMD.ResItemChange },
        			{ typeof(TestStruct),CMD.TestStruct },
        			{ typeof(A),CMD.A },
        			{ typeof(B),CMD.B },
        			{ typeof(UserInfo),CMD.UserInfo },
        			{ typeof(ReqLogin),CMD.ReqLogin },
        			{ typeof(ResLogin),CMD.ResLogin },
        			{ typeof(ResLevelUp),CMD.ResLevelUp },
        			{ typeof(HeartBeat),CMD.HeartBeat },
        			{ typeof(ResErrorCode),CMD.ResErrorCode },
        			{ typeof(ResPrompt),CMD.ResPrompt },
        			{ typeof(Place),CMD.Place },
        			{ typeof(MoveMessage),CMD.MoveMessage },
        			{ typeof(ErrorAck),CMD.ErrorAck },
        			{ typeof(PingReq),CMD.PingReq },
        			{ typeof(PingAck),CMD.PingAck },
        			{ typeof(TestNtf),CMD.TestNtf },
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