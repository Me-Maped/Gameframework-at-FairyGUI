// 该文件自动生成，请勿手动修改!!!
// 生成时间: 2023-08-03 17:34:27.7154116 &#43;0800 CST m=&#43;0.002298601	
// - by Maped

using System;
using System.Collections.Generic;

namespace Pb
{
	//协议号定义
	public enum ProtoType
	{	
		ErrorAck = 1001,
		PingReq = 1002,
		PingAck = 1003,
		TestNtf = 1004,
		SyncPid = 2001,
		SyncPlayers = 2002,
		Player = 2003,
		Position = 2004,
		MovePackage = 2005,
		BroadCast = 2006,
		Talk = 2007,
	}

	public class PbTypeHelper
	{
		private static Dictionary<Type, ProtoType> _protoTypeDic = new()
		{
			
			{typeof(ErrorAck), ProtoType.ErrorAck },
			{typeof(PingReq), ProtoType.PingReq },
			{typeof(PingAck), ProtoType.PingAck },
			{typeof(TestNtf), ProtoType.TestNtf },
			{typeof(SyncPid), ProtoType.SyncPid },
			{typeof(SyncPlayers), ProtoType.SyncPlayers },
			{typeof(Player), ProtoType.Player },
			{typeof(Position), ProtoType.Position },
			{typeof(MovePackage), ProtoType.MovePackage },
			{typeof(BroadCast), ProtoType.BroadCast },
			{typeof(Talk), ProtoType.Talk },
		};

		public static int Get<T>()
		{
			if (!_protoTypeDic.ContainsKey(typeof(T))) return -1;
			return (int)_protoTypeDic[typeof(T)];
		}

		public static int Get(Type type)
		{
			if (!_protoTypeDic.ContainsKey(type)) return -1;
			return (int)_protoTypeDic[type];
		}
	}
}