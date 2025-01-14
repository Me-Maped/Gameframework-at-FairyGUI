/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace GameLogic.Login
{
    public partial class UI_login_main : GComponent
    {
        public GTextField m_auto_l10n_txt;
        public GButton m_test_btn;
        public GTextField m_handle_l10n_txt;

        public const string URL = "ui://rr499cgxotfk0";
        public const string PKG_NAME = "Login";
        public const string RES_NAME = "login_main";

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_auto_l10n_txt = (GTextField)GetChildAt(0);
            m_test_btn = (GButton)GetChildAt(1);
            m_handle_l10n_txt = (GTextField)GetChildAt(3);
        }
    }
}