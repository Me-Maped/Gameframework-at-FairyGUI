using System;
using System.Reflection;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameMain
{
    public class ProcedureStartGame : ProcedureBase
    {
        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            SetVersion().Forget();
            StartGame().Forget();
        }

        protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
        }

        private async UniTaskVoid SetVersion()
        {
            TextAsset versionBytes = await GameModule.Resource.LoadAssetAsync<TextAsset>(SettingsUtils.FrameworkGlobalSettings.VersionFilePath);
            var versionBean = Utility.Json.ToObject<VersionBean>(versionBytes.text);
            MVersionHelper.SetVersion(versionBean);
            GameModule.Resource.UnloadAsset(versionBytes);
            MVersionHelper.PrintVersion();
        }


        private async UniTaskVoid StartGame()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(2f));
            RunMainLogicMethod();
            UILoadMgr.Instance.SetProgress(100, "加载完成");
        }

        private void RunMainLogicMethod()
        {
            Assembly mainLogicAssembly = null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (string.Compare(SettingsUtils.HybridCLRCustomGlobalSettings.LogicMainDllName,
                        $"{assembly.GetName().Name}.dll",
                        StringComparison.Ordinal) == 0)
                {
                    mainLogicAssembly = assembly;
                }
            }
            if (mainLogicAssembly == null)
            {
                Log.Fatal("Main logic assembly missing.");
                return;
            }
            var appType = mainLogicAssembly.GetType("GameApp");
            if (appType == null)
            {
                Log.Fatal("Main logic type 'GameApp' missing.");
                return;
            }
            var entryMethod = appType.GetMethod("StartGameLogic");
            if (entryMethod == null)
            {
                Log.Fatal("Main logic entry method 'StartGameLogic' missing.");
                return;
            }
            entryMethod.Invoke(appType, null);
        }
    }
}