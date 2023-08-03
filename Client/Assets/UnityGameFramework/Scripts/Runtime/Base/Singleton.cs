/// <summary>
/// 全局单例对象（非线程安全）
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> where T : Singleton<T>, new()
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (null == _instance)
            {
                _instance = new T();
                _instance.Init();
            }

            return _instance;
        }
    }

    public static bool IsValid
    {
        get { return _instance != null; }
    }

    protected Singleton()
    {

    }

    protected virtual void Init()
    {

    }

    public virtual void Active()
    {

    }

    public virtual void Release()
    {
        _instance = null;
    }
}