using GameFramework;
using GameFramework.Network;

namespace GameMain
{
    public class PacketHeader : IPacketHeader,IReference
    {
        public int CmdId { get; set; }
        public int PacketLength { get; set; }
        public bool IsValid => CmdId > 0;
        public void Clear()
        {
            PacketLength = 0;
            CmdId = 0;
        }
    }
}