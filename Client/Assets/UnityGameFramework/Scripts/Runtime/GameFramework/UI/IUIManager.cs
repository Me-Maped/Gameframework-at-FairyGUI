using System;
using GameFramework.ObjectPool;
using GameFramework.Resource;

namespace GameFramework.UI
{
    public interface IUIManager
    {
        /// <summary>
        /// 获取或设置界面实例对象池自动释放可释放对象的间隔秒数。
        /// </summary>
        float InstanceAutoReleaseInterval
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置界面实例对象池的容量。
        /// </summary>
        int InstanceCapacity
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置界面实例对象池对象过期秒数。
        /// </summary>
        float InstanceExpireTime
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置界面实例对象池的优先级。
        /// </summary>
        int InstancePriority
        {
            get;
            set;
        }
        
        /// <summary>
        /// 获取界面组数量。
        /// </summary>
        int UIGroupCount { get; }
        
        /// <summary>
        /// 适配辅助器
        /// </summary>
        IUIFitHelper FitHelper { get; }
        
        /// <summary>
        /// 跳转辅助器
        /// </summary>
        IUIJumpHelper JumpHelper { get; }
        
        /// <summary>
        /// 相机辅助器
        /// </summary>
        IUICameraHelper CameraHelper { get; }
        
        /// <summary>
        /// 多语言辅助器
        /// </summary>
        IUIL10NHelper L10NHelper { get; }
        
        /// <summary>
        /// 加载Form资源成功事件
        /// </summary>
        event EventHandler<LoadFormSuccessEventArgs> LoadFormSuccess;
        
        /// <summary>
        /// 加载Form资源失败事件
        /// </summary>
        event EventHandler<LoadFormFailureEventArgs> LoadFormFailure;
        
        /// <summary>
        /// 加载Form资源更新事件
        /// </summary>
        event EventHandler<LoadFormUpdateEventArgs> LoadFormUpdate;
        
        /// <summary>
        /// 加载Form资源时加载依赖资源事件
        /// </summary>
        event EventHandler<LoadFormDependencyEventArgs> LoadFormDependency;
        
        /// <summary>
        /// 关闭Form完成事件
        /// </summary>
        event EventHandler<CloseFormCompleteEventArgs> CloseFormComplete; 

        /// <summary>
        /// 对象池
        /// </summary>
        /// <param name="objectPoolManager"></param>
        void SetObjectPoolManager(IObjectPoolManager objectPoolManager);
        
        /// <summary>
        /// 资源管理器
        /// </summary>
        /// <param name="resourceManager"></param>
        void SetResourceManager(IResourceManager resourceManager);
        
        /// <summary>
        /// 设置UI适配器
        /// </summary>
        /// <param name="uiFitHelper"></param>
        void SetUIFitHelper(IUIFitHelper uiFitHelper);
        
        /// <summary>
        /// 设置UI跳转器
        /// </summary>
        /// <param name="uiJumpHelper"></param>
        void SetUIJumpHelper(IUIJumpHelper uiJumpHelper);
        
        /// <summary>
        /// 设置UI摄像机辅助器
        /// </summary>
        /// <param name="uiCameraHelper"></param>
        void SetUICameraHelper(IUICameraHelper uiCameraHelper);

        /// <summary>
        /// 设置多语言辅助器
        /// </summary>
        /// <param name="uiL10NHelper"></param>
        void SetUIL10NHelper(IUIL10NHelper uiL10NHelper);
        
        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="formType"></param>
        /// <param name="closeOther"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        UIFormBase OpenForm(Type formType, bool closeOther, object userData);
        
        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="closeOther">是否关闭同组下其他界面</param>
        /// <param name="userData"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        UIFormBase OpenForm<T>(bool closeOther, object userData) where T : UIFormBase;
       
        /// <summary>
        /// 获取界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        UIFormBase GetForm<T>() where T : UIFormBase;
        
        /// <summary>
        /// 获取界面
        /// </summary>
        /// <param name="formType"></param>
        /// <returns></returns>
        UIFormBase GetForm(System.Type formType);
        
        /// <summary>
        /// 根据界面名称获取界面
        /// </summary>
        /// <param name="formName"></param>
        /// <returns></returns>
        UIFormBase GetFormByName(string formName);

        /// <summary>
        /// 获取最上层的界面
        /// </summary>
        /// <returns></returns>
        UIFormBase GetTopForm();
        
        /// <summary>
        /// 获取某个组最上层的界面
        /// </summary>
        /// <param name="groupEnum"></param>
        /// <returns></returns>
        UIFormBase GetTopForm(UIGroupEnum groupEnum);
        
        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void CloseForm<T>() where T : UIFormBase;
        
        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <param name="uiForm"></param>
        void CloseForm(Type uiForm);
        
        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <param name="uiForm"></param>
        void CloseForm(UIFormBase uiForm);
        
        /// <summary>
        /// 关闭所有界面
        /// </summary>
        void CloseAllForm();

        /// <summary>
        /// 关闭某一类型所有界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void CloseForms<T>() where T : UIFormBase;

        /// <summary>
        /// 关闭某一类型所有界面
        /// </summary>
        /// <param name="formType"></param>
        void CloseForms(Type formType);

        /// <summary>
        /// 立即关闭界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void CloseFormImmediately<T>() where T : UIFormBase;

        /// <summary>
        /// 立即关闭界面
        /// </summary>
        /// <param name="formType"></param>
        void CloseFormImmediately(Type formType);
        
        /// <summary>
        /// 立即关闭界面
        /// </summary>
        /// <param name="uiForm"></param>
        void CloseFormImmediately(UIFormBase uiForm);
        
        /// <summary>
        /// 关闭某一组的界面
        /// </summary>
        /// <param name="groupEnum"></param>
        void CloseFormByGroup(UIGroupEnum groupEnum);
        
        /// <summary>
        /// 存在某个界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool HasForm<T>() where T : UIFormBase;
        
        /// <summary>
        /// 存在某个界面
        /// </summary>
        /// <param name="formType"></param>
        /// <returns></returns>
        bool HasForm(System.Type formType);
        
        /// <summary>
        /// 获取界面组
        /// </summary>
        /// <param name="groupEnum"></param>
        /// <returns></returns>
        IUIGroup GetUIGroup(UIGroupEnum groupEnum);

        /// <summary>
        /// 增加界面组
        /// </summary>
        /// <param name="groupEnum"></param>
        /// <returns></returns>
        IUIGroup AddUIGroup(UIGroupEnum groupEnum);
        
        /// <summary>
        /// 增加界面到舞台
        /// </summary>
        /// <param name="uiForm"></param>
        void AddToStage(UIFormBase uiForm);
        
        /// <summary>
        /// 从舞台移除界面
        /// </summary>
        /// <param name="uiForm"></param>
        void RemoveFromStage(UIFormBase uiForm);
        
        /// <summary>
        /// 设置界面实例是否被加锁
        /// </summary>
        /// <param name="uiFormInstance"></param>
        /// <param name="locked"></param>
        void SetUIFormInstanceLocked(object uiFormInstance, bool locked);
        
        /// <summary>
        /// 设置界面实例优先级
        /// </summary>
        /// <param name="uiFormInstance"></param>
        /// <param name="priority"></param>
        void SetUIFormInstancePriority(object uiFormInstance, int priority);

        /// <summary>
        /// 切换Fairy中的UI分支
        /// </summary>
        /// <param name="branchName">分支名称</param>
        void SwitchFairyBranch(string branchName);
    }
}