using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace SunBurstDefanged
{
	public class IPAddressesHelper
	{
		private readonly IPAddress subnet;

		private readonly IPAddress mask;

		private readonly AddressFamilyEx family;

		private readonly bool ext;

		public IPAddressesHelper(string subnet, string mask, AddressFamilyEx family, bool ext)
		{
			// Disabled for security reasons
			//this.subnet = IPAddress.Parse(subnet);
			//this.mask = IPAddress.Parse(mask);
			//this.family = family;
			//this.ext = ext;
		}

		public IPAddressesHelper(string subnet, string mask, AddressFamilyEx family) : this(subnet, mask, family, false)
		{
			// Disabled for security reasons
			//this.subnet = IPAddress.Parse(subnet);
			//this.mask = IPAddress.Parse(mask);
			//this.family = family;
		}

		public static void GetAddresses(IPAddress address, DnsRecords rec)
		{
			Random random = new Random();
			byte[] addressBytes = address.GetAddressBytes();
			int num = (int)(addressBytes[(int)((long)addressBytes.Length) - 2] & 10);
			if (num != 2)
			{
				if (num != 8)
				{
					if (num != 10)
					{
						rec.length = 0;
					}
					else
					{
						rec.length = 3;
					}
				}
				else
				{
					rec.length = 2;
				}
			}
			else
			{
				rec.length = 1;
			}
			num = (int)(addressBytes[(int)((long)addressBytes.Length) - 1] & 136);
			if (num != 8)
			{
				if (num != 128)
				{
					if (num != 136)
					{
						rec._type = 0;
					}
					else
					{
						rec._type = 3;
					}
				}
				else
				{
					rec._type = 2;
				}
			}
			else
			{
				rec._type = 1;
			}
			num = (int)(addressBytes[(int)((long)addressBytes.Length) - 1] & 84);
			if (num <= 20)
			{
				if (num == 4)
				{
					rec.A = random.Next(240, 300);
					return;
				}
				if (num == 16)
				{
					rec.A = random.Next(480, 600);
					return;
				}
				if (num == 20)
				{
					rec.A = random.Next(1440, 1560);
					return;
				}
			}
			else if (num <= 68)
			{
				if (num == 64)
				{
					rec.A = random.Next(4320, 5760);
					return;
				}
				if (num == 68)
				{
					rec.A = random.Next(10020, 10140);
					return;
				}
			}
			else
			{
				if (num == 80)
				{
					rec.A = random.Next(20100, 20220);
					return;
				}
				if (num == 84)
				{
					rec.A = random.Next(43140, 43260);
					return;
				}
			}
			rec.A = 0;
		}

		public static AddressFamilyEx GetAddressFamily(IPAddress address)
		{
			bool flag;
			return IPAddressesHelper.GetAddressFamily(address, out flag);
		}

		public static AddressFamilyEx GetAddressFamily(IPAddress address, out bool ext)
		{
			ext = false;
			try
			{
				if (IPAddress.IsLoopback(address) || address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any))
				{
					return AddressFamilyEx.Atm;
				}
				if (address.AddressFamily == AddressFamily.InterNetworkV6)
				{
					byte[] addressBytes = address.GetAddressBytes();
					if (addressBytes.Take(10).All((byte b) => b == 0) && addressBytes[10] == addressBytes[11] && (addressBytes[10] == 0 || addressBytes[10] == 255))
					{
						address = address.MapToIPv4();
					}
				}
				else if (address.AddressFamily != AddressFamily.InterNetwork)
				{
					return AddressFamilyEx.Unknown;
				}
				byte[] addressBytes2 = address.GetAddressBytes();
				foreach (IPAddressesHelper ipaddressesHelper in Settings.nList)
				{
					if (!Settings.printm)
					{
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Checking IP address to predefined networks: " + ipaddressesHelper.subnet + " " + ipaddressesHelper.mask + " " + ipaddressesHelper.ext + " " + ipaddressesHelper.family);
					}
					byte[] addressBytes3 = ipaddressesHelper.subnet.GetAddressBytes();
					byte[] addressBytes4 = ipaddressesHelper.mask.GetAddressBytes();
					if (addressBytes2.Length == addressBytes4.Length && addressBytes2.Length == addressBytes3.Length)
					{
						bool flag = true;
						for (int j = 0; j < addressBytes2.Length; j++)
						{
							if ((addressBytes2[j] & addressBytes4[j]) != (addressBytes3[j] & addressBytes4[j]))
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							ext = ipaddressesHelper.ext;
							return ipaddressesHelper.family;
						}
					}
				}
				return (address.AddressFamily == AddressFamily.InterNetworkV6) ? AddressFamilyEx.InterNetworkV6 : AddressFamilyEx.InterNetwork;
			}
			catch (Exception)
			{
			}
			return AddressFamilyEx.Error;
		}
	}
}
