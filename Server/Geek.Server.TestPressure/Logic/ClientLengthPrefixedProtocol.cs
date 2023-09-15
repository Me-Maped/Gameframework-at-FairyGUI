using Bedrock.Framework;
using Bedrock.Framework.Protocols;
using System.Buffers;
using Geek.Server.Core.Reference;

namespace Geek.Server.TestPressure.Logic
{
    public class ClientLengthPrefixedProtocol : IProtocal<Message>
    {
         private static readonly NLog.Logger LOGGER = NLog.LogManager.GetCurrentClassLogger();

        private long lastReceiveTime = 0;
        private int lastOrder = 0;

        private const int HEAD_LEN_SIZE = 4;
        private const int HEAD_ORDER_SIZE = 4;
        private const int HEAD_CMD_SIZE = 4;
        private const int HEAD_SIZE = HEAD_LEN_SIZE + HEAD_ORDER_SIZE + HEAD_CMD_SIZE;

        private const int MAX_RECV_SIZE = 1024 * 1024 * 5; /// 从客户端接收的包大小最大值（单位：字节 5M）

        public bool TryParseMessage(in ReadOnlySequence<byte> input, ref SequencePosition consumed, ref SequencePosition examined, out Message message)
        {
            message = Message.Create();
            var bufEnd = input.End;
            var reader = new SequenceReader<byte>(input);

            // 长度小于包头长度，告诉read task，到这里为止还不满足一个消息的长度，继续等待更多数据
            if (reader.Remaining < HEAD_SIZE)
            {
                consumed = bufEnd;
                return false;
            }
            
            if (!reader.TryReadBigEndian(out int msgLen))
            {
                consumed = bufEnd;
                return false;
            }

            if (!CheckMsgLen(msgLen,reader.Remaining))
            {
                throw new ProtocalParseErrorException("消息长度异常");
            }

            reader.TryReadBigEndian(out int order);  //4
            if (!CheckOrder(order))
            {
                throw new ProtocalParseErrorException("消息order错乱");
            }

            reader.TryReadBigEndian(out int msgId);  //4

            consumed = reader.Sequence.End;
            examined = consumed;

            if (!PBHelper.ProtoTypeDic.ContainsValue(msgId))
            {
                LOGGER.Error("消息ID:{} 找不到对应的Msg.", msgId);
            }
            else
            {
                message.Body = input.Slice(reader.Position,msgLen).ToArray();
                message.Cmd = msgId;
                message.UniId = order;

                // LOGGER.Debug($"收到消息:(Len:{msgLen},Order:{order},Cmd:{msgId})");
            }
            return true;
        }

        public void WriteMessage(Message msg, IBufferWriter<byte> output)
        {
            int dataLen = msg.Body.Length;
            int len = HEAD_SIZE + dataLen;
            var span = output.GetSpan(len);
            span.Clear();
            int offset = 0;
            // lastOrder++;
            span.WriteInt(dataLen, ref offset);
            span.WriteInt(msg.UniId, ref offset);
            span.WriteInt(msg.Cmd, ref offset);
            span.WriteBytesWithoutLength(msg.Body, ref offset);
            output.Advance(len);
            // ReferencePool.Release(msg);
        }

        public bool CheckOrder(int order)
        {
            // if (order != lastOrder + 1)
            // {
            //     LOGGER.Error("包序列出错, order=" + order + ", lastOrder=" + lastOrder);
            //     return false;
            // }
            // lastOrder = order;
            return true;
        }

        /// <summary>
        /// 检查消息长度是否合法
        /// </summary>
        /// <param name="dataLen"></param>
        /// <param name="msgLen"></param>
        /// <returns></returns>
        public bool CheckMsgLen(int dataLen,long msgLen)
        {
            //消息长度+magic+消息id+数据
            //4 + 4 + 4 + data
            if (msgLen < HEAD_SIZE-HEAD_LEN_SIZE)//(消息长度已经被读取)
            {
                LOGGER.Error("从客户端接收的包大小异常:" + msgLen + ":至少16个字节");
                return false;
            }

            if (dataLen > MAX_RECV_SIZE)
            {
                LOGGER.Error("从客户端接收的包大小超过限制：" + dataLen + "字节，最大值：" + MAX_RECV_SIZE / 1024 + "字节");
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
            if (lastReceiveTime > time)
            {
                LOGGER.Error("时间戳出错，time=" + time + ", lastTime=" + lastReceiveTime);
                return false;
            }
            lastReceiveTime = time;
            return true;
        }
    }
}
