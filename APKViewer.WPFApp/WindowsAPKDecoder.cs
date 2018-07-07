using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APKViewer.WPFApp
{
	public class WindowsAPKDecoder : IApkDecoder
	{
		private Uri targetFilePath;
		private APKDataModel dataModel;

		public event Action decodeFinished;

		public WindowsAPKDecoder()
		{
			dataModel = null;
		}

		public void SetApkFilePath(Uri fileUri)
		{
			targetFilePath = fileUri;
		}

		public void Decode()
		{
			//./aapt d badging "D:\Android\YahooWeatherProvider.apk"

			Console.WriteLine("Decode Test start.");
			Process cmd = new Process();
			cmd.StartInfo.FileName = ExternalToolBinPath.AAPT;
			cmd.StartInfo.Arguments = "d badging " + targetFilePath.AbsolutePath;
			cmd.StartInfo.RedirectStandardInput = true;
			cmd.StartInfo.RedirectStandardOutput = true;
			cmd.StartInfo.CreateNoWindow = true;
			cmd.StartInfo.UseShellExecute = false;
			Console.WriteLine("Decode Test start 2.");
			cmd.Start();
			cmd.WaitForExit();
			Console.WriteLine("Decode Test start 3.");
			dataModel = new APKDataModel();
			dataModel.rawDumpBadging = cmd.StandardOutput.ReadToEnd();
			Console.WriteLine("Decode Test finished. result="+dataModel.rawDumpBadging);

			decodeFinished?.Invoke();
		}

		public APKDataModel GetDataModel()
		{
			return dataModel;
		}

	}
}
