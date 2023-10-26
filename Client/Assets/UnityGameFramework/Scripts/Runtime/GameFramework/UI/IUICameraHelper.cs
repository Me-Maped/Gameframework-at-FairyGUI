namespace GameFramework.UI
{
    public interface IUICameraHelper
    {
        /// <summary>
        /// 初始化相机设置
        /// </summary>
        void InitCamera();
        
        /// <summary>
        /// 将UI相机挂在到目标相机上，如果项目使用URP，可以在这里添加后处理
        /// </summary>
        /// <param name="targetCamera"></param>
        void UICameraAttach(UnityEngine.Camera targetCamera);
    }
}