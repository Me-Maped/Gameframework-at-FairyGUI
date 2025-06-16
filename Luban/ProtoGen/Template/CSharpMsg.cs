using System;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace {{gen_namespace}}
{
    public class {{info.protocol_name}}Msg : IMessage
    {
        // 协议号
        public const int Id = (int){{pb_namespace}}.{{type_file_name}}.{{info.protocol_name}};

        // Protobuf消息实例
        private {{info.protocol_name}} _pb;

        public virtual IMessage Pb
        {
            get => _pb ??= new {{info.protocol_name}}();
            set => _pb = ({{info.protocol_name}})value;
        }

        // 消息类型
        public static readonly MessageDescriptor Descriptor = {{info.protocol_name}}.Descriptor;

        // 构造函数
        public {{info.protocol_name}}Msg() { }
        public {{info.protocol_name}}Msg({{info.protocol_name}} pb) => _pb = pb;

        // 序列化
        public byte[] ToByteArray() => Pb.ToByteArray();

        // 反序列化
        public void FromByteArray(byte[] data) => _pb = {{info.protocol_name}}.Parser.ParseFrom(data);

        // 类型转换
        public static implicit operator {{info.protocol_name}}({{info.protocol_name}}Msg msg) => msg.Pb;
        public static implicit operator {{info.protocol_name}}Msg({{info.protocol_name}} pb) => new {{info.protocol_name}}Msg(pb);
    }
}