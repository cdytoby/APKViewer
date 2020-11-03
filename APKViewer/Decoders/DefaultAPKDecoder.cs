
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using APKViewer.Localize;
using APKViewer.Utility;

namespace APKViewer.Decoders
{
	public class DefaultAPKDecoder: IFileDecoder
	{
		private ICmdPathProvider pathProvider;
		private Uri targetFilePath;
		private PackageDataModel dataModel;

		public event Action decodeProgressCallbackEvent;

		public DefaultAPKDecoder(ICmdPathProvider newPathProvider)
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
			Debug.WriteLine("DefaultAPKDecoder.Decode() decode start.");
			dataModel = new PackageDataModel();

			Debug.WriteLine("DefaultAPKDecoder.Decode() Decode_Badging start.");
			await Decode_Badging();
			decodeProgressCallbackEvent?.Invoke();
			Debug.WriteLine("DefaultAPKDecoder.Decode() Decode_Icon start.");
			await Decode_Icon();
			decodeProgressCallbackEvent?.Invoke();
			Debug.WriteLine("DefaultAPKDecoder.Decode() Decode_Signature start.");
			await Decode_Signature();
			decodeProgressCallbackEvent?.Invoke();
			Debug.WriteLine("DefaultAPKDecoder.Decode() Decode_Hash start.");
			Decode_Hash();

			Debug.WriteLine("DefaultAPKDecoder.Decode() decode finish.");
			decodeProgressCallbackEvent?.Invoke();
		}

		private async Task Decode_Badging()
		{
			ProcessStartInfo psi = new ProcessStartInfo()
			{
				FileName = pathProvider.GetAAPTPath(),
				Arguments = " d badging \"" + targetFilePath.OriginalString + "\""
			};
			Debug.WriteLine("DefaultAPKDecoder.Decode_Badging(), path=" + targetFilePath.OriginalString);
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
			await DesktopJavaUtil.TestJavaExist();

			if (!DesktopJavaUtil.javaExist)
			{
				dataModel.Signature = LocalizationCenter.currentDataModel.Msg_JavaNotFound_APKSignFail;
				return;
			}

			ProcessStartInfo psiAPKSigner = new ProcessStartInfo
			{
				FileName = "java",
				Arguments = "-jar " + pathProvider.GetAPKSignerPath() +
					" verify --verbose --print-certs" +
					" \"" + targetFilePath.OriginalString + "\"",
			};
			string processResult = await ProcessExecuter.ExecuteProcess(psiAPKSigner, false, false);

			dataModel.RawDumpSignature = processResult;
			DesktopCMDAPKSignerUtil.ReadAPKSignature(dataModel, processResult);
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