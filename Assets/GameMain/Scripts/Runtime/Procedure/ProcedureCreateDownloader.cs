﻿using System;
using Cysharp.Threading.Tasks;
using GameFramework.Procedure;
using GameMain.Data;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain
{
    public class ProcedureCreateDownloader : ProcedureBase
    {

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            Log.Info("创建补丁下载器");
            
            UILoadMgr.Instance.SetProgress("创建补丁下载器...");
            
            CreateDownloader(procedureOwner).Forget();
        }

        private async UniTaskVoid CreateDownloader(ProcedureOwner procedureOwner)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

            int downloadingMaxNum = 10;
            int failedTryAgain = 3;
            var downloader = GameModule.Resource.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

            if (downloader.TotalDownloadCount == 0)
            {
                Log.Info("Not found any download files !");
                ChangeState<ProcedureDownloadOver>(procedureOwner);
            }
            else
            {
                //A total of 10 files were found that need to be downloaded
                Log.Info($"Found total {downloader.TotalDownloadCount} files that need download ！");

                // 发现新更新文件后，挂起流程系统
                // 注意：开发者需要在下载前检测磁盘空间不足
                int totalDownloadCount = downloader.TotalDownloadCount;
                long totalDownloadBytes = downloader.TotalDownloadBytes;

                float sizeMb = totalDownloadBytes / 1048576f;
                sizeMb = UnityEngine.Mathf.Clamp(sizeMb, 0.1f, float.MaxValue);
                string totalSizeMb = sizeMb.ToString("f1");

                UILoadMgr.Instance.ShowPop("Tips",
                    $"Found update patch files, Total count {totalDownloadCount} Total size {totalSizeMb}MB",
                    BtnStyleEnum.TWO, () =>
                    {
                        StartDownFile(procedureOwner: procedureOwner);
                    }, UnityEngine.Application.Quit);
            }
        }

        void StartDownFile(ProcedureOwner procedureOwner)
        {
            ChangeState<ProcedureDownloadFile>(procedureOwner);
        }
    }
}