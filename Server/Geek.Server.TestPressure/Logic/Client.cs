using Bedrock.Framework;
using Newtonsoft.Json;
using System.Net.Sockets;
using Geek.Server.Core.Net.BaseHandler;
using Geek.Server.Core.Net.Tcp;
using Geek.Server.Core.Net.Websocket;
using System.Net.WebSockets;
using Geek.Server.Core.Reference;

namespace Geek.Server.TestPressure.Logic
{
    public enum ServerErrorCode
    {
        Success = 0,
        ConfigErr = 400, //配置表错误
        ParamErr, //客户端传递参数错误
        CostNotEnough, //消耗不足

        Notice = 100000, //正常通知
        FuncNotOpen, //功能未开启，主消息屏蔽
        Other //其他
    }

    public class Client
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        long id;
        INetChannel netChannel;
        MsgWaiter msgWaiter = new();
        int msgUniId = 1;

        public Client(long id)
        {
            this.id = id;
        }

        public async void Start()
        {
            if (TestSettings.Ins.useWebSocket)
            {
                var ws = new ClientWebSocket();
                await ws.ConnectAsync(new Uri(TestSettings.Ins.webSocketServerUrl), CancellationToken.None);

                if (ws.State == WebSocketState.Open)
                {
                    Log.Info($"Connected to {TestSettings.Ins.webSocketServerUrl}");
                    netChannel = new WebSocketChannel(ws, TestSettings.Ins.webSocketServerUrl,
                        new ClientLengthPrefixedProtocol(), OnReceive, OnDisConnected);
                    _ = netChannel.StartAsync();
                }
                else
                {
                    Log.Error($"连接服务器失败...");
                    return;
                }
            }
            else
            {
                var context = await new SocketConnection(AddressFamily.InterNetwork, TestSettings.Ins.serverIp,
                    TestSettings.Ins.serverPort).StartAsync(5000);
                if (context != null)
                {
                    Log.Info($"Connected to {context.LocalEndPoint}");
                    netChannel = new TcpChannel(context, new ClientLengthPrefixedProtocol(), OnReceive, OnDisConnected);
                    _ = netChannel.StartAsync();
                }
                else
                {
                    Log.Error($"连接服务器失败...");
                    return;
                }
            }


            await ReqLogin();

            for (int i = 0; i < 5; i++)
            {
                await ReqBagInfo();
                await Task.Delay(1000);
            }

            await ReqComposePet();
        }

        private Task<bool> ReqLogin()
        {
            //登陆
            var req = new ReqLogin();
            req.SdkType = 0;
            req.SdkToken = "555";
            req.UserName = "name" + id;
            req.Device = new Random().NextInt64().ToString();
            req.Platform = "android";
            return SendMsgAndWaitBack(Message.Create(CMD.ReqLogin, req));
        }

        private Task ReqBagInfo()
        {
            return SendMsgAndWaitBack(Message.Create(CMD.ReqBagInfo, new ReqBagInfo()));
        }

        private Task ReqComposePet()
        {
            return SendMsgAndWaitBack(Message.Create(CMD.ReqComposePet, new ReqComposePet { FragmentId = 1000 }));
        }

        void SendMsg(Message msg)
        {
            msg.UniId = msgUniId++;
            Log.Info($"{id} 发送消息:(UniId:{msg.UniId},Cmd:{msg.Cmd})");
            netChannel.Write(msg);
        }

        async Task<bool> SendMsgAndWaitBack(Message msg)
        {
            SendMsg(msg);
            return await msgWaiter.StartWait(msg.UniId);
        }

        public void OnDisConnected()
        {
        }


        private void OnReceive(Message msg)
        {
            // Log.Error($"收到消息:{msg.Cmd} {MsgFactory.GetType(msg.Cmd)}");

            if (msg.Cmd == CMD.ResErrorCode)
            {
                ResErrorCode errMsg = msg.Deserialize<ResErrorCode>();
                switch (errMsg.ErrCode)
                {
                    case (int)ServerErrorCode.Success:
                        //do some thing
                        break;
                    case (int)ServerErrorCode.ConfigErr:
                        //do some thing
                        break;
                    //case ....
                    default:
                        break;
                }

                msgWaiter.EndWait(errMsg.UniId, errMsg.ErrCode == (int)ServerErrorCode.Success);
                if (!string.IsNullOrEmpty(errMsg.Desc))
                    Log.Info("服务器提示:" + errMsg.Desc);
                return;
            }

            msgWaiter.EndWait(msg.UniId);
            Log.Info($"{id} 收到消息:(UniId:{msg.UniId},Cmd:{msg.Cmd})");
        }
    }
}