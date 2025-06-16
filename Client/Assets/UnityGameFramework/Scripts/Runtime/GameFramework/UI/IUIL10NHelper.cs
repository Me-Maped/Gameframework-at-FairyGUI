namespace GameFramework.UI
{
    public interface IUIL10NHelper
    {
        /// <summary>
        /// 根据key获取多语言文本
        /// </summary>
        /// <param name="language">需要翻译的语言</param>
        /// <param name="key">多语言ID</param>
        /// <returns></returns>
        string GetL10NString(Localization.Language language,string key);
    }
}