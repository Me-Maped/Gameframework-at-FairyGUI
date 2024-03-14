using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameMain
{
    public class MVersionHelper : Version.IVersionHelper
    {
        private const string UNMODIFIED_VERSION = "0.0.0";
        
        private static string DefaultInternalGameVersion = UNMODIFIED_VERSION;
        private static string DefaultResourceVersion = UNMODIFIED_VERSION;
        private static string DefaultCodeVersion = UNMODIFIED_VERSION;
        private static string DefaultConfigVersion = UNMODIFIED_VERSION;

        public static void SetVersion(VersionBean version)
        {
            DefaultInternalGameVersion = version.internalGameVersion;
            DefaultResourceVersion = version.resourceVersion;
            DefaultCodeVersion = version.codeVersion;
            DefaultConfigVersion = version.configVersion;
        }

        public static void PrintVersion()
        {
            Log.Info(
                "[Version] InternalGameVersion: {0}; ResourceVersion: {1}; CodeVersion: {2}; ConfigVersion: {3} ",
                DefaultInternalGameVersion, DefaultResourceVersion, DefaultCodeVersion, DefaultConfigVersion);
        }

        public string GameVersion => Application.version;
        public string InternalGameVersion => DefaultInternalGameVersion;
        public string ResourceVersion => DefaultResourceVersion;
        public string CodeVersion => DefaultCodeVersion;
        public string ConfigVersion => DefaultConfigVersion;
    }
}