//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using System.Collections.Generic;


namespace GameConfig.Battle
{
public sealed partial class SkillBaseConfig :  Bright.Config.BeanBase 
{
    public SkillBaseConfig(ByteBuf _buf) 
    {
        Id = _buf.ReadInt();
        Name = _buf.ReadString();
        Desc = _buf.ReadString();
        SkillType = _buf.ReadInt();
        SkillDispID = _buf.ReadInt();
        PostInit();
    }

    public static SkillBaseConfig DeserializeSkillBaseConfig(ByteBuf _buf)
    {
        return new Battle.SkillBaseConfig(_buf);
    }

    /// <summary>
    /// 技能ID
    /// </summary>
    public int Id { get; private set; }
    /// <summary>
    /// 名字
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// 描述
    /// </summary>
    public string Desc { get; private set; }
    /// <summary>
    /// 技能类型
    /// </summary>
    public int SkillType { get; private set; }
    /// <summary>
    /// 技能特效表现ID
    /// </summary>
    public int SkillDispID { get; private set; }

    public const int __ID__ = 2067672430;
    public override int GetTypeId() => __ID__;

    public  void Resolve(Dictionary<string, object> _tables)
    {
        PostResolve();
    }

    public  void TranslateText(System.Func<string, string, string> translator)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "Id:" + Id + ","
        + "Name:" + Name + ","
        + "Desc:" + Desc + ","
        + "SkillType:" + SkillType + ","
        + "SkillDispID:" + SkillDispID + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}

}