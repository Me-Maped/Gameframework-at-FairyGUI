using FairyGUI;
using GameFramework.UI;

namespace UnityGameFramework.Runtime
{
    public abstract class UIFormPart<T> : UIFormPartBase where T : GComponent
    {
        protected T View;
        protected override void SerializeChild()
        {
            View = Instance as T;
        }
    }
}