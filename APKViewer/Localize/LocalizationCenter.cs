using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using APKViewer.Utility;

namespace APKViewer.Localize
{
	public static class LocalizationCenter
	{
		private const string DEFAULT_LOCALIZATION_RESOURCE_NAME = "APKViewer.Localize.Default.json";
		private const string DEFAULT_LOCALIZATION_RESOURCE_LANGCODE = "en-us";

		public static string currentLangCode { get; private set; }
		public static LocalizeTextDataModel currentDataModel { get; private set; }

		static LocalizationCenter()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();

			using (Stream stream = assembly.GetManifestResourceStream(DEFAULT_LOCALIZATION_RESOURCE_NAME))
			using (StreamReader reader = new StreamReader(stream))
			{
				string result = reader.ReadToEnd();
				currentDataModel = JsonConvert.DeserializeObject<LocalizeTextDataModel>(result);
				currentLangCode = DEFAULT_LOCALIZATION_RESOURCE_LANGCODE;
			}
		}

		public static string RequestModelJson()
		{
			JsonSerializerSettings settings = new JsonSerializerSettings()
			{
				ContractResolver = new JsonCustomization.NullToEmptyStringResolver()
			};

			return JsonConvert.SerializeObject(new LocalizeTextDataModel(), Formatting.Indented, settings);
		}
	}
}
