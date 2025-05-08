using GameFramework.Localization;

namespace UnityGameFramework.Runtime
{
    public class DefaultUIL10NHelper : UIL10NHelperBase
    {
        public override string GetL10NString(Language language, string key)
        {
            // 由于使用了Luban作为配置系统，默认多语言辅助器将不对多语言进行额外处理
            return key;
        }
    }
}