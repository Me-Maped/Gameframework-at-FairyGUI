/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace GameMain.Launch
{
    public partial class UI_launch_test : GComponent
    {
        public GGraph m_bg;
        public GTextField m_title;
        public const string URL = "ui://r4vl0iulx7af8";

        public static UI_launch_test CreateInstance()
        {
            return (UI_launch_test)UIPackage.CreateObject("Launch", "launch_test");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_bg = (GGraph)GetChildAt(0);
            m_title = (GTextField)GetChildAt(1);
        }
    }
}