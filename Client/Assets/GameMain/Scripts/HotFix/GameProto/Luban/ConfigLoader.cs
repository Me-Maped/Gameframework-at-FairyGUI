using System.Collections.Generic;
using System.Threading.Tasks;
using Bright.Serialization;
using GameConfig;
using UnityEngine;
using UnityGameFramework.Runtime;
using Task = System.Threading.Tasks.Task;

/// <summary>
/// 配置加载器
/// </summary>
public class ConfigLoader:Singleton<ConfigLoader>
{
    private bool m_init;
    private Tables m_tables;
    public Tables Tables
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

    private Dictionary<string, TextAsset> m_Configs = new Dictionary<string, TextAsset>();

    /// <summary>
    /// 表加载
    /// </summary>
    public async Task Load()
    {
        m_tables = new Tables();
        await m_tables.LoadAsync(LoadByteBuf);
        m_init = true;
    }
    
    /// <summary>
    /// 注册配置资源。
    /// </summary>
    /// <param name="key">资源Key</param>
    /// <param name="value">资源TextAsset</param>
    /// <returns></returns>
    public bool RegisterTextAssets(string key, TextAsset value)
    {
        if (string.IsNullOrEmpty(key))
        {
            return false;
        }

        if (value == null)
        {
            return false;
        }
        m_Configs[key] = value;
        return true;
    }

    /// <summary>
    /// 加载二进制配置
    /// </summary>
    /// <param name="file">FileName</param>
    /// <returns>ByteBuf</returns>
    private async Task<ByteBuf> LoadByteBuf(string file)
    {
        byte[] ret;
        var key = $"{file}.bytes";
        if (m_Configs.TryGetValue(key, out var config))
        {
            ret = config.bytes;
        }
        else
        {
            var textAssets = await GameModule.Resource.LoadAssetTaskAsync<TextAsset>($"{SettingsUtils.FrameworkGlobalSettings.ConfigFolderName}{file}.bytes");
            ret = textAssets.bytes;
            RegisterTextAssets(file, textAssets);
        }
        return new ByteBuf(ret);
    }
}