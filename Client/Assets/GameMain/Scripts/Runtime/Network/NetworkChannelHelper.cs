using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using GameFramework;
using GameFramework.Event;
using GameFramework.Network;
using UnityGameFramework.Runtime;
using NetworkClosedEventArgs = UnityGameFramework.Runtime.NetworkClosedEventArgs;
using NetworkConnectedEventArgs = UnityGameFramework.Runtime.NetworkConnectedEventArgs;
using NetworkCustomErrorEventArgs = UnityGameFramework.Runtime.NetworkCustomErrorEventArgs;
using NetworkErrorEventArgs = UnityGameFramework.Runtime.NetworkErrorEventArgs;
using NetworkMissHeartBeatEventArgs = UnityGameFramework.Runtime.NetworkMissHeartBeatEventArgs;

namespace GameMain
{
    public class NetworkChannelHelper : INetworkChannelHelper, IReference
    {
        private readonly Dictionary<int, Type> _s2cPacketTypes;
        private MemoryStream _cachedStream;
        private INetworkChannel _netChannel;
        private byte[] _cachedByte;
        private int _netPacketLength;
        private int _netCmdIdLength;
        private int _netOrderLength;

        public int PacketHeaderLength => _netPacketLength + _netOrderLength + _netCmdIdLength;

        public NetworkChannelHelper()
        {
            _s2cPacketTypes = new Dictionary<int, Type>();
            _cachedStream = new MemoryStream(1024);
            _netChannel = null;
            _cachedByte = null;
        }

        public void Initialize(INetworkChannel netChannel)
        {
            _netPacketLength = SettingsUtils.FrameworkGlobalSettings.NetPacketLength;
            _netCmdIdLength = SettingsUtils.FrameworkGlobalSettings.NetCmdIdLength;
            _netOrderLength = SettingsUtils.FrameworkGlobalSettings.NetOrderLength;
            _cachedByte = new byte[PacketHeaderLength];
            _netChannel = netChannel;
            // 反射注册包和包处理函数。
            Type packetBaseType = typeof(SCNetPacket);
            Type packetHandlerBaseType = typeof(PacketHandlerBase);
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();
            foreach (var t in types)
            {
                if (!t.IsClass || t.IsAbstract) continue;
                if (t.BaseType == packetBaseType)
                {
                    PacketBase packetBase = (PacketBase)Activator.CreateInstance(t);
                    Type packetType = GetServerToClientPacketType(packetBase.Id);
                    if (packetType != null)
                    {
                        Log.Warning(
                            $"Already exist packet type '{packetBase.Id}', check '{packetType.Name}' or '{packetBase.GetType().Name}'?.");
                        continue;
                    }
                    _s2cPacketTypes.Add(packetBase.Id, t);
                }
                else if (t.BaseType == packetHandlerBaseType)
                {
                    var packetHandler = (IPacketHandler)Activator.CreateInstance(t);
                    _netChannel.RegisterHandler(packetHandler);
                }
            }
            GameModule.Event.Subscribe(NetworkConnectedEventArgs.EventId, OnNetConnected);
            GameModule.Event.Subscribe(NetworkClosedEventArgs.EventId, OnNetClosed);
            GameModule.Event.Subscribe(NetworkMissHeartBeatEventArgs.EventId, OnNetMissHeartBeat);
            GameModule.Event.Subscribe(NetworkErrorEventArgs.EventId, OnNetError);
            GameModule.Event.Subscribe(NetworkConnectedEventArgs.EventId, OnNetCustomError);
        }

        public void Shutdown()
        {
            GameModule.Event.Unsubscribe(NetworkConnectedEventArgs.EventId, OnNetConnected);
            GameModule.Event.Unsubscribe(NetworkClosedEventArgs.EventId, OnNetClosed);
            GameModule.Event.Unsubscribe(NetworkMissHeartBeatEventArgs.EventId, OnNetMissHeartBeat);
            GameModule.Event.Unsubscribe(NetworkErrorEventArgs.EventId, OnNetError);
            GameModule.Event.Unsubscribe(NetworkConnectedEventArgs.EventId, OnNetCustomError);
            _netChannel = null;
        }

        public void PrepareForConnecting()
        {
            _netChannel.Socket.ReceiveBufferSize = SettingsUtils.FrameworkGlobalSettings.ReceiveBufferSize;
            _netChannel.Socket.SendBufferSize =SettingsUtils.FrameworkGlobalSettings.SendBufferSize;
        }

        public bool SendHeartBeat()
        {
            //TODO
            return true;
        }

        public bool Serialize<T>(T packet, Stream destination) where T : Packet
        {
            var packetImpl = packet as CSNetPacket;
            if (packetImpl == null)
            {
                Log.Warning("Packet is invalid");
                return false;
            }

            if (packetImpl.PacketType != PacketType.C2S)
            {
                Log.Warning("Packet type is invalid");
                return false;
            }

            _cachedStream.Seek(0, SeekOrigin.Begin);
            _cachedStream.SetLength(0);
            Array.Clear(_cachedByte,0,_cachedByte.Length);
            _cachedByte.WriteToReverse(0, packetImpl.ProtoBody.Length);
            _cachedByte.WriteToReverse(_netPacketLength, packetImpl.UniId);
            _cachedByte.WriteToReverse( _netPacketLength+_netOrderLength, packetImpl.Id);
            _cachedStream.Write(_cachedByte, 0, PacketHeaderLength);
            _cachedStream.Write(packetImpl.ProtoBody, 0, packetImpl.ProtoBody.Length);
            _cachedStream.WriteTo(destination);
            ReferencePool.Release(packetImpl);
            return true;
        }

        public IPacketHeader DeserializePacketHeader(Stream source, out object customErrorData)
        {
            customErrorData = null;
            var packetHeader = ReferencePool.Acquire<PacketHeader>();
            if (source is MemoryStream memoryStream)
            {
                var bytes = memoryStream.ToArray();
                var packetSize = bytes.ReadToReverse(0);
                var packetUniId = bytes.ReadToReverse(_netPacketLength);
                var packetCmdId = bytes.ReadToReverse(_netPacketLength);
                packetHeader.PacketLength = packetSize;
                packetHeader.CmdId = packetCmdId;
                packetHeader.UniId = packetUniId;
                Log.Info(Utility.Text.Format("PacketHeader size={0},UniId={1},cmdId={2}", packetSize,
                    packetUniId, packetCmdId));
                return packetHeader;
            }
            return null;
        }

        public Packet DeserializePacket(IPacketHeader packetHeader, Stream source,
            out object customErrorData)
        {
            customErrorData = null;
            var scPacketHeader = packetHeader as PacketHeader;
            if (scPacketHeader == null)
            {
                Log.Warning("PacketHeader is invalid");
                return null;
            }

            var scProtoPacket = ReferencePool.Acquire<SCNetPacket>();
            if (scPacketHeader.IsValid)
            {
                if (source is MemoryStream memoryStream)
                {
                    scProtoPacket.CmdId = scPacketHeader.CmdId;
                    scProtoPacket.UniId = scPacketHeader.UniId;
                    scProtoPacket.ProtoBody = memoryStream.ToArray();
                }
            }
            ReferencePool.Release(scPacketHeader);
            GameModule.Event.Fire(this,NetworkSuccessEventArgs.Create(scProtoPacket));
            return scProtoPacket;
        }

        public void Clear()
        {
            _s2cPacketTypes.Clear();
            _cachedStream = null;
            _netChannel = null;
            _cachedByte = null;
        }
        
        private Type GetServerToClientPacketType(int id)
        {
            return _s2cPacketTypes.TryGetValue(id, out var type) ? type : null;
        }

        private void OnNetConnected(object sender, GameEventArgs e)
        {
            var ne = e as NetworkConnectedEventArgs;
            if (ne == null || ne.NetworkChannel != _netChannel) return;
            Log.Info("网络连接成功......");
        }

        private void OnNetClosed(object sender, GameEventArgs e)
        {
            var ne = e as NetworkClosedEventArgs;
            if (ne==null || ne.NetworkChannel != _netChannel) return;
            Log.Info("网络连接关闭......");
        }

        private void OnNetMissHeartBeat(object sender, GameEventArgs e)
        {
            var ne = e as NetworkMissHeartBeatEventArgs;
            if (ne == null || ne.NetworkChannel != _netChannel) return;
            Log.Warning(Utility.Text.Format("Network channel '{0}' miss heart beat '{1}' times.",ne.NetworkChannel.Name, ne.MissCount));
            // if (ne.MissCount < 2) return;
            // ne.NetChannel.Close();
        }

        private void OnNetError(object sender, GameEventArgs e)
        {
            var ne = e as NetworkErrorEventArgs;
            if (ne == null || ne.NetworkChannel != _netChannel) return;
            Log.Error(Utility.Text.Format("Network channel '{0}' error, error code is '{1}', error message is '{2}'.",ne.NetworkChannel.Name ,ne.ErrorCode, ne.ErrorMessage));
            //ne.NetworkChannel.Close();
        }

        private void OnNetCustomError(object sender, GameEventArgs e)
        {
            var ne = e as NetworkCustomErrorEventArgs;
            if (ne == null || ne.NetworkChannel != _netChannel) return;
        }
    }
}