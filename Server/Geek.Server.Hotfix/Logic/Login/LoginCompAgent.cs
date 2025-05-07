using Geek.Server.App.Common;
using Geek.Server.App.Common.Session;
using Geek.Server.App.Logic.Login;
using Geek.Server.Core.Actors;
using Geek.Server.Core.Hotfix.Agent;
using Geek.Server.Core.Net;
using Geek.Server.Core.Utils;
using Server.Logic.Logic.Role.Base;
using Server.Logic.Logic.Server;

namespace Server.Logic.Logic.Login
{
    public class LoginCompAgent : StateCompAgent<LoginComp, LoginState>
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public async Task OnLogin(NetChannel channel, Message msg)
        {
            var reqLogin = msg.Deserialize<ReqLogin>();
            if (string.IsNullOrEmpty(reqLogin.UserName))
            {
                ResErrorCode res = new ResErrorCode
                {
                    UniId = msg.UniId,
                    ErrCode = (int)StateCode.AccountCannotBeNull,
                    Desc = "Account cannot be null"
                };
                channel.Write(Message.Create(res, msg.UniId));
                return;
            }

            if (reqLogin.Platform != "android" && reqLogin.Platform != "ios" && reqLogin.Platform != "unity")
            {
                //验证平台合法性
                ResErrorCode res = new ResErrorCode
                {
                    UniId = msg.UniId,
                    ErrCode = (int)StateCode.UnknownPlatform,
                    Desc = "Unknown platform"
                }; 
                channel.Write(Message.Create(res, msg.UniId));
                return;
            }

            //查询角色账号，这里设定每个服务器只能有一个角色
            var roleId = GetRoleIdOfPlayer(reqLogin.UserName, reqLogin.SdkType);
            var isNewRole = roleId <= 0;
            if (isNewRole)
            {
                //没有老角色，创建新号
                roleId = IdGenerator.GetActorID(ActorType.Role);
                CreateRoleToPlayer(reqLogin.UserName, reqLogin.SdkType, roleId);
                Log.Info("创建新号:" + roleId);
            }

            //添加到session
            var session = new Session
            {
                Id = roleId,
                Time = DateTime.Now,
                Channel = channel,
                Sign = reqLogin.Device
            };
            SessionManager.Add(session);

            //登陆流程
            var roleComp = await ActorMgr.GetCompAgent<RoleCompAgent>(roleId);
            //从登录线程-->调用Role线程 所以需要入队
            var resLogin = await roleComp.OnLogin(reqLogin, isNewRole);
            channel.Write(Message.Create(resLogin,msg.UniId));

            //加入在线玩家
            var serverComp = await ActorMgr.GetCompAgent<ServerCompAgent>();
            await serverComp.AddOnlineRole(ActorId);
        }

        private long GetRoleIdOfPlayer(string userName, int sdkType)
        {
            var playerId = $"{sdkType}_{userName}";
            if (State.PlayerMap.TryGetValue(playerId, out var state))
            {
                if (state.RoleMap.TryGetValue(Settings.ServerId, out var roleId))
                    return roleId;
                return 0;
            }
            return 0;
        }

        private void CreateRoleToPlayer(string userName, int sdkType, long roleId)
        {
            var playerId = $"{sdkType}_{userName}";
            State.PlayerMap.TryGetValue(playerId, out var info);
            if (info == null)
            {
                info = new PlayerInfo();
                info.playerId = playerId;
                info.SdkType = sdkType;
                info.UserName = userName;
                State.PlayerMap[playerId] = info;
            } 
            info.RoleMap[Settings.ServerId] = roleId;
        }

    }
}