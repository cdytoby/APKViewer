using System;

namespace APKViewer.MacApp
{
	public class MacCmdProvider: ICmdPathProvider
	{
		public const string MY_AAPT =
			@"/Users/cdytoby/Library/Developer/Xamarin/android-sdk-macosx/build-tools/23.0.1/aapt";
		public const string MY_ADB = @"/Users/cdytoby/Library/Developer/Xamarin/android-sdk-macosx/platform-tools/adb";

		public string GetAAPTPath()
		{
			return MY_AAPT;
		}

		public string GetAPKSignerPath()
		{
			return string.Empty;
		}

		public string GetADBPath()
		{
			return MY_ADB;
		}

		public string GetBundleToolPath()
		{
			return string.Empty;
		}
	}
}