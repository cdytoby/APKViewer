using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace APKViewer
{
	public static class DesktopCMDAAPTUtil
	{
		private const char SPLITTER_MAINKEYVALUE = ':';
		private const char SPLITTER_SUBKEYVALUE = '=';
		private const string KEY_PACKAGEGROUP = "package";
		private const string KEY_MINSDK = "sdkVersion";
		private const string KEY_TARGETSDK = "targetSdkVersion";
		private const string KEY_APP = "application";
		private const string KEY_APPLABEL = "application-label";
		private const string KEY_PERMISSION = "uses-permission";

		private const string SUBKEY_NAME = "name";
		private const string SUBKEY_LABEL = "label";
		private const string SUBKEY_ICON = "icon";
		private const string SUBKEY_VERSIONCODE = "versionCode";
		private const string SUBKEY_VERSIONNAME = "versionName";
		private const string SUBKEY_BUILDPLATFORMNAME = "platformBuildVersionName";

		public static void ReadBadging(APKDataModel targetModel, string badgingResult)
		{
			if (targetModel == null)
				targetModel = new APKDataModel();
			if (targetModel.Permissions == null)
				targetModel.Permissions = new List<string>();

			string[] textLines = badgingResult.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string line in textLines)
			{
				(string, string) splitResult = SplitKeyValue(line);
				string key = splitResult.Item1;
				string value = splitResult.Item2;

				//Console.WriteLine("key=" + key);
				//Console.WriteLine("value=" + value);

				switch (key)
				{
					case KEY_PACKAGEGROUP:
						Dictionary<string, string> packageDict = SplitSubDict(value);
						targetModel.PackageName = packageDict[SUBKEY_NAME];
						targetModel.VersionCode = int.Parse(packageDict[SUBKEY_VERSIONCODE]);
						targetModel.VersionString = packageDict[SUBKEY_VERSIONNAME];
						break;
					case KEY_MINSDK:
						targetModel.MinSDKCode = int.Parse(value.Trim('\''));
						break;
					case KEY_TARGETSDK:
						targetModel.MaxSDKCode = int.Parse(value.Trim('\''));
						break;
					case KEY_APPLABEL:
						targetModel.AppName = value.Trim('\'');
						break;
					case KEY_APP:
						Dictionary<string, string> labelDict = SplitSubDict(value);
						targetModel.defaultIconZipEntry = labelDict[SUBKEY_ICON];
						break;
					case KEY_PERMISSION:
						Dictionary<string, string> permissionDict = SplitSubDict(value);
						targetModel.Permissions.Add(permissionDict[SUBKEY_NAME]);
						break;
					default:
						//Console.WriteLine("Key not processed, key=" + key);
						break;
				}
			}
		}

		private static (string, string) SplitKeyValue(string lineText)
		{
			if (lineText.Contains(SPLITTER_MAINKEYVALUE.ToString()))
			{
				string[] splitResult = lineText.Split(new char[] { SPLITTER_MAINKEYVALUE }, 2);
				if (splitResult.Length == 2)
					return (splitResult[0], splitResult[1]);
			}

			return (lineText, string.Empty);
		}

		private static Dictionary<string, string> SplitSubDict(string lineValueFull)
		{
			Dictionary<string, string> result = new Dictionary<string, string>();

			lineValueFull = lineValueFull.Trim();
			string[] splitFieldResult = lineValueFull.Split(new string[] { "\' " }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string field in splitFieldResult)
			{
				string[] fieldKeyValue = field.Split(new char[] { SPLITTER_SUBKEYVALUE }, 2, StringSplitOptions.RemoveEmptyEntries);
				result.Add(fieldKeyValue[0], fieldKeyValue[1].Trim('\''));
			}

			return result;
		}
	}
}
