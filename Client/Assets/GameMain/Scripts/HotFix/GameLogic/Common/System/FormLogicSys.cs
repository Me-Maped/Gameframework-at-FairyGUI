using UnityGameFramework.Runtime;

namespace GameLogic.Common
{
    public class FormLogicSys : BaseLogicSys<FormLogicSys>
    {
        public override bool OnInit()
        {
            UIBinder.BindAll();
            return true;
        }
    }
}