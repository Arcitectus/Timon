using Bib3;
using BotEngine.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;
using Timon.UI;

namespace Timon.Exe
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		static public string AssemblyDirectoryPath => Bib3.FCL.Glob.ZuProcessSelbsctMainModuleDirectoryPfaadBerecne().EnsureEndsWith(@"\");

		private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			try
			{
				var FilePath = AssemblyDirectoryPath.PathToFilesysChild(DateTime.Now.SictwaiseKalenderString(".", 0) + " Exception");

				FilePath.WriteToFileAndCreateDirectoryIfNotExisting(Encoding.UTF8.GetBytes(e.Exception.SictString()));

				var Message = "exception written to file: " + FilePath;

				MessageBox.Show(Message, Message, MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
			catch (Exception PersistException)
			{
				Bib3.FCL.GBS.Extension.MessageBoxException(PersistException);
			}

			Bib3.FCL.GBS.Extension.MessageBoxException(e.Exception);

			e.Handled = true;
		}

		static IEnumerable<KeyValuePair<string, string>> SetScriptIncludedConstruct()
		{
			var Assembly = typeof(App).Assembly;

			var SetResourceName = Assembly?.GetManifestResourceNames();

			var ScriptPrefix = Assembly.GetName().Name + ".sample.script.";

			foreach (var ResourceName in SetResourceName.EmptyIfNull())
			{
				var ScriptIdMatch = ResourceName.RegexMatchIfSuccess(Regex.Escape(ScriptPrefix) + @"(.*)");

				if (null == ScriptIdMatch)
					continue;

				var ScriptUTF8 = Assembly.GetManifestResourceStream(ResourceName)?.LeeseGesamt();

				if (null == ScriptUTF8)
					continue;

				yield return new KeyValuePair<string, string>(ScriptIdMatch?.Groups?[1]?.Value, Encoding.UTF8.GetString(ScriptUTF8));
			}
		}

		public App()
		{
			MainView.SetScriptIncludedConstructDelegate = SetScriptIncludedConstruct;
			MainView.AssemblyDirectoryPathDelegate = () => AssemblyDirectoryPath;
		}
	}
}