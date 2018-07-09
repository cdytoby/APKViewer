using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
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
			int timeout = 5000;

			// https://stackoverflow.com/questions/139593/processstartinfo-hanging-on-waitforexit-why
			using (Process process = new Process())
			{
				process.StartInfo.FileName = ExternalToolBinPath.AAPT;
				process.StartInfo.Arguments = " d badging \"" + targetFilePath.OriginalString + "\"";
				Console.WriteLine("WindowsAPKDecoder.Decode_Badging()\r\n" + process.StartInfo.FileName + process.StartInfo.Arguments);
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardError = true;

				StringBuilder output = new StringBuilder();
				StringBuilder error = new StringBuilder();

				using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
				using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
				{
					process.OutputDataReceived += (sender, e) =>
					{
						if (e.Data == null)
						{
							outputWaitHandle.Set();
						}
						else
						{
							output.AppendLine(e.Data);
						}
					};
					process.ErrorDataReceived += (sender, e) =>
					{
						if (e.Data == null)
						{
							errorWaitHandle.Set();
						}
						else
						{
							error.AppendLine(e.Data);
						}
					};

					process.Start();

					process.BeginOutputReadLine();
					process.BeginErrorReadLine();

					if (process.WaitForExit(timeout) &&
						outputWaitHandle.WaitOne(timeout) &&
						errorWaitHandle.WaitOne(timeout))
					{
						dataModel.RawDumpBadging = output.ToString();
						DesktopCMDAAPTUtil.ReadBadging(dataModel, dataModel.RawDumpBadging);
					}
					else
					{
						// Timed out.
					}
				}
			}
		}

		private void Decode_Icon()
		{
			if (string.IsNullOrEmpty(dataModel.maxIconZipEntry))
				return;
			using (ZipArchive za = ZipFile.Open(targetFilePath.OriginalString, ZipArchiveMode.Read))
			{
				Console.WriteLine("Feature Test try to get entry");
				try
				{
					ZipArchiveEntry iconEntry = za.GetEntry(dataModel.maxIconZipEntry);
					Console.WriteLine(iconEntry.FullName);
					Console.WriteLine("Feature Test try entry got");

					using (Stream s = iconEntry.Open())
					using (MemoryStream ms = new MemoryStream())
					{
						Console.WriteLine("Feature Test ready to copy s");
						s.CopyTo(ms);
						dataModel.maxIconContent = ms.ToArray();
						Console.WriteLine("Feature Test copy finished");
					}
				}
				catch (Exception ee)
				{ }
			}
		}

		public APKDataModel GetDataModel()
		{
			return dataModel;
		}

	}
}
