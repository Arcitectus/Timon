using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Timon
{
	static public class RasterImage
	{
		static public KeyValuePair<UInt32[], int> RasterORGBFromImage(this BitmapImage image)
		{
			if (null == image)
				return default(KeyValuePair<UInt32[], int>);

			var Array = new UInt32[image.PixelWidth * image.PixelHeight];

			image.CopyPixels(Array, image.PixelWidth * 4, 0);

			return new KeyValuePair<uint[], int>(Array, image.PixelWidth);
		}

		static public KeyValuePair<UInt32[], int>? RasterORGBFromFile(this byte[] file) =>
			null == file ? null : Bib3.FCL.Glob.SictBitmapImageBerecne(file)?.RasterORGBFromImage();

		static public KeyValuePair<UInt32[], int> AsRaster2D(this UInt32[] listPixelValue, int lengthA) =>
			new KeyValuePair<uint[], int>(listPixelValue, (listPixelValue?.Length ?? 0) / lengthA);
	}
}
