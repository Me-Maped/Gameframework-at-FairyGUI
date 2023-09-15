
using Geek.Server.App.Common;
using Geek.Server.Core.Net.BaseHandler;
using Google.Protobuf;

namespace Server.Logic.Common.Handler;

public static class NetChannelExtensions
{
    public static void Write(this INetChannel channel, Message msg, int uniId, StateCode code = StateCode.Success, string desc = "")
    {
        if (msg != null)
        {
            msg.UniId = uniId;
            channel.Write(msg);
        }
        else if (uniId > 0)
        {
            ResErrorCode res = new ResErrorCode
            {
                UniId = uniId,
                ErrCode = (int)code,
                Desc = desc
            };
            var message = Message.Create(res);
            message.UniId = uniId;
            channel.Write(message);
        }
    }
}