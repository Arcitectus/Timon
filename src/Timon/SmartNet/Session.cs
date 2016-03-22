using System;
using System.Collections.Generic;
using System.Threading;
using SmartNet.Native;
using System.Runtime.InteropServices;

namespace SmartNet
{
	public class Session : ISmartSession
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

		public void Start()
		{
			string url = settings.RunescapePath;

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
		}

		public void Stop()
		{
			SmartRemote.KillClient(SmartRemote.GetClientPID(SmartTargetHandle));

			SmartTargetHandle = IntPtr.Zero;
		}

		public KeyValuePair<uint[], int>? TakeImageRasterFromRectOffsetAndLength(int offsetX, int offsetY, int width, int height)
		{
			var arrayPtr = SmartRemote?.GetImageArray(SmartTargetHandle);

			if (!arrayPtr.HasValue)
				return null;

			if (IntPtr.Zero == arrayPtr)
				return null;

			var sourceWidth = this.Width;
			var sourceHeight = this.Height;

			if (width < 0)
				throw new ArgumentOutOfRangeException("width", width, "");

			if (height < 0)
				throw new ArgumentOutOfRangeException("height", height, "");

			if (offsetX < 0 || offsetY < 0 || sourceWidth < offsetX + width || sourceHeight < offsetY + height)
				throw new NotImplementedException();

			var rectArray = new Int32[width * height];

			for (int destY = 0; destY < height; destY++)
			{
				var destPixelIndex = destY * width;

				Marshal.Copy(arrayPtr.Value + destPixelIndex * 4, rectArray, destPixelIndex, width);
			}

			//	set each pixel to max. opacity
			for (int i = 0; i < rectArray.Length; i++)
				rectArray[i] = rectArray[i] | (0xff << 24);

			return new KeyValuePair<UInt32[], int>((UInt32[])(object)rectArray, width);
		}

		public void MouseClick(int x, int y, bool left) =>
			SmartRemote?.ClickMouse(SmartTargetHandle, x, y, left);

		public void MouseMove(int x, int y) =>
			SmartRemote?.MoveMouse(SmartTargetHandle, x, y);

		public void TextEntry(string text, int keyWait, int keyModWait) =>
			SmartRemote?.SendKeys(SmartTargetHandle, text, keyWait, keyModWait);

		public void KeyDown(int code) =>
			SmartRemote?.HoldKey(SmartTargetHandle, code);

		public void KeyUp(int code) =>
			SmartRemote?.ReleaseKey(SmartTargetHandle, code);
	}
}
