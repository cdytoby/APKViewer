using APKViewer.Localize;
using APKViewer.Utility;
using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
using Newtonsoft.Json;

namespace APKViewer.XUnitTest
{
	public class RequestStandardFile : TestBase
	{
		private const string jsonFile = "result.json";
		private static string workingDirectory { get { return Environment.GetFolderPath(Environment.SpecialFolder.Desktop); } }
		private const string localizationPath = @"F:\Workspaces\VisualStudio\APKViewer\OtherAssets\Local";

		public RequestStandardFile(ITestOutputHelper output) : base(output)
		{
		}


		[Fact]
		public void RequestAndroidSDKTable()
		{
			string result = AndroidSDKData.RequestTableJsonString();
			Console.WriteLine(result);
			File.WriteAllText(Path.Combine(workingDirectory, jsonFile), result);
		}

		[Fact]
		public void RequestEnglishLocalizationFile()
		{
			string result = LocalizationCenter.RequestModelJson();
			Console.WriteLine(result);
			File.WriteAllText(Path.Combine(workingDirectory, jsonFile), result);
		}

		[Fact]
		public void RenewAllLocalizationFileStructure()
		{
			string[] fileList = Directory.GetFiles(localizationPath);
			foreach (string filePath in fileList)
			{
				Console.WriteLine("processing " + filePath);
				LocalizeTextDataModel ltdm = JsonConvert.DeserializeObject<LocalizeTextDataModel>(File.ReadAllText(filePath));

				File.WriteAllText(filePath, JsonConvert.SerializeObject(ltdm, Formatting.Indented));
				Console.WriteLine("processing finish. path=" + filePath);
			}
		}
	}
}
