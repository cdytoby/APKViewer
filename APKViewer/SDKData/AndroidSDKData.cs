using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace APKViewer
{
	public static class AndroidSDKData
	{
		private const string DEFAULT_SDK_RESOURCE_NAME = "APKViewer.SDKData.DefaultData.json";
		public const string FILE_ANDROIDTABLE = "AndroidVersionTable.json";

		private static Dictionary<int, (string, string)> tableDictionary = new Dictionary<int, (string, string)>();

		static AndroidSDKData()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();

			using (Stream stream = assembly.GetManifestResourceStream(DEFAULT_SDK_RESOURCE_NAME))
			using (StreamReader reader = new StreamReader(stream))
			{
				SetTableFullString(reader.ReadToEnd());
			}
		}

		public static void SetTableFullString(string sourceTable)
		{
			tableDictionary = JsonConvert.DeserializeObject<Dictionary<int, (string, string)>>(sourceTable);
		}

		public static string GetFullString(int sdkInt)
		{
			if (sdkInt <= 0)
				return string.Empty;
			if (!tableDictionary.ContainsKey(sdkInt))
				return "API " + sdkInt;
			return "API " + sdkInt + " Android " + tableDictionary[sdkInt].Item1 + " " + tableDictionary[sdkInt].Item2;
		}

		public static string RequestTableJsonString()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();

			using (Stream stream = assembly.GetManifestResourceStream(DEFAULT_SDK_RESOURCE_NAME))
			using (StreamReader reader = new StreamReader(stream))
			{
				return reader.ReadToEnd();
			}
		}
	}
}
