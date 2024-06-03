using FairyGUI;
using UnityEngine;

namespace GameFramework.UI
{
    public class UIFormBg
    {
        private UILoader m_Loader;
        private string m_BgUrl;

        public string Key;
        public string BgUrl => m_BgUrl;
        public UILoader Loader
        {
            get
            {
                if (m_Loader == null)
                {
                    m_Loader = new UILoader
                    {
                        gameObjectName = $"UIFormBg_{Key}",
                        touchable = false,
                        align = AlignType.Center,
                        verticalAlign = VertAlignType.Middle,
                        fill = FillType.ScaleFree,
                        position = Vector3.zero,
                        sortingOrder = -(int)UIGroupEnum.BACKGROUND,
                    };
                    var scaleX = GRoot.inst.width / SettingsUtils.FrameworkGlobalSettings.UIWidth;
                    var scaleY = GRoot.inst.height / SettingsUtils.FrameworkGlobalSettings.UIHeight;
                    var scale = scaleX> scaleY ? scaleX : scaleY;
                    var w = (int) (SettingsUtils.FrameworkGlobalSettings.UIBgWidth * scale);
                    var h = (int) (SettingsUtils.FrameworkGlobalSettings.UIBgHeight * scale);
                    m_Loader.SetSize(w, h);
                    m_Loader.x = -(w - GRoot.inst.width) * 0.5f;
                    m_Loader.y = -(h - GRoot.inst.height) * 0.5f;
                    m_Loader.z = (int)UIGroupEnum.BACKGROUND;
                }
                return m_Loader;
            }
        }

        public void Load(string url, string name)
        {
            Loader.visible = true;
            Loader.name = name;
            m_BgUrl = url;
            m_Loader.url = url;
        }

        public void Unload()
        {
            Loader.visible = false;
            Loader.url = null;
            m_BgUrl = null;
        }

        public void Dispose()
        {
            if (Loader == null || Loader.displayObject.gameObject == null) return;
            Loader.visible = false;
            Loader.RemoveFromParent();
            Loader.Dispose();
            m_Loader = null;
            m_BgUrl = null;
        }

        public void Open(float time)
        {
            if (Loader == null) return;
            GTween.Kill(Loader);
            Loader.visible = true;
            if (time <= 0)
            {
                Loader.alpha = 1;
            }
            else
            {
                Loader.alpha = 0.5f;
                Loader.TweenFade(1, time);
            }
        }

        public void Close(float time)
        {
            GTween.Kill(Loader);
            if (time <= 0)
            {
                Loader.alpha = 0;
            }
            else
            {
                Loader.TweenFade(0, time);
            }
        }
        
        public void SetVisible(bool visible)
        {
            if (visible)
            {
                Loader.alpha = 1;
                Loader.visible = true;
            }
            else
            {
                Loader.TweenFade(0, 0.2f).OnComplete(() => { Loader.visible = false; });
            }
        }
        
        public void CloseImmediately()
        {
            GTween.Kill(Loader);
            Loader.alpha = 0;
            Loader.visible = false;
        }
    }
}