using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace APKViewer.WPFApp
{
	public partial class MainWindow : Window, IOpenRawDialogService
	{
		private APKViewModel bindedViewModel;

		public MainWindow()
		{
			InitializeComponent();
			bindedViewModel = (APKViewModel)DataContext;
			bindedViewModel.SetDecoder(new WindowsAPKDecoder());
			bindedViewModel.SetDialogService(this);

			OpenFileArgProcess();
		}

		private void OpenFileArgProcess()
		{
			string[] envArgs = Environment.GetCommandLineArgs();
			foreach (string arg in envArgs)
			{
				Console.WriteLine("MainWindow.OpenFileArgProcess(), Get arg= " + arg);
			}
			if (envArgs.Length > 1)
				bindedViewModel.SetNewFile(new Uri(envArgs[1]));
		}

		private void FileDrop(object sender, DragEventArgs e)
		{
			Console.WriteLine("MainWindow.FileDrop(), something dropped.");
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
				if (files.Length > 0)
				{
					foreach (string fileName in files)
					{
						if (fileName.EndsWith("apk"))
						{
							bindedViewModel.SetNewFile(new Uri(fileName));
						}
					}
				}
			}
		}

		public void OpenViewRawDialog()
		{
			RawDataDialog dialog = new RawDataDialog(bindedViewModel);
			dialog.ShowDialog();
		}
	}
}
