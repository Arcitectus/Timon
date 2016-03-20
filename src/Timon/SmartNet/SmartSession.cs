using System;
using System.Collections.Generic;

namespace SmartNet
{
	public interface ISmartSession
	{
		KeyValuePair<UInt32[], int>? TakeScreenshot();

		void MouseClick(int x, int y, bool left);

		void MouseMove(int x, int y);
	}
}
