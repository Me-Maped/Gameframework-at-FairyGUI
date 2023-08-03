using FairyGUI;
using GameFramework.UI;

namespace UnityGameFramework.Runtime
{
    public abstract class UIForm<T> : UIFormBase where T : GComponent
    {
        protected T View;
        protected abstract override UIFormConfig CreateConfig();
        protected override void SerializeChild()
        {
            View = Instance as T;
        }

        #region Public Methods
        public void Close(bool immediately = false)
        {
            if (!immediately) GameEntry.GetComponent<UIComponent>().CloseForm(this);
            else GameEntry.GetComponent<UIComponent>().CloseFormImmediately(this);
        }

        #endregion
    }
}