using System;
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
		public string AppVersion => targetAPKData?.VersionString + " " + targetAPKData?.VersionCode;
		public string PackageName => targetAPKData?.PackageName;
		public string minSDK => AndroidSDKData.GetFullString(targetAPKData?.MinSDKCode ?? 0);
		public string maxSDK => AndroidSDKData.GetFullString(targetAPKData?.MaxSDKCode ?? 0);
		public string screenSize => StringGroupToString(targetAPKData?.ScreenSize, false);
		public string densities => StringGroupToString(targetAPKData?.Densities, false);
		public string permissions => StringGroupToString(targetAPKData?.Permissions);
		public string features =>
			"Required: " + Environment.NewLine +
			StringGroupToString(targetAPKData?.Feature_Require) +
			Environment.NewLine + "Not Required: " + Environment.NewLine +
			StringGroupToString(targetAPKData?.Feature_NotRequire);
		public string signature => targetAPKData?.Signature;
		public string imgToolTip => targetAPKData?.MaxIconZipEntry;
		public byte[] iconPngByte => targetAPKData?.MaxIconContent;

		public string rawTest => targetAPKData?.RawDumpBadging;

		public APKViewModel()
		{
		}

		public void SetDecoder(IApkDecoder newDecoder)
		{
			apkDecoder = newDecoder;
			apkDecoder.decodeFinishedEvent += GetDataFromDecoder;
		}

		public void SetNewFile(Uri newFileUri)
		{
			FileLocation = newFileUri;
			apkDecoder.SetApkFilePath(newFileUri);
			apkDecoder.Decode();
		}

		private void GetDataFromDecoder()
		{
			targetAPKData = apkDecoder.GetDataModel();
		}

		private string StringGroupToString(IEnumerable<string> stringIEnum, bool newLine = true)
		{
			if (stringIEnum == null)
				return string.Empty;
			string result = string.Empty;
			foreach (string line in stringIEnum)
			{
				result += (line + (newLine ? Environment.NewLine : " "));
			}
			return result;
		}
		
	}
}
