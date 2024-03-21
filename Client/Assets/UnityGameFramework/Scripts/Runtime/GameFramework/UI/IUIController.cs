namespace GameFramework.UI
{
    public interface IUIController : IReference
    {
        internal void Init(UIFormBase uiForm, IUIModel uiModel);
        
        internal void Open();

        internal void Close();

        internal void Update(float elapseSeconds, float realElapseSeconds);
    }
}