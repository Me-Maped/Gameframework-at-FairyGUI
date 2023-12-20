using HybridCLR.Editor;
using HybridCLR.Editor.Commands;
using UnityEditor;
using UnityEngine;

namespace UnityGameFramework.Editor
{
    public static class BuildDLLCommand
    {
        [MenuItem("HybridCLR/Build/BuildAssets And CopyTo AssemblyTextAssetPath")]
        public static void BuildAndCopyDlls()
        {
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            CompileDllCommand.CompileDll(target);
            CopyAOTHotUpdateDlls(target);
        }
        
        public static void BuildAndCopyDlls(BuildTarget target)
        {
            CompileDllCommand.CompileDll(target);
            CopyAOTHotUpdateDlls(target);
        }

        public static void CopyAOTHotUpdateDlls(BuildTarget target)
        {
            CopyAOTAssembliesToAssetPath(target);
            CopyHotUpdateAssembliesToAssetPath(target);
            AssetDatabase.Refresh();
        }

        public static void CopyAOTAssembliesToAssetPath(BuildTarget target)
        {
            string aotAssembliesSrcDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(target);
            string aotAssembliesDstDir = Application.dataPath + "/" +
                                         SettingsUtils.HybridCLRCustomGlobalSettings.AssemblyTextAssetPath;

            foreach (var dll in SettingsUtils.HybridCLRCustomGlobalSettings.AOTMetaAssemblies)
            {
                string srcDllPath = $"{aotAssembliesSrcDir}/{dll}";
                if (!System.IO.File.Exists(srcDllPath))
                {
                    Debug.LogError(
                        $"ab中添加AOT补充元数据dll:{srcDllPath} 时发生错误,文件不存在。裁剪后的AOT dll在BuildPlayer时才能生成，因此需要你先构建一次游戏App后再打包。");
                    continue;
                }

                string dllBytesPath = $"{aotAssembliesDstDir}/{dll}.bytes";
                System.IO.File.Copy(srcDllPath, dllBytesPath, true);
                Debug.Log($"[CopyAOTAssembliesToStreamingAssets] copy AOT dll {srcDllPath} -> {dllBytesPath}");
            }
        }

        public static void CopyHotUpdateAssembliesToAssetPath(BuildTarget target)
        {
            string hotfixDllSrcDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);
            string hotfixAssembliesDstDir = Application.dataPath + "/" +
                                            SettingsUtils.HybridCLRCustomGlobalSettings.AssemblyTextAssetPath;
            foreach (var dll in SettingsUtil.HotUpdateAssemblyFilesExcludePreserved)
            {
                string dllPath = $"{hotfixDllSrcDir}/{dll}";
                string dllBytesPath = $"{hotfixAssembliesDstDir}/{dll}.bytes";
                System.IO.File.Copy(dllPath, dllBytesPath, true);
                Debug.Log($"[CopyHotUpdateAssembliesToStreamingAssets] copy hotfix dll {dllPath} -> {dllBytesPath}");
            }
        }
    }
}