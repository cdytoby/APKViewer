using APKViewer.Localize;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Windows;
using System.Windows.Navigation;

namespace APKViewer.WPFApp
{
	public partial class App: Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			LoadAndroidSDKTable();
			LoadLocalization();
		}

		private static void LoadAndroidSDKTable()
		{
			Console.WriteLine("App.LoadAndroidSDKTable() Ready to load sdk table from current directory.");
			string filePath = Path.Combine(AppUtility.GetCurrentExePath(), AndroidSDKData.FILE_ANDROIDTABLE);
			if (File.Exists(filePath))
			{
				AndroidSDKData.SetTableFullString(File.ReadAllText(filePath));
			}
		}

		private static void LoadLocalization()
		{
			Console.WriteLine("App.LoadLocalization() Ready to load localization from current directory.");
			string settingFilePath = Path.Combine(AppUtility.GetCurrentExePath(),
				LocalizationCenter.FOLDER_LOCALIZATION, "localSetting");
			string langCode = string.Empty;
			if (!File.Exists(settingFilePath))
			{
				File.WriteAllText(settingFilePath, LocalizationCenter.DEFAULT_LOCALIZATION_NOTSET);
				langCode = CultureInfo.CurrentCulture.Name;
			}
			else
			{
				langCode = File.ReadAllText(settingFilePath);
				if (langCode.Equals(LocalizationCenter.DEFAULT_LOCALIZATION_NOTSET))
					langCode = CultureInfo.CurrentCulture.Name;
			}

			string localFilePath = Path.Combine(AppUtility.GetCurrentExePath(),
				LocalizationCenter.FOLDER_LOCALIZATION, langCode + ".json");
			if (!File.Exists(localFilePath))
			{
				langCode = LocalizationCenter.DEFAULT_LOCALIZATION_RESOURCE_LANGCODE;
				return;
			}

			LocalizationCenter.SetLocalizationData(langCode, File.ReadAllText(localFilePath));
		}
	}
}