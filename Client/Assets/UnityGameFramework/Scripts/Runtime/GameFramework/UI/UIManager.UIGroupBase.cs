using System;
using System.Collections.Generic;
using FairyGUI;
using GameFramework.ObjectPool;
using UnityEngine;

namespace GameFramework.UI
{
    internal sealed partial class UIManager : GameFrameworkModule, IUIManager
    {
        private class UIGroupBase : IUIGroup
        {
            private List<Type> _cachedFormTypes = new();

            protected const string MASK_NAME = "MASK";
            protected readonly List<UIFormBase> m_UIForms;
            protected readonly List<UIFormBase> m_CachedList;
            protected readonly UIGroupEnum m_GroupEnum;
            protected readonly IObjectPool<UIFormInstanceObject> m_InstancePool;

            protected GComponent m_GroupRoot;
            protected GGraph m_Mask;
            protected UIFormBase m_TopForm;
            protected bool m_CanClick = true;
            public GComponent Instance => m_GroupRoot;
            public List<UIFormBase> UIForms => m_UIForms;
            public UIGroupEnum GroupEnum => m_GroupEnum;
            public bool CloseOthers { get; set; }

            public GGraph Mask
            {
                get
                {
                    if (m_Mask == null)
                    {
                        m_Mask = new GGraph
                        {
                            name = MASK_NAME
                        };
                        m_Mask.DrawRect(GRoot.inst.width, GRoot.inst.height, 0,
                            Color.black, Color.black);
                        m_Mask.SetSize(GRoot.inst.width, GRoot.inst.height);
                        Instance.AddChild(m_Mask);
                    }

                    return m_Mask;
                }
            }

            public UIGroupBase(UIGroupEnum groupEnum,IObjectPool<UIFormInstanceObject> instancePool)
            {
                m_GroupEnum = groupEnum;
                m_InstancePool = instancePool;
                m_UIForms = new List<UIFormBase>();
                m_CachedList = new List<UIFormBase>();
                string groupName = groupEnum.ToString();
                m_GroupRoot = new GComponent { gameObjectName = groupName, name = groupName };
                m_GroupRoot.SetPosition(0, 0, (int)groupEnum);
                m_GroupRoot.SetSize(GRoot.inst.width, GRoot.inst.height);
                m_GroupRoot.AddRelation(GRoot.inst, RelationType.Size);
                m_GroupRoot.sortingOrder = -(int)groupEnum;
                GRoot.inst.AddChild(m_GroupRoot);
            }

            public virtual void OpenForm(UIFormBase uiFormBase)
            {
                if (CloseOthers)
                {
                    foreach (var uiForm in m_UIForms)
                    {
                        if (uiForm == uiFormBase) continue;
                        _cachedFormTypes.Add(uiForm.GetType());
                    }
                    foreach (var formType in _cachedFormTypes)
                    {
                        GameFrameworkEntry.GetModule<IUIManager>().CloseForm(formType);
                    }
                    _cachedFormTypes.Clear();
                    CloseOthers = false;
                }
                m_UIForms.Remove(uiFormBase);
                m_UIForms.Add(uiFormBase);
                uiFormBase.InternalOpen();
                uiFormBase.Instance.z = (int)GroupEnum - m_UIForms.Count;
            }

            public virtual void CloseForm(UIFormBase uiFormBase)
            {
                if(uiFormBase.Instance!=null) m_InstancePool.Unspawn(uiFormBase.Instance);
                if (!m_UIForms.Contains(uiFormBase)) return;
                m_UIForms.Remove(uiFormBase);
                if (uiFormBase.IsWaitingForData)
                {
                    uiFormBase.CloseOffline();
                }
                else
                {
                    uiFormBase.InternalClose();
                }
            }

            public virtual void CloseFormImmediately(UIFormBase uiFormBase)
            {
                if(uiFormBase.Instance!=null) m_InstancePool.Unspawn(uiFormBase.Instance);
                if (!m_UIForms.Contains(uiFormBase)) return;
                m_UIForms.Remove(uiFormBase);
                if (uiFormBase.IsWaitingForData)
                {
                    uiFormBase.CloseOffline();
                }
                else
                {
                    uiFormBase.InternalCloseImmediately();
                }
            }

            public virtual void CloseAllForm()
            {
                if (m_UIForms.Count <= 0) return;
                for (int i = m_UIForms.Count - 1; i >= 0; i--)
                {
                    UIFormBase uiForm = m_UIForms[i];
                    if (uiForm.Instance != null) m_InstancePool.Unspawn(uiForm.Instance);
                    if (uiForm.IsWaitingForData)
                    {
                        uiForm.CloseOffline();
                    }
                    else
                    {
                        uiForm.InternalClose();
                    }
                }

                m_UIForms.Clear();
            }

            public virtual void AddToStage()
            {
                ChangeMask();
            }

            public UIFormBase GetForm(Type formType)
            {
                return m_UIForms.Find(x => x.GetType() == formType);
            }

            public UIFormBase GetForm(string formName)
            {
                return m_UIForms.Find(x => x.Config.InstName.Equals(formName));
            }

            public virtual void RemoveFromStage(UIFormBase uiFormBase)
            {
                uiFormBase.Instance?.RemoveFromParent();
                m_Mask?.RemoveFromParent();
                m_UIForms.Remove(uiFormBase);
                if (m_UIForms.Count > 0)
                {
                    m_TopForm = m_UIForms[m_UIForms.Count - 1];
                    ChangeMask();
                }
                else
                {
                    ChangeMask();
                }
            }

            public virtual UIFormBase GetTopForm()
            {
                return m_TopForm;
            }

            public virtual void Update(float elapseSeconds, float realElapseSeconds)
            {
                foreach (var uiForm in m_UIForms)
                {
                    uiForm.Update(elapseSeconds, realElapseSeconds);
                }
            }

            public virtual void Shutdown()
            {
                foreach (var uiForm in m_UIForms)
                {
                    uiForm.Shutdown();
                }

                m_UIForms.Clear();
                m_TopForm = null;
                m_Mask?.Dispose();
                m_GroupRoot.Dispose();
            }

            private void ChangeMask()
            {
                m_TopForm = null;
                Timers.inst.Remove(AddClickNextFrame);
                if (m_UIForms.Count == 0)
                {
                    Mask?.RemoveFromParent();
                    return;
                }

                int modalIndex = -1;
                bool isModalTop = false;
                UIFormBase top = m_UIForms[m_UIForms.Count - 1];
                for (int i = m_UIForms.Count - 1; i >= 0; i--)
                {
                    UIFormBase uiForm = m_UIForms[i];
                    if (uiForm.Config.IsModal)
                    {
                        modalIndex = i;
                        isModalTop = i == m_UIForms.Count - 1;
                        break;
                    }
                }

                if (modalIndex == -1)
                {
                    Mask?.RemoveFromParent();
                    m_GroupRoot.AddChild(top.Instance);
                    m_TopForm = top;
                    return;
                }

                Mask.alpha = top.Config.ModalAlpha;
                Mask.onClick.Remove(OnClickMask);
                m_CanClick = false;
                if (isModalTop) m_GroupRoot.AddChild(Mask);
                m_GroupRoot.AddChild(top.Instance);
                m_TopForm = top;
                Timers.inst.Add(0.15f, 1, AddClickNextFrame);
            }

            private void AddClickNextFrame(object obj)
            {
                m_Mask.onClick.Set(OnClickMask);
                m_CanClick = true;
            }

            private void OnClickMask(EventContext context)
            {
                if (m_TopForm == null || m_UIForms.Count <= 0 || m_UIForms[m_UIForms.Count - 1] != m_TopForm) return;
                if (!m_TopForm.Config.ModalLogic || m_TopForm.InHideMode || !m_CanClick) return;
                m_CanClick = false;
                m_Mask.onClick.Remove(OnClickMask);
                GameFrameworkEntry.GetModule<IUIManager>().CloseForm(m_TopForm);
            }
        }
    }
}