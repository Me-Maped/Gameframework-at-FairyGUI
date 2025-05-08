using Geek.Server.Core.Net.BaseHandler;

namespace Server.Logic.Logic.Role.Bag
{
    [MsgMapping(CMD.ReqComposePet)]
    public class ReqComposePetHandler : RoleCompHandler<BagCompAgent>
    {
        public override async Task ActionAsync()
        {
            await Comp.ComposePet(Msg);
        }
    }
}
