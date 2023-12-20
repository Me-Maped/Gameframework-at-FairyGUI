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
        /// 是否有CDN
        /// </summary>
        public bool HasCdn;

        /// <summary>
        /// YooAsset运行模式
        /// </summary>
        public YooAsset.EPlayMode YooMode;
}

public static class Define
{
        public static PkgArg PkgArg => new()
        {
#if CHANNEL_LOCAL
        HybridLoadMeta = true,
        Debug = true,
        HasCdn = false,
        YooMode = YooAsset.EPlayMode.OfflinePlayMode,
#elif CHANNEL_CDN_LOCAL
        HybridLoadMeta = true,
        Debug = true,
        HasCdn = true,
        YooMode = YooAsset.EPlayMode.HostPlayMode,
#elif CHANNEL_ANDROID_EXPERIENCE
        HybridLoadMeta = true,
        Debug = true,
        HasCdn = false,
        YooMode = YooAsset.EPlayMode.OfflinePlayMode,
#elif CHANNEL_IOS_EXPERIENCE
        HybridLoadMeta = true,
        Debug = false,
        HasCdn = false,
        YooMode = YooAsset.EPlayMode.OfflinePlayMode,
#elif CHANNEL_GOOGLE
        HybridLoadMeta = true,
        Debug = true,
        HasCdn = true,
        YooMode = YooAsset.EPlayMode.HostPlayMode,
#elif CHANNEL_APPSTORE
        HybridLoadMeta = true,
        Debug = true,
        HasCdn = true,
        YooMode = YooAsset.EPlayMode.HostPlayMode,
#else
                HybridLoadMeta = false,
                Debug = true,
                HasCdn = false,
                YooMode = YooAsset.EPlayMode.OfflinePlayMode,
#endif
        };
}