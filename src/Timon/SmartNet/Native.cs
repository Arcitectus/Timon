using SmartNet.WinApi;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SmartNet.Native
{
	public class SmartRemote : IDisposable
	{

		#region delegates

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int GetClientsDelegate(bool paired);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int GetAvailablePIDDelegate(int pid);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate bool KillClientDelegate(int pid);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr SpawnClientDelegate(
		  string javaPath, string remotePath, string root, string parameters,
		  int width, int height, string initSeq, string useragent,
		  string javaArgs, string plugins);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr PairClientDelegate(int pid);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int GetClientPIDDelegate(IntPtr target);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void FreeClientDelegate(IntPtr target);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr GetImageArrayDelegate(IntPtr target);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr GetDebugArrayDelegate(IntPtr target);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int GetRefreshDelegate(IntPtr target);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void SetRefreshDelegate(IntPtr target);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void SetTransparentColorDelegate(IntPtr target, int color);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void SetDebugDelegate(IntPtr target, bool enabled);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void SetGraphicsDelegate(IntPtr target, bool enabled);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void SetEnabledDelegate(IntPtr target, bool enabled);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate bool IsActiveDelegate(IntPtr target);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate bool IsBlockingDelegate(IntPtr target);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void GetMousePosDelegate(IntPtr target, ref int x, ref int y);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void HoldMouseDelegate(IntPtr target, int x, int y, bool left);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void ReleaseMouseDelegate(IntPtr target, int x, int y, bool left);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void HoldMousePlusDelegate(IntPtr target, int x, int y, int button);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void ReleaseMousePlusDelegate(IntPtr target, int x, int y, int button);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void MoveMouseDelegate(IntPtr target, int x, int y);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void WindMouseDelegate(IntPtr target, int x, int y);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void ClickMouseDelegate(IntPtr target, int x, int y, bool left);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void ClickMousePlusDelegate(IntPtr target, int x, int y, int button);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate bool IsMouseButtonHeldDelegate(IntPtr target, int button);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void ScrollMouseDelegate(IntPtr target, int x, int y, int lines);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void SendKeysDelegate(IntPtr target, string text, int keyWait, int keyModWait);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void HoldKeyDelegate(IntPtr target, int code);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void ReleaseKeyDelegate(IntPtr target, int code);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate bool IsKeyDownDelegate(IntPtr target, int code);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void SetCaptureDelegate(IntPtr target, bool enabled);

		#endregion

		#region fields

		private readonly IntPtr hModule;

		#endregion

		#region delegates

		public GetClientsDelegate GetClients;
		public GetAvailablePIDDelegate GetAvailablePID;
		public KillClientDelegate KillClient;
		public SpawnClientDelegate SpawnClient;
		public PairClientDelegate PairClient;
		public GetClientPIDDelegate GetClientPID;
		public FreeClientDelegate FreeClient;

		public GetImageArrayDelegate GetImageArray;
		public GetDebugArrayDelegate GetDebugArray;
		public GetRefreshDelegate GetRefresh;
		public SetRefreshDelegate SetRefresh;
		public SetTransparentColorDelegate SetTransparentColor;
		public SetDebugDelegate SetDebug;
		public SetGraphicsDelegate SetGraphics;
		public SetEnabledDelegate SetEnabled;
		public IsActiveDelegate IsActive;
		public IsBlockingDelegate IsBlocking;
		public GetMousePosDelegate GetMousePos;
		public HoldMouseDelegate HoldMouse;
		public ReleaseMouseDelegate ReleaseMouse;
		public HoldMousePlusDelegate HoldMousePlus;
		public ReleaseMousePlusDelegate ReleaseMousePlus;
		public MoveMouseDelegate MoveMouse;
		public WindMouseDelegate WindMouse;
		public ClickMouseDelegate ClickMouse;
		public ClickMousePlusDelegate ClickMousePlus;
		public IsMouseButtonHeldDelegate IsMouseButtonHeld;
		public ScrollMouseDelegate ScrollMouse;
		public SendKeysDelegate SendKeys;
		public HoldKeyDelegate HoldKey;
		public ReleaseKeyDelegate ReleaseKey;
		public IsKeyDownDelegate IsKeyDown;
		public SetCaptureDelegate SetCapture;

		#endregion

		#region constructor

		public SmartRemote(string path)
		{
			string dllPath = Path.Combine(path, "libsmartremote32.dll");
			if (!File.Exists(Path.Combine(dllPath)))
				throw new Exception("Could not locate libsmartremote32.dll");

			hModule = Kernel32.LoadLibrary(dllPath);
			if (hModule == IntPtr.Zero)
				throw new Exception("Could not load libsmartremote32.dll");

			LoadSmartMethods();
		}

		#endregion

		#region private methods

		private void LoadSmartMethods()
		{
			IntPtr functionAddress = Kernel32.GetProcAddress(hModule, "exp_getClients");
			GetClients = Marshal.GetDelegateForFunctionPointer<GetClientsDelegate>(functionAddress);

			functionAddress = Kernel32.GetProcAddress(hModule, "exp_getAvailablePID");
			GetAvailablePID = Marshal.GetDelegateForFunctionPointer<GetAvailablePIDDelegate>(functionAddress);

			functionAddress = Kernel32.GetProcAddress(hModule, "exp_killClient");
			KillClient = Marshal.GetDelegateForFunctionPointer<KillClientDelegate>(functionAddress);

			functionAddress = Kernel32.GetProcAddress(hModule, "exp_spawnClient");
			SpawnClient = Marshal.GetDelegateForFunctionPointer<SpawnClientDelegate>(functionAddress);

			functionAddress = Kernel32.GetProcAddress(hModule, "exp_pairClient");
			PairClient = Marshal.GetDelegateForFunctionPointer<PairClientDelegate>(functionAddress);

			functionAddress = Kernel32.GetProcAddress(hModule, "exp_getClientPID");
			GetClientPID = Marshal.GetDelegateForFunctionPointer<GetClientPIDDelegate>(functionAddress);

			functionAddress = Kernel32.GetProcAddress(hModule, "exp_freeClient");
			FreeClient = Marshal.GetDelegateForFunctionPointer<FreeClientDelegate>(functionAddress);

			functionAddress = Kernel32.GetProcAddress(hModule, "exp_getImageArray");
			GetImageArray = Marshal.GetDelegateForFunctionPointer<GetImageArrayDelegate>(functionAddress);

			functionAddress = Kernel32.GetProcAddress(hModule, "exp_getDebugArray");
			GetDebugArray = Marshal.GetDelegateForFunctionPointer<GetDebugArrayDelegate>(functionAddress);

			functionAddress = Kernel32.GetProcAddress(hModule, "exp_getRefresh");
			GetRefresh = Marshal.GetDelegateForFunctionPointer<GetRefreshDelegate>(functionAddress);

			functionAddress = Kernel32.GetProcAddress(hModule, "exp_setRefresh");
			SetRefresh = Marshal.GetDelegateForFunctionPointer<SetRefreshDelegate>(functionAddress);

			functionAddress = Kernel32.GetProcAddress(hModule, "exp_setTransparentColor");
			SetTransparentColor = Marshal.GetDelegateForFunctionPointer<SetTransparentColorDelegate>(functionAddress);

			functionAddress = Kernel32.GetProcAddress(hModule, "exp_setGraphics");
			SetGraphics = Marshal.GetDelegateForFunctionPointer<SetGraphicsDelegate>(functionAddress);

			functionAddress = Kernel32.GetProcAddress(hModule, "exp_setEnabled");
			SetEnabled = Marshal.GetDelegateForFunctionPointer<SetEnabledDelegate>(functionAddress);

			functionAddress = Kernel32.GetProcAddress(hModule, "exp_isActive");
			IsActive = Marshal.GetDelegateForFunctionPointer<IsActiveDelegate>(functionAddress);

			functionAddress = Kernel32.GetProcAddress(hModule, "exp_getMousePos");
			GetMousePos = Marshal.GetDelegateForFunctionPointer<GetMousePosDelegate>(functionAddress);

			functionAddress = Kernel32.GetProcAddress(hModule, "exp_holdMouse");
			HoldMouse = Marshal.GetDelegateForFunctionPointer<HoldMouseDelegate>(functionAddress);

			functionAddress = Kernel32.GetProcAddress(hModule, "exp_releaseMouse");
			ReleaseMouse = Marshal.GetDelegateForFunctionPointer<ReleaseMouseDelegate>(functionAddress);

			functionAddress = Kernel32.GetProcAddress(hModule, "exp_holdMousePlus");
			HoldMousePlus = Marshal.GetDelegateForFunctionPointer<HoldMousePlusDelegate>(functionAddress);

			functionAddress = Kernel32.GetProcAddress(hModule, "exp_releaseMousePlus");
			ReleaseMousePlus = Marshal.GetDelegateForFunctionPointer<ReleaseMousePlusDelegate>(functionAddress);

			functionAddress = Kernel32.GetProcAddress(hModule, "exp_moveMouse");
			MoveMouse = Marshal.GetDelegateForFunctionPointer<MoveMouseDelegate>(functionAddress);

			functionAddress = Kernel32.GetProcAddress(hModule, "exp_windMouse");
			WindMouse = Marshal.GetDelegateForFunctionPointer<WindMouseDelegate>(functionAddress);

			functionAddress = Kernel32.GetProcAddress(hModule, "exp_clickMouse");
			ClickMouse = Marshal.GetDelegateForFunctionPointer<ClickMouseDelegate>(functionAddress);

			functionAddress = Kernel32.GetProcAddress(hModule, "exp_clickMousePlus");
			ClickMousePlus = Marshal.GetDelegateForFunctionPointer<ClickMousePlusDelegate>(functionAddress);

			functionAddress = Kernel32.GetProcAddress(hModule, "exp_isMouseButtonHeld");
			IsMouseButtonHeld = Marshal.GetDelegateForFunctionPointer<IsMouseButtonHeldDelegate>(functionAddress);

			functionAddress = Kernel32.GetProcAddress(hModule, "exp_scrollMouse");
			ScrollMouse = Marshal.GetDelegateForFunctionPointer<ScrollMouseDelegate>(functionAddress);

			functionAddress = Kernel32.GetProcAddress(hModule, "exp_sendKeys");
			SendKeys = Marshal.GetDelegateForFunctionPointer<SendKeysDelegate>(functionAddress);

			functionAddress = Kernel32.GetProcAddress(hModule, "exp_holdKey");
			HoldKey = Marshal.GetDelegateForFunctionPointer<HoldKeyDelegate>(functionAddress);

			functionAddress = Kernel32.GetProcAddress(hModule, "exp_releaseKey");
			ReleaseKey = Marshal.GetDelegateForFunctionPointer<ReleaseKeyDelegate>(functionAddress);

			functionAddress = Kernel32.GetProcAddress(hModule, "exp_isKeyDown");
			IsKeyDown = Marshal.GetDelegateForFunctionPointer<IsKeyDownDelegate>(functionAddress);

			functionAddress = Kernel32.GetProcAddress(hModule, "exp_setCapture");
			SetCapture = Marshal.GetDelegateForFunctionPointer<SetCaptureDelegate>(functionAddress);
		}

		#endregion

		#region public methods

		public void Dispose()
		{
			Kernel32.FreeLibrary(hModule);
		}

		#endregion

	}
}