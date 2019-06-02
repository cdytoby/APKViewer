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

		public WindowsAABDecoder()
		{
			dataModel = null;
		}

		public void SetFilePath(Uri fileUri)
		{
			targetFilePath = fileUri;
		}

		public async Task Decode()
		{
			Debug.WriteLine("WindowsAABDecoder.Decode() decode start.");
			dataModel = new APKDataModel();

			Debug.WriteLine("WindowsAABDecoder.Decode() start test java.");
			await Decode_JavaTest();

			if (DesktopJavaUtil.javaTested && DesktopJavaUtil.javaExist)
			{
				Debug.WriteLine("WindowsAABDecoder.Decode() Decode_Manifest start.");
				await Decode_Manifest();
				Debug.WriteLine("WindowsAABDecoder.Decode() Decode_AppName start.");
				await Decode_AppName();
				Debug.WriteLine("WindowsAPKDecoder.Decode_AppIconEntry() Decode_AppIconEntry start.");
				await Decode_AppIconEntry();
				Debug.WriteLine("WindowsAPKDecoder.Decode() Decode_Icon start.");
				await Decode_Icon();
				// statusReportEvent?.Invoke("WindowsAABDecoder.Decode() Decode_Signature start.");
				// await Decode_Signature();
			}
			Debug.WriteLine("WindowsAABDecoder.Decode() Decode_Hash start.");
			Decode_Hash();

			dataModel.Signature = LocalizationCenter.currentDataModel.Msg_AABNotSupport;
			dataModel.RawDumpSignature = LocalizationCenter.currentDataModel.Msg_AABNotSupport;

			Debug.WriteLine("WindowsAABDecoder.Decode() decode finish.");
			decodeFinishedEvent?.Invoke();
		}

		private async Task Decode_JavaTest()
		{
			await AppUtility.TestJavaExist();

			if (!DesktopJavaUtil.javaExist)
			{
				dataModel.AppName = LocalizationCenter.currentDataModel.Msg_JavaNotFound_AABFail;
				dataModel.RawDumpBadging = LocalizationCenter.currentDataModel.Msg_JavaNotFound_AABFail;
			}

		}

		private async Task Decode_Manifest()
		{
			ProcessStartInfo psi = new ProcessStartInfo()
			{
				FileName = ExternalToolBinPath.GetBundleToolPath(),
				Arguments = "dump manifest --bundle=\"" + targetFilePath.OriginalString + "\""
			};
			Debug.WriteLine("WindowsAABDecoder.Decode_Manifest(), path=" + targetFilePath.OriginalString);
			string processResult = await ProcessExecuter.ExecuteProcess(psi);
			while (!processResult.StartsWith("<"))
			{
				if (string.IsNullOrEmpty(processResult))
					break;
				var lines = processResult.Split(new string[] { "\r\n" }, StringSplitOptions.None).Skip(1);
				processResult = string.Join(Environment.NewLine, lines.ToArray());
			}
			processResult = processResult.Trim();
			Debug.WriteLine("WindowsAABDecoder.Decode_Manifest(), result=" + processResult);

			dataModel.RawDumpBadging = processResult;
			Debug.WriteLine("WindowsAABDecoder.Decode_Manifest(), start read manifest");
			DesktopCMDAABUtil.ReadManifest(dataModel, dataModel.RawDumpBadging);
			Debug.WriteLine("WindowsAABDecoder.Decode_Manifest(), end read manifest");
		}

		private async Task Decode_AppName()
		{
			ProcessStartInfo psi = new ProcessStartInfo()
			{
				FileName = ExternalToolBinPath.GetBundleToolPath(),
				Arguments = "dump resources --bundle=\"" + targetFilePath.OriginalString
					+ "\" --resource=\"" + dataModel.AppNameResourceEntry + "\" --values=true"
			};
			Debug.WriteLine("WindowsAABDecoder.Decode_AppName(), path=" + targetFilePath.OriginalString);
			string processResult = await ProcessExecuter.ExecuteProcess(psi);
			while (!processResult.StartsWith("Package"))
			{
				if (string.IsNullOrEmpty(processResult))
					break;
				var lines = processResult.Split(new[] { "\r\n" }, StringSplitOptions.None).Skip(1);
				processResult = string.Join(Environment.NewLine, lines.ToArray());
			}
			processResult = processResult.Trim();
			Debug.WriteLine("WindowsAABDecoder.Decode_AppName(), result=" + processResult);

			DesktopCMDAABUtil.ReadAppName(dataModel, processResult);
		}

		private async Task Decode_AppIconEntry()
		{
			ProcessStartInfo psi = new ProcessStartInfo()
			{
				FileName = ExternalToolBinPath.GetBundleToolPath(),
				Arguments = "dump resources --bundle=\"" + targetFilePath.OriginalString
					+ "\" --resource=\"" + dataModel.AppIconResourceEntry + "\" --values=true"
			};
			Debug.WriteLine("WindowsAABDecoder.Decode_AppIconEntry(), path=" + targetFilePath.OriginalString);
			string processResult = await ProcessExecuter.ExecuteProcess(psi);
			while (!processResult.StartsWith("Package"))
			{
				if (string.IsNullOrEmpty(processResult))
					break;
				var lines = processResult.Split(new[] { "\r\n" }, StringSplitOptions.None).Skip(1);
				processResult = string.Join(Environment.NewLine, lines.ToArray());
			}
			processResult = processResult.Trim();
			Debug.WriteLine("WindowsAABDecoder.Decode_AppIconEntry(), result=" + processResult);

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