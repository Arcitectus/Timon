using System.Collections.Generic;

namespace SmartNet.Extension
{
	static public class SessionExtension
	{
		static public void MouseClickLeft(this ISmartSession session, int x, int y) =>
			session.MouseClick(x, y, true);

		static public void MouseClickRight(this ISmartSession session, int x, int y) =>
			session.MouseClick(x, y, false);

		static public KeyValuePair<uint[], int>? TakeImageRaster(this ISmartSession session) =>
			session?.TakeImageRasterFromRectOffsetAndLength(0, 0, session.Width, session.Height);
	}
}
