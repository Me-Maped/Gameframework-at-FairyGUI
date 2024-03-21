using System;
using FairyGUI;
using GameFramework.ObjectPool;

namespace GameFramework.UI
{
    internal sealed partial class UIManager : GameFrameworkModule, IUIManager
    {
        /// <summary>
        /// 界面实例对象。
        /// </summary>
        private sealed class UIFormInstanceObject : ObjectBase
        {
            private UIFormBase m_Form;
            private OnFormInstanceReleaseCall m_ReleaseCall;

            public UIFormInstanceObject()
            {
                m_Form = null;
                m_ReleaseCall = null;
            }


            internal static UIFormInstanceObject Create(UIFormBase uiForm, OnFormInstanceReleaseCall releaseCall)
            {
                UIFormInstanceObject uiFormInstanceObject = ReferencePool.Acquire<UIFormInstanceObject>();
                uiFormInstanceObject.Initialize(uiForm.Config.ResName, uiForm.Instance);
                uiFormInstanceObject.m_ReleaseCall = releaseCall;
                uiFormInstanceObject.m_Form = uiForm;
                return uiFormInstanceObject;
            }

            public override void Clear()
            {
                base.Clear();
                m_Form = null;
                m_ReleaseCall = null;
            }

            protected internal override void Release(bool isShutdown)
            {
                if (isShutdown) return;
                if (Target is GComponent component) component.Dispose();
                m_ReleaseCall(m_Form);
            }
        }
    }
}