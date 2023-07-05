using System;
using FairyGUI;
using GameFramework;
using GameFramework.UI;
using GameMain.Data;
using GameMain.Launch;

namespace GameMain
{
    public class LaunchPop : Window
    {
        private UI_launch_pop _view;
        private Action _confirmCB;
        private Action _cancelCB;

        protected override void OnInit()
        {
            _view = UI_launch_pop.CreateInstance();
            contentPane = _view;
            if (_view == null) throw new GameFrameworkException("UI_launch_pop is invalid");
            _view.m_confirm_btn.onClick.Set(OnConfirmClick);
            _view.m_cancel_btn.onClick.Set(OnCancelClick);
            Center();
            sortingOrder = -(int)UIGroupEnum.ERROR;
            z = (int)UIGroupEnum.ERROR;
            _view.m_model.SetSize(GRoot.inst.width,GRoot.inst.height);
        }

        private void OnCancelClick()
        {
            _cancelCB?.Invoke();
            _cancelCB = null;
            Hide();
        }

        private void OnConfirmClick()
        {
            _confirmCB?.Invoke();
            _confirmCB = null;
            Hide();
        }

        public void SetContent(string title, string content, BtnStyleEnum style, Action confirm, Action cancel)
        {
            _view.m_title.text = title;
            _view.m_content.text = content;
            _confirmCB = confirm;
            _cancelCB = cancel;
            _view.m_one_c.selectedIndex = (int)style;
        }
    }
}