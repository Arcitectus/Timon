using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SmartNet
{
	static public class Extension
	{
		static public KeyValuePair<UInt32[], int>? GetScreenshot(this Session session)
		{
			if (null == session)
				return null;

			var arrayPtr = session.SmartRemote?.GetImageArray(session.SmartHandle);

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
	}
}
