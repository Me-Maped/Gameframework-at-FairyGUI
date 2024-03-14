using GameFramework;
using UnityEngine;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    /// 默认版本号辅助器。
    /// </summary>
    public class DefaultVersionHelper : Version.IVersionHelper
    {
        /// <summary>
        /// 获取游戏版本号。
        /// </summary>
        public string GameVersion => Application.version;

        /// <summary>
        /// 获取内部游戏版本号。
        /// </summary>
        public string InternalGameVersion => "0";

        public string ResourceVersion => "0.1";
        public string CodeVersion => "0.1";
        public string ConfigVersion => "0.1";
    }
}
