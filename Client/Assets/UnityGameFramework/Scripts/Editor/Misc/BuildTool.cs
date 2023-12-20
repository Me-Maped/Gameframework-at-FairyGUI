using System;
using System.IO;
using HybridCLR.Editor.Commands;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using YooAsset.Editor;

namespace UnityGameFramework.Editor
{
    public static class BuildTool
    {
        public static void ChangeChannel()
        {
            string channel = CommandLineReader.GetCustomArgument("channel");
            if (string.IsNullOrEmpty(channel))
            {
                Debug.LogError("Build Asset Bundle Error！channel is null");
                return;
            }
            DefineController.ChangeChannel(channel);
        }
        
        public static void BuildDll()
        {
            string platform = CommandLineReader.GetCustomArgument("platform");
            if (string.IsNullOrEmpty(platform))
            {
                Debug.LogError("Build Asset Bundle Error！platform is null");
                return;
            }
            BuildTarget target = GetBuildTarget(platform);
            BuildDLLCommand.BuildAndCopyDlls(target);
        }

        public static void BuildAssetBundle()
        {
            string assetsPath = CommandLineReader.GetCustomArgument("assetsPath");
            if (string.IsNullOrEmpty(assetsPath))
            {
                Debug.LogWarning("Build Asset Bundle: assetsPath is null, use default assetsPath");
            }

            string packageVersion = CommandLineReader.GetCustomArgument("packageVersion");
            if (string.IsNullOrEmpty(packageVersion))
            {
                Debug.LogWarning("Build Asset Bundle: packageVersion is null, use default version");
            }

            string platform = CommandLineReader.GetCustomArgument("platform");
            if (string.IsNullOrEmpty(platform))
            {
                Debug.LogError("Build Asset Bundle Error！platform is null");
                return;
            }

            BuildTarget target = GetBuildTarget(platform);
            BuildInternal(target, assetsPath, packageVersion);
        }

        private static void Prebuild(bool hotfix = false)
        {
            string platform = CommandLineReader.GetCustomArgument("platform");
            if (string.IsNullOrEmpty(platform))
            {
                Debug.LogError("Build Asset Bundle Error！platform is null");
                return;
            }
            BuildTarget target = GetBuildTarget(platform);
            BuildTargetGroup targetGroup = GetBuildTargetGroup(platform);
            EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, target);
            AssetDatabase.Refresh();
            if(!hotfix) PrebuildCommand.GenerateAll();
        }

        private static void Prebuild(BuildTargetGroup targetGroup, BuildTarget target)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, target);
            AssetDatabase.Refresh();
            PrebuildCommand.GenerateAll();
        }

        public static void BuildPackage()
        {
            string outputPath = CommandLineReader.GetCustomArgument("outputPath");
            if (string.IsNullOrEmpty(outputPath))
            {
                Debug.LogWarning("Build Asset Bundle: outputPath is null, use default outputPath");
            }
            string platform = CommandLineReader.GetCustomArgument("platform");
            if (string.IsNullOrEmpty(platform))
            {
                Debug.LogError("Build Asset Bundle Error！platform is null");
                return;
            }
            BuildTarget target = GetBuildTarget(platform);
            BuildTargetGroup targetGroup = GetBuildTargetGroup(platform);
            outputPath ??= $"{Application.dataPath}/../Builds/{target}/{SettingsUtils.FrameworkGlobalSettings.ScriptVersion}/{PlayerSettings.productName}";
            BuildImp(targetGroup, target, outputPath);
        }

        private static BuildTarget GetBuildTarget(string platform)
        {
            BuildTarget target = BuildTarget.NoTarget;
            switch (platform)
            {
                case "Android":
                    target = BuildTarget.Android;
                    break;
                case "IOS":
                    target = BuildTarget.iOS;
                    break;
                case "Windows":
                    target = BuildTarget.StandaloneWindows64;
                    break;
                case "MacOS":
                    target = BuildTarget.StandaloneOSX;
                    break;
                case "WebGL":
                    target = BuildTarget.WebGL;
                    break;
            }
            return target;
        }

        private static BuildTargetGroup GetBuildTargetGroup(string platform)
        {
            BuildTargetGroup targetGroup = BuildTargetGroup.Unknown;
            switch (platform)
            {
                case "Android":
                    targetGroup = BuildTargetGroup.Android;
                    break;
                case "IOS":
                    targetGroup = BuildTargetGroup.iOS;
                    break;
                case "Windows":
                    targetGroup = BuildTargetGroup.Standalone;
                    break;
                case "MacOS":
                    targetGroup = BuildTargetGroup.Standalone;
                    break;
                case "WebGL":
                    targetGroup = BuildTargetGroup.WebGL;
                    break;
            }
            return targetGroup;
        }

        private static void BuildInternal(BuildTarget buildTarget, string assetsPath = null, string packageVersion = null)
        {
            BuildParameters.SBPBuildParameters sbpBuildParameters = new BuildParameters.SBPBuildParameters
            {
                WriteLinkXML = true
            };
            // 构建参数
            BuildParameters buildParameters = new BuildParameters
            {
                StreamingAssetsRoot = AssetBundleBuilderHelper.GetDefaultStreamingAssetsRoot(),
                BuildOutputRoot = assetsPath ?? AssetBundleBuilderHelper.GetDefaultBuildOutputRoot(),
                BuildTarget = buildTarget,
                BuildPipeline = EBuildPipeline.ScriptableBuildPipeline,
                BuildMode = EBuildMode.IncrementalBuild,
                PackageName = SettingsUtils.FrameworkGlobalSettings.DefaultPkgName,
                PackageVersion = packageVersion ?? GetBuildPackageVersion(),
                VerifyBuildingResult = true,
                SharedPackRule = new ZeroRedundancySharedPackRule(),
                CompressOption = ECompressOption.LZ4,
                OutputNameStyle = EOutputNameStyle.BundleName_HashName,
                CopyBuildinFileOption = ECopyBuildinFileOption.ClearAndCopyAll,
                SBPParameters = sbpBuildParameters,
            };

            Debug.Log($"开始构建 : platform={buildTarget}, outputRoot={buildParameters.BuildOutputRoot}, packageVersion={buildParameters.PackageVersion}");

            // 执行构建
            AssetBundleBuilder builder = new AssetBundleBuilder();
            var buildResult = builder.Run(buildParameters);
            if (buildResult.Success)
            {
                Debug.Log($"构建成功 : {buildResult.OutputPackageDirectory}");
            }
            else
            {
                Debug.LogError($"构建失败 : {buildResult.ErrorInfo}");
            }
        }
        
        // 构建版本相关
        private static string GetBuildPackageVersion()
        {
            int totalMinutes = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
            return DateTime.Now.ToString("yyyy-MM-dd") + "-" + totalMinutes;
        }

        public static void AutomationBuild()
        {
            Prebuild();
            AssetDatabase.Refresh();
            ChangeChannel();
            AssetDatabase.Refresh();
            BuildDll();
            AssetDatabase.Refresh();
            BuildAssetBundle();
            AssetDatabase.Refresh();
            BuildPackage();
        }

        public static void AutomationHotfix()
        {
            Prebuild(true);
            AssetDatabase.Refresh();
            BuildDll();
            AssetDatabase.Refresh();
            BuildAssetBundle();
            AssetDatabase.Refresh();     
        }

        [MenuItem("Game Framework/Build/Windows", false, 230)]
        public static void AutomationBuildWindows()
        {
            Prebuild(BuildTargetGroup.Standalone,BuildTarget.StandaloneWindows64);
            AssetDatabase.Refresh();
            DefineController.ChannelLocal();
            AssetDatabase.Refresh();
            BuildDLLCommand.BuildAndCopyDlls(BuildTarget.StandaloneWindows64);
            AssetDatabase.Refresh();
            BuildInternal(BuildTarget.StandaloneWindows64);
            AssetDatabase.Refresh();
            BuildImp(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64, $"{Application.dataPath}/../Builds/Windows/{SettingsUtils.FrameworkGlobalSettings.ScriptVersion}/{PlayerSettings.productName}.exe");
        }

        [MenuItem("Game Framework/Build/Android", false, 230)]
        public static void AutomationBuildAndroid()
        {
            Prebuild(BuildTargetGroup.Android, BuildTarget.Android);
            AssetDatabase.Refresh();
            DefineController.ChannelAndroidExperience();
            AssetDatabase.Refresh();
            BuildDLLCommand.BuildAndCopyDlls(BuildTarget.Android);
            AssetDatabase.Refresh();
            BuildInternal(BuildTarget.Android);
            AssetDatabase.Refresh();
            BuildImp(BuildTargetGroup.Android, BuildTarget.Android, $"{Application.dataPath}/../Builds/Android/{SettingsUtils.FrameworkGlobalSettings.ScriptVersion}/{PlayerSettings.productName}.apk");
        }
        
        [MenuItem("Game Framework/Build/IOS", false, 230)]
        public static void AutomationBuildIOS()
        {
            Prebuild(BuildTargetGroup.iOS, BuildTarget.iOS);
            AssetDatabase.Refresh();
            DefineController.ChannelIosExperience();
            AssetDatabase.Refresh();
            BuildDLLCommand.BuildAndCopyDlls(BuildTarget.iOS);
            AssetDatabase.Refresh();
            BuildInternal(BuildTarget.iOS);
            AssetDatabase.Refresh();
            BuildImp(BuildTargetGroup.iOS, BuildTarget.iOS, $"{Application.dataPath}/../Builds/IOS/{SettingsUtils.FrameworkGlobalSettings.ScriptVersion}/{PlayerSettings.productName}");
        }
        
        [MenuItem("Game Framework/Hotfix/Active Platform", false, 230)]
        public static void HotfixCurrentPlatform()
        {
            BuildDLLCommand.BuildAndCopyDlls();
            AssetDatabase.Refresh();
            BuildInternal(EditorUserBuildSettings.activeBuildTarget);
            AssetDatabase.Refresh();     
        }

        public static void BuildImp(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget, string locationPathName)
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/UnityGameFramework/main.unity" },
                locationPathName = locationPathName,
                targetGroup = buildTargetGroup,
                target = buildTarget,
                options = Define.PkgArg.Debug ? BuildOptions.Development : BuildOptions.None,
            };
            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;
            if (summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.Log($"Build success: {summary.totalSize / 1024 / 1024} MB");
                OpenFolder.Execute(Path.GetDirectoryName(locationPathName));
            }
            else
            {
                Debug.Log("Build Failed" + summary.result);
            }
        }
    }
}