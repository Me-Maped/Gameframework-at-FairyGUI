using System;
using UnityEngine;

namespace GameMain
{
    [Serializable]
    public class VersionBean
    {
        [SerializeField] 
        public string internalGameVersion; // 内部游戏版本号
        [SerializeField]
        public string resourceVersion; // 资源版本号
        [SerializeField]
        public string codeVersion; // 代码版本号
        [SerializeField] 
        public string configVersion; // 配置版本号
    }
}