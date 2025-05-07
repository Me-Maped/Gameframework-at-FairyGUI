using Geek.Server.Core.Net.BaseHandler;

namespace Server.Logic.Logic.Role.Bag
{
    [MsgMapping(CMD.ReqBagInfo)]
    public class ReqBagInfoHandler : RoleCompHandler<BagCompAgent>
    {
        public override async Task ActionAsync()
        {
            await Comp.GetBagInfo(Msg);
        }
    }
}
