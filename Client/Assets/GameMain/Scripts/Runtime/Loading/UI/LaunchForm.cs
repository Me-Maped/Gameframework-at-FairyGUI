using System;
using FairyGUI;
using GameFramework;
using GameFramework.UI;
using GameMain.Launch;
using UnityGameFramework.Runtime;

namespace GameMain
{
    public class LaunchForm : Window
    {
        private UI_launch_main _view;

        protected override void OnInit()
        {
            _view = UI_launch_main.CreateInstance();
            contentPane = _view;
            if (_view == null) throw new GameFrameworkException("UI_launch_main is invalid");
            
            sortingOrder = -(int)UIGroupEnum.LOADING;
            SetPosition(0, 0, (int)UIGroupEnum.LOADING);
            SetSize(GRoot.inst.width, GRoot.inst.height);
            AddRelation(GRoot.inst, RelationType.Size);
            _view.SetSize(GRoot.inst.width,GRoot.inst.height);
            _view.AddRelation(this, RelationType.Size);
            _view.m_progress.value = 0;
        }
        
        public double GetCurProgress()
        {
            return _view.m_progress.value;
        }

        public void SetProgress(float progress, string desc)
        {
            _view.m_progress.TweenValue(progress, 0.5f);
            _view.m_desc.text = desc;
        }

        protected override void OnHide()
        {
            _view.m_progress.value = 100;
            base.OnHide();
        }

        public void TweenHide(Action hideCallback)
        {
            Log.Warning("TweenHideLaunchForm");
            _view.m_progress.TweenValue(100, 0.5f).OnComplete(() =>
            {
                hideCallback?.Invoke();
                Hide();
            });
        }
    }
}