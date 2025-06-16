using GameFramework.Localization;
using GameFramework.UI;
using UnityEngine;

namespace UnityGameFramework.Runtime
{
    public abstract class UIL10NHelperBase : MonoBehaviour, IUIL10NHelper
    {
        public abstract string GetL10NString(Language language, string key);
    }
}