using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace APKViewer
{
	public class AndroidSDKData
	{
		public const string FILE_ANDROIDTABLE = "AndroidVersionTable.json";
		private const string DEFAULT_SDK_RESOURCE_NAME = "APKViewer.SDKData.DefaultData.json";

		private Dictionary<int, (string, string)> tableDictionary = new();

		public AndroidSDKData()
		{
			SetTableFullString(RequestDefaultAndroidTableJsonString());
		}

		public void SetTableFullString(string sourceTable)
		{
			tableDictionary = JsonConvert.DeserializeObject<Dictionary<int, (string, string)>>(sourceTable);
		}

		public string GetFullString(string sdkStr)
		{
			if (string.IsNullOrWhiteSpace(sdkStr))
				return string.Empty;
			int sdkInt = 0;
			bool parseSuccess = int.TryParse(sdkStr, out sdkInt);
			if (!parseSuccess || sdkInt <= 0)
				return string.Empty;
			if (!tableDictionary.ContainsKey(sdkInt))
				return $"API {sdkInt}";
			return $"API {sdkInt} Android {tableDictionary[sdkInt].Item1} {tableDictionary[sdkInt].Item2}";
		}

		public static string RequestDefaultAndroidTableJsonString()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();

			using Stream stream = assembly.GetManifestResourceStream(DEFAULT_SDK_RESOURCE_NAME);
			using StreamReader reader = new StreamReader(stream);
			return reader.ReadToEnd();
		}
	}
}