//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using System.Collections.Generic;


namespace GameConfig.basecorps
{
   
public partial class TbCoprs
{
    private readonly Dictionary<int, basecorps.basecorps> _dataMap;
    private readonly List<basecorps.basecorps> _dataList;
    
    public TbCoprs(ByteBuf _buf)
    {
        _dataMap = new Dictionary<int, basecorps.basecorps>();
        _dataList = new List<basecorps.basecorps>();
        
        for(int n = _buf.ReadSize() ; n > 0 ; --n)
        {
            basecorps.basecorps _v;
            _v = basecorps.basecorps.Deserializebasecorps(_buf);
            _dataList.Add(_v);
            _dataMap.Add(_v.Id, _v);
        }
        PostInit();
    }

    public Dictionary<int, basecorps.basecorps> DataMap => _dataMap;
    public List<basecorps.basecorps> DataList => _dataList;

    public basecorps.basecorps GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public basecorps.basecorps Get(int key) => _dataMap[key];
    public basecorps.basecorps this[int key] => _dataMap[key];

    public void Resolve(Dictionary<string, object> _tables)
    {
        foreach(var v in _dataList)
        {
            v.Resolve(_tables);
        }
        PostResolve();
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        foreach(var v in _dataList)
        {
            v.TranslateText(translator);
        }
    }
    
    partial void PostInit();
    partial void PostResolve();
}

}