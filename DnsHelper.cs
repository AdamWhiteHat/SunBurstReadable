using System;
using System.Net;
using System.Net.Sockets;

namespace SunBurstDefanged
{
	public static class DnsHelper
	{
		public static bool CheckServerConnection(string hostName)
		{
			try
			{

				IPHostEntry iphostEntry = DnsHelper.GetIPHostEntry(hostName);
				if (iphostEntry != null)
				{
					IPAddress[] addressList = iphostEntry.AddressList;
					for (int i = 0; i < addressList.Length; i++)
					{
						AddressFamilyEx addressFamily = IPAddressesHelper.GetAddressFamily(addressList[i]);
						if (addressFamily != AddressFamilyEx.Error && addressFamily != AddressFamilyEx.Atm)
						{
							return true;
						}
					}
				}
			}
			catch (Exception)
			{
			}
			return false;
		}

		public static IPHostEntry GetIPHostEntry(string hostName)
		{
			int[][] array = new int[][]
			{
					new int[]
					{
						25,
						30
					},
					new int[]
					{
						55,
						60
					}
			};
			int num = array.GetLength(0) + 1;
			for (int i = 1; i <= num; i++)
			{
				try
				{
					return Dns.GetHostEntry(hostName);
				}
				catch (SocketException)
				{
				}
				if (i + 1 <= num)
				{
					Utilities.DelayMs((double)(array[i - 1][0] * 1000), (double)(array[i - 1][1] * 1000));
				}
			}
			return null;
		}

		public static AddressFamilyEx GetAddressFamily(string hostName, DnsRecords rec)
		{
			rec.cname = null;
			try
			{
				IPHostEntry iphostEntry = DnsHelper.GetIPHostEntry(hostName);
				if (iphostEntry == null)
				{
					Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Unable to get IP addresses for " + hostName);
					return AddressFamilyEx.Error;
				}
				IPAddress[] addressList = iphostEntry.AddressList;
				int i = 0;
				while (i < addressList.Length)
				{
					IPAddress ipaddress = addressList[i];
					Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Ip address resolved for " + hostName + " " + ipaddress);

					if (ipaddress.AddressFamily == AddressFamily.InterNetwork)
					{
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Address family is InterNetwork");
						if (!(iphostEntry.HostName != hostName) || string.IsNullOrEmpty(iphostEntry.HostName))
						{
							IPAddressesHelper.GetAddresses(ipaddress, rec);
							Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Geting addresses for " + ipaddress + "Rec" + rec);
							return IPAddressesHelper.GetAddressFamily(ipaddress, out rec.dnssec);
						}
						rec.cname = iphostEntry.HostName;
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Rec.cname is now " + iphostEntry.HostName);
						if (IPAddressesHelper.GetAddressFamily(ipaddress) == AddressFamilyEx.Atm)
						{
							Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Address family is InterNetwork");
							return AddressFamilyEx.Atm;
						}
						if (rec.dnssec)
						{
							Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - rec.DNSSEC is true");
							rec.dnssec = false;
							Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Address family is Netbios");
							return AddressFamilyEx.NetBios;
						}
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Unable to identify address family");
						return AddressFamilyEx.Error;
					}
					else
					{
						i++;
					}
				}
				return AddressFamilyEx.Unknown;
			}
			catch (Exception)
			{
			}
			return AddressFamilyEx.Error;
		}
	}
}
