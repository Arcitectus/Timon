using Bib3;
using BotEngine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Timon.UI.ViewModel;

namespace Timon.Exe
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		readonly ConfigViewModel ConfigViewModel = new ConfigViewModel();

		DispatcherTimer Timer;

		BotSharp.UI.Wpf.IDE ScriptIDE => MainView.ScriptIDE;

		BotSharp.ScriptRun ScriptRun => ScriptIDE?.ScriptRun;

		public string TitleComputed =>
			"Timon v" + (TryFindResource("AppVersionId") ?? "");

		void TimerConstruct()
		{
			Timer = new DispatcherTimer(TimeSpan.FromSeconds(1.0 / 10), DispatcherPriority.Normal, Timer_Tick, Dispatcher);

			Timer.Start();
		}

		void Timer_Tick(object sender, object e)
		{
			UIPresent();
		}

		void UIPresent()
		{
			UIPresentScript();
		}

		public MainWindow()
		{
			InitializeComponent();

			MainView.ConfigView.DataContext = ConfigViewModel;

			Setup();
		}

		Script.Impl.ToScriptGlobals ToScriptGlobalsConstruct(Action ScriptExecutionCheck) =>
			new Script.Impl.ToScriptGlobals()
			{
				Timon = new Script.Impl.HostToScriptDelegate
				{
				}
			};

		void Setup()
		{
			ScriptIDE.ScriptRunGlobalsFunc = ToScriptGlobalsConstruct;

			ScriptIDE.ScriptParamBase = new BotSharp.ScriptParam()
			{
				ImportAssembly = Script.Impl.ToScriptImport.ImportAssembly?.ToArray(),
				ImportNamespace = Script.Impl.ToScriptImport.ImportNamespace?.ToArray(),
				CompilationOption = new BotSharp.CodeAnalysis.CompilationOption()
				{
					InstrumentationOption = BotSharp.CodeAnalysis.Default.InstrumentationOption,
				},
				PreRunCallback = new Action<BotSharp.ScriptRun>(ScriptRun =>
				{
					ScriptRun.InstrumentationCallbackSynchronousFirstTime = new Action<BotSharp.SourceLocation>(ScriptSourceLocation =>
					{
						//	make sure script runs on same culture independend of host culture.
						System.Threading.Thread.CurrentThread.CurrentCulture = Script.Impl.ToScriptImport.ScriptDefaultCulture;
					});
				}),
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

		static string AssemblyDirectoryPath => App.AssemblyDirectoryPath;

		static public string ConfigFilePath =>
			AssemblyDirectoryPath.PathToFilesysChild("config");

		string ScriptDirectoryPath => AssemblyDirectoryPath.PathToFilesysChild(@"script\");

		string DefaultScriptPath => ScriptDirectoryPath.PathToFilesysChild("default.cs");

		KeyValuePair<string, string>[] ListScriptIncluded =
			SetScriptIncludedConstruct()?.ExceptionCatch(Bib3.FCL.GBS.Extension.MessageBoxException)
			?.OrderBy(ScriptNameAndContent => !ScriptNameAndContent.Key.RegexMatchSuccessIgnoreCase("travel"))
			?.ToArray();

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

		public Config ConfigReadFromUI() =>
			ConfigViewModel.PropagateFromDependencyPropertyToClrMember();

		public void ConfigFromModelToView(Config config) =>
			ConfigViewModel.PropagateFromClrMemberToDependencyProperty(config);

		Bib3.FCL.GBS.ToggleButtonHorizBinär ToggleButtonMotionEnable => MainView?.ToggleButtonMotionEnable;

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

	}
}
