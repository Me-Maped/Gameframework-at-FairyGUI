using System;
using GameLogic.Common;
using GameLogic.Login;
using GameMain;
using UnityGameFramework.Runtime;

public class GameApp : Singleton<GameApp>
{
    /// <summary>
    /// 热更域App主入口。
    /// </summary>
    /// <param name="objects"></param>
    public static void Entrance(object[] objects)
    {
        Log.Warning("======= 看到此条日志代表你成功运行了热更新代码 =======");
        Log.Warning("======= Entrance GameApp =======");
    }

    /// <summary>
    /// 开始游戏业务层逻辑。
    /// <remarks>显示UI、加载场景等。</remarks>
    /// </summary>
    public static async void StartGameLogic()
    {
        Log.Warning("======= StartGameLogic GameApp =======");
        // 加载配置
        await ConfigLoader.Instance.Load();

        // 打开默认场景
        await ScenesLogicSys.Instance.EnterDefaultScene();

        // 初始化系统
        Instance.InitSystem();
        
        // TODO 测试打开同一个界面
        GameModule.UI.OpenForm<LoginForm>(closeOther: true);
        GameModule.UI.OpenForm<LoginForm>(closeOther: true);
        
        UILoadMgr.Instance.TweenHide();
    }

    /// <summary>
    /// 关闭游戏。
    /// </summary>
    /// <param name="shutdownType">关闭游戏框架类型。</param>
    [Obsolete("Please Use UnityGameFramework.Runtime.GameEntry.Shutdown")]
    public static void Shutdown(ShutdownType shutdownType)
    {
        UnityGameFramework.Runtime.GameEntry.Shutdown(shutdownType);
    }
    
    private void InitSystem()
    {
        InitSystemSetting();
        RegisterAllSystem();
    }
    
    /// <summary>
    /// 设置一些通用的系统属性。
    /// </summary>
    private void InitSystemSetting()
    {
        
    }

    /// <summary>
    /// 注册所有逻辑系统
    /// </summary>
    private void RegisterAllSystem()
    {
        GameModule.Logic.AddLogicSys(ScenesLogicSys.Instance);
        GameModule.Logic.AddLogicSys(FormLogicSys.Instance);
        GameModule.Logic.AddLogicSys(LoginLogicSys.Instance);
    }
}