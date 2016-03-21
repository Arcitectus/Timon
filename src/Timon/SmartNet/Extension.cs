namespace SmartNet.Extension
{
	static public class SessionExtension
	{
		static public void MouseClickLeft(this ISmartSession session, int x, int y) =>
			session.MouseClick(x, y, true);

		static public void MouseClickRight(this ISmartSession session, int x, int y) =>
			session.MouseClick(x, y, false);
	}
}
