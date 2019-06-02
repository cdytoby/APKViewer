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
		public const string FOLDER_LOCALIZATION = "Local";
		public const string DEFAULT_LOCALIZATION_NOTSET = "default";
		public const string DEFAULT_LOCALIZATION_RESOURCE_LANGCODE = "en-us";
		private const string DEFAULT_LOCALIZATION_RESOURCE_NAME = "APKViewer.Localize.Default.json";

		public static string currentLangCode { get; private set; }
		public static LocalizeTextDataModel currentDataModel { get; private set; }

		static LocalizationCenter()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();

			using (Stream stream = assembly.GetManifestResourceStream(DEFAULT_LOCALIZATION_RESOURCE_NAME))
			using (StreamReader reader = new StreamReader(stream))
			{
				SetLocalizationData(DEFAULT_LOCALIZATION_RESOURCE_LANGCODE, reader.ReadToEnd());
			}
		}

		public static void SetLocalizationData(string newLangCode, string newLangJsonStr)
		{
			LocalizeTextDataModel newModel = JsonConvert.DeserializeObject<LocalizeTextDataModel>(newLangJsonStr);
			if (newModel != null)
			{
				currentDataModel = newModel;
				currentLangCode = newLangCode;
			}
		}

		public static string RequestModelJson()
		{
			JsonSerializerSettings settings = new JsonSerializerSettings()
			{
				ContractResolver = new JsonCustomization.NullToEmptyStringResolver()
			};

			return JsonConvert.SerializeObject(currentDataModel, Formatting.Indented, settings);
		}
	}
}
