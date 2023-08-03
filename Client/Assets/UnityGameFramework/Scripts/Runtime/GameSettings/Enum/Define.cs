/// <summary>
/// 宏定义类
/// </summary>
public enum PkgEnum
{
    /// <summary>
    /// Editor模式
    /// </summary>
    NONE,
    
    /// <summary>
    /// 本地测试包
    /// </summary>
    CHANNEL_LOCAL,
    
    /// <summary>
    /// 本地CDN测试包
    /// </summary>
    CHANNEL_CDN_LOCAL,
    
    /// <summary>
    /// 安卓外网体验包
    /// </summary>
    CHANNEL_ANDROID_EXPERIENCE,
    
    /// <summary>
    /// 安卓正式包
    /// </summary>
    CHANNEL_GOOGLE,
    
    /// <summary>
    /// IOS外网体验包
    /// </summary>
    CHANNEL_IOS_EXPERIENCE,
    
    /// <summary>
    /// IOS正式包
    /// </summary>
    CHANNEL_APPSTORE,
}

/// <summary>
/// YooAsset运行模式
/// </summary>
public enum YooMode
{
    /// <summary>
    /// 编辑器下的模拟模式
    /// </summary>
    EDITOR,

    /// <summary>
    /// 离线运行模式
    /// </summary>
    OFFLINE,

    /// <summary>
    /// 联机运行模式
    /// </summary>
    HOST,
}

public struct PkgArg
{
    /// <summary>
    /// 是否使用Hybrid加载Meta
    /// </summary>
    public bool HybridLoadMeta;
    
    /// <summary>
    /// 是否是开发者模式
    /// </summary>
    public bool Debug;

    /// <summary>
    /// 是否有SDK
    /// </summary>
    public bool HasSdk;

    /// <summary>
    /// 是否有CDN
    /// </summary>
    public bool HasCdn;

    /// <summary>
    /// 是否是SDK登录
    /// </summary>
    public bool SdkLogin;

    /// <summary>
    /// 是否是SDK支付
    /// </summary>
    public bool SdkPay;
    
    /// <summary>
    /// 包类型
    /// </summary>
    public PkgEnum PkgType;
    
    /// <summary>
    /// YooAsset运行模式
    /// </summary>
    public YooMode YooMode;
}

public static class Define
{
    public static PkgArg PkgArg => new PkgArg()
    {
#if CHANNEL_LOCAL
        HybridLoadMeta = true,
        Debug = true,
        HasSdk = false,
        HasCdn = false,
        SdkLogin = false,
        SdkPay = false,
        YooMode = YooMode.EDITOR,
        PkgType = PkgEnum.CHANNEL_LOCAL,
#elif CHANNEL_CDN_LOCAL
        HybridLoadMeta = true,
        Debug = true,
        HasSdk = false,
        HasCdn = false,
        SdkLogin = false,
        SdkPay = false,
        YooMode = YooMode.EDITOR,
        PkgType = PkgEnum.CHANNEL_CDN_LOCAL,
#elif CHANNEL_ANDROID_EXPERIENCE
        HybridLoadMeta = true,
        Debug = true,
        HasSdk = false,
        HasCdn = false,
        SdkLogin = false,
        SdkPay = false,
        YooMode = YooMode.OFFLINE,
        PkgType = PkgEnum.CHANNEL_CDN_LOCAL_WEBGL,
#elif CHANNEL_IOS_EXPERIENCE
        HybridLoadMeta = true,
        Debug = false,
        HasSdk = false,
        HasCdn = false,
        SdkLogin = false,
        SdkPay = false,
        YooMode = YooMode.OFFLINE,
        PkgType = PkgEnum.CHANNEL_WEBGL,
#elif CHANNEL_GOOGLE
        HybridLoadMeta = true,
        Debug = true,
        HasSdk = false,
        HasCdn = false,
        SdkLogin = false,
        SdkPay = false,
        YooMode = YooMode.EDITOR,
        PkgType = PkgEnum.CHANNEL_LOCAL,
#elif CHANNEL_APPSTORE
        HybridLoadMeta = true,
        Debug = true,
        HasSdk = false,
        HasCdn = false,
        SdkLogin = false,
        SdkPay = false,
        YooMode = YooMode.EDITOR,
        PkgType = PkgEnum.CHANNEL_LOCAL,
#else
        HybridLoadMeta = false,
        Debug = true,
        HasSdk = false,
        HasCdn = false,
        SdkLogin = false,
        SdkPay = false,
        PkgType = PkgEnum.NONE,
        YooMode = YooMode.EDITOR,
#endif
    };
}