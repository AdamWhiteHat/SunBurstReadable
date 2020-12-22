using System;
using System.Linq;
using System.Collections.Generic;

namespace SunBurstDefanged
{
	public class ServiceConfiguration
	{
		// (get) Token: 0x06000975 RID: 2421 RVA: 0x000433F8 File Offset: 0x000415F8
		// (set) Token: 0x06000976 RID: 2422 RVA: 0x0004343C File Offset: 0x0004163C
		public bool stopped
		{
			get
			{
				object @lock = this._lock;
				bool stopped;
				lock (@lock)
				{
					stopped = this._stopped;
				}
				return stopped;
			}
			set
			{
				object @lock = this._lock;
				lock (@lock)
				{
					this._stopped = value;
				}
			}
		}

		// (get) Token: 0x06000977 RID: 2423 RVA: 0x00043480 File Offset: 0x00041680
		// (set) Token: 0x06000978 RID: 2424 RVA: 0x000434C4 File Offset: 0x000416C4
		public bool running
		{
			get
			{
				object @lock = this._lock;
				bool running;
				lock (@lock)
				{
					running = this._running;
				}
				return running;
			}
			set
			{
				object @lock = this._lock;
				lock (@lock)
				{
					this._running = value;
				}
			}
		}

		// (get) Token: 0x06000979 RID: 2425 RVA: 0x00043508 File Offset: 0x00041708
		// (set) Token: 0x0600097A RID: 2426 RVA: 0x0004354C File Offset: 0x0004174C
		public bool disabled
		{
			get
			{
				object @lock = this._lock;
				bool disabled;
				lock (@lock)
				{
					disabled = this._disabled;
				}
				return disabled;
			}
			set
			{
				object @lock = this._lock;
				lock (@lock)
				{
					this._disabled = value;
				}
			}
		}

		public ulong[] timeStamps;

		private readonly object _lock = new object();

		private volatile bool _stopped;

		private volatile bool _running;

		private volatile bool _disabled;

		public Service[] Svc;

		public class Service
		{
			public ulong timeStamp;

			public uint DefaultValue;

			public bool started;
		}
	}
}
