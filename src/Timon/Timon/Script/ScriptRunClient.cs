using Bib3;
using System;
using BotSharp.ScriptRun;

namespace Timon.Script.Impl
{
	public class ScriptRunClient : IScriptRunClient
	{
		readonly Func<ScriptRunConfig> ConfigDelegate;

		ScriptRunConfig Config => ConfigDelegate?.Invoke();

		readonly ScriptRun Run;

		SmartNet.Native.SmartRemote SmartRemote;

		readonly Lazy<SmartNet.Session> SmartSession;

		public Action FromScriptExecutionControlCheck { set; get; }

		public ScriptRun.ToScriptGlobals ToScriptGlobals =>
			new ToScriptGlobals
			{
				Timon = new HostToScriptDelegate
				{
					SmartSessionDelegate = () => SmartSession?.Value,
				},
			};

		public void ExecutionStatusChanged(ScriptRun run)
		{
			if (!(run.Status == ScriptRunExecutionStatus.Pausing || run.Status == ScriptRunExecutionStatus.Running))
			{
				if (SmartSession.IsValueCreated && (Config?.SmartSessionStopOnScriptEnd ?? false))
					SmartSession?.Value?.Stop();

				SmartRemote?.Dispose();
			}
		}

		public void RunThreadEnterBefore(ScriptRun run)
		{
			//	make sure script runs on same culture independend of host culture.
			System.Threading.Thread.CurrentThread.CurrentCulture = ToScriptImport.ScriptDefaultCulture;
		}

		public ScriptRunClient(
			ScriptRun run,
			Func<ScriptRunConfig> configDelegate)
		{
			this.Run = run;
			this.ConfigDelegate = configDelegate;

			var config = configDelegate?.Invoke();

			var smartConfig = config?.SmartConfig;

			SmartSession = new Lazy<SmartNet.Session>(() =>
			{
				var smartPath = smartConfig?.SmartPath;
				var javaPath = smartConfig?.JavaPath;

				if (smartPath.IsNullOrEmpty())
					throw new ArgumentOutOfRangeException("smartPath", smartPath);

				if (javaPath.IsNullOrEmpty())
					throw new ArgumentOutOfRangeException("javaPath", javaPath);

				SmartRemote = new SmartNet.Native.SmartRemote(smartPath);

				var session = new SmartNet.Session(SmartRemote, smartConfig);

				session.Start();

				return session;
			});
		}
	}
}
