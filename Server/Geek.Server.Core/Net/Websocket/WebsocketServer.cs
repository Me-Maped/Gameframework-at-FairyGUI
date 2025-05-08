using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using NLog.Web;
using System.Net;
using Geek.Server.Core.Net.Websocket;

namespace Geek.Server.Core.Net.Tcp
{
    /// <summary>
    /// TCP server
    /// </summary>
    public static class WebSocketServer
    {
        static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();
        static WebApplication app { get; set; }

        public static Task Start(string url, WebSocketConnectionHandler hander)
        {
            var builder = WebApplication.CreateBuilder(); 

            builder.WebHost.UseUrls(url).UseNLog();
            app = builder.Build();

            app.UseWebSockets(); 

            app.Map("/ws", async context =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    var clientAddress = $"{context.Connection?.RemoteIpAddress}:{context.Connection?.RemotePort}";
                    await hander.OnConnectedAsync(webSocket, clientAddress);
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            });
            Log.Info("启动websocket服务...");
            return app.StartAsync();
        }

        /// <summary>
        /// 停止
        /// </summary>
        public static Task Stop()
        {
            if (app != null)
            {
                Log.Info("停止websocket服务...");
                var task = app.StopAsync();
                app = null;
                return task;
            }
            return Task.CompletedTask;
        }
    }
}
