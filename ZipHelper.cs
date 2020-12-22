using System;
using System.IO;
using System.Text;
using System.IO.Compression;

namespace SunBurstDefanged
{
	public static class ZipHelper
	{
		public static byte[] Compress(byte[] input)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream(input))
			{
				using (MemoryStream memoryStream2 = new MemoryStream())
				{
					using (DeflateStream deflateStream = new DeflateStream(memoryStream2, CompressionMode.Compress))
					{
						memoryStream.CopyTo(deflateStream);
					}
					result = memoryStream2.ToArray();
				}
			}
			return result;
		}

		public static byte[] Decompress(byte[] input)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream(input))
			{
				using (MemoryStream memoryStream2 = new MemoryStream())
				{
					using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress))
					{
						deflateStream.CopyTo(memoryStream2);
					}
					result = memoryStream2.ToArray();
				}
			}
			return result;
		}

		public static string Zip(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return input;
			}
			string result;
			try
			{
				result = Convert.ToBase64String(ZipHelper.Compress(Encoding.UTF8.GetBytes(input)));
			}
			catch (Exception)
			{
				result = "";
			}
			return result;
		}

		public static string Unzip(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return input;
			}
			string result;
			try
			{
				byte[] bytes = ZipHelper.Decompress(Convert.FromBase64String(input));
				result = Encoding.UTF8.GetString(bytes);
			}
			catch (Exception)
			{
				result = input;
			}
			return result;
		}
	}
}
