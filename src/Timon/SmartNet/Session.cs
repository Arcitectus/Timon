using System;
using System.Threading;
using SmartNet.Native;

namespace SmartNet
{
	public class Session
	{
		private static int sessionCount = 0;

		private readonly ManualResetEvent resetEvent;
		private readonly SessionSettings settings;

		private Thread smartThread;

		public string Id => SmartTargetHandle.ToString();

		public bool Alive => SmartTargetHandle != IntPtr.Zero;

		public SmartRemote SmartRemote { get; private set; }

		public IntPtr SmartTargetHandle { get; private set; }

		public int Width { private set; get; } = 800;

		public int Height { private set; get; } = 600;

		public Session(SmartRemote smartRemote, SessionSettings settings)
		{
			SmartRemote = smartRemote;
			this.settings = settings;
			resetEvent = new ManualResetEvent(false);
		}

		private void StartSmart(SessionSettings settings)
		{
			string url = settings.SessionType == SessionType.RS3 ? "http://world37.runescape.com/" : "http://oldschool81.runescape.com/";

			int availableClients = SmartRemote.GetClients(true);

			for (int i = 0; i < availableClients; i++)
			{
				int availableClient = SmartRemote.GetAvailablePID(i);
				IntPtr handle = SmartRemote.PairClient(availableClient);
				if (handle != IntPtr.Zero)
				{
					SmartTargetHandle = handle;
					break;
				}
			}

			if (SmartTargetHandle == IntPtr.Zero)
				SmartTargetHandle = SmartRemote.SpawnClient(settings.JavaPath, settings.SmartPath, url, "", Width, Height, null, null, null, null);

			resetEvent.WaitOne();

			SmartRemote.KillClient(SmartRemote.GetClientPID(SmartTargetHandle));

			SmartTargetHandle = IntPtr.Zero;
		}

		public void Start()
		{
			smartThread = new Thread(() => StartSmart(settings));
			Interlocked.Increment(ref sessionCount);
			smartThread.Name = $"SessionThread-{sessionCount}";
			smartThread.IsBackground = true;
			smartThread.Start();
		}

		public void Stop()
		{
			resetEvent.Set();
			smartThread?.Join();
		}
	}
}
