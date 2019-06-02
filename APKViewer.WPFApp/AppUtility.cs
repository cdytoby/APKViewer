using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using APKViewer.Localize;
using APKViewer.Utility;

namespace APKViewer.WPFApp
{
	public static class AppUtility
	{
		public static string GetCurrentExePath()
		{
			return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
		}

		public static async Task TestJavaExist()
		{
			string processResult = string.Empty;

			if (!DesktopJavaUtil.javaTested)
			{
				// java -jar apksigner.jar verify --verbose --print-certs FDroid.apk
				// java -version

				ProcessStartInfo psiJavaVersion = new ProcessStartInfo()
				{
					FileName = "cmd.exe",
					Arguments = "/c java -version"
				};
				processResult = await ProcessExecuter.ExecuteProcess(psiJavaVersion, true);
				DesktopJavaUtil.SetJavaExist(processResult);
			}
		}
	}
}
