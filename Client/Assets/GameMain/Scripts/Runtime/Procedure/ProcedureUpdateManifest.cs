﻿using System;
using Cysharp.Threading.Tasks;
using GameFramework.Procedure;
using GameMain.Data;
using UnityGameFramework.Runtime;
using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain
{
    /// <summary>
    /// 流程 => 用户尝试更新清单
    /// </summary>
    public class ProcedureUpdateManifest: ProcedureBase
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            Log.Info("更新资源清单！！！");
            
            UILoadMgr.Instance.SetProgress("更新清单文件...");
            
            UpdateManifest(procedureOwner).Forget();
        }

        private async UniTaskVoid UpdateManifest(ProcedureOwner procedureOwner)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            
            var operation = GameModule.Resource.UpdatePackageManifestAsync(GameModule.Resource.PackageVersion);
            
            await operation.ToUniTask();
            
            if(operation.Status == EOperationStatus.Succeed)
            {
                ChangeState<ProcedureCreateDownloader>(procedureOwner);
            }
            else
            {
                Log.Error(operation.Error);
                
                UILoadMgr.Instance.ShowPop("Tips",
                    $"用户尝试更新清单失败！点击确认重试 \n \n [color=#FF0000]原因{operation.Error}[/color]", BtnStyleEnum.TWO,
                    () => { ChangeState<ProcedureUpdateManifest>(procedureOwner); }, UnityEngine.Application.Quit);
            }
        }
    }
}