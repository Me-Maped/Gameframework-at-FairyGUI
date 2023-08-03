using UnityEditor;
using UnityEngine;

namespace UnityGameFramework.Editor
{
    [InitializeOnLoad]
    public class DefineController
    {
        private static readonly string[] Defines = 
        {
            "CHANNEL_LOCAL", 
            "CHANNEL_CDN_LOCAL",
            "CHANNEL_ANDROID_EXPERIENCE",
            "CHANNEL_GOOGLE",
            "CHANNEL_IOS_EXPERIENCE",
            "CHANNEL_APPSTORE"
        };

        static DefineController()
        {
            string curChannel = GetCurDefine();
            Menu.SetChecked("Game Framework/Define/EDITOR", curChannel.Equals("EDITOR"));
            Menu.SetChecked("Game Framework/Define/CHANNEL_LOCAL", curChannel.Equals("CHANNEL_LOCAL"));
            Menu.SetChecked("Game Framework/Define/CHANNEL_CDN_LOCAL", curChannel.Equals("CHANNEL_CDN_LOCAL"));
            Menu.SetChecked("Game Framework/Define/CHANNEL_ANDROID_EXPERIENCE", curChannel.Equals("CHANNEL_ANDROID_EXPERIENCE"));
            Menu.SetChecked("Game Framework/Define/CHANNEL_GOOGLE", curChannel.Equals("CHANNEL_GOOGLE"));
            Menu.SetChecked("Game Framework/Define/CHANNEL_IOS_EXPERIENCE", curChannel.Equals("CHANNEL_IOS_EXPERIENCE"));
            Menu.SetChecked("Game Framework/Define/CHANNEL_APPSTORE", curChannel.Equals("CHANNEL_APPSTORE"));
            Debug.Log("当前渠道：" + curChannel);
        }
        [MenuItem("Game Framework/Define/EDITOR")]
        public static void ChannelEditor()
        {
            Clear();
            Debug.Log("切换EDITOR");
        }
        
        [MenuItem("Game Framework/Define/CHANNEL_LOCAL")]
        public static void ChannelLocal()
        {
            Clear();
            SetDefine("CHANNEL_LOCAL");
            Debug.Log("切换CHANNEL_LOCAL");
        }
        
        [MenuItem("Game Framework/Define/CHANNEL_CDN_LOCAL")]
        public static void ChannelCdnLocal()
        {
            Clear();
            SetDefine("CHANNEL_CDN_LOCAL");
            Debug.Log("切换CHANNEL_CDN_LOCAL");
        }
        
        [MenuItem("Game Framework/Define/CHANNEL_ANDROID_EXPERIENCE")]
        public static void ChannelAndroidExperience()
        {
            Clear();
            SetDefine("CHANNEL_ANDROID_EXPERIENCE");
            Debug.Log("切换CHANNEL_ANDROID_EXPERIENCE");
        }
        
        [MenuItem("Game Framework/Define/CHANNEL_GOOGLE")]
        public static void ChannelGoogle()
        {
            Clear();
            SetDefine("CHANNEL_GOOGLE");
            Debug.Log("切换CHANNEL_GOOGLE");
        }
        
        [MenuItem("Game Framework/Define/CHANNEL_IOS_EXPERIENCE")]
        public static void ChannelIosExperience()
        {
            Clear();
            SetDefine("CHANNEL_IOS_EXPERIENCE");
            Debug.Log("切换CHANNEL_IOS_EXPERIENCE");
        }
        
        [MenuItem("Game Framework/Define/CHANNEL_APPSTORE")]
        public static void ChannelAppstore()
        {
            Clear();
            SetDefine("CHANNEL_APPSTORE");
            Debug.Log("切换CHANNEL_APPSTORE");
        }
        
        private static void Clear()
        {
            string[] defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';');
            for (int i = 0; i < defines.Length; i++)
            {
                foreach (var originDefine in Defines)
                {
                    if (defines[i].Equals(originDefine))
                    {
                        defines[i] = string.Empty;
                    }
                }
            }
            SetDefine(string.Join(";", defines));
            Debug.Log("清理宏定义");
        }

        private static void SetDefine(string define)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, define);
            
            //保存
            AssetDatabase.SaveAssets();
            
            // 重新编译
            EditorApplication.LockReloadAssemblies();
            AssetDatabase.Refresh();
            EditorApplication.UnlockReloadAssemblies();
        }

        private static string GetCurDefine()
        {
            string[] defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';');
            foreach (var nowDefine in defines)
            {
                foreach (var originDefine in Defines)
                {
                    if (nowDefine.Equals(originDefine))
                    {
                        return originDefine;
                    }
                }
            }
            return "EDITOR"; // 默认返回Editor
        }
    }
}