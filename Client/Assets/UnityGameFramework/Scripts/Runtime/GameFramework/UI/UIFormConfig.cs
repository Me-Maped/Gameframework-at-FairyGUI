﻿using System;

namespace GameFramework.UI
{
    public class UIFormConfig : IReference
    {
        /// <summary>
        /// 界面包名
        /// </summary>
        public string PkgName { get; private set; }
        /// <summary>
        /// 界面UI资源名
        /// </summary>
        public string ResName { get; private set; }
        /// <summary>
        /// 界面实例名
        /// </summary>
        public string InstName { get; private set; }
        /// <summary>
        /// 背景资源路径
        /// </summary>
        public string BgUrl  { get; private set; }
        /// <summary>
        /// 界面依赖的资源包名
        /// </summary>
        public string[] Depends { get; private set; }
        /// <summary>
        /// 界面风格
        /// </summary>
        public UIFormStyleEnum StyleEnum { get; private set; }
        /// <summary>
        /// 界面所属的界面组
        /// </summary>
        public UIGroupEnum GroupEnum { get; private set; }
        /// <summary>
        /// 是否是模态窗口
        /// </summary>
        public bool IsModal { get; private set; }
        /// <summary>
        /// 模态窗口背景透明度
        /// </summary>
        public float ModalAlpha { get; private set; }
        /// <summary>
        /// 模态逻辑，即是否可以点击空白处关闭
        /// </summary>
        public bool ModalLogic { get; private set; }
        /// <summary>
        /// 是否加入返回队列中
        /// </summary>
        public bool InBackList { get; private set; }
        /// <summary>
        /// 关闭提示,只有在ModalLogic为true时才有效
        /// </summary>
        public string CloseTip { get; private set; }
        /// <summary>
        /// 界面在界面组中的深度
        /// </summary>
        public int DepthInUIGroup { get; private set; }
        /// <summary>
        /// 界面的子界面
        /// </summary>
        public Type[] FormParts { get; private set; }

        public UIFormConfig()
        {
            PkgName = null;
            ResName = null;
            InstName = null;
            BgUrl = null;
            Depends = null;
            StyleEnum = UIFormStyleEnum.FULL_SCREEN;
            GroupEnum = UIGroupEnum.NONE;
            IsModal = false;
            ModalAlpha = 0;
            ModalLogic = false;
            InBackList = false;
            CloseTip = null;
            DepthInUIGroup = 0;
            FormParts = null;
        }
        
        public void Clear()
        {
            PkgName = null;
            ResName = null;
            InstName = null;
            BgUrl = null;
            Depends = null;
            StyleEnum = UIFormStyleEnum.FULL_SCREEN;
            GroupEnum = UIGroupEnum.NONE;
            IsModal = false;
            ModalAlpha = 0;
            ModalLogic = false;
            InBackList = false;
            CloseTip = null;
            DepthInUIGroup = 0;
            FormParts = null;
        }

        /// <summary>
        /// 创建界面配置
        /// </summary>
        /// <param name="pkgName">界面包名</param>
        /// <param name="resName">界面UI资源名</param>
        /// <param name="instName">界面实例名</param>
        /// <param name="bgUrl">背景资源路径</param>
        /// <param name="depends">界面依赖的资源包名</param>
        /// <param name="styleEnum">界面风格</param>
        /// <param name="groupEnum">界面所属的界面组</param>
        /// <param name="isModal">是否是模态窗口</param>
        /// <param name="modalAlpha">模态窗口背景透明度</param>
        /// <param name="modalLogic">模态逻辑</param>
        /// <param name="inBackList">是否加入返回队列中</param>
        /// <param name="closeTip">关闭提示</param>
        /// <param name="depthInUIGroup">界面在界面组中的深度</param>
        /// <param name="formParts">界面的子界面</param>
        /// <returns></returns>
        public static UIFormConfig Create(string pkgName, string resName, 
            string instName = null, string bgUrl = null, string[] depends = null, 
            UIFormStyleEnum styleEnum = UIFormStyleEnum.FULL_SCREEN,
            UIGroupEnum groupEnum = UIGroupEnum.PANEL, bool isModal = false,
            float modalAlpha = 0.9f, bool modalLogic = false, bool inBackList = true, 
            string closeTip = null, int depthInUIGroup = 0, Type[] formParts = null)
        {
            UIFormConfig uiFormConfig = ReferencePool.Acquire<UIFormConfig>();
            uiFormConfig.PkgName = pkgName;
            uiFormConfig.ResName = resName;
            uiFormConfig.InstName = instName ?? resName;
            uiFormConfig.BgUrl = bgUrl;
            uiFormConfig.Depends = depends;
            uiFormConfig.StyleEnum = styleEnum;
            uiFormConfig.GroupEnum = groupEnum;
            uiFormConfig.IsModal = isModal;
            uiFormConfig.ModalAlpha = modalAlpha;
            uiFormConfig.ModalLogic = modalLogic;
            uiFormConfig.InBackList = inBackList;
            uiFormConfig.CloseTip = closeTip;
            uiFormConfig.DepthInUIGroup = depthInUIGroup;
            uiFormConfig.FormParts = formParts;
            return uiFormConfig;
        }
    }
}