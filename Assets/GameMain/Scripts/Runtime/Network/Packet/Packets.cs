using System;
using GameFramework;
using Google.Protobuf;
using UnityGameFramework.Runtime;

namespace GameMain
{
    public class CSProtoPacket : CSPacketBase
    {
        public override void Clear()
        {
            CmdId = -1;
            Array.Clear(ProtoBody, 0, ProtoBody.Length);
            ProtoBody = null;
        }

        public static void Send(int cmdId, IMessage msg)
        {
            GameModule.Network.GetNetworkChannel("TCP").Send(Create(cmdId, msg));
        }

        public static CSProtoPacket Create(int cmdId,IMessage msg)
        {
            if (msg == null) Log.Fatal("Msg is invalid");
            if (cmdId <= 0) Log.Fatal("CmdId is invalid");
            var packet = ReferencePool.Acquire<CSProtoPacket>();
            packet.ProtoBody = ProtobufUtils.Serialize(msg);
            packet.CmdId = cmdId;
            return packet;
        }
    }
    
    public class SCProtoPacket : SCPacketBase
    {
        public override void Clear()
        {
            CmdId = -1;
            Array.Clear(ProtoBody, 0, ProtoBody.Length);
            ProtoBody = null;
        }
        public T Deserialize<T>() where T : IMessage, new()
        {            
            if (ProtoBody == null) Log.Fatal("ProtoBody is invalid");
            return ProtobufUtils.Deserialize<T>(ProtoBody);
        }
    }
}