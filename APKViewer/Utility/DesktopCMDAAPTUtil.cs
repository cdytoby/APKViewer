using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace APKViewer.Utility
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
		private const string KEY_APPICON = "application-icon";
		private const string KEY_PERMISSION = "uses-permission";
		private const string KEY_SCREENSIZE = "supports-screens";
		private const string KEY_DENSITY = "densities";
		private const string KEY_DENSITY_ANY = "supports-any-density";
		private const string KEY_GLES = "uses-gl-es";
		private const string KEY_FEATURE_REQUIRED = "uses-feature";
		private const string KEY_FEATURE_NOTREQUIRED = "uses-feature-not-required";

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
			if (targetModel.Densities == null)
				targetModel.Densities = new List<string>();
			if (targetModel.ScreenSize == null)
				targetModel.ScreenSize = new List<string>();
			if (targetModel.Feature_Require == null)
				targetModel.Feature_Require = new List<string>();
			if (targetModel.Feature_NotRequire == null)
				targetModel.Feature_NotRequire = new List<string>();
			if (targetModel.AppNameLangDict == null)
				targetModel.AppNameLangDict = new Dictionary<string, string>();

			string[] textLines = badgingResult.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string line in textLines)
			{
				(string, string) splitResult = SplitKeyValue(line);
				string key = splitResult.Item1.Trim();
				string value = splitResult.Item2.Trim('\'', ' ');

				//Console.WriteLine("key=" + key);
				//Console.WriteLine("value=" + value);

				if (key.StartsWith(KEY_APPICON))
				{
					if (string.IsNullOrEmpty(targetModel.MaxIconZipEntry))
						targetModel.MaxIconZipEntry = value;
					else if (targetModel.MaxIconZipEntry.Count(x => x.Equals('x')) < value.Count(x => x.Equals('x')))
					{
						targetModel.MaxIconZipEntry = value;
					}
				}
				if (key.StartsWith(KEY_APPLABEL))
				{
					if (key.Equals(KEY_APPLABEL))
					{
						targetModel.AppNameLangDict.Add(StringConstant.LangCode_Key_Default, value);
					}
					else if (targetModel.AppNameLangDict.ContainsKey(StringConstant.LangCode_Key_Default) &&
						!value.Equals(targetModel.AppNameLangDict[StringConstant.LangCode_Key_Default]))
					{
						targetModel.AppNameLangDict.Add(key.Substring(KEY_APPLABEL.Length + 1), value);
					}
				}

				switch (key)
				{
					case KEY_PACKAGEGROUP:
						Dictionary<string, string> packageDict = SplitSubDict(value);
						targetModel.PackageName = packageDict[SUBKEY_NAME];
						int tempVC = 0;
						int.TryParse(packageDict[SUBKEY_VERSIONCODE], out tempVC);
						targetModel.VersionCode = tempVC;
						targetModel.VersionString = packageDict[SUBKEY_VERSIONNAME];
						break;
					case KEY_MINSDK:
						targetModel.MinSDKCode = int.Parse(value);
						break;
					case KEY_TARGETSDK:
						targetModel.MaxSDKCode = int.Parse(value);
						break;
					case KEY_APP:
						Dictionary<string, string> labelDict = SplitSubDict(value);
						targetModel.AppName = labelDict[SUBKEY_LABEL];
						break;
					case KEY_SCREENSIZE:
						targetModel.ScreenSize.AddRange(SplitSubArray(value));
						break;
					case KEY_DENSITY:
						targetModel.Densities.AddRange(SplitSubArray(value));
						break;
					case KEY_DENSITY_ANY:
						if (value.Equals(true.ToString().ToLower()))
							targetModel.Densities.Add("any");
						break;
					case KEY_PERMISSION:
						Dictionary<string, string> permissionDict = SplitSubDict(value);
						targetModel.Permissions.Add(permissionDict[SUBKEY_NAME]);
						break;
					case KEY_FEATURE_REQUIRED:
						Dictionary<string, string> featureDict = SplitSubDict(value);
						targetModel.Feature_Require.Add(featureDict[SUBKEY_NAME]);
						break;
					case KEY_FEATURE_NOTREQUIRED:
						Dictionary<string, string> featureNRDict = SplitSubDict(value);
						targetModel.Feature_NotRequire.Add(featureNRDict[SUBKEY_NAME]);
						break;
					case KEY_GLES:
						targetModel.OpenGLVersion = StringConstant.FieldHead_OpenGL + OpenGLVersionParse(value);
						break;
					default:
						//Console.WriteLine("Key not processed, key=" + key);
						break;
				}
			}
		}

		public static string OpenGLVersionParse(string originalValue)
		{
			int hexValue = Convert.ToInt32(originalValue, 16);
			int versionMain = hexValue >> 0x00000010;
			int versionSub = hexValue & 0xFFFF;
			return versionMain + "." + versionSub;
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
				if (fieldKeyValue.Length > 1)
					result.Add(fieldKeyValue[0], fieldKeyValue[1].Trim('\''));
				else
					result.Add(fieldKeyValue[0], string.Empty);
			}

			return result;
		}

		private static List<string> SplitSubArray(string valueFull)
		{
			List<string> result = new List<string>();
			valueFull = valueFull.Trim();
			string[] splitFieldResult = valueFull.Split(new string[] { "\' \'" }, StringSplitOptions.RemoveEmptyEntries);
			result.AddRange(splitFieldResult);
			return result;
		}
	}
}
