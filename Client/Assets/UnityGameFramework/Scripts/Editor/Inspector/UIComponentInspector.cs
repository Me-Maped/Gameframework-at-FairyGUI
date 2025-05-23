﻿using UnityEditor;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Editor
{
    [CustomEditor(typeof(UIComponent))]
    internal sealed class UIComponentInspector : GameFrameworkInspector
    {
        private SerializedProperty m_EnableOpenUIFormSuccessEvent = null;
        private SerializedProperty m_EnableOpenUIFormFailureEvent = null;
        private SerializedProperty m_EnableOpenUIFormUpdateEvent = null;
        private SerializedProperty m_EnableOpenUIFormDependencyAssetEvent = null;
        private SerializedProperty m_EnableCloseUIFormCompleteEvent = null;
        private SerializedProperty m_InstanceAutoReleaseInterval = null;
        private SerializedProperty m_InstanceCapacity = null;
        private SerializedProperty m_InstanceExpireTime = null;
        private SerializedProperty m_InstancePriority = null;
        private SerializedProperty m_InstanceRoot = null;
        private SerializedProperty m_LaunchTipsSettingName = null;
        private SerializedProperty m_LaunchPkgNames = null;
        
        private HelperInfo<UIFitHelperBase> m_FitHelperInfo = new HelperInfo<UIFitHelperBase>("UIFit");
        private HelperInfo<UIJumpHelperBase> m_JumpHelperInfo = new HelperInfo<UIJumpHelperBase>("UIJump");
        private HelperInfo<UICameraHelperBase> m_CameraHelperInfo = new HelperInfo<UICameraHelperBase>("UICamera");
        private HelperInfo<UIL10NHelperBase> m_L10NHelperInfo = new HelperInfo<UIL10NHelperBase>("UIL10N");
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            UIComponent t = (UIComponent)target;

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                EditorGUILayout.PropertyField(m_EnableOpenUIFormSuccessEvent);
                EditorGUILayout.PropertyField(m_EnableOpenUIFormFailureEvent);
                EditorGUILayout.PropertyField(m_EnableOpenUIFormUpdateEvent);
                EditorGUILayout.PropertyField(m_EnableOpenUIFormDependencyAssetEvent);
                EditorGUILayout.PropertyField(m_EnableCloseUIFormCompleteEvent);
                EditorGUILayout.PropertyField(m_LaunchTipsSettingName);
                EditorGUILayout.PropertyField(m_LaunchPkgNames, true);
                m_FitHelperInfo.Draw();
                m_JumpHelperInfo.Draw();
                m_CameraHelperInfo.Draw();
                m_L10NHelperInfo.Draw();
            }
            EditorGUI.EndDisabledGroup();

            float instanceAutoReleaseInterval = EditorGUILayout.DelayedFloatField("Instance Auto Release Interval", m_InstanceAutoReleaseInterval.floatValue);
            if (instanceAutoReleaseInterval != m_InstanceAutoReleaseInterval.floatValue)
            {
                if (EditorApplication.isPlaying)
                {
                    t.InstanceAutoReleaseInterval = instanceAutoReleaseInterval;
                }
                else
                {
                    m_InstanceAutoReleaseInterval.floatValue = instanceAutoReleaseInterval;
                }
            }

            int instanceCapacity = EditorGUILayout.DelayedIntField("Instance Capacity", m_InstanceCapacity.intValue);
            if (instanceCapacity != m_InstanceCapacity.intValue)
            {
                if (EditorApplication.isPlaying)
                {
                    t.InstanceCapacity = instanceCapacity;
                }
                else
                {
                    m_InstanceCapacity.intValue = instanceCapacity;
                }
            }

            float instanceExpireTime = EditorGUILayout.DelayedFloatField("Instance Expire Time", m_InstanceExpireTime.floatValue);
            if (instanceExpireTime != m_InstanceExpireTime.floatValue)
            {
                if (EditorApplication.isPlaying)
                {
                    t.InstanceExpireTime = instanceExpireTime;
                }
                else
                {
                    m_InstanceExpireTime.floatValue = instanceExpireTime;
                }
            }

            int instancePriority = EditorGUILayout.DelayedIntField("Instance Priority", m_InstancePriority.intValue);
            if (instancePriority != m_InstancePriority.intValue)
            {
                if (EditorApplication.isPlaying)
                {
                    t.InstancePriority = instancePriority;
                }
                else
                {
                    m_InstancePriority.intValue = instancePriority;
                }
            }
            
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                EditorGUILayout.PropertyField(m_InstanceRoot);
            }
            EditorGUI.EndDisabledGroup();

            if (EditorApplication.isPlaying && IsPrefabInHierarchy(t.gameObject))
            {
                EditorGUILayout.LabelField("UI Group Count", t.UIGroupCount.ToString());
            }

            serializedObject.ApplyModifiedProperties();

            Repaint();
        }

        protected override void OnCompileComplete()
        {
            base.OnCompileComplete();

            RefreshTypeNames();
        }

        private void OnEnable()
        {
            m_EnableOpenUIFormSuccessEvent = serializedObject.FindProperty("m_EnableOpenUIFormSuccessEvent");
            m_EnableOpenUIFormFailureEvent = serializedObject.FindProperty("m_EnableOpenUIFormFailureEvent");
            m_EnableOpenUIFormUpdateEvent = serializedObject.FindProperty("m_EnableOpenUIFormUpdateEvent");
            m_EnableOpenUIFormDependencyAssetEvent = serializedObject.FindProperty("m_EnableOpenUIFormDependencyAssetEvent");
            m_EnableCloseUIFormCompleteEvent = serializedObject.FindProperty("m_EnableCloseUIFormCompleteEvent");
            m_InstanceAutoReleaseInterval = serializedObject.FindProperty("m_InstanceAutoReleaseInterval");
            m_InstanceCapacity = serializedObject.FindProperty("m_InstanceCapacity");
            m_InstanceExpireTime = serializedObject.FindProperty("m_InstanceExpireTime");
            m_InstancePriority = serializedObject.FindProperty("m_InstancePriority");
            m_InstanceRoot = serializedObject.FindProperty("m_InstanceRoot");
            m_LaunchTipsSettingName = serializedObject.FindProperty("m_LaunchTipsSettingName");
            m_LaunchPkgNames = serializedObject.FindProperty("m_LaunchPkgNames");
            m_FitHelperInfo.Init(serializedObject);
            m_JumpHelperInfo.Init(serializedObject);
            m_CameraHelperInfo.Init(serializedObject);
            m_L10NHelperInfo.Init(serializedObject);
            RefreshTypeNames();
        }

        private void RefreshTypeNames()
        {
            m_FitHelperInfo.Refresh();
            m_JumpHelperInfo.Refresh();
            m_CameraHelperInfo.Refresh();
            m_L10NHelperInfo.Refresh();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
