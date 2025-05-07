﻿
using Geek.Server.Core.Net.BaseHandler;

namespace Server.Logic.Logic.Login
{
    [MsgMapping(CMD.ReqLogin)]
    internal class ReqLoginHandler : GlobalCompHandler<LoginCompAgent>
    {
        public override async Task ActionAsync()
        {
            await Comp.OnLogin(Channel, Msg);
        }
    }
}
