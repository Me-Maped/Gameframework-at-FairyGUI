using Geek.Server.Core.Net;
using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;

namespace Geek.Server.TestPressure.Logic
{
    public class ClientTcpChannel : NetChannel
    {
        static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        protected Action<Message> onMessage;
        protected Pipe recvPipe;
        protected TcpClient socket;
        public ClientTcpChannel(TcpClient socket, Action<Message> onMessage = null)
        {
            this.socket = socket;
            this.onMessage = onMessage;
            recvPipe = new Pipe();
        }
        public override async Task StartAsync()

        {
            try
            {
                _ = recvNetData();
                var cancelToken = closeSrc.Token;
                while (!cancelToken.IsCancellationRequested)
                {
                    var result = await recvPipe.Reader.ReadAsync(cancelToken);
                    var buffer = result.Buffer;
                    if (buffer.Length > 0)
                    {
                        while (TryParseMessage(ref buffer)) { };
                        recvPipe.Reader.AdvanceTo(buffer.Start, buffer.End);
                    }
                    else if (result.IsCanceled || result.IsCompleted)
                    {
                        break;
                    }
                }
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception e)
            {
                LOGGER.Error(e.Message);
            }
        }

        async Task recvNetData()
        {
            byte[] readBuffer = new byte[2048];
            var dataPipeWriter = recvPipe.Writer;
            var cancelToken = closeSrc.Token;
            while (!cancelToken.IsCancellationRequested)
            { 
                var length = await socket.GetStream().ReadAsync(readBuffer, cancelToken);
                if (length > 0)
                {
                    dataPipeWriter.Write(readBuffer.AsSpan()[..length]);
                    var flushTask = dataPipeWriter.FlushAsync();
                    if (!flushTask.IsCompleted)
                    {
                        await flushTask.ConfigureAwait(false);
                    }
                }
                else
                {
                    break;
                }
            }
            LOGGER.Error($"退出socket接收");
        }

        protected virtual bool TryParseMessage(ref ReadOnlySequence<byte> input)
        {
            var reader = new SequenceReader<byte>(input);
            
            // 1. 读取消息头（4字节长度）
            if (!reader.TryReadBigEndian(out int msgLen) || msgLen < 16)
                return false;

            // 2. 校验剩余数据是否足够（总长度 - 已读的4字节长度）
            if (reader.Remaining < msgLen - 4)
                return false;

            // 3. 读取元数据（8字节时间戳 + 4字节order + 4字节消息ID）
            if (!reader.TryReadBigEndian(out long time) ||
                !reader.TryReadBigEndian(out int order) ||
                !reader.TryReadBigEndian(out int msgId))
            {
                return false;
            }

            // 5. 读取消息体（总长度 - 头部长度）
            int bodyLen = msgLen - 20;
            if (bodyLen < 0)
            {
                LOGGER.Error($"消息长度异常 msgLen:{msgLen}");
                return false;
            }

            if (!PBHelper.Contain(msgId))
            {
                LOGGER.Error($"消息ID:{msgId} 找不到对应的Msg.");
            }
            else
            {
                var message = Message.Create(input.Slice(reader.Position, bodyLen).ToArray(),msgId,order);
                onMessage(message);
            }
            input = input.Slice(input.GetPosition(msgLen));
            return true;
        }

        private const int Magic = 0x1234;
        int count = 0;
        public override void Write(Message msg)
        {
            if (IsClose()) return;
            var bytes = msg.Body;
            int len = 20 + bytes.Length;
            Span<byte> target = stackalloc byte[len];

            int magic = Magic + ++count;
            magic ^= Magic << 8;
            magic ^= len;

            int offset = 0;
            target.WriteInt(len, ref offset);
            target.WriteLong(DateTime.Now.Ticks, ref offset);
            target.WriteInt(msg.UniId, ref offset);
            target.WriteInt(msg.MsgId, ref offset);
            target.WriteBytesWithoutLength(bytes, ref offset);
            lock(socket)
            { 
                socket.GetStream().Write(target);
            }
        }
    }
}
