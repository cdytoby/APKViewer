using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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
			//C:\CLIProgram\Android\AndroidSDK\build-tools\27.0.0\aapt.exe
			//./aapt d badging "D:\Android\YahooWeatherProvider.apk"

			Console.WriteLine("WindowsAPKDecoder.Decode() decode start.");
			dataModel = new APKDataModel();

			Decode_Badging();
			Decode_Icon();

			Console.WriteLine("WindowsAPKDecoder.Decode() decode finish.");
			decodeFinished?.Invoke();
		}

		private void Decode_Badging()
		{
			Process cmd = new Process();
			cmd.StartInfo.FileName = ExternalToolBinPath.AAPT;
			cmd.StartInfo.Arguments = "d badging " + targetFilePath.AbsolutePath;
			cmd.StartInfo.RedirectStandardInput = true;
			cmd.StartInfo.RedirectStandardOutput = true;
			cmd.StartInfo.CreateNoWindow = true;
			cmd.StartInfo.UseShellExecute = false;
			cmd.Start();
			cmd.WaitForExit(200);
			dataModel.RawDumpBadging = cmd.StandardOutput.ReadToEnd();
			DesktopCMDAAPTUtil.ReadBadging(dataModel, dataModel.RawDumpBadging);

			if (!cmd.HasExited)
				cmd.Kill();
		}

		private void Decode_Icon()
		{
			if (string.IsNullOrEmpty(dataModel.maxIconZipEntry))
				return;
			using (ZipArchive za = ZipFile.Open(targetFilePath.AbsolutePath, ZipArchiveMode.Read))
			{
				ZipArchiveEntry iconEntry = za.GetEntry(dataModel.maxIconZipEntry);
				Console.WriteLine(iconEntry.FullName);

				using (Stream s = iconEntry.Open())
				using (MemoryStream ms = new MemoryStream())
				{
					s.CopyTo(ms);
					dataModel.maxIconContent = ms.ToArray();
				}
			}
		}

		public APKDataModel GetDataModel()
		{
			return dataModel;
		}

	}
}
