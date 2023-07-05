using System;
using Cysharp.Threading.Tasks;
using GameFramework.Procedure;
using GameMain.Data;
using UnityEngine;
using UnityGameFramework.Runtime;
using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain
{
    /// <summary>
    /// 流程 => 用户尝试更新静态版本
    /// </summary>
    public class ProcedureUpdateVersion : ProcedureBase
    {

        private ProcedureOwner _procedureOwner;
        
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            _procedureOwner = procedureOwner;
            
            base.OnEnter(procedureOwner);
            
            UILoadMgr.Instance.SetProgress("更新静态版本文件...");

            //检查设备是否能够访问互联网
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Log.Warning("The device is not connected to the network");
                UILoadMgr.Instance.SetProgress(UILoadMgr.Instance.Tips.NetUnReachable);
                UILoadMgr.Instance.ShowPop("Tips","",BtnStyleEnum.TWO,GetStaticVersion().Forget,
                    ()=>
                    {
                        ChangeState<ProcedureInitResources>(procedureOwner);
                    });
            }
            UILoadMgr.Instance.SetProgress(UILoadMgr.Instance.Tips.RequestVersionIng);

            // 用户尝试更新静态版本。
            GetStaticVersion().Forget();
        }

        /// <summary>
        /// 向用户尝试更新静态版本。
        /// </summary>
        private async UniTaskVoid GetStaticVersion()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

            var operation = GameModule.Resource.UpdatePackageVersionAsync();

            await operation.ToUniTask();

            if (operation.Status == EOperationStatus.Succeed)
            {
                //线上最新版本operation.PackageVersion
                GameModule.Resource.PackageVersion = operation.PackageVersion;
                
                ChangeState<ProcedureUpdateManifest>(_procedureOwner);
            }
            else
            {
                Log.Error(operation.Error);
                
                UILoadMgr.Instance.ShowPop("Tips",
                    $"用户尝试更新静态版本失败！点击确认重试 \n \n [color=#FF0000]原因{operation.Error}[/color]", BtnStyleEnum.TWO,
                    GetStaticVersion().Forget,
                    Application.Quit);
            }
        }
    }
}