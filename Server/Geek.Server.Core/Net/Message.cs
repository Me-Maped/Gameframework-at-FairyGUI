using Geek.Server.Core.Hotfix;
using Geek.Server.Core.Reference;
using Google.Protobuf;

public class Message : IReference
{
    /// <summary>
    /// 消息唯一id
    /// </summary>
    public int UniId { get; set; }
    
    /// <summary>
    /// 协议号
    /// </summary>
    public int MsgId { get; set; }
    
    /// <summary>
    /// 协议内容
    /// </summary>
    public byte[] Body { get; set; }

    public Message()
    {
        Clear();
    }

    public T Deserialize<T>() where T : IMessage, new()
    {
        T msg = new T();
        if (Body == null) return msg;
        msg = (T)msg.Descriptor.Parser.ParseFrom(Body);
        return msg;
    }

    public void Clear()
    {
        UniId = -1;
        MsgId = -1;
        if (Body != null && Body.Length > 0)
        {
            Array.Clear(Body, 0, Body.Length);
        }
        Body = null;
    }
    
    public static Message Create(IMessage msg,int uniId = -1)
    {
        var newMsg = ReferencePool.Acquire<Message>();
        newMsg.Body = msg.ToByteArray();
        newMsg.MsgId = HotfixMgr.GetMsgCmd(msg.GetType());
        newMsg.UniId = uniId;
        return newMsg;
    }

    public static Message Create(byte[] body, int msgId, int uniId)
    {
        var newMsg = ReferencePool.Acquire<Message>();
        newMsg.Body = body;
        newMsg.MsgId = msgId;
        newMsg.UniId = uniId;
        return newMsg;
    }
    
    [Obsolete("Only for test client")]
    public static Message Create(int cmd, IMessage msg)
    {
        var newMsg = ReferencePool.Acquire<Message>();
        newMsg.Body = msg.ToByteArray();
        newMsg.MsgId = cmd;
        return newMsg;
    }
}