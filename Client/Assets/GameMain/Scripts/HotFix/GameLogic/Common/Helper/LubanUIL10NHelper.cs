using GameFramework.Localization;
using UnityGameFramework.Runtime;

namespace GameLogic.Common
{
    public class LubanUIL10NHelper : UIL10NHelperBase
    {
        public override string GetL10NString(Language language,string key)
        {
            return key.GetL10N();
        }
    }
}