namespace GameMain
{
    public abstract class PacketBase : GameFramework.Network.Packet
    {
        public byte[] ProtoBody;
        public abstract PacketType PacketType { get; }
    }
}