using GameFramework;

namespace GameMain
{
    public class MLogHelper: GameFrameworkLog.ILogHelper
    {
        public void Log(GameFrameworkLogLevel level, object message)
        {
            switch (level)
            {
                case GameFrameworkLogLevel.Debug:
                    MLogger.LogInfo(Utility.Text.Format("<color=#888888>{0}</color>", message));
                    break;
                case GameFrameworkLogLevel.Info:
                    MLogger.LogInfo(message.ToString());
                    break;

                case GameFrameworkLogLevel.Warning:
                    MLogger.LogWarning(message.ToString());
                    break;

                case GameFrameworkLogLevel.Error:
                    MLogger.LogError(message.ToString());
                    break;
                case GameFrameworkLogLevel.Fatal:
                    MLogger.LogException(message.ToString());
                    break;
                default:
                    throw new GameFrameworkException(message.ToString());
            }
        }
    }
}