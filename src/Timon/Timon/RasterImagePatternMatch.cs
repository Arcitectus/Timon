using Bib3.Geometrik;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Timon
{
	static public class RasterImagePatternMatch
	{
		static public IEnumerable<Vektor2DInt> SubsetInRect(this IEnumerable<Vektor2DInt> source, RectInt rect) =>
			source?.Where(point => rect.ContainsPointForMinInclusiveAndMaxExclusive(point));

		static public Vektor2DInt RasterLengthVektor(this KeyValuePair<UInt32[], int> raster) =>
			new Vektor2DInt(raster.Value, (raster.Key?.Length ?? 0) / raster.Value);

		static public IEnumerable<Vektor2DInt> FindSetMatchInRegion(
			this KeyValuePair<UInt32[], int> rasterToSearch,
			Func<KeyValuePair<UInt32[], int>, int, int, bool> perOffsetPredicate,
			RectInt searchRegion)
		{
			if (null == perOffsetPredicate)
				yield break;

			var rasterLength = rasterToSearch.RasterLengthVektor();

			for (var searchOffsetB = (int)searchRegion.Min1; searchOffsetB < searchRegion.Max1; searchOffsetB++)
			{
				for (var searchOffsetA = (int)searchRegion.Min0; searchOffsetA < searchRegion.Max0; searchOffsetA++)
				{
					if (!perOffsetPredicate(rasterToSearch, searchOffsetA, searchOffsetB))
						continue;

					yield return new Vektor2DInt(searchOffsetA, searchOffsetB);
				}
			}
		}

		static public IEnumerable<Vektor2DInt> FindSetMatch(
			this KeyValuePair<UInt32[], int> rasterToSearch,
			Func<KeyValuePair<UInt32[], int>, int, int, bool> perOffsetPredicate) =>
			FindSetMatchInRegion(rasterToSearch, perOffsetPredicate, new RectInt(0, 0, rasterToSearch.RasterLengthVektor().A, rasterToSearch.RasterLengthVektor().B));

		static public Func<KeyValuePair<UInt32[], int>, int, int, bool> PatternFromRasterPredicate(
			this KeyValuePair<UInt32[], int> patternRaster,
			int tolerancePerPixel) =>
			new Func<KeyValuePair<uint[], int>, int, int, bool>((rasterToSearch, searchOffsetA, searchOffsetB) =>
			{
				var patternLength = patternRaster.RasterLengthVektor();
				var rasterLength = rasterToSearch.RasterLengthVektor();

				var searchOffsetMax = rasterLength - patternLength;

				if (searchOffsetA < 0 || searchOffsetB < 0 || searchOffsetMax.A < searchOffsetA || searchOffsetMax.B < searchOffsetB)
					return false;

				for (int patternB = 0; patternB < patternLength.B; patternB++)
				{
					for (int patternA = 0; patternA < patternLength.A; patternA++)
					{
						var patternPixelIndex = patternB * patternLength.A + patternA;
						var rasterPixelIndex = (searchOffsetB + patternB) * rasterLength.A + patternA + searchOffsetA;

						var patternPixelValue = patternRaster.Key[patternPixelIndex];
						var rasterPixelValue = rasterToSearch.Key[rasterPixelIndex];

						var patternPixelValO = (patternPixelValue >> 24) & 0xff;

						if (patternPixelValO < 0x80)
							continue;

						var patternPixelValR = (patternPixelValue >> 16) & 0xff;
						var patternPixelValG = (patternPixelValue >> 8) & 0xff;
						var patternPixelValB = (patternPixelValue >> 0) & 0xff;

						var rasterPixelValR = (rasterPixelValue >> 16) & 0xff;
						var rasterPixelValG = (rasterPixelValue >> 8) & 0xff;
						var rasterPixelValB = (rasterPixelValue >> 0) & 0xff;

						var diffR = (int)rasterPixelValR - (int)patternPixelValR;
						var diffG = (int)rasterPixelValG - (int)patternPixelValG;
						var diffB = (int)rasterPixelValB - (int)patternPixelValB;

						if (tolerancePerPixel < Math.Abs(diffR) ||
							tolerancePerPixel < Math.Abs(diffG) ||
							tolerancePerPixel < Math.Abs(diffB))
							return false;
					}
				}

				return true;
			});

		static public Func<KeyValuePair<UInt32[], int>, int, int, bool> FindColorPredicate(
			int colorR, int colorG, int colorB, int tolerance) =>
			new Func<KeyValuePair<uint[], int>, int, int, bool>((rasterToSearch, searchOffsetA, searchOffsetB) =>
			{
				var rasterLength = rasterToSearch.RasterLengthVektor();

				var rasterPixelIndex = searchOffsetB * rasterLength.A + searchOffsetA;

				var rasterPixelValue = rasterToSearch.Key[rasterPixelIndex];

				var rasterPixelValR = (rasterPixelValue >> 16) & 0xff;
				var rasterPixelValG = (rasterPixelValue >> 8) & 0xff;
				var rasterPixelValB = (rasterPixelValue >> 0) & 0xff;

				var diffR = (int)rasterPixelValR - (int)colorR;
				var diffG = (int)rasterPixelValG - (int)colorG;
				var diffB = (int)rasterPixelValB - (int)colorB;

				return
					!(tolerance < Math.Abs(diffR) ||
					tolerance < Math.Abs(diffG) ||
					tolerance < Math.Abs(diffB));
			});

		static public Func<KeyValuePair<UInt32[], int>, int, int, bool> HealthBarPredicate(
			int requiredLengthA = 28,
			int requiredLengthB = 5) =>
			new Func<KeyValuePair<uint[], int>, int, int, bool>((rasterToSearch, searchOffsetA, searchOffsetB) =>
			{
				var rasterLength = rasterToSearch.RasterLengthVektor();

				var startA = searchOffsetA - requiredLengthA / 2;
				var startB = searchOffsetB - requiredLengthB / 2;

				var endA = startA + requiredLengthA;
				var endB = startB + requiredLengthB;

				if (startA < 0 || startB < 0 || rasterLength.A < endA || rasterLength.B < endB)
					return false;

				for (int locA = startA; locA < endA; locA++)
				{
					var columnMatchCount = 0;

					for (int locB = startB; locB < endB; locB++)
					{
						var rasterPixelIndex = locB * rasterLength.A + locA;

						var rasterPixelValueRGB = rasterToSearch.Key[rasterPixelIndex] & 0xffffff;

						if (0xff0000 != rasterPixelValueRGB && 0x00ff00 != rasterPixelValueRGB)
							continue;

						++columnMatchCount;
					}

					if (columnMatchCount < requiredLengthB - 1)
						return false;
				}

				return true;
			});
	}
}
