using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityGameFramework.Editor
{
    public static class DefineController
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

        [InitializeOnLoadMethod]
        private static void RefreshSelectDefine()
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
            SetDefine(null);
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
        
        public static void ChangeChannel(string channel)
        {
            if (!Defines.Contains(channel))
            {
                Debug.LogError($"{channel}宏定义不存在");
                return;
            }
            Clear();
            SetDefine(channel);
            Debug.Log("切换" + channel);
        }
        
        private static void Clear()
        {
            foreach (var define in Defines)
            {
                ScriptingDefineSymbols.RemoveScriptingDefineSymbol(define);
            }
            Debug.Log("清理宏定义");
        }

        private static void SetDefine(string define)
        {
            if(!string.IsNullOrEmpty(define)) ScriptingDefineSymbols.AddScriptingDefineSymbol(define);
            
            //保存
            AssetDatabase.SaveAssets();
            AssetDatabase.RefreshSettings();
            
            // 重新编译
            EditorApplication.LockReloadAssemblies();
            AssetDatabase.Refresh();
            EditorApplication.UnlockReloadAssemblies();
        }

        private static string GetCurDefine()
        {
            foreach (var define in Defines)
            {
                if (ScriptingDefineSymbols.HasScriptingDefineSymbol(EditorUserBuildSettings.selectedBuildTargetGroup,
                        define)) return define;
            }
            return "EDITOR"; // 默认返回Editor
        }
    }
}