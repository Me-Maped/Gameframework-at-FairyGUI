using FairyGUI;
using UnityEngine;
using UnityGameFramework.Runtime;

#if HDRP_ENABLE
using UnityEngine.Rendering.HighDefinition;
#endif

namespace GameMain
{
    public class HDRPUICameraHelper:UICameraHelperBase
    {
#if HDRP_ENABLE
        private HDCameraUI _hdCamera;
#endif
        
        public override void InitCamera()
        {
#if HDRP_ENABLE
            GameObject stageCamera = StageCamera.main.gameObject;
            HDAdditionalCameraData cameraData = stageCamera.GetOrAddComponent<HDAdditionalCameraData>();
            cameraData.clearColorMode = HDAdditionalCameraData.ClearColorMode.Color;
            cameraData.backgroundColorHDR = Color.black;
            cameraData.volumeLayerMask = 0;
            cameraData.probeLayerMask = 0;
            DontDestroyOnLoad(StageCamera.main);
#endif
        }

        public override void UICameraAttach(Camera targetCamera)
        {
#if HDRP_ENABLE
            _hdCamera ??= StageCamera.main.gameObject.GetOrAddComponent<HDCameraUI>();

            if (targetCamera == null)
            {
                _hdCamera.targetCamera = HDCameraUI.TargetCamera.Main;
            }
            else
            {
                _hdCamera.targetCamera = HDCameraUI.TargetCamera.Specific;
                _hdCamera.targetCameraObject = targetCamera;
            }
#endif
        }
    }
}