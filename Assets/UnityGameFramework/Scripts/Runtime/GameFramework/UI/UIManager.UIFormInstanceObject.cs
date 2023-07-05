using System;
using FairyGUI;
using GameFramework.Event;
using GameFramework.ObjectPool;
using GameFramework.Resource;

namespace GameFramework.UI
{
    internal sealed partial class UIManager : GameFrameworkModule, IUIManager
    {
        /// <summary>
        /// 界面实例对象。
        /// </summary>
        private sealed class UIFormInstanceObject : ObjectBase
        {
            private string m_PkgName;
            private Type m_FormType;
            private OnFormInstanceReleaseCall m_ReleaseCall;

            public UIFormInstanceObject()
            {
                m_PkgName = null;
                m_FormType = null;
                m_ReleaseCall = null;
            }

            public static UIFormInstanceObject Create(string name, string pkgName,Type formType,OnFormInstanceReleaseCall releaseCall, object uiFormInstance)
            {
                UIFormInstanceObject uiFormInstanceObject = ReferencePool.Acquire<UIFormInstanceObject>();
                uiFormInstanceObject.Initialize(name, uiFormInstance);
                uiFormInstanceObject.m_PkgName = pkgName;
                uiFormInstanceObject.m_FormType = formType;
                uiFormInstanceObject.m_ReleaseCall = releaseCall;
                return uiFormInstanceObject;
            }

            public override void Clear()
            {
                base.Clear();
                m_PkgName = null;
                m_FormType = null;
                m_ReleaseCall = null;
            }

            protected internal override void Release(bool isShutdown)
            {
                if(Target is GComponent component) component.Dispose();
                m_ReleaseCall(m_FormType,m_PkgName);
            }
        }
    }
}