using UnityEngine;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    /// UI相机辅助器基类
    /// </summary>
    public class DefaultUICameraHelper : UICameraHelperBase
    {
        /// <summary>
        /// 初始化相机设置
        /// </summary>
        public override void InitCamera()
        {
            //TODO 如果使用URP，需要初始化为正交相机
        }

        /// <summary>
        /// 将UI相机挂在到目标相机上，如果项目使用URP，可以在这里添加后处理
        /// </summary>
        /// <param name="targetCamera"></param>
        public override void UICameraAttach(Camera targetCamera)
        {
            //TODO 如果使用URP，可以在这里添加后处理
        }
    }
}