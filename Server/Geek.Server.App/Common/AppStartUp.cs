﻿using Geek.Server.Core.Actors.Impl;
using Geek.Server.Core.Comps;
using Geek.Server.Core.Hotfix;
using Geek.Server.Core.Storage;
using Geek.Server.Core.Utils;
using Geek.Server.Proto;
using NLog;
using NLog.Config;
using NLog.LayoutRenderers;
using PolymorphicMessagePack; 

namespace Geek.Server.App.Common
{
    internal class AppStartUp
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();
        public static async Task Enter()
        {
            try
            {
                var flag = Start();
                if (!flag) return; //启动服务器失败

                Log.Info($"launch embedded db...");
                ActorLimit.Init(ActorLimit.RuleType.None);
                GameDB.Init();
                GameDB.Open();
                Log.Info($"regist comps...");
                await CompRegister.Init();
                Log.Info($"load hotfix module");
                await HotfixMgr.LoadHotfixModule();

                Log.Info("进入游戏主循环...");
                Console.WriteLine("***进入游戏主循环***");
                Settings.LauchTime = DateTime.Now;
                Settings.AppRunning = true;
                TimeSpan delay = TimeSpan.FromSeconds(1);
                while (Settings.AppRunning)
                {
                    await Task.Delay(delay);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"服务器执行异常，e:{e}");
                Log.Fatal(e);
            }

            Console.WriteLine($"退出服务器开始");
            await HotfixMgr.Stop();
            Console.WriteLine($"退出服务器成功");
        }

        private static bool Start()
        {
            try
            {
                Settings.Load<AppSetting>("Configs/app_config.json", ServerType.Game);
                Console.WriteLine("init NLog config...");
                LayoutRenderer.Register<NLogConfigurationLayoutRender>("logConfiguration");
                LogManager.Configuration = new XmlLoggingConfiguration("Configs/app_log.config");
                LogManager.AutoShutdown = false;

                PolymorphicTypeMapper.Register(typeof(AppStartUp).Assembly); //app
                PolymorphicRegister.Load();
                PolymorphicResolver.Instance.Init();

                return true;
            }
            catch (Exception e)
            {
                Log.Error($"启动服务器失败,异常:{e}");
                return false;
            }
        }
    }
}
