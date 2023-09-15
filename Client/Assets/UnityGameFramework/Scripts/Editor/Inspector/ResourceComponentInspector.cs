using System;
using UnityEditor;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Editor
{
    [CustomEditor(typeof(ResourceComponent))]
    internal sealed class ResourceComponentInspector : GameFrameworkInspector
    {
        private SerializedProperty _minUnloadUnusedAssetsInterval = null;
        private SerializedProperty _maxUnloadUnusedAssetsInterval = null;
        private SerializedProperty _packageName = null;
        private SerializedProperty _playMode = null;
        private SerializedProperty _vertifyLevel = null;
        private SerializedProperty _readWritePathType = null;
        private SerializedProperty _milliseconds = null;
        private SerializedProperty _downloadMaxNum = null;
        private SerializedProperty _failedTryAgain = null;

        private int _minUnloadUnusedAssetsIntervalValue = 60;
        private int _maxUnloadUnusedAssetsIntervalValue = 300;
        private string _packageNameValue = "MEngine";
        private YooAsset.EPlayMode _playModeValue = YooAsset.EPlayMode.HostPlayMode;
        private YooAsset.EVerifyLevel _verifyLevelValue = YooAsset.EVerifyLevel.Middle;
        private ReadWritePathType _readWritePathTypeValue = ReadWritePathType.Unspecified;
        private int _millisecondsValue = 30;
        private int _downloadMaxNumValue = 1;
        private int _failedTryAgainValue = 1;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            serializedObject.Update();
            
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            EditorGUILayout.PropertyField(_minUnloadUnusedAssetsInterval);
            EditorGUILayout.PropertyField(_maxUnloadUnusedAssetsInterval);
            EditorGUILayout.PropertyField(_packageName);
            EditorGUILayout.PropertyField(_playMode);
            EditorGUILayout.PropertyField(_vertifyLevel);
            EditorGUILayout.PropertyField(_readWritePathType);
            EditorGUILayout.PropertyField(_milliseconds);
            EditorGUILayout.PropertyField(_downloadMaxNum);
            EditorGUILayout.PropertyField(_failedTryAgain);
            EditorGUI.EndDisabledGroup();
            serializedObject.ApplyModifiedProperties();
            
            Repaint();
        }

        private void OnEnable()
        {
            _minUnloadUnusedAssetsInterval = serializedObject.FindProperty("m_MinUnloadUnusedAssetsInterval");
            _maxUnloadUnusedAssetsInterval = serializedObject.FindProperty("m_MaxUnloadUnusedAssetsInterval");
            _packageName = serializedObject.FindProperty("PackageName");
            _playMode = serializedObject.FindProperty("PlayMode");
            _vertifyLevel = serializedObject.FindProperty("VerifyLevel");
            _readWritePathType = serializedObject.FindProperty("m_ReadWritePathType");
            _milliseconds = serializedObject.FindProperty("Milliseconds");
            _downloadMaxNum = serializedObject.FindProperty("m_DownloadingMaxNum");
            _failedTryAgain = serializedObject.FindProperty("m_FailedTryAgain");
        }
    }
}
