using System.Collections.Generic;
using System.Threading.Tasks;
using GameFramework.Localization;
using cfg;
using Luban;
using UnityEngine;
using UnityGameFramework.Runtime;
using Task = System.Threading.Tasks.Task;

/// <summary>
/// Luban配置加载器
/// </summary>
public static class Cfg
{
    private static bool m_init;
    private static Tables m_tables;
    private static Dictionary<string, ByteBuf> m_Configs = new();

    public static Tables Tables
    {
        get
        {
            if (!m_init)
            {
                Log.Error("Config not loaded.");
            }
            return m_tables;
        }
    }

    /// <summary>
    /// 表加载
    /// </summary>
    public static async Task Load()
    {
        m_tables = new Tables();
        await m_tables.LoadAsync(LoadByteBuf);
        await SwitchLanguage(GameModule.Localization.Language);
        m_init = true;
    }

    /// <summary>
    /// 切换语言
    /// </summary>
    /// <param name="language"></param>
    public static async Task SwitchLanguage(Language language)
    {
        switch (language)
        {
            case Language.ChineseSimplified:
                await m_tables.SwitchToLocalizeCN(LoadByteBuf,Translator_CN);
                break;
            case Language.English:
                await m_tables.SwitchToLocalizeEN(LoadByteBuf, Translator_EN);
                break;
            default:
                Log.Fatal($"Not registered language = {language}");
                break;
        }
        GameModule.Localization.Language = language;
    }

    /// <summary>
    /// 根据key获取单个文本多语言
    /// </summary>
    /// <param name="language"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string GetL10N(Language language,string key)
    {
        string result = string.Empty;
        switch (language)
        {
            case Language.ChineseSimplified:
                result = m_tables.LocalizeCN.GetOrDefault(key)?.TextCn;
                break;
            case Language.English:
                result = m_tables.LocalizeEN.GetOrDefault(key)?.TextEn;
                break;
        }

        if (string.IsNullOrEmpty(result))
        {
            Log.Warning("Not registered language = {language},  key = {key}", language, key);
            return key;
        }
        return result;
    }

    /// <summary>
    /// 获取当前语言本地化字符串
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string GetL10N(this string key)
    {
        return GetL10N(GameModule.Localization.Language, key);
    }

    /// <summary>
    /// cn Translator
    /// </summary>
    /// <param name="key"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    private static string Translator_CN(string key, string origin)
    {
        return m_tables.LocalizeCN.GetOrDefault(key)?.TextCn ?? origin;
    }
    
    /// <summary>
    /// en Translator
    /// </summary>
    /// <param name="key"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    private static string Translator_EN(string key, string origin)
    {
        return m_tables.LocalizeEN.GetOrDefault(key)?.TextEn ?? origin;
    }

    /// <summary>
    /// 加载二进制配置
    /// </summary>
    /// <param name="file">FileName</param>
    /// <returns>ByteBuf</returns>
    private static async Task<ByteBuf> LoadByteBuf(string file)
    {
        var key = $"{file}.bytes";
        if (m_Configs.TryGetValue(key, out var buf))
        {
            return buf;
        }
        var textAssets = await GameModule.Resource.LoadAssetTaskAsync<TextAsset>($"{SettingsUtils.FrameworkGlobalSettings.ConfigFolderName}{file}.bytes");
        buf = new ByteBuf(textAssets.bytes);
        m_Configs[key] = buf;
        return buf;
    }
}