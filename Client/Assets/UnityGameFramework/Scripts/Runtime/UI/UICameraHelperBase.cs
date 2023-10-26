using GameFramework.UI;
using UnityEngine;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    /// UI相机辅助器基类
    /// </summary>
    public abstract class UICameraHelperBase : MonoBehaviour, IUICameraHelper
    {
        /// <summary>
        /// 初始化相机设置
        /// </summary>
        public abstract void InitCamera();

        /// <summary>
        /// 将UI相机挂在到目标相机上，如果项目使用URP，可以在这里添加后处理
        /// </summary>
        /// <param name="targetCamera"></param>
        public abstract void UICameraAttach(Camera targetCamera);
    }
}