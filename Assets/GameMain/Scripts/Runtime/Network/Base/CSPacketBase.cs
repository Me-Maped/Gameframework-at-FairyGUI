namespace GameMain
{
    public abstract class CSPacketBase : PacketBase
    {
        public int CmdId = -1;
        public override int Id => CmdId;
        public override PacketType PacketType => PacketType.C2S;
    }
}