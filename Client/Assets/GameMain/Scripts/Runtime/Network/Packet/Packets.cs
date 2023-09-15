using System;
using GameFramework;
using Google.Protobuf;
using UnityGameFramework.Runtime;

namespace GameMain
{
    public class CSNetPacket : PacketBase
    {
        public override int Id => CmdId;
        public override PacketType PacketType => PacketType.C2S;
        public override void Clear()
        {
            CmdId = -1;
            UniId = -1;
            if(ProtoBody!=null) Array.Clear(ProtoBody, 0, ProtoBody.Length);
            ProtoBody = null;
        }

        public static CSNetPacket Create(int cmdId,IMessage msg)
        {
            if (msg == null) Log.Fatal("Msg is invalid");
            if (cmdId <= 0) Log.Fatal("CmdId is invalid");
            var packet = ReferencePool.Acquire<CSNetPacket>();
            packet.ProtoBody = ProtobufUtils.Serialize(msg);
            packet.CmdId = cmdId;
            packet.UniId = PacketUniRecorder.LastUniId;
            return packet;
        }
    }
    
    public class SCNetPacket : PacketBase
    {
        public override int Id => CmdId;
        public override PacketType PacketType => PacketType.S2C;
        public override void Clear()
        {
            CmdId = -1;
            UniId = -1;
            if(ProtoBody != null) Array.Clear(ProtoBody, 0, ProtoBody.Length);
            ProtoBody = null;
        }

        public void Copy(SCNetPacket packet)
        {
            CmdId = packet.CmdId;
            UniId = packet.UniId;
            ProtoBody = new byte[packet.ProtoBody.Length];
            Array.Copy(packet.ProtoBody, ProtoBody, packet.ProtoBody.Length);
        }
        public T Deserialize<T>() where T : IMessage, new()
        {            
            if (ProtoBody == null) Log.Fatal("ProtoBody is invalid");
            return ProtobufUtils.Deserialize<T>(ProtoBody);
        }

        public IMessage Deserialize(Type msgType)
        {
            if (ProtoBody == null) Log.Fatal("ProtoBody is invalid");
            return ProtobufUtils.Deserialize(msgType, ProtoBody);
        }
    }
}