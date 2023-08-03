namespace GameMain.Data
{
    public enum ProgressStyleEnum
    {
        STYLE_DEFAULT = 0,//默认
        STYLE_QUITAPP = 1,//退出应用
        STYLE_RESTARTAPP = 2,//重启应用
        STYLE_RETRY = 3,//重试
        STYLE_STARTUPDATE_NOTICE = 4,//提示更新
        STYLE_DOWNLOADAPK = 5,//下载底包
        STYLE_CLEAR = 6,//修复客户端
        STYLE_DOWNZIP = 7,//继续下载压缩包
    }
}