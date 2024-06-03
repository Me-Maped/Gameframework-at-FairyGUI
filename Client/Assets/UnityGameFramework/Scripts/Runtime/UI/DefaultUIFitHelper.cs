using FairyGUI;
using GameFramework.UI;
using UnityEngine;

namespace UnityGameFramework.Runtime
{
    public class DefaultUIFitHelper : UIFitHelperBase
    {
        public override void FitForm(UIFormBase uiForm)
        {
            if (uiForm.Config.GroupEnum == UIGroupEnum.POP)
                FitPop(uiForm);
            else if(uiForm.Config.GroupEnum == UIGroupEnum.PANEL)
                FitPanel(uiForm);
        }

        public override void FitLoader(GLoader loader)
        {
            var ui = GameEntry.GetComponent<UIComponent>();
            var notch = ui.SafeNotch;
            var scaleX = GRoot.inst.width / SettingsUtils.FrameworkGlobalSettings.UIBgWidth;
            var scaleY = GRoot.inst.height / SettingsUtils.FrameworkGlobalSettings.UIBgHeight;
            var height = GRoot.inst.height + notch.x + notch.y;
            var scale = scaleX > scaleY ? scaleX : scaleY;
            var w = (int)(SettingsUtils.FrameworkGlobalSettings.UIBgWidth * scale);
            var h = (int)(SettingsUtils.FrameworkGlobalSettings.UIBgHeight * scale);
            loader.SetSize(w, h);
            loader.x = -(w - GRoot.inst.width) * 0.5f;
            loader.y = -(h - height) * 0.5f;
        }

        public override void FitComponent(GObject obj)
        {
            var ui = GameEntry.GetComponent<UIComponent>();
            var notch = ui.SafeNotch;
            var moveDown = Mathf.CeilToInt(notch.y / GRoot.contentScaleFactor);
            var moveRight = Mathf.CeilToInt(notch.x / GRoot.contentScaleFactor);
            obj.height = Mathf.CeilToInt(Screen.height / GRoot.contentScaleFactor - moveDown);
            obj.width = Mathf.CeilToInt(Screen.width / GRoot.contentScaleFactor - moveRight);
            obj.y = moveDown;
            obj.x = moveRight;
        }

        private void FitPanel(UIFormBase uiForm)
        {
            if (uiForm.Config.StyleEnum == UIFormStyleEnum.FIX_SIZE)
            {
                uiForm.Instance.Center();
                return;
            }
            var ui = GameEntry.GetComponent<UIComponent>();
            var notch = ui.SafeNotch;
            var groupInst = ui.GetUIGroup(uiForm.Config.GroupEnum).Instance;
            var moveDown = Mathf.CeilToInt(notch.y / GRoot.contentScaleFactor);
            var moveRight = Mathf.CeilToInt(notch.x / GRoot.contentScaleFactor);
            groupInst.height = Mathf.CeilToInt(Screen.height / GRoot.contentScaleFactor - moveDown);
            groupInst.width = Mathf.CeilToInt(Screen.width / GRoot.contentScaleFactor - moveRight);
            groupInst.y = moveDown;
            groupInst.x = moveRight;
            uiForm.Instance.SetSize(groupInst.width, groupInst.height);
            uiForm.Instance.AddRelation(groupInst, RelationType.Size);
            uiForm.Instance.Center();
        }

        private void FitPop(UIFormBase uiForm)
        {
            if (uiForm.Config.StyleEnum == UIFormStyleEnum.FULL_SCREEN) uiForm.Instance.MakeFullScreen();
            uiForm.Instance.Center(true);
        }
    }
}