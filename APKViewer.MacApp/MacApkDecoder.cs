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
using APKViewer.Localize;
using APKViewer.Utility;


namespace APKViewer.MacApp
{
    public class MacApkDecoder : IFileDecoder
	{
		private Uri targetFilePath;
		private APKDataModel dataModel;

		public event Action decodeFinishedEvent;

		public MacApkDecoder()
		{
			dataModel = null;
		}

		public void SetFilePath(Uri fileUri)
		{
			targetFilePath = fileUri;
		}

		public async Task Decode()
		{
            Debug.WriteLine("MacApkDecoder.Decode() decode start.");
            dataModel = new APKDataModel();

            Debug.WriteLine("MacApkDecoder.Decode() Decode_Badging start.");
            await Decode_Badging();
            //Debug.WriteLine("MacApkDecoder.Decode() Decode_Icon start.");
            //await Decode_Icon();
            //Debug.WriteLine("MacApkDecoder.Decode() Decode_Signature start.");
            //await Decode_Signature();
            //Debug.WriteLine("MacApkDecoder.Decode() Decode_Hash start.");
            //Decode_Hash();

            Debug.WriteLine("MacApkDecoder.Decode() decode finish.");
            decodeFinishedEvent?.Invoke();
        }

		private async Task Decode_Badging()
		{
			ProcessStartInfo psi = new ProcessStartInfo()
			{
				//FileName = ExternalToolBinPath.GetAAPTPath(),
				FileName = ExternalToolBinPath.MY_AAPT,
				Arguments = " d badging \"" + targetFilePath.OriginalString + "\""
			};
            Debug.WriteLine("WindowsAPKDecoder.Decode_Badging(), path=" + targetFilePath.OriginalString);
            string processResult = await ProcessExecuter.ExecuteProcess(psi);

            dataModel.RawDumpBadging = processResult;
            DesktopCMDAAPTUtil.ReadBadging(dataModel, dataModel.RawDumpBadging);
        }

		//private async Task Decode_Icon()
		//{
		//	dataModel.MaxIconContent = await FileUtil.ZipExtractData(targetFilePath, dataModel.MaxIconZipEntry);
		//}

		//private async Task Decode_Signature()
		//{
		//	await AppUtility.TestJavaExist();

		//	if (!DesktopJavaUtil.javaExist)
		//	{
		//		dataModel.Signature = LocalizationCenter.currentDataModel.Msg_JavaNotFound_APKSignFail;
		//		return;
		//	}

		//	ProcessStartInfo psiAPKSigner = new ProcessStartInfo
		//	{
		//		FileName = "java.exe",
		//		Arguments = "-jar " + ExternalToolBinPath.GetAPKSignerPath() +
		//			" verify --verbose --print-certs" +
		//			" \"" + targetFilePath.OriginalString + "\"",
		//	};
		//	string processResult = await ProcessExecuter.ExecuteProcess(psiAPKSigner);

		//	dataModel.RawDumpSignature = processResult;
		//	DesktopCMDAPKSignerUtil.ReadAPKSignature(dataModel, processResult);
		//}

		private void Decode_Hash()
		{
			//FileUtil.CalculateSHA1(targetFilePath, dataModel);
		}

		public APKDataModel GetDataModel()
		{
			return dataModel;
		}
	}
}
