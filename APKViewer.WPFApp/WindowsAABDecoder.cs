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
			dataModel.FileExtension = StringConstant.FileExtension_AAB;

			statusReportEvent?.Invoke("WindowsAABDecoder.Decode() start test java.");
			await Decode_JavaTest();

			if (AppUtility.javaTested && AppUtility.javaExist)
			{
				statusReportEvent?.Invoke("WindowsAABDecoder.Decode() Decode_Manifest start.");
				await Decode_Manifest();
				// statusReportEvent?.Invoke("WindowsAPKDecoder.Decode() Decode_Icon start.");
				// await Decode_Icon();
				// statusReportEvent?.Invoke("WindowsAABDecoder.Decode() Decode_Signature start.");
				// await Decode_Signature();
			}
			statusReportEvent?.Invoke("WindowsAABDecoder.Decode() Decode_Hash start.");
			Decode_Hash();

			statusReportEvent?.Invoke("WindowsAABDecoder.Decode() decode finish.");
			decodeFinishedEvent?.Invoke();
		}

		private async Task Decode_JavaTest()
		{
			string processResult = string.Empty;

			if (!AppUtility.javaTested)
			{
				// java -jar apksigner.jar verify --verbose --print-certs FDroid.apk
				// java -version

				ProcessStartInfo psiJavaVersion = new ProcessStartInfo()
				{
					FileName = "cmd.exe",
					Arguments = "/c java -version"
				};
				processResult = await ProcessExecuter.ExecuteProcess(psiJavaVersion, true, statusReportEvent);
				AppUtility.javaExist = AppUtility.JavaExist(processResult);
				AppUtility.javaTested = true;
			}

			if (!AppUtility.javaExist)
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
				var lines = processResult.Split(new string[]{"\r\n"}, StringSplitOptions.None).Skip(1);
				processResult = string.Join(Environment.NewLine, lines.ToArray());
			}
			processResult = processResult.Trim();
			statusReportEvent?.Invoke("WindowsAABDecoder.Decode_Manifest(), result=" + processResult);

			dataModel.RawDumpBadging = processResult;
			statusReportEvent?.Invoke("WindowsAABDecoder.Decode_Manifest(), start read manifest");
			DesktopCMDAABUtil.ReadManifest(dataModel, dataModel.RawDumpBadging);
			statusReportEvent?.Invoke("WindowsAABDecoder.Decode_Manifest(), end read manifest");
		}

		// private async Task Decode_Icon()
		// {
		// 	if (string.IsNullOrEmpty(dataModel.MaxIconZipEntry))
		// 		return;
		// 	using (ZipArchive za = ZipFile.Open(targetFilePath.OriginalString, ZipArchiveMode.Read))
		// 	{
		// 		ZipArchiveEntry iconEntry = null;
		// 		try
		// 		{
		// 			iconEntry = za.GetEntry(dataModel.MaxIconZipEntry);
		// 		}
		// 		catch (Exception)
		// 		{
		// 			//do nothing
		// 		}
		// 		Debug.WriteLine("WindowsAPKDecoder.Decode_Icon() zip entry get. " + iconEntry.FullName);
		//
		// 		if (iconEntry != null)
		// 		{
		// 			using (Stream s = iconEntry.Open())
		// 			using (MemoryStream ms = new MemoryStream())
		// 			{
		// 				Task copyTask = s.CopyToAsync(ms);
		// 				await copyTask;
		// 				dataModel.MaxIconContent = ms.ToArray();
		// 			}
		// 		}
		// 	}
		// }

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
			FileHashCalcUtil.CalculateSHA1(targetFilePath, dataModel);
		}

		public APKDataModel GetDataModel()
		{
			return dataModel;
		}

	}
}