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
		public string AppVersion => targetAPKData?.VersionString + " " + targetAPKData?.VersionCode;
		public string PackageName => targetAPKData?.PackageName;
		public string minSDK => AndroidSDKData.GetFullString(targetAPKData?.MinSDKCode ?? 0);
		public string maxSDK => AndroidSDKData.GetFullString(targetAPKData?.MaxSDKCode ?? 0);
		public string screenSize => StringGroupToString(targetAPKData?.ScreenSize, false);
		public string densities => StringGroupToString(targetAPKData?.Densities, false);
		public string permissions => StringGroupToString(targetAPKData?.Permissions);
		public string features => GetFormattedFeatureString();
		public string signature => targetAPKData?.Signature;
		public string imgToolTip => targetAPKData?.MaxIconZipEntry;
		public byte[] iconPngByte => targetAPKData?.MaxIconContent;

		public string rawTest => targetAPKData?.RawDumpBadging;

		public Command openPlayStore => new Command(OpenGooglePlayStore, CanExecutionActionBase);

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

		private string GetFormattedFeatureString()
		{
			if (targetAPKData == null)
				return string.Empty;

			StringBuilder builder = new StringBuilder();
			if (targetAPKData.Feature_Require.Count > 0)
			{
				builder.AppendLine(StringConstant.Permission_Required);
				builder.AppendLine(StringGroupToString(targetAPKData?.Feature_Require));
			}
			if (targetAPKData.Feature_NotRequire.Count > 0)
			{
				builder.AppendLine();
				builder.AppendLine(StringConstant.Permission_NotRequired);
				builder.AppendLine(StringGroupToString(targetAPKData?.Feature_NotRequire));
			}

			return builder.ToString();
		}

		private void OpenGooglePlayStore()
		{
			Process.Start(StringConstant.Url_Play + targetAPKData.PackageName);
		}

		private bool CanExecutionActionBase()
		{
			return targetAPKData != null;
		}

		private string StringGroupToString(IEnumerable<string> stringIEnum, bool newLine = true)
		{
			if (stringIEnum == null)
				return string.Empty;
			string result = string.Empty;
			using (IEnumerator<string> enumerator = stringIEnum.GetEnumerator())
			{
				bool last = !enumerator.MoveNext();
				while (!last)
				{
					result += enumerator.Current;
					last = !enumerator.MoveNext();
					if (!last)
						result += newLine ? Environment.NewLine : " ";
				}
			}
			//foreach (string line in stringIEnum)
			//{
			//	result += (line + (newLine ? Environment.NewLine : " "));
			//}
			return result;
		}

	}
}
