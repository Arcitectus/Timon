using Bib3;
using BotEngine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
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

		DispatcherTimer Timer;

		BotSharp.ScriptRun ScriptRun => ScriptIDE?.ScriptRun;

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
		}

	}
}
