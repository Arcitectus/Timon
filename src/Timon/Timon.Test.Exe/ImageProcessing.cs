using Bib3.Geometrik;
using NUnit.Framework;
using System;
using System.Linq;

namespace Timon.Test.Exe
{
	public class ImageProcessingTestContainer
	{
		/// <summary>
		/// Pattern generated from image should be found in that image.
		/// </summary>
		[Test]
		public void Raster_FindPattern_Self_Pixel()
		{
			var raster = new UInt32[]
			{
				UInt32.MaxValue,
			}.AsRaster2D(1);

			var predicate = raster.PatternFromRasterPredicate(0);

			var setMatch = raster.FindSetMatch(predicate)?.ToArray();

			Assert.AreEqual(1, setMatch.Length, "match count");
		}

		[Test]
		public void Raster_FindColor()
		{
			var raster = new UInt32[]
			{
				0xff0000ff, 0xffffffff,
				0x00000000, 0xff0000f0
			}.AsRaster2D(2);

			var predicate = RasterImagePatternMatch.FindColorPredicate(1, 4, 0xf8, 10);

			var setMatch = raster.FindSetMatch(predicate)?.ToArray();

			Assert.AreEqual(new Vektor2DInt(0, 0), setMatch[0], "match[0].location");
			Assert.AreEqual(new Vektor2DInt(1, 1), setMatch[1], "match[0].location");
		}
	}
}
