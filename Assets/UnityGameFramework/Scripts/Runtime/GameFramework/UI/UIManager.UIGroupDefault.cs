namespace GameFramework.UI
{
    internal sealed partial class UIManager : GameFrameworkModule, IUIManager
    {
        private sealed class UIGroupDefault : UIGroupBase
        {
            public UIGroupDefault(UIGroupEnum groupEnum) : base(groupEnum)
            {
            }

            public override void OpenForm(UIFormBase uiFormBase)
            {
                base.OpenForm(uiFormBase);
                UpdateVisible();
            }

            public override void CloseForm(UIFormBase uiFormBase)
            {
                base.CloseForm(uiFormBase);
                UpdateVisible();
            }

            public override void CloseFormImmediately(UIFormBase uiFormBase)
            {
                base.CloseFormImmediately(uiFormBase);
                UpdateVisible();
            }

            private void UpdateVisible()
            {
                if (GroupEnum != UIGroupEnum.PANEL) return;
                bool isFullWindow = false;
                for (int i = m_UIForms.Count - 1; i >= 0; i--)
                {
                    UIFormBase uiForm = m_UIForms[i];
                    //关闭全屏下的窗口
                    if (isFullWindow)
                    {
                        GameFrameworkEntry.GetModule<IUIManager>().CloseForm(uiForm);
                        continue;
                    }

                    if (uiForm.Config.StyleEnum == UIFormStyleEnum.FULL_SCREEN) isFullWindow = true;
                    uiForm.SetVisible(true);
                }
            }
        }
    }
}