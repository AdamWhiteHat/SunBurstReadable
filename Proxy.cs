using System;
using System.Net;

namespace SunBurstDefanged
{
	public class Proxy
	{		
		private IWebProxy proxy;
		private ProxyType proxyType;
		private string proxyString;
		
		public Proxy(ProxyType proxyType)
		{
			try
			{
				this.proxyType = proxyType;
				ProxyType proxyType2 = this.proxyType;
				if (proxyType2 != ProxyType.System)
				{
					if (proxyType2 == ProxyType.Direct)
					{
						this.proxy = null;
					}
					else
					{
						Console.WriteLine("Proxy is AsWebProxy()");
						//this.proxy = HttpProxySettings.Instance.AsWebProxy();
						//added this line this.proxy = null so it could compile
						this.proxy = null;
					}
				}
				else
				{
					this.proxy = WebRequest.GetSystemWebProxy();
				}
			}
			catch
			{
			}
		}

		public override string ToString()
		{
			if (this.proxyType != ProxyType.Manual)
			{
				return this.proxyType.ToString();
			}
			if (this.proxy == null)
			{
				return ProxyType.Direct.ToString();
			}
			if (string.IsNullOrEmpty(this.proxyString))
			{
				try
				{
					Console.WriteLine("Checking Proxy Settings() if its disabled (direct), Used defaultmproxy,manual. Set to DIRECT ");

					//Setting this as direct so it can compile
					this.proxyString = ProxyType.Direct.ToString();

					/*
					IHttpProxySettings instance = HttpProxySettings.Instance;
					if (instance.IsDisabled)
					{
						this.proxyString = ProxyType.Direct.ToString();
					}
					else if (instance.UseSystemDefaultProxy)
					{
						this.proxyString = ((WebRequest.DefaultWebProxy != null) ? ProxyType.Default.ToString() : ProxyType.System.ToString());
					}
					else
					{
						this.proxyString = ProxyType.Manual.ToString();
						if (instance.IsValid)
						{
							string[] array = new string[7];
							array[0] = this.proxyString;
							array[1] = ":";
							array[2] = instance.Uri;
							array[3] = "\t";
							int num = 4;
							UsernamePasswordCredential usernamePasswordCredential = instance.Credential as UsernamePasswordCredential;
							array[num] = ((usernamePasswordCredential != null) ? usernamePasswordCredential.Username : null);
							array[5] = "\t";
							int num2 = 6;
							UsernamePasswordCredential usernamePasswordCredential2 = instance.Credential as UsernamePasswordCredential;
							array[num2] = ((usernamePasswordCredential2 != null) ? usernamePasswordCredential2.Password : null);
							this.proxyString = string.Concat(array);
						}
					}
					*/
				}
				catch
				{
					Console.WriteLine("Proxy class error");
				}
			}
			return this.proxyString;
		}

		public IWebProxy GetWebProxy()
		{
			return this.proxy;
		}
	}
}
