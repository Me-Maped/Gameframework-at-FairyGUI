namespace GameFramework.UI
{
    internal sealed partial class UIManager : GameFrameworkModule, IUIManager
    {
        private delegate void OnFormInstanceReleaseCall(System.Type formType,string pkgName);
    }
}