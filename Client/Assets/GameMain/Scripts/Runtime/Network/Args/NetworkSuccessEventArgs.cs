using GameFramework;
using GameFramework.Event;

namespace GameMain
{
    public class NetworkSuccessEventArgs : GameEventArgs
    {
        public static int EventId => typeof(NetworkSuccessEventArgs).GetHashCode();

        public override int Id => EventId;
        
        public int CmdId { get; set; }
        public int UniId { get; set; }
        public SCNetPacket Packet { get; set; }

        public NetworkSuccessEventArgs()
        {
            CmdId = 0;
            UniId = 0;
            Packet = null;
        }

        public override void Clear()
        {
            CmdId = 0;
            UniId = 0;
            Packet = null;
        }
        
        public static NetworkSuccessEventArgs Create(SCNetPacket scNetPacket)
        {
            var arg = ReferencePool.Acquire<NetworkSuccessEventArgs>();
            arg.Packet = scNetPacket;
            arg.CmdId = scNetPacket.CmdId;
            arg.UniId = scNetPacket.UniId;
            return arg;
        }
    }
}