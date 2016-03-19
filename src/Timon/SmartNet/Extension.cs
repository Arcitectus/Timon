using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SmartNet.Extension
{
	static public class SessionExtension
	{
		static public KeyValuePair<UInt32[], int>? GetScreenshot(this Session session)
		{
			if (null == session)
				return null;

			var arrayPtr = session.SmartRemote?.GetImageArray(session.SmartTargetHandle);

			if (!arrayPtr.HasValue)
				return null;

			var width = session.Width;

			var array = new Int32[width * session.Height];

			Marshal.Copy(arrayPtr.Value, array, 0, array.Length);

			//	set each pixel to max. opacity
			for (int i = 0; i < array.Length; i++)
				array[i] = array[i] | (0xff << 24);

			return new KeyValuePair<UInt32[], int>((UInt32[])(object)array, width);
		}

		static public void MouseMove(this Session session, int x, int y) =>
			session?.SmartRemote?.MoveMouse(session.SmartTargetHandle, x, y);

		static public void MouseClick(this Session session, int x, int y, bool left) =>
			session?.SmartRemote?.ClickMouse(session.SmartTargetHandle, x, y, left);

		static public void MouseClickLeft(this Session session, int x, int y) =>
			session.MouseClick(x, y, true);

		static public void MouseClickRight(this Session session, int x, int y) =>
			session.MouseClick(x, y, false);

		static public void TextEntry(this Session session, string text, int keyWait, int keyModWait) =>
			session?.SmartRemote?.SendKeys(session.SmartTargetHandle, text, keyWait, keyModWait);

		static public void KeyDown(this Session session, int code) =>
			session?.SmartRemote?.HoldKey(session.SmartTargetHandle, code);

		static public void KeyUp(this Session session, int code) =>
			session?.SmartRemote?.ReleaseKey(session.SmartTargetHandle, code);
	}
}
