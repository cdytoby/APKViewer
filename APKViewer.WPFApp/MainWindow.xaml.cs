using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace APKViewer.WPFApp
{
	public partial class MainWindow : Window
	{
		private APKViewModel bindedViewModel;

		public MainWindow()
		{
			InitializeComponent();
			bindedViewModel = (APKViewModel)DataContext;
			bindedViewModel.SetDecoder(new WindowsAPKDecoder());
		}

		private void FileDrop(object sender, DragEventArgs e)
		{
			Console.WriteLine("Test file dropped.");
			if(e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
				if(files.Length>0)
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
	}
}
