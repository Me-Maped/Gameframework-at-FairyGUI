using GameFramework.Localization;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameLogic.Common
{
    public class FormLogicSys : BaseLogicSys<FormLogicSys>
    {
        public override bool OnInit()
        {
            InitL10NHelper();
            UIBinder.BindAll();
            return true;
        }

        /// <summary>
        /// 初始化UI多语言辅助器。
        /// 默认辅助器为AOT侧的passthrough占位，此处替换为依赖热更Cfg的Luban实现。
        /// </summary>
        private void InitL10NHelper()
        {
            var helper = new GameObject().AddComponent<LubanUIL10NHelper>();
            GameModule.UI.SetL10NHelper(helper);
            // 触发已注册的L10N文本刷新
            GameModule.Event.Fire(null, LanguageChangeEventArgs.Create(GameModule.Localization.Language));
        }
    }
}