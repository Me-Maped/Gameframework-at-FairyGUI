using UnityEngine;

/// <summary>
/// 游戏入口。
/// </summary>
public partial class GameEntry : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        GameModule.Instance.Active();
        GameModule.Base.AddShutdownCall(OnFrameworkShutdown);
    }

    private void OnFrameworkShutdown()
    {
        GameModule.Instance.Shutdown();
        Destroy(gameObject);
    }
}