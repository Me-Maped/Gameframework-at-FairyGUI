namespace GameFramework.Network
{
    public interface INetworkSocket
    {
        bool Connected { get; }
        object LocalEndPoint { get; }
        object RemoteEndPoint { get; }
        int ReceiveBufferSize { get; set; }
        int SendBufferSize { get; set; }
        void Shutdown(SocketShutdownType socketShutdownType);

        void Close();
    }
}