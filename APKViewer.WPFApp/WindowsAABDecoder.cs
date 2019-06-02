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
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace APKViewer.WPFApp
{
	public class WindowsAABDecoder: IFileDecoder
	{
		private Uri targetFilePath;
		private APKDataModel dataModel;

		public event Action decodeFinishedEvent;
		public event Action<string> statusReportEvent;

		public WindowsAABDecoder()
		{
			dataModel = null;
			statusReportEvent += (x) => Debug.WriteLine(x);
		}

		public void SetFilePath(Uri fileUri)
		{
			targetFilePath = fileUri;
		}

		public async Task Decode()
		{
			// bundletool dump manifest--bundle = ".\testaab-2019-05-14_16-08.aab"

			statusReportEvent?.Invoke("WindowsAABDecoder.Decode() decode start.");
			dataModel = new APKDataModel();

			statusReportEvent?.Invoke("WindowsAABDecoder.Decode() start test java.");
			await Decode_JavaTest();

			if (DesktopJavaUtil.javaTested && DesktopJavaUtil.javaExist)
			{
				statusReportEvent?.Invoke("WindowsAABDecoder.Decode() Decode_Manifest start.");
				await Decode_Manifest();
				statusReportEvent?.Invoke("WindowsAABDecoder.Decode() Decode_AppName start.");
				await Decode_AppName();
				statusReportEvent?.Invoke("WindowsAPKDecoder.Decode_AppIconEntry() Decode_AppIconEntry start.");
				await Decode_AppIconEntry();
				statusReportEvent?.Invoke("WindowsAPKDecoder.Decode() Decode_Icon start.");
				await Decode_Icon();
				// statusReportEvent?.Invoke("WindowsAABDecoder.Decode() Decode_Signature start.");
				// await Decode_Signature();
			}
			statusReportEvent?.Invoke("WindowsAABDecoder.Decode() Decode_Hash start.");
			Decode_Hash();

			dataModel.Signature = LocalizationCenter.currentDataModel.Msg_AABNotSupport;
			dataModel.RawDumpSignature = LocalizationCenter.currentDataModel.Msg_AABNotSupport;

			statusReportEvent?.Invoke("WindowsAABDecoder.Decode() decode finish.");
			decodeFinishedEvent?.Invoke();
		}

		private async Task Decode_JavaTest()
		{
			string processResult = string.Empty;

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
				dataModel.RawDumpBadging = LocalizationCenter.currentDataModel.Msg_JavaNotFound;
				dataModel.RawDumpSignature = LocalizationCenter.currentDataModel.Msg_JavaNotFound;
			}

		}

		private async Task Decode_Manifest()
		{
			ProcessStartInfo psi = new ProcessStartInfo()
			{
				FileName = ExternalToolBinPath.GetBundleToolPath(),
				Arguments = "dump manifest --bundle=\"" + targetFilePath.OriginalString + "\""
			};
			statusReportEvent?.Invoke("WindowsAABDecoder.Decode_Manifest(), path=" + targetFilePath.OriginalString);
			string processResult = await ProcessExecuter.ExecuteProcess(psi, false, statusReportEvent);
			while (!processResult.StartsWith("<"))
			{
				if (string.IsNullOrEmpty(processResult))
					break;
				var lines = processResult.Split(new string[] { "\r\n" }, StringSplitOptions.None).Skip(1);
				processResult = string.Join(Environment.NewLine, lines.ToArray());
			}
			processResult = processResult.Trim();
			statusReportEvent?.Invoke("WindowsAABDecoder.Decode_Manifest(), result=" + processResult);

			dataModel.RawDumpBadging = processResult;
			statusReportEvent?.Invoke("WindowsAABDecoder.Decode_Manifest(), start read manifest");
			DesktopCMDAABUtil.ReadManifest(dataModel, dataModel.RawDumpBadging);
			statusReportEvent?.Invoke("WindowsAABDecoder.Decode_Manifest(), end read manifest");
		}

		private async Task Decode_AppName()
		{
			ProcessStartInfo psi = new ProcessStartInfo()
			{
				FileName = ExternalToolBinPath.GetBundleToolPath(),
				Arguments = "dump resources --bundle=\"" + targetFilePath.OriginalString
					+ "\" --resource=\"string/app_name\" --values=true"
			};
			statusReportEvent?.Invoke("WindowsAABDecoder.Decode_AppName(), path=" + targetFilePath.OriginalString);
			string processResult = await ProcessExecuter.ExecuteProcess(psi, false, statusReportEvent);
			while (!processResult.StartsWith("Package"))
			{
				if (string.IsNullOrEmpty(processResult))
					break;
				var lines = processResult.Split(new[] { "\r\n" }, StringSplitOptions.None).Skip(1);
				processResult = string.Join(Environment.NewLine, lines.ToArray());
			}
			processResult = processResult.Trim();
			statusReportEvent?.Invoke("WindowsAABDecoder.Decode_AppName(), result=" + processResult);

			DesktopCMDAABUtil.ReadAppName(dataModel, processResult);
		}

		private async Task Decode_AppIconEntry()
		{
			ProcessStartInfo psi = new ProcessStartInfo()
			{
				FileName = ExternalToolBinPath.GetBundleToolPath(),
				Arguments = "dump resources --bundle=\"" + targetFilePath.OriginalString
					+ "\" --resource=\"drawable/app_icon\" --values=true"
			};
			statusReportEvent?.Invoke("WindowsAABDecoder.Decode_AppIconEntry(), path=" + targetFilePath.OriginalString);
			string processResult = await ProcessExecuter.ExecuteProcess(psi, false, statusReportEvent);
			while (!processResult.StartsWith("Package"))
			{
				if (string.IsNullOrEmpty(processResult))
					break;
				var lines = processResult.Split(new[] { "\r\n" }, StringSplitOptions.None).Skip(1);
				processResult = string.Join(Environment.NewLine, lines.ToArray());
			}
			processResult = processResult.Trim();
			statusReportEvent?.Invoke("WindowsAABDecoder.Decode_AppIconEntry(), result=" + processResult);

			DesktopCMDAABUtil.ReadAppIconEntry(dataModel, processResult);
		}

		private async Task Decode_Icon()
		{
			dataModel.MaxIconContent = await FileUtil.ZipExtractData(targetFilePath, dataModel.MaxIconZipEntry);
		}

		// private async Task Decode_Signature()
		// {
		// 	string processResult;
		//
		// 	ProcessStartInfo psiAPKSigner = new ProcessStartInfo
		// 	{
		// 		FileName = "java.exe",
		// 		Arguments = "-jar " + ExternalToolBinPath.GetAPKSignerPath() +
		// 			" verify --verbose --print-certs" +
		// 			" \"" + targetFilePath.OriginalString + "\"",
		// 	};
		// 	processResult = await ProcessExecuter.ExecuteProcess(psiAPKSigner, false, statusReportEvent);
		//
		// 	dataModel.RawDumpSignature = processResult;
		// 	DesktopCMDAPKSignerUtil.ReadAPKSignature(dataModel, processResult);
		// }

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