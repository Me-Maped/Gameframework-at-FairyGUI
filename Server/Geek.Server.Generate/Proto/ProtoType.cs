// 该文件自动生成，请勿手动修改!!!
// 生成时间: 2023-09-15 15:50:48.482804 &#43;0800 CST m=&#43;0.001501501	
// - by Maped

using System;
using System.Collections.Generic;

namespace Geek.Server.Proto
{
	//协议号定义
	public class CMD
	{	
		public const int ErrorAck = 1001;
		public const int PingReq = 1002;
		public const int PingAck = 1003;
		public const int TestNtf = 1004;
		public const int ReqBagInfo = 3001;
		public const int ResBagInfo = 3002;
		public const int ReqComposePet = 3003;
		public const int ResComposePet = 3004;
		public const int ReqUseItem = 3005;
		public const int ReqSellItem = 3006;
		public const int ResItemChange = 3007;
		public const int TestStruct = 3008;
		public const int A = 3009;
		public const int B = 3010;
		public const int UserInfo = 3011;
		public const int ReqLogin = 3012;
		public const int ResLogin = 3013;
		public const int ResLevelUp = 3014;
		public const int HeartBeat = 3015;
		public const int ResErrorCode = 3016;
		public const int ResPrompt = 3017;
		public const int Place = 3018;
		public const int MoveMessage = 3019;
		public const int SyncPid = 2001;
		public const int SyncPlayers = 2002;
		public const int Player = 2003;
		public const int Position = 2004;
		public const int MovePackage = 2005;
		public const int BroadCast = 2006;
		public const int Talk = 2007;
	}

	public class PBHelper
	{
		public static Dictionary<Type, int> ProtoTypeDic = new()
		{
			
			{typeof(ErrorAck), CMD.ErrorAck },
			{typeof(PingReq), CMD.PingReq },
			{typeof(PingAck), CMD.PingAck },
			{typeof(TestNtf), CMD.TestNtf },
			{typeof(ReqBagInfo), CMD.ReqBagInfo },
			{typeof(ResBagInfo), CMD.ResBagInfo },
			{typeof(ReqComposePet), CMD.ReqComposePet },
			{typeof(ResComposePet), CMD.ResComposePet },
			{typeof(ReqUseItem), CMD.ReqUseItem },
			{typeof(ReqSellItem), CMD.ReqSellItem },
			{typeof(ResItemChange), CMD.ResItemChange },
			{typeof(TestStruct), CMD.TestStruct },
			{typeof(A), CMD.A },
			{typeof(B), CMD.B },
			{typeof(UserInfo), CMD.UserInfo },
			{typeof(ReqLogin), CMD.ReqLogin },
			{typeof(ResLogin), CMD.ResLogin },
			{typeof(ResLevelUp), CMD.ResLevelUp },
			{typeof(HeartBeat), CMD.HeartBeat },
			{typeof(ResErrorCode), CMD.ResErrorCode },
			{typeof(ResPrompt), CMD.ResPrompt },
			{typeof(Place), CMD.Place },
			{typeof(MoveMessage), CMD.MoveMessage },
			{typeof(SyncPid), CMD.SyncPid },
			{typeof(SyncPlayers), CMD.SyncPlayers },
			{typeof(Player), CMD.Player },
			{typeof(Position), CMD.Position },
			{typeof(MovePackage), CMD.MovePackage },
			{typeof(BroadCast), CMD.BroadCast },
			{typeof(Talk), CMD.Talk },
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