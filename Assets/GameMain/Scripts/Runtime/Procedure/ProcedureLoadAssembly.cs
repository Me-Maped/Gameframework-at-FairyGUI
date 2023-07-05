﻿using System;
using System.Collections.Generic;
using System.IO;
using GameFramework;
using GameFramework.Procedure;
using GameFramework.Resource;
using GameFramework.Fsm;

using HybridCLR;
using UnityEngine;
using UnityGameFramework.Runtime;
using System.Reflection;
using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain
{
    /// <summary>
    /// 流程加载器 - 代码初始化
    /// </summary>
    public class ProcedureLoadAssembly : ProcedureBase
    {
        /// <summary>
        /// 是否需要加载热更新DLL
        /// </summary>
        public bool NeedLoadDll => GameModule.Resource.PlayMode == EPlayMode.HostPlayMode || GameModule.Resource.PlayMode == EPlayMode.OfflinePlayMode;

        private bool m_enableAddressable = false;
        private LoadAssetCallbacks m_LoadAssetCallbacks;
        private LoadAssetCallbacks m_LoadMetadataAssetCallbacks;
        private int m_LoadAssetCount;
        private int m_LoadMetadataAssetCount;
        private int m_FailureAssetCount;
        private int m_FailureMetadataAssetCount;
        private bool m_LoadAssemblyComplete;
        private bool m_LoadMetadataAssemblyComplete;
        private bool m_LoadAssemblyWait;
        private bool m_LoadMetadataAssemblyWait;
        private Assembly m_MainLogicAssembly;
        private List<Assembly> m_HotfixAssemblys;
        private ProcedureOwner _procedureOwner;

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            _procedureOwner = procedureOwner;
            Log.Info("HyBridCLR ProcedureLoadAssembly OnEnter");
            m_LoadAssemblyComplete = false;
            m_HotfixAssemblys = new List<Assembly>();

            if (!NeedLoadDll || GameModule.Resource.PlayMode == EPlayMode.EditorSimulateMode)
            {
                m_MainLogicAssembly = GetMainLogicAssembly();
            }
            else
            {
                if (SettingsUtils.HybridCLRCustomGlobalSettings.Enable)
                {
                    m_LoadAssetCallbacks ??= new LoadAssetCallbacks(LoadAssetSuccess, LoadAssetFailure);
                    foreach (string hotUpdateDllName in SettingsUtils.HybridCLRCustomGlobalSettings.HotUpdateAssemblies)
                    {
                        string assetLocation = hotUpdateDllName;
                        if (!m_enableAddressable)
                        {
                            assetLocation = Utility.Path.GetRegularPath(
                                Path.Combine(
                                    "Assets",
                                    SettingsUtils.HybridCLRCustomGlobalSettings.AssemblyTextAssetPath,
                                    $"{hotUpdateDllName}{SettingsUtils.HybridCLRCustomGlobalSettings.AssemblyTextAssetExtension}"));
                        }
                           
                        Log.Debug($"LoadAsset: [ {assetLocation} ]");
                        m_LoadAssetCount++;
                        GameModule.Resource.LoadAssetAsync(assetLocation,typeof(UnityEngine.TextAsset), m_LoadAssetCallbacks, hotUpdateDllName);
                    }

                    m_LoadAssemblyWait = true;
                }
                else
                {
                    m_MainLogicAssembly = GetMainLogicAssembly();
                }
            }

            if (SettingsUtils.HybridCLRCustomGlobalSettings.Enable)
            {
                if (Define.PkgArg.HybridLoadMeta)
                {
                    m_LoadMetadataAssemblyComplete = false;
                    LoadMetadataForAOTAssembly();
                }
                else
                {
                    m_LoadMetadataAssemblyComplete = true;
                }
            }
            else
            {
                m_LoadMetadataAssemblyComplete = true;
            }

            if (m_LoadAssetCount == 0)
            {
                m_LoadAssemblyComplete = true;
            }
        }
        
        protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner,float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner,elapseSeconds, realElapseSeconds);
            if (!m_LoadAssemblyComplete)
            {
                return;
            }
            if (!m_LoadMetadataAssemblyComplete)
            {
                return;
            }
            AllAssemblyLoadComplete();
        }
        
        private void AllAssemblyLoadComplete()
        {
            if (m_MainLogicAssembly == null)
            {
                Log.Fatal("Main logic assembly missing.");
                return;
            }
            var appType = m_MainLogicAssembly.GetType("GameApp");
            if (appType == null)
            {
                Log.Fatal("Main logic type 'GameApp' missing.");
                return;
            }
            var entryMethod = appType.GetMethod("Entrance");
            if (entryMethod == null)
            {
                Log.Fatal("Main logic entry method 'Entrance' missing.");
                return;
            }
            object[] objects = { new object[] { m_HotfixAssemblys } };
            entryMethod.Invoke(appType, objects);
            ChangeState<ProcedureStartGame>(_procedureOwner);
        }

        private Assembly GetMainLogicAssembly()
        {
            Assembly mainLogicAssembly = null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (string.Compare(SettingsUtils.HybridCLRCustomGlobalSettings.LogicMainDllName, $"{assembly.GetName().Name}.dll",
                        StringComparison.Ordinal) == 0)
                {
                    mainLogicAssembly = assembly;
                }

                foreach (var hotUpdateDllName in SettingsUtils.HybridCLRCustomGlobalSettings.HotUpdateAssemblies)
                {
                    if (hotUpdateDllName == $"{assembly.GetName().Name}.dll")
                    {
                        m_HotfixAssemblys.Add(assembly);
                    }
                }

                if (mainLogicAssembly != null && m_HotfixAssemblys.Count == SettingsUtils.HybridCLRCustomGlobalSettings.HotUpdateAssemblies.Count)
                {
                    break;
                }
            }

            return mainLogicAssembly;
        }

        /// <summary>
        /// 加载代码资源成功回调。
        /// </summary>
        /// <param name="assetName">资源名称。</param>
        /// <param name="asset">资源实例。</param>
        /// <param name="duration">加载耗时。</param>
        /// <param name="userData">用户数据。</param>
        private void LoadAssetSuccess(string assetName, object asset, float duration, object userData)
        {
            m_LoadAssetCount--;
            Log.Debug($"LoadAssetSuccess, assetName: [ {assetName} ], duration: [ {duration} ], userData: [ {userData} ]");
            var textAsset = asset as TextAsset;
            if (textAsset == null)
            {
                Log.Warning($"Load text asset [ {assetName} ] failed.");
                return;
            }

            try
            {
                Assembly assembly = Assembly.Load(textAsset.bytes);
                if (string.Compare(SettingsUtils.HybridCLRCustomGlobalSettings.LogicMainDllName, userData as string, StringComparison.Ordinal) == 0)
                {
                    m_MainLogicAssembly = assembly;
                }
                m_HotfixAssemblys.Add(assembly);
                Log.Debug($"Assembly [ {assembly.GetName().Name} ] loaded");
            }
            catch (Exception e)
            {
                m_FailureAssetCount++;
                Log.Fatal(e);
                throw;
            }
            finally
            {
                m_LoadAssemblyComplete = m_LoadAssemblyWait && 0 == m_LoadAssetCount;
            }
        }

        /// <summary>
        /// 加载代码资源失败回调。
        /// </summary>
        /// <param name="assetName">资源名称。</param>
        /// <param name="status">加载状态。</param>
        /// <param name="errorMessage">错误信息。</param>
        /// <param name="userData">自定义数据。</param>
        private void LoadAssetFailure(string assetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            Log.Fatal($"LoadAssetFailure, assetName: [ {assetName} ], status: [ {status} ], errorMessage: [ {errorMessage} ], userData: [ {userData} ]");
            m_LoadAssetCount--;
            m_FailureAssetCount++;
        }
        
        /// <summary>
        /// 为Aot Assembly加载原始metadata， 这个代码放Aot或者热更新都行。
        /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行。
        /// </summary>
        public void LoadMetadataForAOTAssembly()
        {
            // 可以加载任意aot assembly的对应的dll。但要求dll必须与unity build过程中生成的裁剪后的dll一致，而不能直接使用原始dll。
            // 我们在BuildProcessor_xxx里添加了处理代码，这些裁剪后的dll在打包时自动被复制到 {项目目录}/HybridCLRData/AssembliesPostIl2CppStrip/{Target} 目录。

            // 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
            // 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
            if (SettingsUtils.HybridCLRCustomGlobalSettings.AOTMetaAssemblies.Count == 0)
            {
                m_LoadMetadataAssemblyComplete = true;
                return;
            }
            m_LoadMetadataAssetCallbacks ??= new LoadAssetCallbacks(LoadMetadataAssetSuccess, LoadMetadataAssetFailure);
            foreach (var aotDllName in SettingsUtils.HybridCLRCustomGlobalSettings.AOTMetaAssemblies)
            {
                var assetLocation = aotDllName;
                if (!m_enableAddressable)
                {
                    assetLocation = Utility.Path.GetRegularPath(
                        Path.Combine(
                            "Assets",
                            SettingsUtils.HybridCLRCustomGlobalSettings.AssemblyTextAssetPath,
                            $"{aotDllName}{SettingsUtils.HybridCLRCustomGlobalSettings.AssemblyTextAssetExtension}"));
                }
                Log.Debug($"LoadMetadataAsset: [ {assetLocation} ]");
                m_LoadMetadataAssetCount++;
                GameModule.Resource.LoadAssetAsync(assetLocation,typeof(UnityEngine.TextAsset), m_LoadMetadataAssetCallbacks, aotDllName);
            }
            m_LoadMetadataAssemblyWait = true;
        }
        
        /// <summary>
        /// 加载元数据资源成功回调。
        /// </summary>
        /// <param name="assetName">资源名称。</param>
        /// <param name="asset">资源实例。</param>
        /// <param name="duration">加载耗时。</param>
        /// <param name="userData">用户数据。</param>
        private unsafe void LoadMetadataAssetSuccess(string assetName, object asset, float duration, object userData)
        {
            m_LoadMetadataAssetCount--;
            Log.Debug($"LoadMetadataAssetSuccess, assetName: [ {assetName} ], duration: [ {duration} ], userData: [ {userData} ]");
            var textAsset = asset as TextAsset;
            if (null == textAsset)
            {
                Log.Debug($"LoadMetadataAssetSuccess:Load text asset [ {assetName} ] failed.");
                return;
            }

            try
            {
                byte[] dllBytes = textAsset.bytes;
                fixed (byte* ptr = dllBytes)
                {
                    // 加载assembly对应的dll，会自动为它hook。一旦Aot泛型函数的native函数不存在，用解释器版本代码
                    HomologousImageMode mode = HomologousImageMode.SuperSet;
                    LoadImageErrorCode err = (LoadImageErrorCode)HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(dllBytes,mode); 
                    Log.Warning($"LoadMetadataForAOTAssembly:{userData as string}. mode:{mode} ret:{err}");
                }
            }
            catch (Exception e)
            {
                m_FailureMetadataAssetCount++;
                Log.Fatal(e.Message);
                throw;
            }
            finally
            {
                m_LoadMetadataAssemblyComplete = m_LoadMetadataAssemblyWait && 0 == m_LoadMetadataAssetCount;
            }
        }
        
        /// <summary>
        /// 加载元数据资源失败回调。
        /// </summary>
        /// <param name="assetName">资源名称。</param>
        /// <param name="status">加载状态。</param>
        /// <param name="errorMessage">错误信息。</param>
        /// <param name="userData">自定义数据。</param>
        private void LoadMetadataAssetFailure(string assetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            Log.Warning($"LoadAssetFailure, assetName: [ {assetName} ], status: [ {status} ], errorMessage: [ {errorMessage} ], userData: [ {userData} ]");
            m_LoadMetadataAssetCount--;
            m_FailureMetadataAssetCount++;
        }
    }
}