using FairyGUI;

namespace GameFramework.UI
{
    public interface IUIFitHelper
    {
        /// <summary>
        /// 适配界面
        /// </summary>
        /// <param name="uiFormBase"></param>
        void FitForm(UIFormBase uiFormBase);

        /// <summary>
        /// 适配加载器
        /// </summary>
        /// <param name="loader"></param>
        void FitLoader(GLoader loader);
        
        /// <summary>
        /// 适配组件
        /// </summary>
        /// <param name="obj"></param>
        void FitComponent(GObject obj);
    }
}