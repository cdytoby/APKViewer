using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;

namespace APKViewer.WPFApp
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			LoadAndroidSDKTable();
		}

		protected void LoadAndroidSDKTable()
		{
			Console.WriteLine("App.LoadAndroidSDKTable() Ready to load sdk table from current directory.");
			string filePath = Path.Combine(Environment.CurrentDirectory, AndroidSDKData.FILE_ANDROIDTABLE);
			if (File.Exists(filePath))
			{
				AndroidSDKData.SetTableFullString(File.ReadAllText(filePath));
			}
		}
	}
}
