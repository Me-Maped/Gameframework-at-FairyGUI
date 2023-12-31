using GameFramework.Network;

namespace GameMain
{
    public abstract class PacketHandlerBase : IPacketHandler
    {
        public abstract int Id { get; }
        public abstract void Handle(object sender, SCNetPacket packet);

        public void Handle(object sender, Packet packet)
        {
            var scPacket = packet as SCNetPacket;
            if (scPacket == null || scPacket.PacketType != PacketType.S2C) return;
            Handle(sender, scPacket);
        }
    }
}