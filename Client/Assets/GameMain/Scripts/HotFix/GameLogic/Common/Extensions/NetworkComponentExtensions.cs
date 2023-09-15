using System.Threading.Tasks;
using Google.Protobuf;
using Pb;
using UnityGameFramework.Runtime;

namespace GameLogic.Common
{
    public static class NetworkComponentExtensions
    {
        public static async Task<T> SendAsync<T>(this NetworkComponent networkComponent, IMessage msg)
            where T : IMessage
        {
            var ack = await UGFExtensions.Await.AwaitableExtensions.SendAsync<T>(networkComponent, PBHelper.Get(msg.GetType()),
                msg);
            return (T)ack;
        }
    }
}