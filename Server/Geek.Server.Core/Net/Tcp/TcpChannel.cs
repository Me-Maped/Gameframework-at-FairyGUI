using Geek.Server.Core.Hotfix;
using Microsoft.AspNetCore.Connections;
using System.Buffers;
using System.IO.Pipelines;

namespace Geek.Server.Core.Net.Tcp
{
    public class TcpChannel : NetChannel
    {
        static readonly NLog.Logger LOGGER = NLog.LogManager.GetCurrentClassLogger();
        public ConnectionContext Context { get; protected set; }
        protected PipeReader Reader { get; set; }
        protected PipeWriter Writer { get; set; }


        private readonly SemaphoreSlim sendSemaphore = new(0);

        protected Func<Message, Task> onMessage;

        protected long lastReviceTime = 0;
        protected int lastOrder = 0;

        // 批量处理的消息数量
        private const int BATCH_SIZE = 20;

        /// 从客户端接收的包大小最大值（单位：字节 5M）
        private const int MAX_RECV_SIZE = 1024 * 1024 * 5;
        
        // 消息头长度(数据长度+时间戳+magic+msgId)
        private const int HEADER_LEN = 20;

        public TcpChannel(ConnectionContext context, Func<Message, Task> onMessage = null)
        {
            this.onMessage = onMessage;
            Context = context;
            Reader = context.Transport.Input;
            Writer = context.Transport.Output;
            RemoteAddress = context.RemoteEndPoint?.ToString();
        }

        public override async Task StartAsync()
        {
            Task reading = ReadAsync();
            Task writing = SendAsync();
            await Task.WhenAll(reading, writing);
            Close();
        }

        async Task ReadAsync()
        {
            try
            {
                var token = Context.ConnectionClosed;
                while (!token.IsCancellationRequested)
                {
                    var result = await Reader.ReadAsync(token);
                    var buffer = result.Buffer;
                    try
                    {
                        if (result.IsCanceled)
                            break;

                        int count = 0;
                        while (TryParseMessage(ref buffer, out var msg))
                        {
                            await onMessage?.Invoke(msg);
                            if (++count > BATCH_SIZE)
                            {
                                await Task.Yield();
                                count = 0;
                            }
                        }

                        ;
                        if (result.IsCompleted)
                        {
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        LOGGER.Error(e);
                        break;
                    }
                    finally
                    {
                        Reader.AdvanceTo(buffer.Start, buffer.End);
                    }
                }
            }
            catch
            {
            }
            finally
            {
                await Reader.CompleteAsync();
            }
        }

        async Task SendAsync()
        {
            var token = Context.ConnectionClosed;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        await sendSemaphore.WaitAsync(token);
                        var flush = await Writer.FlushAsync(token);
                        if (flush.IsCompleted || flush.IsCanceled)
                            break;
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            catch
            {
            }
            finally
            {
                await Writer.CompleteAsync();
            }
        }

        protected virtual bool TryParseMessage(ref ReadOnlySequence<byte> input, out Message msg)
        {
            msg = null;
            var reader = new SequenceReader<byte>(input);

            // 1. 读取消息头（4字节长度）
            if (!reader.TryReadBigEndian(out int msgLen) || !CheckMsgLen(msgLen))
                return false;

            // 2. 校验剩余数据是否足够（总长度 - 已读的4字节长度）
            if (reader.Remaining < msgLen - 4)
                return false;

            // 3. 读取元数据（8字节时间戳 + 4字节uniId + 4字节消息ID）
            if (!reader.TryReadBigEndian(out long time) ||
                !reader.TryReadBigEndian(out int uniId) ||
                !reader.TryReadBigEndian(out int msgId))
            {
                return false;
            }

            // 4. 业务校验（时间戳 + 顺序号 + 消息ID）
            if (!CheckTime(time) || !CheckMagicNumber(uniId, msgLen) || !HotfixMgr.IsMsgContain(msgId))
            {
                LOGGER.Error($"消息校验失败 time:{time} uniId:{uniId} msgId:{msgId}");
                return false;
            }

            // 5. 读取消息体（总长度 - 头部长度）
            int bodyLen = msgLen - HEADER_LEN;
            if (bodyLen < 0)
            {
                LOGGER.Error($"消息长度异常 msgLen:{msgLen}");
                return false;
            }

            msg = Message.Create(input.Slice(reader.Position, bodyLen).ToArray(),msgId,uniId);

            // 6. 移动读取位置
            input = input.Slice(input.GetPosition(msgLen));
            return true;
        }


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

        /// <summary>
        /// 检查消息长度是否合法
        /// </summary>
        /// <param name="msgLen"></param>
        /// <returns></returns>
        public bool CheckMsgLen(int msgLen)
        {
            if (msgLen < HEADER_LEN-4)
            {
                LOGGER.Error($"消息头不完整，长度:{msgLen}");
                return false;
            }
            else if (msgLen > MAX_RECV_SIZE)
            {
                LOGGER.Error("从客户端接收的包大小超过限制：" + msgLen + "字节，最大值：" + MAX_RECV_SIZE / 1024 + "字节");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 时间戳检查(可以防止客户端游戏过程中修改时间)
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool CheckTime(long time)
        {
            if (lastReviceTime > time)
            {
                LOGGER.Error("时间戳出错，time=" + time + ", lastTime=" + lastReviceTime);
                return false;
            }

            lastReviceTime = time;
            return true;
        }

        public override void Write(Message msg)
        {
            if (IsClose()) return;
            var bytes = msg.Body;
            int len = HEADER_LEN + bytes.Length;
            Span<byte> span = stackalloc byte[len];
            int offset = 0;
            span.WriteInt(len, ref offset);
            span.WriteLong(DateTime.Now.Ticks, ref offset);
            span.WriteInt(msg.UniId, ref offset);
            span.WriteInt(msg.MsgId, ref offset);
            span.WriteBytesWithoutLength(bytes, ref offset);
            LOGGER.Info($"---------------发送消息:{msg.MsgId} UniId:{msg.UniId}----------------");

            lock (Writer)
            {
                Writer.Write(span);
            }

            if (sendSemaphore.CurrentCount == 0)
                sendSemaphore.Release();
        }

        public override void Close()
        {
            if (IsClose())
                return;

            lock (this)
            {
                if (Context == null)
                    return;
                try
                {
                    sendSemaphore.Release(short.MaxValue);
                    sendSemaphore?.Dispose();
                }
                catch
                {
                }

                try
                {
                    Context.Abort();
                }
                catch
                {
                }

                try
                {
                    Context.DisposeAsync();
                }
                catch
                {
                }

                Context = null;
            }
        }

        public override bool IsClose()
        {
            return Context == null || Context.ConnectionClosed.IsCancellationRequested;
        }
    }
}