using FairyGUI;
using UnityEngine;
using UnityGameFramework.Runtime;

#if URP_ENABLE
using UnityEngine.Rendering.Universal;
#endif


namespace GameMain
{
    public class URPUICameraHelper : UICameraHelperBase
    {
        public override void InitCamera()
        {
#if URP_ENABLE
            UniversalAdditionalCameraData cameraData = StageCamera.main.GetUniversalAdditionalCameraData();
            // 启动场景没有相机，ui相机需要设置为base
            cameraData.renderType = CameraRenderType.Base;
            Camera camera = StageCamera.main;
            camera.orthographic = true;
            camera.farClipPlane = 1000;
            camera.nearClipPlane = -1000;
            DontDestroyOnLoad(StageCamera.main);
#endif
        }

        public override void UICameraAttach(Camera targetCamera)
        {
#if URP_ENABLE
            Camera stageCamera = StageCamera.main;
            // ui相机挂在到场景上需要修改renderType为Overlay
            stageCamera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Overlay;
            UniversalAdditionalCameraData cameraData = targetCamera.GetUniversalAdditionalCameraData();
            if (cameraData.cameraStack.Contains(stageCamera)) return;
            cameraData.cameraStack.Add(stageCamera);
#endif
        }
    }
}