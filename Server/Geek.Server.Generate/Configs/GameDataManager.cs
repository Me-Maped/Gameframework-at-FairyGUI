using cfg;
using Luban;
using NLog;

namespace Geek.Server.Config
{
	public class GameDataManager
	{
		private const string BYTES_DIR = "Bytes";
		
		private static readonly NLog.Logger LOGGER = LogManager.GetCurrentClassLogger();
		public static GameDataManager Instance { get; private set; }

		private static bool m_init;
		private static Tables m_tables;
		private static Dictionary<string, ByteBuf> m_Configs;

		public DateTime ReloadTime { get; private set; }

		public static Tables Tables
		{
			get
			{
				if (!m_init)
				{
					LOGGER.Error("Config not loaded!!!");
				}

				return m_tables;
			}
		}

		public static async Task<(bool, string)> ReloadAll()
		{
			try
			{
				var data = new GameDataManager();
				await data.LoadAll(true);
				data.ReloadTime = DateTime.Now;
				Instance = data;
				return (true, "");
			}
			catch (Exception e)
			{
				LOGGER.Error(e.Message);
				return (false, e.Message);
			}
		}

		private GameDataManager()
		{
			m_Configs = new Dictionary<string, ByteBuf>();
		}

		public async Task LoadAll(bool forceReload = false)
		{
			if (forceReload)
			{
				m_init = false;
				m_Configs.Clear();
			}
			if (m_init) return;
			m_tables = new Tables();
			await m_tables.LoadAsync(LoadByteBuf);
			m_init = true;
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
			var configAssets = await File.ReadAllBytesAsync(Path.Combine(System.Environment.CurrentDirectory, BYTES_DIR, key));
			buf = new ByteBuf(configAssets);
			m_Configs[key] = buf;
			return buf;
		}
	}
}