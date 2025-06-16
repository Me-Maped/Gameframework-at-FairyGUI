// 修改后的CSharpEnum.txt模板
// 该文件自动生成，请勿手动修改!!!
// 生成时间: {{generate_time}}
// - by Maped
namespace {{enum_namespace}}
{
    {{for info in infos}}
    public enum {{info.enum_type}}
    { 
    {{for e in info.enums}}
        {{e.enum_name}} = {{e.enum_id}},{{end}}
    }
    {{end}}
}