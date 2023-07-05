/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace GameMain.Launch
{
    public partial class UI_launch_pop : GComponent
    {
        public Controller m_one_c;
        public GGraph m_model;
        public GGraph m_bg;
        public GTextInput m_title;
        public GTextField m_content;
        public GButton m_cancel_btn;
        public GButton m_confirm_btn;
        public const string URL = "ui://r4vl0iulnyr02";

        public static UI_launch_pop CreateInstance()
        {
            return (UI_launch_pop)UIPackage.CreateObject("Launch", "launch_pop");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_one_c = GetControllerAt(0);
            m_model = (GGraph)GetChildAt(0);
            m_bg = (GGraph)GetChildAt(1);
            m_title = (GTextInput)GetChildAt(2);
            m_content = (GTextField)GetChildAt(3);
            m_cancel_btn = (GButton)GetChildAt(4);
            m_confirm_btn = (GButton)GetChildAt(5);
        }
    }
}