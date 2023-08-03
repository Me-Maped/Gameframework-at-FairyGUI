using FairyGUI;
using GameMain.Data;
using GameMain.Launch;
using UnityEngine;

namespace GameMain
{
    public class UILoadMgr : Singleton<UILoadMgr>
    {
        private LaunchPop _launchPop;
        private LaunchForm _launchForm;
        private LabelContent _tipsSettings;
        public LabelContent Tips => _tipsSettings;

        public void InitUI()
        {
            var settings = Resources.Load<LaunchTipsSettings>(GameModule.UI.LaunchTipsSettingName);
            _tipsSettings = settings.GetCurLanguageTips(GameModule.Localization.SystemLanguage.ToString());
            foreach (var pkgName in GameModule.UI.LaunchPkgNames)
            {
                UIPackage.AddPackage(pkgName);
            }
            LaunchBinder.BindAll();
        }

        public void HideAll()
        {
            _launchForm?.Hide();
            _launchPop?.Hide();
        }

        public void TweenHide(System.Action hideCallback = null)
        {
            _launchPop?.HideImmediately();
            _launchForm?.TweenHide(hideCallback);
        }

        public void ShowForm()
        {
            _launchForm ??= new LaunchForm();
            if (!_launchForm.isShowing) _launchForm.Show();
        }

        public void SetProgress(string desc)
        {
            _launchForm ??= new LaunchForm();
            if (!_launchForm.isShowing) _launchForm.Show();
            double curProgress = _launchForm.GetCurProgress();
            if (curProgress > 98) return;
            _launchForm.SetProgress((float)curProgress + 2, desc);
        }

        public void SetProgress(float progress, string desc)
        {
            _launchForm ??= new LaunchForm();
            if (!_launchForm.isShowing) _launchForm.Show();
            _launchForm.SetProgress(progress, desc);
        }

        public void ShowPop(string title, string content, BtnStyleEnum style, System.Action confirm = null,
            System.Action cancel = null)
        {
            _launchPop ??= new LaunchPop();
            if (!_launchPop.isShowing) _launchPop.Show();
            _launchPop.SetContent(title, content, style, confirm, cancel);
        }
    }
}