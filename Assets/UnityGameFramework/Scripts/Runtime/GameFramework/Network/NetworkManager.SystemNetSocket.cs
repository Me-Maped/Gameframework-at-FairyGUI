using System;
using System.Net;
using System.Net.Sockets;

namespace GameFramework.Network
{
    internal sealed partial class NetworkManager : GameFrameworkModule, INetworkManager
    {
        private sealed class SystemNetSocket : INetworkSocket
        {
            private Socket m_Socket;
            public bool Connected => m_Socket.Connected;
            public object LocalEndPoint => m_Socket.LocalEndPoint;
            public object RemoteEndPoint => m_Socket.RemoteEndPoint;

            public int ReceiveBufferSize
            {
                get => m_Socket.ReceiveBufferSize;
                set
                {
                    if (value <= 0) throw new GameFrameworkException("Receive buffer size is invalid.");
                    m_Socket.ReceiveBufferSize = value;
                }
            }

            public int SendBufferSize
            {
                get => m_Socket.SendBufferSize;
                set
                {
                    if (value <= 0) throw new GameFrameworkException("Send buffer size is invalid.");
                    m_Socket.SendBufferSize = value;
                }
            }

            public int Available => m_Socket.Available;

            public void Shutdown(SocketShutdownType socketShutdownType)
            {
                m_Socket.Shutdown((SocketShutdown)(int)socketShutdownType);
            }
            public void Close()
            {
               m_Socket.Close();
            }
            
            public SystemNetSocket(System.Net.Sockets.AddressFamily ipAddressAddressFamily, SocketType stream, ProtocolType tcp)
            {
                m_Socket = new Socket(ipAddressAddressFamily, stream, tcp);
            }

            public void BeginSend(byte[] getBuffer, int streamPosition, int streamLength, SocketFlags none, AsyncCallback mSendCallback, INetworkSocket mSocket)
            {
                m_Socket.BeginSend(getBuffer, streamPosition, streamLength, none, mSendCallback, mSocket);
            }

            public void BeginReceive(byte[] getBuffer, int streamPosition, int streamLength, SocketFlags none, AsyncCallback mReceiveCallback, INetworkSocket mSocket)
            {
                m_Socket.BeginReceive(getBuffer, streamPosition, streamLength, none, mReceiveCallback, mSocket);
            }

            public void BeginConnect(IPAddress ipAddress, int port, AsyncCallback mConnectCallback, ConnectState connectState)
            {
                m_Socket.BeginConnect(ipAddress, port, mConnectCallback, connectState);
            }

            public void EndConnect(IAsyncResult ar)
            {
                m_Socket.EndConnect(ar);
            }

            public int Receive(byte[] getBuffer, int streamPosition, int streamLength, SocketFlags none)
            {
                return m_Socket.Receive(getBuffer, streamPosition, streamLength, none);
            }
        }
    }
}