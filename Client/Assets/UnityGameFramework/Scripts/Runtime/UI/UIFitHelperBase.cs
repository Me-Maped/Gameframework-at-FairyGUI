using FairyGUI;
using GameFramework.UI;
using UnityEngine;

namespace UnityGameFramework.Runtime
{
    public abstract class UIFitHelperBase : MonoBehaviour, IUIFitHelper
    {
        public abstract void FitForm(UIFormBase uiFormBase);

        public abstract void FitLoader(GLoader loader);

        public abstract void FitComponent(GObject obj);
    }
}