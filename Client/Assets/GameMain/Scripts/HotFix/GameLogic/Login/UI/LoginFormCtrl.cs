using FairyGUI;
using UnityGameFramework.Runtime;

namespace GameLogic.Login
{
    public class LoginFormCtrl : UICtrlBase<LoginForm,LoginFormModel>
    {
        public override void Clear()
        {
            
        }

        protected override void OnOpen()
        {
            Timers.inst.Add(2f,10, TestNum);
        }

        protected override void OnClose()
        {
            Timers.inst.Remove(TestNum);
        }

        private void TestNum(object o)
        {
            Model.Test();
        }
    }
}