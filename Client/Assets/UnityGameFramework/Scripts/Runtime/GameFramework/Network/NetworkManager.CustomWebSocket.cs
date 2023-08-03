using System;
using System.IO;
using UnityWebSocket;
using ErrorEventArgs = UnityWebSocket.ErrorEventArgs;

namespace GameFramework.Network
{
    internal sealed partial class NetworkManager : GameFrameworkModule, INetworkManager
    {
        private sealed class CustomWebSocket : INetworkSocket
        {
            private IWebSocket m_Socket;
            public bool Connected => m_Socket.ReadyState == WebSocketState.Open;
            public object LocalEndPoint => string.Empty;
            public object RemoteEndPoint => string.Empty;
            public int ReceiveBufferSize { get; set; }
            public int SendBufferSize { get; set; }
            private MemoryStream m_CachedStream;

            public void Shutdown(SocketShutdownType socketShutdownType)
            {
                m_Socket.CloseAsync();
            }

            public void Close()
            {
                m_Socket.CloseAsync();
            }
            
            public CustomWebSocket(string url, EventHandler<OpenEventArgs> onWebSocketOpen, EventHandler<MessageEventArgs> onWebSocketMessage, EventHandler<CloseEventArgs> onWebSocketClose, EventHandler<ErrorEventArgs> onWebSocketError)
            {
                m_CachedStream = new MemoryStream(1024);
                m_Socket = new WebSocket(url);
                m_Socket.OnOpen += onWebSocketOpen;
                m_Socket.OnMessage += onWebSocketMessage;
                m_Socket.OnError += onWebSocketError;
                m_Socket.OnClose += onWebSocketClose;
            }

            public void BeginConnect()
            {
                m_Socket.ConnectAsync();
            }

            public void BeginClose()
            {
                m_Socket.CloseAsync();
            }

            public void BeginSend(byte[] data)
            {
                m_Socket.SendAsync(data);
            }

            public void BeginSend(string data)
            {
                m_Socket.SendAsync(data);
            }

            public void BeginSend(byte[] data, int streamPosition, int streamLength)
            {
                m_CachedStream.Seek(0, SeekOrigin.Begin);
                m_CachedStream.SetLength(0);
                m_CachedStream.Write(data, streamPosition, streamLength);
                m_Socket.SendAsync(m_CachedStream.ToArray());
            }
        }
    }
}