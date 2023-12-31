﻿using System;
using Cysharp.Threading.Tasks;
using FairyGUI;
using GameFramework;
using GameFramework.Procedure;
using GameMain.Data;
using UnityGameFramework.Runtime;
using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain
{
    public class ProcedureDownloadFile:ProcedureBase
    {
        private ProcedureOwner _procedureOwner;

        private float _currentDownloadTime;
        private float CurrentSpeed =>
            (GameModule.Resource.Downloader.TotalDownloadBytes -
             GameModule.Resource.Downloader.CurrentDownloadBytes) / _currentDownloadTime;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            _procedureOwner = procedureOwner;
            _currentDownloadTime = 1;
            Timers.inst.Add(1, -1, OnTimeCountDown);

            Log.Info("开始下载更新文件！");
            
            UILoadMgr.Instance.SetProgress("开始下载更新文件...");
            
            BeginDownload().Forget();
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            Timers.inst.Remove(OnTimeCountDown);
        }

        private void OnTimeCountDown(object o)
        {
            _currentDownloadTime++;
        }

        private async UniTaskVoid BeginDownload()
        {
            var downloader = GameModule.Resource.Downloader;

            // 注册下载回调
            downloader.OnDownloadErrorCallback = OnDownloadErrorCallback;
            downloader.OnDownloadProgressCallback = OnDownloadProgressCallback;
            downloader.BeginDownload();
            await downloader;

            // 检测下载结果
            if (downloader.Status != EOperationStatus.Succeed)
                return;

            ChangeState<ProcedureDownloadOver>(_procedureOwner);
        }

        private void OnDownloadErrorCallback(string fileName, string error)
        {
            UILoadMgr.Instance.ShowPop("Tips", $"Failed to download file : {fileName} Error : {error}",
                BtnStyleEnum.TWO, () => { ChangeState<ProcedureCreateDownloader>(_procedureOwner);}, UnityEngine.Application.Quit);
        }

        private void OnDownloadProgressCallback(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes)
        {
            string currentSizeMb = (currentDownloadBytes / 1048576f).ToString("f1");
            string totalSizeMb = (totalDownloadBytes / 1048576f).ToString("f1");
            // UILoadMgr.Show(UIDefine.UILoadUpdate,$"{currentDownloadCount}/{totalDownloadCount} {currentSizeMb}MB/{totalSizeMb}MB");
            string descriptionText = Utility.Text.Format("正在更新，已更新{0}，总更新{1}，已更新大小{2}，总更新大小{3}，更新进度{4}，当前网速{5}/s", 
                currentSizeMb, 
                totalSizeMb,
                Utility.File.GetByteLengthString(currentDownloadBytes), 
                Utility.File.GetByteLengthString(totalDownloadBytes), 
                GameModule.Resource.Downloader.Progress, 
                Utility.File.GetByteLengthString((int)CurrentSpeed));
            UILoadMgr.Instance.SetProgress(GameModule.Resource.Downloader.Progress,descriptionText);

            int needTime = 0;
            if (CurrentSpeed > 0)
            {
                needTime = (int)((totalDownloadBytes - currentDownloadBytes) / CurrentSpeed);
            }
            
            TimeSpan ts = new TimeSpan(0, 0, needTime);
            string timeStr = ts.ToString(@"mm\:ss");
            string updateProgress = Utility.Text.Format("剩余时间 {0}({1}/s)", timeStr, Utility.File.GetLengthString((int)CurrentSpeed));
            Log.Info(updateProgress);
        }
    }
}