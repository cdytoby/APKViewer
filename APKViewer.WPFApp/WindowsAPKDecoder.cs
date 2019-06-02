using APKViewer.Localize;
using APKViewer.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace APKViewer.WPFApp
{
	public class WindowsAPKDecoder: IFileDecoder
	{
		private Uri targetFilePath;
		private APKDataModel dataModel;

		public event Action decodeFinishedEvent;

		public WindowsAPKDecoder()
		{
			dataModel = null;
		}

		public void SetFilePath(Uri fileUri)
		{
			targetFilePath = fileUri;
		}

		public async Task Decode()
		{
			//C:\CLIProgram\Android\AndroidSDK\build-tools\27.0.0\aapt.exe
			//./aapt d badging "D:\Android\YahooWeatherProvider.apk"

			Debug.WriteLine("WindowsAPKDecoder.Decode() decode start.");
			dataModel = new APKDataModel();

			Debug.WriteLine("WindowsAPKDecoder.Decode() Decode_Badging start.");
			await Decode_Badging();
			Debug.WriteLine("WindowsAPKDecoder.Decode() Decode_Icon start.");
			await Decode_Icon();
			Debug.WriteLine("WindowsAPKDecoder.Decode() Decode_Signature start.");
			await Decode_Signature();
			Debug.WriteLine("WindowsAPKDecoder.Decode() Decode_Hash start.");
			Decode_Hash();

			Debug.WriteLine("WindowsAPKDecoder.Decode() decode finish.");
			decodeFinishedEvent?.Invoke();
		}

		private async Task Decode_Badging()
		{
			ProcessStartInfo psi = new ProcessStartInfo()
			{
				FileName = ExternalToolBinPath.GetAAPTPath(),
				Arguments = " d badging \"" + targetFilePath.OriginalString + "\""
			};
			Debug.WriteLine("WindowsAPKDecoder.Decode_Badging(), path=" + targetFilePath.OriginalString);
			string processResult = await ProcessExecuter.ExecuteProcess(psi);

			dataModel.RawDumpBadging = processResult;
			DesktopCMDAAPTUtil.ReadBadging(dataModel, dataModel.RawDumpBadging);
		}

		private async Task Decode_Icon()
		{
			dataModel.MaxIconContent = await FileUtil.ZipExtractData(targetFilePath, dataModel.MaxIconZipEntry);
		}

		private async Task Decode_Signature()
		{
			await AppUtility.TestJavaExist();

			if (!DesktopJavaUtil.javaExist)
			{
				dataModel.Signature = LocalizationCenter.currentDataModel.Msg_JavaNotFound_APKSignFail;
				return;
			}

			ProcessStartInfo psiAPKSigner = new ProcessStartInfo
			{
				FileName = "java.exe",
				Arguments = "-jar " + ExternalToolBinPath.GetAPKSignerPath() +
					" verify --verbose --print-certs" +
					" \"" + targetFilePath.OriginalString + "\"",
			};
			string processResult = await ProcessExecuter.ExecuteProcess(psiAPKSigner);

			dataModel.RawDumpSignature = processResult;
			DesktopCMDAPKSignerUtil.ReadAPKSignature(dataModel, processResult);
		}

		private void Decode_Hash()
		{
			FileUtil.CalculateSHA1(targetFilePath, dataModel);
		}

		public APKDataModel GetDataModel()
		{
			return dataModel;
		}

	}
}