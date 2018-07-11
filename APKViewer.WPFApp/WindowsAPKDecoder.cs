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

		public event Action decodeFinishedEvent;

		public WindowsAPKDecoder()
		{
			dataModel = null;
		}

		public void SetApkFilePath(Uri fileUri)
		{
			targetFilePath = fileUri;
		}

		public async Task Decode()
		{
			//C:\CLIProgram\Android\AndroidSDK\build-tools\27.0.0\aapt.exe
			//./aapt d badging "D:\Android\YahooWeatherProvider.apk"

			Console.WriteLine("WindowsAPKDecoder.Decode() decode start.");
			dataModel = new APKDataModel();

			await Decode_Badging();
			await Decode_Icon();
			await Decode_Signature();

			Console.WriteLine("WindowsAPKDecoder.Decode() decode finish.");
			decodeFinishedEvent?.Invoke();
		}

		private async Task Decode_Badging()
		{
			ProcessStartInfo psi = new ProcessStartInfo()
			{
				FileName = ExternalToolBinPath.GetAAPTPath(),
				Arguments = " d badging \"" + targetFilePath.OriginalString + "\""
			};
			string processResult = await ExecuteProcess(psi);

			dataModel.RawDumpBadging = processResult;
			DesktopCMDAAPTUtil.ReadBadging(dataModel, dataModel.RawDumpBadging);
		}

		private async Task Decode_Icon()
		{
			if (string.IsNullOrEmpty(dataModel.MaxIconZipEntry))
				return;
			using (ZipArchive za = ZipFile.Open(targetFilePath.OriginalString, ZipArchiveMode.Read))
			{
				Console.WriteLine("Feature Test try to get entry");

				ZipArchiveEntry iconEntry = za.GetEntry(dataModel.MaxIconZipEntry);
				Console.WriteLine(iconEntry.FullName);
				Console.WriteLine("Feature Test try entry got");

				using (Stream s = iconEntry.Open())
				using (MemoryStream ms = new MemoryStream())
				{
					Console.WriteLine("Feature Test ready to copy s");
					Task copyTask = s.CopyToAsync(ms);
					await copyTask;
					dataModel.MaxIconContent = ms.ToArray();
					Console.WriteLine("Feature Test copy finished");
				}
			}
		}

		private async Task Decode_Signature()
		{
			string processResult;

			// java -jar apksigner.jar verify --verbose --print-certs FDroid.apk
			// java -version

			ProcessStartInfo psiJavaVersion = new ProcessStartInfo()
			{
				FileName = "cmd.exe",
				Arguments = "/c java -version"
			};
			processResult = await ExecuteProcess(psiJavaVersion, 100, true);
			if (!processResult.StartsWith("java version"))
			{
				dataModel.Signature = "Java is not found, can't read apk signature.";
				return;
			}

			ProcessStartInfo psiAPKSigner = new ProcessStartInfo()
			{
				FileName = "java.exe",
				Arguments = "-jar " + ExternalToolBinPath.GETAPKSignerPath() +
					" verify --verbose --print-certs" +
					" \"" + targetFilePath.OriginalString + "\"",
			};
			processResult = await ExecuteProcess(psiAPKSigner);

			dataModel.Signature = processResult.ToString();
		}

		private async Task<string> ExecuteProcess(ProcessStartInfo startInfo, int timeout = 5000, bool useError = false)
		{
			string result = string.Empty;

			using (Process process = new Process())
			{
				process.StartInfo = startInfo;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardError = true;
				process.EnableRaisingEvents = true;
				Console.WriteLine("WindowsAPKDecoder.ExecuteProcess() setup: \r\n" + process.StartInfo.FileName + " " + process.StartInfo.Arguments);

				StringBuilder output = new StringBuilder();
				TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
				if (!useError)
				{
					process.OutputDataReceived +=
						(sender, e) =>
						{
							if (e.Data != null)
							{
								//Console.WriteLine("WindowsAPKDecoder.ExecuteProcess(): process.OutputDataReceived=\r\n" + e.Data);
								output.AppendLine(e.Data);
							}
						};
				}
				else
				{
					process.ErrorDataReceived +=
						(sender, e) =>
						{
							if (e.Data != null)
							{
								//Console.WriteLine("WindowsAPKDecoder.ExecuteProcess(): process.OutputDataReceived=\r\n" + e.Data);
								output.AppendLine(e.Data);
							}
						};
				}
				process.Exited +=
					(sender, e) =>
					{
						tcs.SetResult(output.ToString());
					};

				process.Start();
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();

				await tcs.Task;

				Console.WriteLine("WindowsAPKDecoder.ExecuteProcess() Final result=\r\n" + output.ToString());

				return output.ToString();
			}
		}

		public APKDataModel GetDataModel()
		{
			return dataModel;
		}

	}
}
