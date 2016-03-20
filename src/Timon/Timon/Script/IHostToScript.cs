using SmartNet;
using System;

namespace Timon.Script.Impl
{
	public interface IHostToScript
	{
		ISmartSession SmartSession { get; }
	}

	public class HostToScriptDelegate : IHostToScript
	{
		public Func<ISmartSession> SmartSessionDelegate;

		public ISmartSession SmartSession => SmartSessionDelegate?.Invoke();
	}

	public class ToScriptGlobals : BotSharp.ScriptRun.ToScriptGlobals
	{
		public IHostToScript Timon;
	}
}
