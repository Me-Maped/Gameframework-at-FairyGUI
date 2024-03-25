using FairyGUI;
using GameFramework.UI;
using UnityGameFramework.Runtime;

namespace GameLogic.Login
{
    public class LoginForm : UIForm<UI_login_main>
    {
        protected override UIFormConfig CreateConfig()
        {
            return UIFormConfig.Create(UI_login_main.PKG_NAME, UI_login_main.RES_NAME, groupEnum: UIGroupEnum.PANEL);
        }

        protected override void OnInit()
        {
            InitMVC<LoginFormCtrl>(LoginFormModel.Inst);

            LoginFormModel.Inst.Register<int>(nameof(LoginFormModel.TestNum), OnTestNumChange);
        }

        protected override void OnOpen()
        {
            // 表测试
            Timers.inst.Add(2f, 1, _ => { View.m_title.text = ConfigLoader.Instance.Tables.TbSkill.DataList[0].Name; });
            Timers.inst.Add(3f, 1, o => { View.m_title.visible = false; });
            // 测试包卸载
            // Timers.inst.Add(6f, 1, _ => { Close(); });

            // prompt同时打开测试
            // for (int i = 0; i < 10; i++)
            // {
            //     GameModule.UI.OpenForm<PromptTestForm>(userData: i);
            // }

            // prompt测试，可以基于Waitable接口做出队列等待效果
            var form = GameModule.UI.OpenForm<PromptTestForm>(userData: 0);
            form.Wait(() => GameModule.UI.OpenForm<PromptTestForm>(userData: 1));
        }
        
        private void OnTestNumChange(int num)
        {
            Log.Info($"MVC Test : {num}");
        }
    }
}