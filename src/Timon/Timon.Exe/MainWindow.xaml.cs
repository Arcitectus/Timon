using System.Windows;

namespace Timon.Exe
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public string TitleComputed =>
			"Timon v" + (TryFindResource("AppVersionId") ?? "");

		public MainWindow()
		{
			InitializeComponent();
		}
	}
}
