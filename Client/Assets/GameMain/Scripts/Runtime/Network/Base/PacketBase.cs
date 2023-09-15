namespace GameMain
{
    public abstract class PacketBase : GameFramework.Network.Packet
    {
        public byte[] ProtoBody;
        public int CmdId;
        public int UniId;
        public abstract PacketType PacketType { get; }
    }
}