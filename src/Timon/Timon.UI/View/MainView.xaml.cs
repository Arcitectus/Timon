using Bib3;
using Bib3.RateLimit;
using Bib3.Synchronization;
using BotEngine;
using BotEngine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Timon.UI.ViewModel;

namespace Timon.UI
{
	/// <summary>
	/// Interaction logic for MainView.xaml
	/// </summary>
	public partial class MainView : UserControl
	{
		public MainView()
		{
			InitializeComponent();

			ConfigView.DataContext = ConfigViewModel;

			Setup();
		}

		readonly ConfigViewModel ConfigViewModel = new ConfigViewModel();

		readonly object timerLock = new object();

		DispatcherTimer Timer;

		BotSharp.ScriptRun.ScriptRun ScriptRun => ScriptIDE?.ScriptRun;

		static public Func<string> AssemblyDirectoryPathDelegate;

		static string AssemblyDirectoryPath => AssemblyDirectoryPathDelegate?.Invoke();

		static public string ConfigFilePath =>
			AssemblyDirectoryPath.PathToFilesysChild("config");

		string ScriptDirectoryPath => AssemblyDirectoryPath.PathToFilesysChild(@"script\");

		string DefaultScriptPath => ScriptDirectoryPath.PathToFilesysChild("default.cs");

		public static Func<IEnumerable<KeyValuePair<string, string>>> SetScriptIncludedConstructDelegate;

		KeyValuePair<string, string>[] ListScriptIncluded =
			SetScriptIncludedConstructDelegate?.Invoke()?.ExceptionCatch(Bib3.FCL.GBS.Extension.MessageBoxException)
			?.OrderBy(ScriptNameAndContent => !ScriptNameAndContent.Key.RegexMatchSuccessIgnoreCase("travel"))
			?.ToArray();

		public Config ConfigReadFromUI() =>
			ConfigViewModel.PropagateFromDependencyPropertyToClrMember();

		public void ConfigFromModelToView(Config config) =>
			ConfigViewModel.PropagateFromClrMemberToDependencyProperty(config);

		readonly IRateLimitStateInt configPersistRateLimit = new RateLimitStateIntSingle();

		readonly PersistManager configPersistManager = new PersistManager
		{
			FilePath = ConfigFilePath,
		};

		Config ConfigCache;

		public Config ConfigInUI
		{
			set
			{
				ConfigFromModelToView(value);
			}

			get
			{
				return ConfigReadFromUI();
			}
		}

		ScriptRunConfig ScriptRunConfig =>
			new ScriptRunConfig
			{
				SmartConfig = new SmartNet.SessionSettings
				{
					SmartPath = ConfigCache?.SmartPath,
					JavaPath = ConfigCache?.JavaPath,
					RunescapePath = ConfigCache?.RunescapePath,
				},

				SmartSessionStopOnScriptEnd = ConfigCache?.SmartSessionStopOnScriptEnd ?? false,
			};

		BotSharp.ScriptRun.IScriptRunClient ScriptRunClientBuild(BotSharp.ScriptRun.ScriptRun run)
		{
			var config = this.ConfigInUI;

			return new Script.Impl.ScriptRunClient(run, () => ScriptRunConfig);
		}

		void Setup()
		{
			ConfigInUI =
				(Bib3.Extension.ReturnValueOrException(() =>
				(configPersistManager.ReadFromFileSerialized()?.DeserializeFromUtf8<Config>())) as Config).CompletedWithDefault();

			ScriptIDE.ScriptParamBase = new BotSharp.ScriptParam
			{
				ImportAssembly = Script.Impl.ToScriptImport.ImportAssembly?.ToArray(),
				ImportNamespace = Script.Impl.ToScriptImport.ImportNamespace?.ToArray(),
				CompilationOption = new BotSharp.CodeAnalysis.CompilationOption
				{
					InstrumentationOption = BotSharp.CodeAnalysis.Default.InstrumentationOption,
				},

				CompilationGlobalsType = ScriptRunClientBuild(null)?.ToScriptGlobals?.GetType(),

				ScriptRunClientBuildDelegate = ScriptRunClientBuild,
			};

			ScriptIDE.ChooseScriptFromIncludedScripts.SetScript =
					ListScriptIncluded?.Select(ScriptIdAndContent => new KeyValuePair<string, Func<string>>(ScriptIdAndContent.Key, () => ScriptIdAndContent.Value))?.ToArray();

			ScriptIDE.ScriptWriteToOrReadFromFile.DefaultFilePath = DefaultScriptPath;
			ScriptIDE.ScriptWriteToOrReadFromFile?.ReadFromFile();
			if (!(0 < ScriptIDE.Editor.Document.Text?.Length))
				ScriptIDE.Editor.Document.Text = ListScriptIncluded?.FirstOrDefault().Value ?? "";

			AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ButtonClicked));

			TimerConstruct();
		}

		void ButtonClicked(object sender, RoutedEventArgs e)
		{
			var OriginalSource = e?.OriginalSource;

			if (null != OriginalSource)
			{
				if (OriginalSource == ToggleButtonMotionEnable?.ButtonLinx)
					ScriptRunPause();

				if (OriginalSource == ToggleButtonMotionEnable?.ButtonRecz)
					ScriptRunPlay();
			}
		}

		void UIPresent()
		{
			UIPresentScript();
		}

		void ScriptRunPlay()
		{
			ScriptIDE.ScriptRunContinueOrStart();

			UIPresentScript();
		}

		void ScriptRunPause()
		{
			ScriptIDE.ScriptPause();

			UIPresentScript();
		}

		void UIPresentScript()
		{
			ToggleButtonMotionEnable?.ButtonRecz?.SetValue(ToggleButton.IsCheckedProperty, ScriptRun?.IsRunning ?? false);

			ScriptIDE?.Present();
		}

		void TimerConstruct()
		{
			Timer = new DispatcherTimer(TimeSpan.FromSeconds(1.0 / 10), DispatcherPriority.Normal, Timer_Tick, Dispatcher);

			Timer.Start();
		}

		void Timer_Tick(object sender, object e)
		{
			UIPresent();

			ConfigCache = ConfigInUI;

			Task.Run(() =>
			{
				new Action(() =>
				{
					if (configPersistRateLimit.AttemptPassStopwatchMilli(1000))
						configPersistManager.ValueChanged(ConfigCache.SerializeToUtf8());
				})
				.InvokeIfNotLocked(timerLock);
			});
		}

	}
}
