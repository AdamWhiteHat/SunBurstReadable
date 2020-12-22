using System;
using System.Configuration;

namespace SunBurstDefanged
{
	public static class ConfigManager
	{
		public static bool ReadReportStatus(out ReportStatus status)
		{
			try
			{
				string s;
				int num;
				if (ConfigManager.ReadConfig(Settings.reportStatusName, out s) && int.TryParse(s, out num))
				{
					switch (num)
					{
						case 3:
							status = ReportStatus.Truncate;
							return true;
						case 4:
							status = ReportStatus.New;
							return true;
						case 5:
							status = ReportStatus.Append;
							return true;
					}
				}
			}
			catch (ConfigurationErrorsException)
			{
			}
			status = ReportStatus.New;
			return false;
		}

		public static bool ReadServiceStatus(bool _readonly)
		{
			try
			{
				string s;
				int num;
				if (ConfigManager.ReadConfig(Settings.serviceStatusName, out s) && int.TryParse(s, out num) && num >= 250 && num % 5 == 0 && num <= 250 + ((1 << Settings.svcList.Length) - 1) * 5)
				{
					num = (num - 250) / 5;
					if (!_readonly)
					{
						for (int i = 0; i < Settings.svcList.Length; i++)
						{
							Settings.svcList[i].stopped = ((num & 1 << i) != 0);
						}
					}
					return true;
				}
			}
			catch (Exception)
			{
			}
			if (!_readonly)
			{
				for (int j = 0; j < Settings.svcList.Length; j++)
				{
					Settings.svcList[j].stopped = true;
				}
			}
			return false;
		}

		public static bool WriteReportStatus(ReportStatus status)
		{
			ReportStatus reportStatus;
			if (ConfigManager.ReadReportStatus(out reportStatus))
			{
				switch (status)
				{
					// 4
					case ReportStatus.New:
						return ConfigManager.WriteConfig(Settings.reportStatusName, ZipHelper.Unzip("MwEA"));
					// 5
					case ReportStatus.Append:
						return ConfigManager.WriteConfig(Settings.reportStatusName, ZipHelper.Unzip("MwUA"));
					// 3
					case ReportStatus.Truncate:
						return ConfigManager.WriteConfig(Settings.reportStatusName, ZipHelper.Unzip("MwYA"));
				}
			}
			return false;
		}

		public static bool WriteServiceStatus()
		{
			if (ConfigManager.ReadServiceStatus(true))
			{
				int num = 0;
				for (int i = 0; i < Settings.svcList.Length; i++)
				{
					num |= (Settings.svcList[i].stopped ? 1 : 0) << i;
				}
				return ConfigManager.WriteConfig(Settings.serviceStatusName, (num * 5 + 250).ToString());
			}
			return false;
		}

		private static bool ReadConfig(string key, out string sValue)
		{
			sValue = null;
			try
			{
				sValue = ConfigurationManager.AppSettings[key];
				return true;
			}
			catch (Exception)
			{
			}
			return false;
		}

		private static bool WriteConfig(string key, string sValue)
		{
			try
			{
				Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
				KeyValueConfigurationCollection settings = configuration.AppSettings.Settings;
				if (settings[key] != null)
				{
					settings[key].Value = sValue;
					configuration.Save(ConfigurationSaveMode.Modified);
					ConfigurationManager.RefreshSection(configuration.AppSettings.SectionInformation.Name);
					return true;
				}
			}
			catch (Exception)
			{
			}
			return false;
		}
	}
}
