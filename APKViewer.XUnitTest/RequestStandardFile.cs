using APKViewer.Localize;
using APKViewer.Utility;
using System;
using System.IO;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
using Newtonsoft.Json;

namespace APKViewer.XUnitTest
{
	public class RequestStandardFile: TestBase
	{
		private const string jsonFile = "result.json";
		private static string workingDirectory
		{
			get { return Environment.GetFolderPath(Environment.SpecialFolder.Desktop); }
		}
		private const string localizationSubPath = @"OtherAssets\Local";

		public RequestStandardFile(ITestOutputHelper output): base(output)
		{
		}

		private string GetSolutionFolder()
		{
			FileInfo binFile = new FileInfo(Assembly.GetExecutingAssembly().Location);
			DirectoryInfo sluDir = binFile.Directory.Parent.Parent.Parent.Parent;
			return sluDir.FullName;
		}


		[Fact]
		public void RequestAndroidSDKTable()
		{
			string result = AndroidSDKData.RequestDefaultAndroidTableJsonString();
			_output.WriteLine(result);
			File.WriteAllText(Path.Combine(workingDirectory, jsonFile), result);
		}

		[Fact]
		public void RequestEnglishLocalizationFile()
		{
			string result = LocalizationCenter.RequestModelJson();
			_output.WriteLine(result);
			File.WriteAllText(Path.Combine(workingDirectory, jsonFile), result);
		}

		[Fact]
		public void TestSolutionFolder()
		{
			string targetFolder = GetSolutionFolder();
			_output.WriteLine("location= " + targetFolder);
		}

		[Fact]
		public void RenewAllLocalizationFileStructure()
		{
			string localizationPath = Path.Combine(GetSolutionFolder(), localizationSubPath);
			string[] fileList = Directory.GetFiles(localizationPath);
			foreach (string filePath in fileList)
			{
				_output.WriteLine("processing " + filePath);
				LocalizeTextDataModel ltdm =
					JsonConvert.DeserializeObject<LocalizeTextDataModel>(File.ReadAllText(filePath));

				File.WriteAllText(filePath, JsonConvert.SerializeObject(ltdm, Formatting.Indented));
				_output.WriteLine("processing finish. path=" + filePath);
			}
		}
	}
}