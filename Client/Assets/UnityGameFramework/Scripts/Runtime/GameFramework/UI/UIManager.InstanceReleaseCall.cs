namespace GameFramework.UI
{
    internal sealed partial class UIManager : GameFrameworkModule, IUIManager
    {
        private delegate void OnFormInstanceReleaseCall(UIFormBase uiForm);
    }
}