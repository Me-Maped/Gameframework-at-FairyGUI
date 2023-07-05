using System;
using UnityEngine;

namespace GameMain.Data
{
    [CreateAssetMenu(fileName = "LaunchTipsSettings", menuName = "Game Framework/LaunchTipsSettings")]
    public class LaunchTipsSettings : ScriptableObject
    {
        [SerializeField]
        public TipsLanguage[] tips;
        
        public LabelContent GetCurLanguageTips(string curLanguage)
        {
            foreach (var tip in tips)
            {
                if (tip.language == curLanguage)
                {
                    return tip.content;
                }
            }
            return tips[0].content;
        }
    }

    [Serializable]
    public class TipsLanguage
    {
        [SerializeField]
        public string language;
        [SerializeField]
        public LabelContent content;
    }

    [Serializable]
    public class LabelContent
    {
        public string LoadProgress = "正在下载资源文件，请耐心等待\n当前下载速度：{0}/s 资源文件大小：{1}";
        public string LoadFirstUnpack = "首次进入游戏，正在初始化游戏资源...（此过程不消耗网络流量）";
        public string LoadUnpacking = "正在更新本地资源版本，请耐心等待...（此过程不消耗网络流量）";
        public string LoadChecking = "检测版本文件{0}...";
        public string LoadChecked = "最新版本检测完成";
        public string LoadPackage = "当前使用的版本过低，请下载安装最新版本";
        public string LoadPlantform = "当前使用的版本过低，请前往应用商店安装最新版本";
        public string LoadNotice = "检测到可选资源更新，推荐完成更新提升游戏体验";
        public string LoadForce = "检测到版本更新，取消更新将导致无法进入游戏";
        public string LoadForceWIFI =
            "检测到有新的游戏内容需要更新，更新包大小[color=#BA3026]{0}[/color], 取消更新将导致无法进入游戏，您当前已为[color=#BA3026]wifi网络[/color]，请开始更新";
        public string LoadForceNOWIFI =
            "检测到有新的游戏内容需要更新，更新包大小[color=#BA3026]{0}[/color], 取消更新将导致无法进入游戏，请开始更新";
        public string LoadError = "更新参数错误{0}，请点击确定重新启动游戏";
        public string LoadFirstEntrerGameError = "首次进入游戏资源异常";
        public string LoadUnpackComplete = "正在加载最新资源文件...（此过程不消耗网络流量）";
        public string LoadUnPackError = "资源解压失败，请点击确定重新启动游戏";
        public string LoadLoadProgress = "正在载入...{0}%";
        public string LoadDownloadProgress = "正在下载...{0}%";
        public string LoadInit = "初始化...";
        public string NetUnReachable = "当前网络不可用，请检查本地网络设置后点击确认进行重试";
        public string NetReachableViaCarrierDataNetwork = "当前是移动网络，是否继续下载";
        public string NetError = "网络异常，请重试";
        public string NetChanged = "网络切换,正在尝试重连,{0}次";
        public string DataEmpty = "数据异常";
        public string MemoryLow = "初始化资源加载失败，请检查本地内存是否充足";
        public string MemoryLowLoad = "内存是否充足,无法更新";
        public string MemoryUnZipLow = "内存不足，无法解压";
        public string Appid = "游戏版本号:{0}";
        public string Resid = "资源版本号:{0}";
        public string ClearComfirm = "是否清理本地资源?(清理完成后会关闭游戏且重新下载最新资源)";
        public string RestartApp = "本次更新需要重启应用，请点击确定重新启动游戏";
        public string DownLoadFailed = "网络太慢，是否继续下载";
        public string ClearConfig = "清除环境配置，需要重启应用";
        public string RegionInfoIllegal = "区服信息为空";
        public string RemoteUrlisNull = "热更地址为空";
        public string FirstPackageNotFound = "首包资源加载失败";
        public string RequestReginInfo = "正在请求区服信息{0}次";
        public string RequestTimeOut = "请求区服信息超时,是否重试？";
        public string RegionArgumentError = "参数错误";
        public string RegionIndexOutOfRange = "索引越界";
        public string RegionNonConfigApplication = "未配置此应用";
        public string RegionSystemError = "系统异常";
        public string PreventionOfAddiction = "著作人权：XX市TEngine有限公司 软著登记号：2022SR0000000\n抵制不良游戏，拒绝盗版游戏。注意自我保护，谨防受骗上当。适度游戏益脑，" +
            "沉迷游戏伤身。合理安排时间，享受健康生活。";
        public string BtnUpdate = "确定";
        public string BtnIgnore = "取消";
        public string BtnPackage = "更新";
        public string DlcConfigVerificateStage = "配置校验中...";
        public string DlcConfigLoadingStage = "下载配置中...";
        public string DlcAssetsLoading = "下载资源中...";
        public string DlcLoadingFinish = "下载结束";
        public string DlcLoadForceWIFI =
            "检测到有新的游戏内容需要更新, 取消更新将导致无法进入游戏，您当前已为[color=#BA3026]wifi网络[/color]，请开始更新";
        public string DlcLoadForceNOWIFI =
            "检测到有新的游戏内容需要更新, 取消更新将导致无法进入游戏，请开始更新";
        public string HadUpdate = "检测到有版本更新...";
        public string RequestVersionIng = "正在向服务器请求版本信息中...";
        public string RequestVersionInfo = "正在向服务器请求版本信息{0}次";
    }
}