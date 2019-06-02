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
		public event Action<string> statusReportEvent;

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
			statusReportEvent?.Invoke("WindowsAPKDecoder.Decode() decode start.");
			dataModel = new APKDataModel();

			statusReportEvent?.Invoke("WindowsAPKDecoder.Decode() Decode_Badging start.");
			await Decode_Badging();
			statusReportEvent?.Invoke("WindowsAPKDecoder.Decode() Decode_Icon start.");
			await Decode_Icon();
			statusReportEvent?.Invoke("WindowsAPKDecoder.Decode() Decode_Signature start.");
			await Decode_Signature();
			statusReportEvent?.Invoke("WindowsAPKDecoder.Decode() Decode_Hash start.");
			Decode_Hash();

			Debug.WriteLine("WindowsAPKDecoder.Decode() decode finish.");
			statusReportEvent?.Invoke("WindowsAPKDecoder.Decode() decode finish.");
			decodeFinishedEvent?.Invoke();
		}

		private async Task Decode_Badging()
		{
			ProcessStartInfo psi = new ProcessStartInfo()
			{
				FileName = ExternalToolBinPath.GetAAPTPath(),
				Arguments = " d badging \"" + targetFilePath.OriginalString + "\""
			};
			statusReportEvent?.Invoke("WindowsAPKDecoder.Decode_Badging(), path=" + targetFilePath.OriginalString);
			string processResult = await ProcessExecuter.ExecuteProcess(psi, false, statusReportEvent);

			dataModel.RawDumpBadging = processResult;
			DesktopCMDAAPTUtil.ReadBadging(dataModel, dataModel.RawDumpBadging);
		}

		private async Task Decode_Icon()
		{
			dataModel.MaxIconContent = await FileUtil.ZipExtractData(targetFilePath, dataModel.MaxIconZipEntry);
		}

		private async Task Decode_Signature()
		{
			string processResult;

			if (!DesktopJavaUtil.javaTested)
			{
				// java -jar apksigner.jar verify --verbose --print-certs FDroid.apk
				// java -version

				ProcessStartInfo psiJavaVersion = new ProcessStartInfo()
				{
					FileName = "cmd.exe",
					Arguments = "/c java -version"
				};
				processResult = await ProcessExecuter.ExecuteProcess(psiJavaVersion, true, statusReportEvent);
				DesktopJavaUtil.SetJavaExist(processResult);
			}

			if (!DesktopJavaUtil.javaExist)
			{
				dataModel.Signature = LocalizationCenter.currentDataModel.Msg_JavaNotFound;
				return;
			}

			ProcessStartInfo psiAPKSigner = new ProcessStartInfo
			{
				FileName = "java.exe",
				Arguments = "-jar " + ExternalToolBinPath.GetAPKSignerPath() +
					" verify --verbose --print-certs" +
					" \"" + targetFilePath.OriginalString + "\"",
			};
			processResult = await ProcessExecuter.ExecuteProcess(psiAPKSigner, false, statusReportEvent);

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