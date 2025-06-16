using GameFramework.Event;
using GameFramework.Localization;
using GameFramework.UI;
using UnityGameFramework.Runtime;

namespace GameLogic.Login
{
    public class LoginForm : UIForm<UI_login_main,LoginFormModel, LoginFormCtrl>
    {
        protected override UIFormConfig CreateConfig()
        {
            return UIFormConfig.Create(UI_login_main.PKG_NAME, UI_login_main.RES_NAME, groupEnum: UIGroupEnum.PANEL);
        }

        protected override void OnInit()
        {
            Model.Register<int>(nameof(LoginFormModel.TestNum), OnTestNumChange);
        }

        protected override void OnOpen()
        {
            // 表测试
            // Timers.inst.Add(2f, 1, _ => { View.m_title.text = Cfg.Tables.Config.EntranceName[0]; });
            // Timers.inst.Add(3f, 1, o => { View.m_title.visible = false; });
            
            // 多语言
            RegisterL10NEvent(View.m_handle_l10n_txt,Cfg.Tables.TbMusicItem.DataList[1].Desc);
            
            // 测试包卸载
            // Timers.inst.Add(6f, 1, _ => { Close(); });

            // prompt同时打开测试
            // for (int i = 0; i < 10; i++)
            // {
            //     GameModule.UI.OpenForm<PromptTestForm>(userData: i);
            // }

            // prompt测试，可以基于Waitable接口做出队列等待效果
            // var form = GameModule.UI.OpenForm<PromptTestForm>(userData: 0);
            // form.Wait(() => GameModule.UI.OpenForm<PromptTestForm>(userData: 1));

            AddClick(View.m_test_btn, OnTestBtnClick);
            AddEvent(LanguageChangeEventArgs.EventId, OnLanguageChange);
        }
        
        private void OnTestNumChange(int num)
        {
            Log.Info($"MVC Test : {num}");
        }

        private void OnTestBtnClick()
        {
            Cfg.SwitchLanguage(Language.English);
        }

        private void OnLanguageChange(object sender,GameEventArgs gameEventArgs)
        {
            var lang = ((LanguageChangeEventArgs)gameEventArgs).Lang;
            GameModule.UI.SwitchFairyBranch("en");
            Log.Info("OnLanguageChange:",lang.ToString());
        }
    }
}