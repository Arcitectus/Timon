using System;
using System.Collections.Generic;

namespace SmartNet
{
	public interface ISmartSession
	{
		int Width { get; }

		int Height { get; }

		KeyValuePair<UInt32[], int>? TakeImageRasterFromRectOffsetAndLength(int offsetX, int offsetY, int width, int height);

		void MouseClick(int x, int y, bool left);

		void MouseMove(int x, int y);

		void TextEntry(string text, int keyWait, int keyModWait);

		void KeyDown(int code);

		void KeyUp(int code);
	}
}
