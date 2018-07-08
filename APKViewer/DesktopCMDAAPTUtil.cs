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
		private const string KEY_PERMISSION = "uses-permission";
		private const string KEY_APPLABEL = "application-label";

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

				Console.WriteLine("key=" + key);
				Console.WriteLine("value=" + value);

				switch (key)
				{
					case KEY_MINSDK:
						targetModel.MinSDKCode = int.Parse(value.Trim('\''));
						break;
					case KEY_TARGETSDK:
						targetModel.MaxSDKCode = int.Parse(value.Trim('\''));
						break;
					case KEY_APPLABEL:
						targetModel.AppName = value.Trim('\'');
						break;
					case KEY_PERMISSION:
						targetModel.Permissions.Add(value);
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
	}
}
