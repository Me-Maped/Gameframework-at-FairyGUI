using System;

namespace GameMain
{
    /// <summary>
    /// 通用Native调用U3D委托
    /// </summary>
    /// <param name="jsonContent">Native To Unity数据</param>
    /// <returns>返回给Native端的数据</returns>
    public delegate string OnNativeCallbackSync(string jsonContent);

    /// <summary>
    /// 通用Native调用U3D委托 异步 带回调
    /// </summary>
    /// <param name="jsonContent">Native To Unity数据</param>
    /// <param name="callNativeAction">Unity To Native数据</param>
    public delegate void OnNativeCallbackAsync(string jsonContent, Action<string> callNativeAction);
}