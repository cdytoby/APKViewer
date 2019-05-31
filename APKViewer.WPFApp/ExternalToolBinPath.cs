using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace APKViewer.WPFApp
{
	public static class ExternalToolBinPath
	{
		//public const string MY_AAPT = @"C:\CLIProgram\Android\AndroidSDK\build-tools\27.0.0\aapt.exe";
		//public const string MY_ADB = @"C:\CLIProgram\Android\AndroidSDK\platform-tools\adb.exe";

		public const string FOLDER_EXTERNAL_TOOL = "ExternalTools";
		public const string EXE_AAPT = "aapt.exe";
		public const string JAR_APKSIGNER = "apksigner.jar";
		public const string EXE_ADB = "adb.exe";
		public const string BAT_BUNDLETOOL = "bundletool.bat";

		public static string GetAAPTPath()
		{
			return Path.Combine(AppUtility.GetCurrentExePath(), FOLDER_EXTERNAL_TOOL, EXE_AAPT);
		}

		public static string GetAPKSignerPath()
		{
			return Path.Combine(AppUtility.GetCurrentExePath(), FOLDER_EXTERNAL_TOOL, JAR_APKSIGNER);
		}

		public static string GetADBPath()
		{
			return Path.Combine(AppUtility.GetCurrentExePath(), FOLDER_EXTERNAL_TOOL, EXE_ADB);
		}

		public static string GetBundleToolPath()
		{
			return Path.Combine(AppUtility.GetCurrentExePath(), FOLDER_EXTERNAL_TOOL, BAT_BUNDLETOOL);
		}
	}
}