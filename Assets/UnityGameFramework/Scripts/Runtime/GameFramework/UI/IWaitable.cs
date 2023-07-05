namespace GameFramework.UI
{
    public interface IWaitable
    {
        void Wait(System.Action callback);
    }
}