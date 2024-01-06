using System.Collections.Generic;
using FairyGUI;

namespace GameFramework.UI
{
    public interface IUIGroup
    {
        /// <summary>
        /// 界面组根节点
        /// </summary>
        GComponent Instance { get; }

        /// <summary>
        /// 界面组中的所有界面
        /// </summary>
        List<UIFormBase> UIForms { get; }

        /// <summary>
        /// 界面组枚举
        /// </summary>
        UIGroupEnum GroupEnum { get; }

        /// <summary>
        /// 是否关闭其他界面
        /// </summary>
        bool CloseOthers { get; set; }

        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="uiFormBase"></param>
        void OpenForm(UIFormBase uiFormBase);

        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <param name="uiFormBase"></param>
        void CloseForm(UIFormBase uiFormBase);

        /// <summary>
        /// 立即关闭界面
        /// </summary>
        /// <param name="uiFormBase"></param>
        void CloseFormImmediately(UIFormBase uiFormBase);

        /// <summary>
        /// 关闭所有界面
        /// </summary>
        void CloseAllForm();

        /// <summary>
        /// 添加到舞台上
        /// </summary>
        void AddToStage();

        /// <summary>
        /// 从舞台上移除
        /// </summary>
        /// <param name="uiFormBase"></param>
        void RemoveFromStage(UIFormBase uiFormBase);

        /// <summary>
        /// 根据类型获取Form
        /// </summary>
        /// <param name="formType"></param>
        /// <returns></returns>
        UIFormBase GetForm(System.Type formType);

        /// <summary>
        /// 根据名称获取Form
        /// </summary>
        /// <param name="formName"></param>
        /// <returns></returns>
        UIFormBase GetForm(string formName);

        /// <summary>
        /// 获取顶部界面
        /// </summary>
        /// <returns></returns>
        UIFormBase GetTopForm();

        /// <summary>
        /// 周期更新
        /// </summary>
        /// <param name="elapseSeconds"></param>
        /// <param name="realElapseSeconds"></param>
        void Update(float elapseSeconds, float realElapseSeconds);

        /// <summary>
        /// 关闭界面组
        /// </summary>
        void Shutdown();
    }
}