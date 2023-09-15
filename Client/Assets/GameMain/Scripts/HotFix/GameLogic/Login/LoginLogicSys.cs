using System;
using System.Threading.Tasks;
using FairyGUI;
using GameFramework.Event;
using GameFramework.Network;
using GameLogic.Common;
using GameMain;
using GameMain.Data;
using Pb;
using UGFExtensions.Await;
using UnityEngine;
using UnityGameFramework.Runtime;
using NetworkConnectedEventArgs = UnityGameFramework.Runtime.NetworkConnectedEventArgs;
using NetworkErrorEventArgs = UnityGameFramework.Runtime.NetworkErrorEventArgs;

namespace GameLogic.Login
{
    public class LoginLogicSys : BaseLogicSys<LoginLogicSys>
    {
        public string Token { get; private set; }
        public int ServerId { get; private set; }
        public int UserId { get; private set; }
        public string OpenId { get; private set; }
        
        public override bool OnInit()
        {
            GameModule.WebRequest.SetRequestHeader("Content-Type", "application/json");
            return true;
        }

        public override async void OnStart()
        {
            await OnLoginCountReq();
            UILoadMgr.Instance.SetProgress("正在连接服务器...");
            var channel = GameModule.Network.CreateNetworkChannel("TCP", ServiceType.Tcp, new NetworkChannelHelper());
            channel.Connect("127.0.0.1", 8899, null);
            GameModule.Event.Subscribe(NetworkConnectedEventArgs.EventId, OnWebSocketConnected);
            GameModule.Event.Subscribe(NetworkErrorEventArgs.EventId, OnWebSocketError);
        }

        public override void OnDestroy()
        {
            Token = null;
            ServerId = 0;
            UserId = 0;
            OpenId = null;
        }

        /// <summary>
        /// 账服登录请求
        /// </summary>
        public async Task OnLoginCountReq()
        {
        }

        /// <summary>
        /// 连接游戏服
        /// </summary>
        public async void OnLoginServerReq()
        {
            var req = new ReqLogin();
            req.SdkType = 0;
            req.SdkToken = "555";
            req.UserName = "maped";
            req.Device = "123123";
            req.Platform = "windows";
            var ack = await GameModule.Network.SendAsync<ResLogin>(req);
            Log.Info("Login res = " + ack);
            await GameModule.Network.SendAsync<PingAck>(new PingReq { CmdCode = 1, ProtocolSwitch = 1 });
        }

        /// <summary>
        /// 连接游戏服成功
        /// </summary>
        public async void OnLoginServerSuccess()
        {
            UILoadMgr.Instance.SetProgress("服务器连接成功！");
            await GameModule.Scene.LoadSceneAsync(ResName.GetScene("Game"));
            UILoadMgr.Instance.TweenHide();
        }
        
        /// <summary>
        /// 网络错误处理
        /// </summary>
        /// <param name="result"></param>
        /// <returns>返回true则表示出错</returns>
        private bool CheckWebResultError(WebResult result)
        {
            //TODO 根据不同的错误码做不同的处理
            UILoadMgr.Instance.ShowPop("登录失败", result.ErrorMessage, BtnStyleEnum.CONFIRM_ONLY,
                Application.Quit);
            return true;
        }
        
        /// <summary>
        /// 连接成功事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWebSocketConnected(object sender, GameEventArgs e)
        {
            OnLoginServerReq();
        }
        
        /// <summary>
        /// 连接失败事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnWebSocketError(object sender, GameEventArgs e)
        {
            UILoadMgr.Instance.ShowPop("错误", "连接服务器失败", BtnStyleEnum.CONFIRM_ONLY,
                Application.Quit);
        }
    }
}