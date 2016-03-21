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

	}
}
