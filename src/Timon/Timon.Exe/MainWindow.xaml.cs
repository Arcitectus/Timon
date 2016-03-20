using System;
using System.Windows;
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
			ScriptIDE?.Present();
		}

		public MainWindow()
		{
			InitializeComponent();

			ConfigView.DataContext = ConfigViewModel;

			TimerConstruct();
		}
	}
}
