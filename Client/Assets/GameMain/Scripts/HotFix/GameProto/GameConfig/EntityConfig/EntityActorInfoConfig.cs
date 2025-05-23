
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;


namespace cfg.EntityConfig
{
public sealed partial class EntityActorInfoConfig : Luban.BeanBase
{
    public EntityActorInfoConfig(ByteBuf _buf) 
    {
        Id = _buf.ReadInt();
        Id_Ref = null;
        IdlePath = _buf.ReadString();
    }

    public static EntityActorInfoConfig DeserializeEntityActorInfoConfig(ByteBuf _buf)
    {
        return new EntityConfig.EntityActorInfoConfig(_buf);
    }

    /// <summary>
    /// id
    /// </summary>
    public readonly int Id;
    public EntityConfig.EntityBaseInfoConfig Id_Ref;
    /// <summary>
    /// idle资源路径
    /// </summary>
    public readonly string IdlePath;
   
    public const int __ID__ = 1487917785;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef(Tables tables)
    {
        Id_Ref = tables.EntityBaseInfo.GetOrDefault(Id);
    }

    public override string ToString()
    {
        return "{ "
        + "id:" + Id + ","
        + "idlePath:" + IdlePath + ","
        + "}";
    }
}

}

