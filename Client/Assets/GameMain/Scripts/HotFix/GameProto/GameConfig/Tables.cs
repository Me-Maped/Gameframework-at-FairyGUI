//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using System.Threading.Tasks;


namespace GameConfig
{
   
public sealed class Tables
{
    public basecorps.TbCoprs TbCoprs {get; private set; }
    public basecorpslevelexp.TbCoprsLevelExp TbCoprsLevelExp {get; private set; }
    public basecorpsadvance.TbCoprsAdvance TbCoprsAdvance {get; private set; }
    public basecorpsstar.TbCoprsStar TbCoprsStar {get; private set; }

    public Tables() { }
    
    public async Task LoadAsync(System.Func<string, Task<ByteBuf>> loader)
    {
        var tables = new System.Collections.Generic.Dictionary<string, object>();
        TbCoprs = new basecorps.TbCoprs(await loader("basecorps_tbcoprs")); 
        tables.Add("basecorps.TbCoprs", TbCoprs);
        TbCoprsLevelExp = new basecorpslevelexp.TbCoprsLevelExp(await loader("basecorpslevelexp_tbcoprslevelexp")); 
        tables.Add("basecorpslevelexp.TbCoprsLevelExp", TbCoprsLevelExp);
        TbCoprsAdvance = new basecorpsadvance.TbCoprsAdvance(await loader("basecorpsadvance_tbcoprsadvance")); 
        tables.Add("basecorpsadvance.TbCoprsAdvance", TbCoprsAdvance);
        TbCoprsStar = new basecorpsstar.TbCoprsStar(await loader("basecorpsstar_tbcoprsstar")); 
        tables.Add("basecorpsstar.TbCoprsStar", TbCoprsStar);

        TbCoprs.Resolve(tables); 
        TbCoprsLevelExp.Resolve(tables); 
        TbCoprsAdvance.Resolve(tables); 
        TbCoprsStar.Resolve(tables); 
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        TbCoprs.TranslateText(translator); 
        TbCoprsLevelExp.TranslateText(translator); 
        TbCoprsAdvance.TranslateText(translator); 
        TbCoprsStar.TranslateText(translator); 
    }
}

}