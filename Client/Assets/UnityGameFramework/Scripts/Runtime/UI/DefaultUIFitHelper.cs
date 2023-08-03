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
            else
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
            obj.height = Mathf.CeilToInt(Screen.height / GRoot.contentScaleFactor - moveDown);
            obj.y = moveDown;
        }

        private void FitPanel(UIFormBase uiForm)
        {
            var ui = GameEntry.GetComponent<UIComponent>();
            var notch = ui.SafeNotch;
            if (notch.y <= 0) return;
            var groupInst = ui.GetUIGroup(uiForm.Config.GroupEnum).Instance;
            var moveDown = Mathf.CeilToInt(notch.y / GRoot.contentScaleFactor);
            groupInst.height = Mathf.CeilToInt(Screen.height / GRoot.contentScaleFactor - moveDown);
            groupInst.y = moveDown;
            uiForm.Instance.SetSize(groupInst.width, groupInst.height);
            uiForm.Instance.AddRelation(groupInst, RelationType.Size);
        }

        private void FitPop(UIFormBase uiForm)
        {
            var ui = GameEntry.GetComponent<UIComponent>();
            var groupInst = ui.GetUIGroup(uiForm.Config.GroupEnum).Instance;
            uiForm.Instance.SetXY((groupInst.width - uiForm.Instance.width)/2, (groupInst.height-uiForm.Instance.height) / 2);
            uiForm.Instance.AddRelation(groupInst, RelationType.Size);
        }
    }
}