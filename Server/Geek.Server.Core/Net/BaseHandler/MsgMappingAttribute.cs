namespace Geek.Server.Core.Net.BaseHandler
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MsgMapping : Attribute
    {
        public int MsgId { get; }

        public MsgMapping(int msgId)
        {
            MsgId = msgId;
        }
    }
}
