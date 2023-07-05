using System;
using System.Collections.Generic;

namespace UnityGameFramework.Runtime
{
    public class DefaultUIJumpHelper : UIJumpHelperBase
    {
        private Stack<Type> m_FormTypes = new Stack<Type>();

        public override void Record(Type formType)
        {
            if (m_FormTypes.Contains(formType))
            {
                // 将当前界面之上的界面全部出栈
                while (m_FormTypes.Peek() != formType)
                {
                    m_FormTypes.Pop();
                }
            }
            else
            {
                m_FormTypes.Push(formType);
            }
            LogStack();
        }

        public override void Back()
        {
            // 如果栈中只有一个界面，直接返回
            if (m_FormTypes.Count <= 1) return;
            // 将当前界面出栈
            GameEntry.GetComponent<UIComponent>().CloseForm(m_FormTypes.Pop());
            GameEntry.GetComponent<UIComponent>().OpenForm(m_FormTypes.Pop());
            LogStack();
        }

        public override void GoHome()
        {
            while (m_FormTypes.Count > 1)
            {
                GameEntry.GetComponent<UIComponent>().CloseForm(m_FormTypes.Pop());
            }
            GameEntry.GetComponent<UIComponent>().OpenForm(m_FormTypes.Pop());
            LogStack();
        }

        private void LogStack()
        {
#if ENABLE_LOG
            // 打印当前栈中的界面
            string log = "当前栈中的界面：";
            foreach (var type in m_FormTypes)
            {
                log += type.Name + "  ";
            }
            Log.Info(log);
#endif
        }
    }
}