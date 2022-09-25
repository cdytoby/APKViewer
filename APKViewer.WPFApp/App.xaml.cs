using APKViewer.Localize;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Windows;
using System.Windows.Navigation;
using APKViewer.Decoders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace APKViewer.WPFApp
{
	public partial class App: Application
	{
		private IHostBuilder hostBuilder;
		private IHost host;
		// private ILogger logger;

		public App()
		{
			hostBuilder = new HostBuilder()
				.ConfigureServices(DiConfigure);
		}

		private void DiConfigure(IServiceCollection serviceCollection)
		{
			serviceCollection.AddSingleton<ICmdPathProvider, WindowsCmdPath>();
			serviceCollection.AddSingleton<IApkInstaller, WindowsApkInstaller>();
			serviceCollection.AddSingleton<IFileDecoder, DefaultAPKDecoder>();
			serviceCollection.AddSingleton<IFileDecoder, DefaultAABDecoder>();
			serviceCollection.AddSingleton<IFileDecoder, DefaultIPADecoder>();
			serviceCollection.AddSingleton<AndroidSDKData>();

			serviceCollection.AddSingleton<MainWindowViewModel>();
			serviceCollection.AddSingleton<MainWindow>();
		}

		private async void App_OnStartup(object sender, StartupEventArgs e)
		{
			host = hostBuilder.Build();
			await host.StartAsync();
			
			LoadAndroidSDKTable(host.Services);
			LoadLocalization();
			
			MainWindow mainWindow = host.Services.GetService<MainWindow>();
			mainWindow.Show();
		}

		private void LoadAndroidSDKTable(IServiceProvider services)
		{
			Console.WriteLine("App.LoadAndroidSDKTable() Ready to load sdk table from current directory.");
			string filePath = Path.Combine(AppUtility.GetCurrentExePath(), AndroidSDKData.FILE_ANDROIDTABLE);
			if (File.Exists(filePath))
			{
				services.GetService<AndroidSDKData>().SetTableFullString(File.ReadAllText(filePath));
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

		private async void App_OnExit(object sender, ExitEventArgs e)
		{
			await host.StopAsync();
		}
	}
}