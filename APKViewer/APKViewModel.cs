﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

namespace APKViewer
{
	public class APKViewModel : BindableModelBase
	{
		private IApkDecoder apkDecoder;

		public APKDataModel targetAPKData { get; protected set; }
		public Uri FileLocation { get; protected set; }

		public string AppName => targetAPKData?.AppName;
		public string AppVersion => targetAPKData?.VersionString +" "+ targetAPKData?.VersionCode;
		public string PackageName => targetAPKData?.PackageName;
		public string minSDK => targetAPKData?.MinSDKCode.ToString();
		public string maxSDK => targetAPKData?.MaxSDKCode.ToString();
		public string permissions => StringGroupToString(targetAPKData?.Permissions);
		public byte[] iconPngByte => targetAPKData?.defaultIconContent;

		public string rawTest => targetAPKData?.RawDumpBadging;

		public APKViewModel()
		{
			//Test();
		}

		public void SetDecoder(IApkDecoder newDecoder)
		{
			apkDecoder = newDecoder;
		}

		public void SetNewFile(Uri newFileUri)
		{
			FileLocation = newFileUri;
			apkDecoder.SetApkFilePath(newFileUri);
			apkDecoder.Decode();
			targetAPKData = apkDecoder.GetDataModel();
		}

		private string StringGroupToString(IEnumerable<string> stringIEnum)
		{
			if (stringIEnum == null)
				return string.Empty;
			string result = string.Empty;
			foreach(string line in stringIEnum)
			{
				result += (line + Environment.NewLine);
			}
			return result;
		}

		private async Task Test()
		{
			Console.WriteLine("APKViewModel Test start");
			while (apkDecoder == null)
			{
				Console.WriteLine("apkDecoder is null");
				await Task.Delay(25);
			}
			Console.WriteLine("APKViewModel Test start 2");
			apkDecoder.SetApkFilePath(new Uri(@"D:\Android\YahooWeatherProvider.apk"));
			Console.WriteLine("APKViewModel Test 2");
			apkDecoder.Decode();
			Console.WriteLine("APKViewModel Test 3");
			targetAPKData = apkDecoder.GetDataModel();
			Console.WriteLine("APKViewModel Test end");
		}
	}
}