namespace GameMain
{
    public abstract class SCPacketBase : PacketBase
    {
        public int CmdId { get; set; }
        public override int Id => CmdId;
        public override PacketType PacketType => PacketType.S2C;
    }
}