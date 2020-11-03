using System;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using APKViewer.Localize;

namespace APKViewer
{
	public class MainWindowViewModel: BindableModelBase
	{
		private IApkInstaller apkInstaller;
		private IOpenRawDialogService dialogService;
		private IMessageBoxService messageBoxService;

		private Dictionary<string, IFileDecoder> decoderDict;
		private IFileDecoder currentFileDecoder;

		public LocalizeTextDataModel localizeModel => LocalizationCenter.currentDataModel;
		public string localizedSupportFiles =>
			string.Format(localizeModel.Msg_SupportFile, string.Join(",", decoderDict.Keys));

		public PackageDataModel targetPackageData { get; protected set; }
		public Uri fileLocation { get; protected set; }
		public string fileExtension { get; protected set; }
		public bool isDecoding { get; protected set; }
		public bool isNotDecoding => !isDecoding;
		public bool isInstalling { get; protected set; }

		public bool isDataEmpty => targetPackageData == null;

		public string AppName => targetPackageData?.AppName;
		public string AppVersion => GetVersionString();
		public string PackageName => targetPackageData?.PackageName;
		public string minSDK
		{
			get
			{
				if (fileExtension.Equals(StringConstant.FileExtension_APK) ||
					fileExtension.Equals(StringConstant.FileExtension_AAB))
					return AndroidSDKData.GetFullString(targetPackageData?.MinSDKCode);
				else
					return targetPackageData.MinSDKCode;
			}
		}
		public string maxSDK
		{
			get
			{
				if (fileExtension.Equals(StringConstant.FileExtension_APK) ||
					fileExtension.Equals(StringConstant.FileExtension_AAB))
					return AndroidSDKData.GetFullString(targetPackageData?.MaxSDKCode);
				else
					return targetPackageData.MaxSDKCode;
			}
		}
		public string screenSize => StringGroupToString(targetPackageData?.ScreenSize, false);
		public string densities => StringGroupToString(targetPackageData?.Densities, false);
		public string architecture => StringGroupToString(targetPackageData?.Architecture, false);
		public string permissions => StringGroupToString(targetPackageData?.Permissions);
		public string features => GetFormattedFeatureString();
		public string openGLString => targetPackageData?.OpenGLVersion;
		public bool openGLExist => openGLString != null;
		public string signature => targetPackageData?.Signature;
		public string hashString => targetPackageData?.SHA1Hash;
		public string imgToolTip => targetPackageData?.MaxIconZipEntry;
		public byte[] iconPngByte => targetPackageData?.MaxIconContent;

		private ObservableCollection<string> m_appNameList;
		public string selectedAppName { get; protected set; }
		public ObservableCollection<string> appNameList => GetAppNameList();

		public string rawBadging => targetPackageData?.RawDumpBadging;
		public string rawSignature => targetPackageData?.RawDumpSignature;

		public Command openPlayStore => new Command(OpenGooglePlayStore, CanExecutionActionBase);
		public Command openRawView => new Command(OpenRawViewDialog, CanExecutionActionBase);
		public Command installApk => new Command(InstallApk, CanInstallApk);

		public MainWindowViewModel()
		{
			m_appNameList = new ObservableCollection<string>();
			decoderDict = new Dictionary<string, IFileDecoder>();
			//SetupMockupDataModel();
		}

		private void SetupMockupDataModel()
		{
			targetPackageData = new PackageDataModel();

			targetPackageData.AppNameLangDict.Add("key1", "value1");
			targetPackageData.AppNameLangDict.Add("key2", "value2");
			targetPackageData.AppNameLangDict.Add("key3", "value3");
			targetPackageData.AppNameLangDict.Add("key4", "value4");
		}

		public void SetDecoder(IFileDecoder newApkDecoder, IFileDecoder newAabDecoder, IFileDecoder newIpaDecoder)
		{
			if (newApkDecoder != null)
			{
				decoderDict.Add(StringConstant.FileExtension_APK, newApkDecoder);
			}
			if (newAabDecoder != null)
			{
				decoderDict.Add(StringConstant.FileExtension_AAB, newAabDecoder);
			}
			if (newIpaDecoder != null)
			{
				decoderDict.Add(StringConstant.FileExtension_IPA, newIpaDecoder);
			}

			OnPropertyChanged(nameof(localizedSupportFiles));
		}

		public bool FileAllowed(string testFileName)
		{
			foreach (string _ext in decoderDict.Keys)
			{
				if (testFileName.EndsWith(_ext))
					return true;
			}
			return false;
		}

		public void SetInstaller(IApkInstaller newInstaller)
		{
			apkInstaller = newInstaller;
			apkInstaller.installFinishedEvent += ApkInstallFinished;
		}

		public void SetDialogService(IOpenRawDialogService newService)
		{
			dialogService = newService;
		}

		public void SetMessageDialog(IMessageBoxService newMessageBoxService)
		{
			messageBoxService = newMessageBoxService;
		}

		private void ApkInstallFinished(bool success, string message)
		{
			Debug.WriteLine("Install finished, success=" + success + " msg=" + message);
			messageBoxService?.OpenDialog(message);
			isInstalling = false;
		}

		public void SetNewFile(Uri newFileUri)
		{
			fileLocation = newFileUri;
			targetPackageData = null;
			fileExtension = Path.GetExtension(newFileUri.AbsolutePath);
			fileExtension = fileExtension.Trim('.');
			if (string.IsNullOrEmpty(fileExtension))
				return;

			if (currentFileDecoder != null)
				currentFileDecoder.decodeProgressCallbackEvent -= GetDataFromDecoder;

			currentFileDecoder = decoderDict[fileExtension];
			if (currentFileDecoder != null)
			{
				currentFileDecoder.decodeProgressCallbackEvent += GetDataFromDecoder;
				currentFileDecoder.SetFilePath(newFileUri);
				currentFileDecoder.Decode();
				isDecoding = true;
			}
		}

		private void GetDataFromDecoder()
		{
			targetPackageData = null;
			targetPackageData = currentFileDecoder.GetDataModel();

			isDecoding = false;
		}

		private string GetVersionString()
		{
			if (targetPackageData == null)
				return string.Empty;
			return targetPackageData.VersionString + " (" + targetPackageData.VersionCode + ")";
		}

		private string GetFormattedFeatureString()
		{
			if (targetPackageData == null)
				return string.Empty;

			StringBuilder builder = new StringBuilder();
			if (targetPackageData.Feature_Require.Count > 0)
			{
				builder.AppendLine(localizeModel.Field_Feature_Required);
				builder.AppendLine(StringGroupToString(targetPackageData?.Feature_Require));
			}
			if (targetPackageData.Feature_NotRequire.Count > 0)
			{
				if (builder.Length != 0)
					builder.AppendLine();
				builder.AppendLine(localizeModel.Field_Feature_NotRequired);
				builder.AppendLine(StringGroupToString(targetPackageData?.Feature_NotRequire));
			}

			return builder.ToString();
		}

		private ObservableCollection<string> GetAppNameList()
		{
			m_appNameList.Clear();
			if (targetPackageData != null)
			{
				IEnumerable<string> newStringIEnum = targetPackageData.AppNameLangDict.Select((x) =>
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
			Process.Start(StringConstant.Url_GooglePlay + targetPackageData.PackageName);
		}

		private void InstallApk()
		{
			isInstalling = true;
			apkInstaller?.InstallApk(fileLocation);
		}

		private bool CanExecutionActionBase()
		{
			return targetPackageData != null;
		}

		private bool CanInstallApk()
		{
			return CanExecutionActionBase() && !isInstalling &&
				fileExtension.Equals(StringConstant.FileExtension_APK) &&
				!string.IsNullOrEmpty(targetPackageData.PackageName);
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