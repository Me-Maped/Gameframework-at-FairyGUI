/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace GameMain.Launch
{
    public partial class UI_launch_main : GComponent
    {
        public GLoader m_bg;
        public GProgressBar m_progress;
        public GTextField m_desc;
        public GTextField m_tips;
        public Transition m_In;
        public Transition m_Out;
        public const string URL = "ui://r4vl0iul9l2f0";

        public static UI_launch_main CreateInstance()
        {
            return (UI_launch_main)UIPackage.CreateObject("Launch", "launch_main");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_bg = (GLoader)GetChildAt(0);
            m_progress = (GProgressBar)GetChildAt(1);
            m_desc = (GTextField)GetChildAt(2);
            m_tips = (GTextField)GetChildAt(3);
            m_In = GetTransitionAt(0);
            m_Out = GetTransitionAt(1);
        }
    }
}