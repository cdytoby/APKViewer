using APKViewer.Localize;
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
			LoadLocalization();
		}

		protected void LoadAndroidSDKTable()
		{
			Console.WriteLine("App.LoadAndroidSDKTable() Ready to load sdk table from current directory.");
			string filePath = Path.Combine(AppUtility.GetCurrentExePath(), AndroidSDKData.FILE_ANDROIDTABLE);
			if (File.Exists(filePath))
			{
				AndroidSDKData.SetTableFullString(File.ReadAllText(filePath));
			}
		}

		protected void LoadLocalization()
		{
			Console.WriteLine("App.LoadLocalization() Ready to load localization from current directory.");
			string settingFilePath = Path.Combine(AppUtility.GetCurrentExePath(), LocalizationCenter.FOLDER_LOCALIZATION, "localSetting");
			string langCode = string.Empty;
			if (!File.Exists(settingFilePath))
			{
				File.WriteAllText(settingFilePath, "en-us");
				langCode = "en-us";
			}
			else
			{
				langCode = File.ReadAllText(settingFilePath);
			}

			string localFilePath = Path.Combine(AppUtility.GetCurrentExePath(), LocalizationCenter.FOLDER_LOCALIZATION, langCode + ".json");
			if (!File.Exists(localFilePath))
			{
				return;
			}

			LocalizationCenter.SetLocalizationData(langCode, File.ReadAllText(localFilePath));
		}
	}
}
