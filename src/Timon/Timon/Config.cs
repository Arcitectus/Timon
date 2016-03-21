namespace Timon
{
	public class Config
	{
		public string JavaPath { set; get; }

		public string SmartPath { set; get; }

		public string RunescapePath { set; get; }

		public bool SmartSessionStopOnScriptEnd;
	}

	public class ScriptRunConfig
	{
		public SmartNet.SessionSettings SmartConfig;

		public bool SmartSessionStopOnScriptEnd;
	}
}
