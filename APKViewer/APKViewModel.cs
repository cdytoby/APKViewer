using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Linq;
using System.Collections.ObjectModel;
using APKViewer.Localize;

namespace APKViewer
{
	public class APKViewModel : BindableModelBase
	{
		private IApkDecoder apkDecoder;
		private IOpenRawDialogService dialogService;

		public LocalizeTextDataModel localizeModel => LocalizationCenter.currentDataModel;

		public APKDataModel targetAPKData { get; protected set; }
		public Uri FileLocation { get; protected set; }
		public bool isDecoding { get; protected set; }
		public bool isNotDecoding => !isDecoding;

		public bool isDataEmpty => targetAPKData == null;

		public string AppName => targetAPKData?.AppName;
		public string AppVersion => GetVersionString();
		public string PackageName => targetAPKData?.PackageName;
		public string minSDK => AndroidSDKData.GetFullString(targetAPKData?.MinSDKCode ?? 0);
		public string maxSDK => AndroidSDKData.GetFullString(targetAPKData?.MaxSDKCode ?? 0);
		public string screenSize => StringGroupToString(targetAPKData?.ScreenSize, false);
		public string densities => StringGroupToString(targetAPKData?.Densities, false);
		public string permissions => StringGroupToString(targetAPKData?.Permissions);
		public string features => GetFormattedFeatureString();
		public string openGLString => targetAPKData?.OpenGLVersion;
		public bool openGLExist => openGLString != null;
		public string signature => targetAPKData?.Signature;
		public string hashString => targetAPKData?.SHA1Hash;
		public string imgToolTip => targetAPKData?.MaxIconZipEntry;
		public byte[] iconPngByte => targetAPKData?.MaxIconContent;

		private ObservableCollection<string> m_appNameList;
		public string selectedAppName { get; protected set; }
		public ObservableCollection<string> appNameList => GetAppNameList();

		public string rawBadging => targetAPKData?.RawDumpBadging;
		public string rawSignature => targetAPKData?.RawDumpSignature;

		public Command openPlayStore => new Command(OpenGooglePlayStore, CanExecutionActionBase);
		public Command openRawView => new Command(OpenRawViewDialog, CanExecutionActionBase);

		public APKViewModel()
		{
			m_appNameList = new ObservableCollection<string>();
			//SetupMockupDataModel();
		}

		private void SetupMockupDataModel()
		{
			targetAPKData = new APKDataModel();
			if (targetAPKData.Permissions == null)
				targetAPKData.Permissions = new List<string>();
			if (targetAPKData.Densities == null)
				targetAPKData.Densities = new List<string>();
			if (targetAPKData.ScreenSize == null)
				targetAPKData.ScreenSize = new List<string>();
			if (targetAPKData.Feature_Require == null)
				targetAPKData.Feature_Require = new List<string>();
			if (targetAPKData.Feature_NotRequire == null)
				targetAPKData.Feature_NotRequire = new List<string>();
			if (targetAPKData.AppNameLangDict == null)
				targetAPKData.AppNameLangDict = new Dictionary<string, string>();

			targetAPKData.AppNameLangDict.Add("key1", "value1");
			targetAPKData.AppNameLangDict.Add("key2", "value2");
			targetAPKData.AppNameLangDict.Add("key3", "value3");
			targetAPKData.AppNameLangDict.Add("key4", "value4");
		}

		public void SetDecoder(IApkDecoder newDecoder)
		{
			apkDecoder = newDecoder;
			apkDecoder.decodeFinishedEvent += GetDataFromDecoder;
		}
		public void SetDialogService(IOpenRawDialogService newService)
		{
			dialogService = newService;
		}

		public void SetNewFile(Uri newFileUri)
		{
			FileLocation = newFileUri;
			targetAPKData = null;
			apkDecoder.SetApkFilePath(newFileUri);
			apkDecoder.Decode();
			isDecoding = true;
		}

		private void GetDataFromDecoder()
		{
			targetAPKData = apkDecoder.GetDataModel();
			isDecoding = false;
		}

		private string GetVersionString()
		{
			if (targetAPKData == null)
				return string.Empty;
			return targetAPKData.VersionString + " (" + targetAPKData.VersionCode + ")";
		}

		private string GetFormattedFeatureString()
		{
			if (targetAPKData == null)
				return string.Empty;

			StringBuilder builder = new StringBuilder();
			if (targetAPKData.Feature_Require.Count > 0)
			{
				builder.AppendLine(localizeModel.Field_Permission_Required);
				builder.AppendLine(StringGroupToString(targetAPKData?.Feature_Require));
			}
			if (targetAPKData.Feature_NotRequire.Count > 0)
			{
				if (builder.Length != 0)
					builder.AppendLine();
				builder.AppendLine(localizeModel.Field_Permission_NotRequired);
				builder.AppendLine(StringGroupToString(targetAPKData?.Feature_NotRequire));
			}

			return builder.ToString();
		}

		private ObservableCollection<string> GetAppNameList()
		{
			m_appNameList.Clear();
			if (targetAPKData != null)
			{
				IEnumerable<string> newStringIEnum = targetAPKData.AppNameLangDict.Select((x) =>
				{
					if (string.IsNullOrEmpty(x.Key))
						return x.Value;
					return x.Key + ": " + x.Value;
				}
				);
				foreach (string s in newStringIEnum)
				{
					m_appNameList.Add(s);
				}
			}

			if (m_appNameList.Count > 0)
				selectedAppName = m_appNameList[0];
			else
				selectedAppName = string.Empty;

			return m_appNameList;
		}

		private void OpenRawViewDialog()
		{
			dialogService?.OpenViewRawDialog();
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
