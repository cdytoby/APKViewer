using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using APKViewer.Localize;
using APKViewer.Utility;

namespace APKViewer.Decoders
{
	public class DefaultAABDecoder: IFileDecoder
	{
		private ICmdPathProvider pathProvider;
		private Uri targetFilePath;
		private PackageDataModel dataModel;

		public event Action decodeProgressCallbackEvent;

		public DefaultAABDecoder(ICmdPathProvider newPathProvider)
		{
			dataModel = null;
			pathProvider = newPathProvider;
		}

		public void SetFilePath(Uri fileUri)
		{
			targetFilePath = fileUri;
		}

		public async Task Decode()
		{
			Debug.WriteLine("DefaultAABDecoder.Decode() decode start.");
			dataModel = new PackageDataModel();

			Debug.WriteLine("DefaultAABDecoder.Decode() start test java.");
			await Decode_JavaTest();

			if (DesktopJavaUtil.javaTested && DesktopJavaUtil.javaExist)
			{
				Debug.WriteLine("DefaultAABDecoder.Decode() Decode_Manifest start.");
				await Decode_Manifest();
				Debug.WriteLine("DefaultAABDecoder.Decode() Decode_AppName start.");
				await Decode_AppName();
				Debug.WriteLine("DefaultAABDecoder.Decode_AppIconEntry() Decode_AppIconEntry start.");
				await Decode_AppIconEntry();
				Debug.WriteLine("DefaultAABDecoder.Decode() Decode_Icon start.");
				await Decode_Icon();
				// statusReportEvent?.Invoke("WindowsAABDecoder.Decode() Decode_Signature start.");
				// await Decode_Signature();
			}
			Debug.WriteLine("DefaultAABDecoder.Decode() Decode_Hash start.");
			Decode_Hash();

			dataModel.Signature = LocalizationCenter.currentDataModel.Msg_AABNotSupport;
			dataModel.RawDumpSignature = LocalizationCenter.currentDataModel.Msg_AABNotSupport;

			Debug.WriteLine("DefaultAABDecoder.Decode() decode finish.");
			decodeProgressCallbackEvent?.Invoke();
		}

		private async Task Decode_JavaTest()
		{
			await DesktopJavaUtil.TestJavaExist();

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
				FileName = pathProvider.GetBundleToolPath(),
				Arguments = "dump manifest --bundle=\"" + targetFilePath.OriginalString + "\""
			};
			Debug.WriteLine("DefaultAABDecoder.Decode_Manifest(), path=" + targetFilePath.OriginalString);
			string processResult = await ProcessExecuter.ExecuteProcess(psi);
			while (!processResult.StartsWith("<"))
			{
				if (string.IsNullOrEmpty(processResult))
					break;
				var lines = processResult.Split(new string[] { "\r\n" }, StringSplitOptions.None).Skip(1);
				processResult = string.Join(Environment.NewLine, lines.ToArray());
			}
			processResult = processResult.Trim();
			Debug.WriteLine("DefaultAABDecoder.Decode_Manifest(), result=" + processResult);

			dataModel.RawDumpBadging = processResult;
			Debug.WriteLine("DefaultAABDecoder.Decode_Manifest(), start read manifest");
			DesktopCMDAABUtil.ReadManifest(dataModel, dataModel.RawDumpBadging);
			Debug.WriteLine("DefaultAABDecoder.Decode_Manifest(), end read manifest");
		}

		private async Task Decode_AppName()
		{
			ProcessStartInfo psi = new ProcessStartInfo()
			{
				FileName = pathProvider.GetBundleToolPath(),
				Arguments = "dump resources --bundle=\"" + targetFilePath.OriginalString
					+ "\" --resource=\"" + dataModel.AppNameResourceEntry + "\" --values=true"
			};
			Debug.WriteLine("DefaultAABDecoder.Decode_AppName(), path=" + targetFilePath.OriginalString);
			string processResult = await ProcessExecuter.ExecuteProcess(psi);
			while (!processResult.StartsWith("Package"))
			{
				if (string.IsNullOrEmpty(processResult))
					break;
				var lines = processResult.Split(new[] { "\r\n" }, StringSplitOptions.None).Skip(1);
				processResult = string.Join(Environment.NewLine, lines.ToArray());
			}
			processResult = processResult.Trim();
			Debug.WriteLine("DefaultAABDecoder.Decode_AppName(), result=" + processResult);

			DesktopCMDAABUtil.ReadAppName(dataModel, processResult);
		}

		private async Task Decode_AppIconEntry()
		{
			ProcessStartInfo psi = new ProcessStartInfo()
			{
				FileName = pathProvider.GetBundleToolPath(),
				Arguments = "dump resources --bundle=\"" + targetFilePath.OriginalString
					+ "\" --resource=\"" + dataModel.AppIconResourceEntry + "\" --values=true"
			};
			Debug.WriteLine("DefaultAABDecoder.Decode_AppIconEntry(), path=" + targetFilePath.OriginalString);
			string processResult = await ProcessExecuter.ExecuteProcess(psi);
			while (!processResult.StartsWith("Package"))
			{
				if (string.IsNullOrEmpty(processResult))
					break;
				var lines = processResult.Split(new[] { "\r\n" }, StringSplitOptions.None).Skip(1);
				processResult = string.Join(Environment.NewLine, lines.ToArray());
			}
			processResult = processResult.Trim();
			Debug.WriteLine("DefaultAABDecoder.Decode_AppIconEntry(), result=" + processResult);

			DesktopCMDAABUtil.ReadAppIconEntry(dataModel, processResult);
		}

		private async Task Decode_Icon()
		{
			dataModel.MaxIconContent = await FileUtil.ZipExtractData(targetFilePath, dataModel.MaxIconZipEntry);
		}

		private void Decode_Hash()
		{
			FileUtil.CalculateSHA1(targetFilePath, dataModel);
		}

		public PackageDataModel GetDataModel()
		{
			return dataModel;
		}


	}
}