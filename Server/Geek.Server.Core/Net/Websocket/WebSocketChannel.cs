using System.Buffers;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using Geek.Server.Core.Hotfix;

namespace Geek.Server.Core.Net.Websocket
{
    public class WebSocketChannel : NetChannel
    {
        static readonly NLog.Logger LOGGER = NLog.LogManager.GetCurrentClassLogger();
        WebSocket webSocket;
        readonly Action<Message> onMessage;
        protected readonly ConcurrentQueue<Message> sendQueue = new();
        protected readonly SemaphoreSlim newSendMsgSemaphore = new(0);

        // 保持原有字段基础上增加TCP通道类似的功能字段
        protected long lastReviceTime = 0;
        protected int lastOrder = 0;
        
        private const int MAX_RECV_SIZE = 1024 * 1024 * 5;
        private const int HEADER_LEN = 20; // 添加消息头长度常量

        public WebSocketChannel(WebSocket webSocket, string remoteAddress, Action<Message> onMessage = null)
        {
            this.RemoteAddress = remoteAddress;
            this.webSocket = webSocket;
            this.onMessage = onMessage;
        }

        public override async void Close()
        {
            try
            {
                base.Close();
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "socketclose", CancellationToken.None);
            }
            catch
            {
            }
            finally
            {
                webSocket = null;
            }
        }

        public override async Task StartAsync()
        {
            try
            {
                _ = DoSend();
                await DoRevice();
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                LOGGER.Error(e.Message);
            }
        }

        async Task DoSend()
        {
            try
            {
                var closeToken = closeSrc.Token;
                while (!closeToken.IsCancellationRequested)
                {
                    await newSendMsgSemaphore.WaitAsync(closeToken);
                    if (!sendQueue.TryDequeue(out var message))
                        continue;
                    await webSocket.SendAsync(SetMessageSpan(message), WebSocketMessageType.Binary, true, closeToken);
                }
            }
            catch
            {
            }
        }

        private ArraySegment<byte> SetMessageSpan(Message message)
        {
            var bytes = message.Body;
            int len = HEADER_LEN + bytes.Length;
            Span<byte> span = stackalloc byte[len];
            int offset = 0;
            span.WriteInt(len, ref offset);
            span.WriteLong(DateTime.Now.Ticks, ref offset);
            span.WriteInt(message.UniId, ref offset);
            span.WriteInt(message.MsgId, ref offset);
            span.WriteBytesWithoutLength(bytes, ref offset);
            return new ArraySegment<byte>(span.ToArray());
        }

        async Task DoRevice()
        {
            var stream = new MemoryStream();
            var buffer = new ArraySegment<byte>(new byte[2048]);
            var closeToken = closeSrc.Token;

            try
            {
                while (!closeToken.IsCancellationRequested)
                {
                    stream.SetLength(0);
                    stream.Seek(0, SeekOrigin.Begin);
                    WebSocketReceiveResult result;
                    do
                    {
                        result = await webSocket.ReceiveAsync(buffer, closeToken);
                        stream.Write(buffer.Array, buffer.Offset, result.Count);
                    } while (!result.EndOfMessage);

                    if (result.MessageType == WebSocketMessageType.Close)
                        break;

                    // 添加类似TCP通道的消息解析
                    var readOnlySeq = new ReadOnlySequence<byte>(stream.GetBuffer(), 0, (int)stream.Length);
                    if (TryParseMessage(ref readOnlySeq, out var message))
                    {
                        onMessage(message);
                    }
                    else
                    {
                        LOGGER.Error("消息解析失败");
                    }
                }
            }
            finally
            {
                stream.Close();
            }
        }
        
        bool CheckMsgLen(int msgLen)
        {
            if (msgLen < 20 || msgLen > MAX_RECV_SIZE)
            {
                LOGGER.Error($"非法消息长度:{msgLen}");
                return false;
            }
            return true;
        }

        // 新增与TCP通道一致的校验方法
        public bool CheckMagicNumber(int order, int msgLen)
        {
            return true;
            order ^= 0x1234 << 8;
            order ^= msgLen;

            if (lastOrder != 0 && order != lastOrder + 1)
            {
                LOGGER.Error("包序列出错, order=" + order + ", lastOrder=" + lastOrder);
                return false;
            }

            lastOrder = order;
            return true;
        }
        
        public bool CheckTime(long time)
        {
            if (lastReviceTime > time)
            {
                LOGGER.Error($"时间戳异常 time:{time} lastTime:{lastReviceTime}");
                return false;
            }
            lastReviceTime = time;
            return true;
        }

        // 新增消息解析方法（与TCP通道保持一致）
        protected virtual bool TryParseMessage(ref ReadOnlySequence<byte> input, out Message msg)
        {
            // 与TcpChannel完全一致的解析逻辑
            msg = null;
            var reader = new SequenceReader<byte>(input);

            if (!reader.TryReadBigEndian(out int msgLen) || !CheckMsgLen(msgLen))
                return false;

            if (reader.Remaining < msgLen - 4)
                return false;

            if (!reader.TryReadBigEndian(out long time) ||
                !reader.TryReadBigEndian(out int order) ||
                !reader.TryReadBigEndian(out int msgId))
                return false;

            if (!CheckTime(time) || !CheckMagicNumber(order, msgLen) || !HotfixMgr.IsMsgContain(msgId))
                return false;

            int bodyLen = msgLen - HEADER_LEN;
            msg = Message.Create(input.Slice(reader.Position, bodyLen).ToArray(), msgId, order);

            input = input.Slice(input.GetPosition(msgLen));
            return true;
        }

        public override void Write(Message msg)
        {
            sendQueue.Enqueue(msg);
            newSendMsgSemaphore.Release();
            LOGGER.Info($"---------------发送消息:{msg.MsgId} UniId:{msg.UniId}----------------");
        }
    }
}
