using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using APKViewer.Decoders;

namespace APKViewer.WPFApp
{
	public partial class MainWindow: Window, IOpenRawDialogService, IMessageBoxService
	{
		public MainWindowViewModel viewModel { get; private set; }
		
		private BindingExpression overlayVisibilityBindingExpress;
		private ICmdPathProvider cmdPathProvider;

		public MainWindow(MainWindowViewModel newViewModel, ICmdPathProvider newCmdProvider)
		{
			viewModel = newViewModel;
			InitializeComponent();
			
			cmdPathProvider = newCmdProvider;
			//decoder.statusReportEvent += ShowTestLog;
			viewModel.SetDialogService(this);
			viewModel.SetMessageDialog(this);
			overlayVisibilityBindingExpress = DropOverlay.GetBindingExpression(Grid.VisibilityProperty);

			OpenFileArgProcess();
		}

		//private void ShowTestLog(string x)
		//{
		//	if (!StatusTestLabel.Content.ToString().Contains("error"))
		//		StatusTestLabel.Content = x;
		//}

		private void OpenFileArgProcess()
		{
			string[] envArgs = Environment.GetCommandLineArgs();
			foreach (string arg in envArgs)
			{
				Debug.WriteLine("MainWindow.OpenFileArgProcess(), Get arg= " + arg);
			}
			if (envArgs.Length > 1)
				viewModel.SetNewFile(new Uri(envArgs[1]));
		}

		private void FileDrop(object sender, DragEventArgs e)
		{
			Debug.WriteLine("MainWindow.FileDrop(), something dropped.");
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
				if (files.Length > 0)
				{
					foreach (string fileName in files)
					{
						if (viewModel.FileAllowed(fileName))
						{
							viewModel.SetNewFile(new Uri(fileName));
							break;
						}
					}
				}
			}

			DropOverlay.SetBinding(Grid.VisibilityProperty, overlayVisibilityBindingExpress.ParentBinding);
		}

		private void FileDragEnter(object sender, DragEventArgs e)
		{
			DropOverlay.Visibility = Visibility.Visible;
		}

		private void FileDragLeave(object sender, DragEventArgs e)
		{
			DropOverlay.SetBinding(Grid.VisibilityProperty, overlayVisibilityBindingExpress.ParentBinding);
		}

		public void OpenViewRawDialog()
		{
			RawDataDialog dialog = new RawDataDialog(viewModel);
			dialog.ShowDialog();
		}

		public bool OpenDialog(string message)
		{
			MessageBoxResult result = MessageBox.Show(message, String.Empty, MessageBoxButton.OK);
			return result == MessageBoxResult.Yes || result == MessageBoxResult.OK;
		}
	}
}