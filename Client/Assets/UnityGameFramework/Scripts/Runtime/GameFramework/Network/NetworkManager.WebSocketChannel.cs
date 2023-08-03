using System;
using System.Net.Sockets;

namespace GameFramework.Network
{
    internal sealed partial class NetworkManager : GameFrameworkModule, INetworkManager
    {
        /// <summary>
        /// TCP 网络频道。
        /// </summary>
        private sealed class WebSocketNetworkChannel : NetworkChannelBase
        {
            /// <summary>
            /// 用户数据
            /// </summary>
            private object m_UserData;
            /// <summary>
            /// 初始化网络频道的新实例。
            /// </summary>
            /// <param name="name">网络频道名称。</param>
            /// <param name="networkChannelHelper">网络频道辅助器。</param>
            public WebSocketNetworkChannel(string name, INetworkChannelHelper networkChannelHelper)
                : base(name, networkChannelHelper)
            {
            }

            /// <summary>
            /// 获取网络服务类型。
            /// </summary>
            public override ServiceType ServiceType
            {
                get
                {
                    return ServiceType.WebSocket;
                }
            }

            /// <summary>
            /// 连接到远程主机。
            /// </summary>
            /// <param name="url">远程主机的 IP 地址。</param>
            /// <param name="port">远程主机的端口号。</param>
            /// <param name="userData">用户自定义数据。</param>
            public override void Connect(string url, int port, object userData)
            {
                if (m_Socket != null)
                {
                    Close();
                    m_Socket = null;
                }
                m_AddressFamily = AddressFamily.IPv4;
                m_SendState.Reset();
                m_ReceiveState.PrepareForPacketHeader(m_NetworkChannelHelper.PacketHeaderLength);
                m_UserData = userData;
                m_Socket = new CustomWebSocket(Utility.Text.Format("{0}:{1}", url, port),OnWebSocketOpen,OnWebSocketMessage,OnWebSocketClose,OnWebSocketError);
                if (m_Socket == null)
                {
                    string errorMessage = "Initialize network channel failure.";
                    if (NetworkChannelError != null)
                    {
                        NetworkChannelError(this, NetworkErrorCode.SocketError, SocketError.Success, errorMessage);
                        return;
                    }
                    throw new GameFrameworkException(errorMessage);
                }
                m_NetworkChannelHelper.PrepareForConnecting();
                ConnectAsync();
            }

            protected override bool ProcessSend()
            {
                if (base.ProcessSend())
                {
                    SendAsync();
                    return true;
                }
                return false;
            }

            protected override bool ProcessPacketHeader()
            {
                try
                {
                    object customErrorData = null;
                    IPacketHeader packetHeader = m_NetworkChannelHelper.DeserializePacketHeader(m_ReceiveState.Stream, out customErrorData);

                    if (customErrorData != null && NetworkChannelCustomError != null)
                    {
                        NetworkChannelCustomError(this, customErrorData);
                    }

                    if (packetHeader == null)
                    {
                        string errorMessage = "Packet header is invalid.";
                        if (NetworkChannelError != null)
                        {
                            NetworkChannelError(this, NetworkErrorCode.DeserializePacketHeaderError, SocketError.Success, errorMessage);
                            return false;
                        }
                        throw new GameFrameworkException(errorMessage);
                    }
                    m_ReceiveState.PrepareForPacket(m_NetworkChannelHelper.PacketHeaderLength,(int)m_ReceiveState.Stream.Length, packetHeader);
                }
                catch (Exception exception)
                {
                    m_Active = false;
                    if (NetworkChannelError != null)
                    {
                        SocketException socketException = exception as SocketException;
                        NetworkChannelError(this, NetworkErrorCode.DeserializePacketHeaderError, socketException != null ? socketException.SocketErrorCode : SocketError.Success, exception.ToString());
                        return false;
                    }
                    throw;
                }

                return true;
            }

            private void ConnectAsync()
            {
                try
                {
                    ((CustomWebSocket)m_Socket).BeginConnect();
                }
                catch (Exception exception)
                {
                    if (NetworkChannelError != null)
                    {
                        NetworkChannelError(this, NetworkErrorCode.ConnectError, 0, exception.ToString());
                        return;
                    }
                    throw;
                }
            }

            private void SendAsync()
            {
                try
                {
                    ((CustomWebSocket)m_Socket).BeginSend(m_SendState.Stream.GetBuffer(), (int)m_SendState.Stream.Position, (int)(m_SendState.Stream.Length - m_SendState.Stream.Position));
                }
                catch (Exception exception)
                {
                    m_Active = false;
                    if (NetworkChannelError != null)
                    {
                        NetworkChannelError(this, NetworkErrorCode.SendError, 0, exception.ToString());
                        return;
                    }
                    throw;
                }
                m_SendState.Reset();
                m_SentPacketCount++;
            }

            private void OnWebSocketError(object sender, UnityWebSocket.ErrorEventArgs e)
            {
                NetworkChannelError(this, NetworkErrorCode.ReceiveError, 0, e.Message);
                throw new GameFrameworkException(e.Message);
            }

            private void OnWebSocketOpen(object sender, UnityWebSocket.OpenEventArgs e)
            {
                m_SentPacketCount = 0;
                m_ReceivedPacketCount = 0;
                lock (m_SendPacketPool)
                {
                    m_SendPacketPool.Clear();
                }
                m_ReceivePacketPool.Clear();
                lock (m_HeartBeatState)
                {
                    m_HeartBeatState.Reset(true);
                }
                if (NetworkChannelConnected != null)
                {
                    NetworkChannelConnected(this, m_UserData);
                }
                m_Active = true;
            }

            private void OnWebSocketClose(object sender, UnityWebSocket.CloseEventArgs e)
            {
                NetworkChannelClosed(this);
            }

            private void OnWebSocketMessage(object sender, UnityWebSocket.MessageEventArgs e)
            {
                if (!m_Socket.Connected) return;
                //清除Stream内的数据
                m_ReceiveState.Stream.Position = 0L;
                m_ReceiveState.Stream.Write(e.RawData, 0, e.RawData.Length);
                if (ProcessPacketHeader() && ProcessPacket())
                {
                    m_ReceivedPacketCount++;
                }
            }
        }
    }
}
