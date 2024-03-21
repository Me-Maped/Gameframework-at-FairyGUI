namespace GameFramework.UI
{
    public interface IUIModel : IReference
    {
        internal void Init(IUIController controller);

        internal void Open();
        
        internal void Close();
        
        internal void Update(float elapseSeconds, float realElapseSeconds);
    }
}