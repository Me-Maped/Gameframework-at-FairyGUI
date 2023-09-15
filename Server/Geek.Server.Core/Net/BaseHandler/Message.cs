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
    public int Cmd { get; set; }
    
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
        Cmd = -1;
        if (Body != null && Body.Length > 0)
        {
            Array.Clear(Body, 0, Body.Length);
        }
        Body = null;
    }
    
    public static Message Create(IMessage msg)
    {
        var newMsg = ReferencePool.Acquire<Message>();
        newMsg.Body = msg.ToByteArray();
        newMsg.Cmd = HotfixMgr.GetMsgCmd(msg.GetType());
        return newMsg;
    }

    public static Message Create()
    {
        return ReferencePool.Acquire<Message>();
    }
    
    [Obsolete("Only for test client")]
    public static Message Create(int cmd, IMessage msg)
    {
        var newMsg = ReferencePool.Acquire<Message>();
        newMsg.Body = msg.ToByteArray();
        newMsg.Cmd = cmd;
        return newMsg;
    }
}
