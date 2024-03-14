namespace GameFramework
{
    public static partial class Version
    {
        /// <summary>
        /// 版本号辅助器接口。
        /// </summary>
        public interface IVersionHelper
        {
            /// <summary>
            /// 获取游戏版本号。
            /// </summary>
            string GameVersion { get; }

            /// <summary>
            /// 获取内部游戏版本号。
            /// </summary>
            string InternalGameVersion { get; }
            
            /// <summary>
            /// 资源版本号。
            /// </summary>
            string ResourceVersion{get;}
            
            /// <summary>
            /// 代码版本号。
            /// </summary>
            string CodeVersion { get; }
            
            /// <summary>
            /// 配置版本号。
            /// </summary>
            string ConfigVersion { get; }
        }
    }
}
