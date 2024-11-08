using FairyGUI;
using GameFramework.UI;
using UnityGameFramework.Runtime;

namespace GameLogic.Login
{
    public class PromptTestForm : UIForm<UI_prompt_text>
    {
        protected override UIFormConfig CreateConfig()
        {
            return UIFormConfig.Create(UI_prompt_text.PKG_NAME, UI_prompt_text.RES_NAME, inBackList: false, groupEnum: UIGroupEnum.TIPS);
        }

        protected override void OnInit()
        {
            Instance.Center();
        }

        protected override void OnOpen()
        {
            Log.Info("{0}", (int)UserData);
            View.m_title.text = "提示: " + (int)UserData;
            Timers.inst.Add(5, 1, o => { SelfClose(); });
        }
    }
}