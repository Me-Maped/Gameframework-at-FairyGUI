namespace GameFramework.Network
{
    internal sealed partial class NetworkManager : GameFrameworkModule, INetworkManager
    {
        private sealed class ConnectState
        {
            private readonly INetworkSocket m_Socket;
            private readonly object m_UserData;

            public ConnectState(INetworkSocket socket, object userData)
            {
                m_Socket = socket;
                m_UserData = userData;
            }

            public INetworkSocket Socket
            {
                get
                {
                    return m_Socket;
                }
            }

            public object UserData
            {
                get
                {
                    return m_UserData;
                }
            }
        }
    }
}
