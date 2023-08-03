using System;

namespace GameFramework.UI
{
    public interface IUIJumpHelper
    {
        /// <summary>
        /// 记录
        /// </summary>
        /// <param name="formType"></param>
        void Record(Type formType);
        /// <summary>
        /// 返回上一层PANEL
        /// </summary>
        void Back();
        /// <summary>
        /// 返回到主界面
        /// </summary>
        void GoHome();
    }
}