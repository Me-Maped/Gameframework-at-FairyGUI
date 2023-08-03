namespace GameMain
{
    public class SCHeartBeatHandler : PacketHandlerBase
    {
        public override int Id => (int) PacketType.S2C;

        public override void Handle(object sender, SCProtoPacket packet)
        {
            
        }
    }
}