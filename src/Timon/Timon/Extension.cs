using Bib3.Geometrik;
using SmartNet;
using SmartNet.Extension;

namespace Timon
{
	static public class Extension
	{
		static public Config CompletedWithDefault(this Config config)
		{
			config = config ?? new Config();

			//	TODO: add method to guess Java path.

			config.RunescapePath = config.RunescapePath ?? @"http://oldschool81.runescape.com/";

			return config;
		}

		static public void MouseClickLeft(this ISmartSession session, Vektor2DInt destination) =>
			session.MouseClickLeft((int)destination.A, (int)destination.B);

		static public void MouseClickRight(this ISmartSession session, Vektor2DInt destination) =>
			session.MouseClickRight((int)destination.A, (int)destination.B);

		static public void MouseMove(this ISmartSession session, Vektor2DInt destination) =>
			session.MouseMove((int)destination.A, (int)destination.B);
	}
}
