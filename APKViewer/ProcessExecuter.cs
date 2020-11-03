using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace APKViewer
{
	public static class ProcessExecuter
	{
		public static async Task<string> ExecuteProcess(ProcessStartInfo startInfo,
			bool useBothErrorAndNormal = false, bool useUTF8 = true)
		{
			// string result = string.Empty;

			using (Process process = new Process())
			{
				//CultureInfo.CurrentCulture.TextInfo.OEMCodePage
				if (useUTF8)
				{
					startInfo.StandardOutputEncoding = Encoding.UTF8;
					startInfo.StandardErrorEncoding = Encoding.UTF8;
				}

				process.StartInfo = startInfo;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardError = true;
				process.EnableRaisingEvents = true;

				Debug.WriteLine("ProcessExecuter.ExecuteProcess() setup: \r\n" +
					process.StartInfo.FileName + " " + process.StartInfo.Arguments);

				StringBuilder output = new StringBuilder();
				TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

				process.OutputDataReceived +=
					(sender, e) =>
					{
						if (e.Data != null)
						{
							//Console.WriteLine("ProcessExecuter.ExecuteProcess(): process.OutputDataReceived=\r\n" + e.Data);
							output.AppendLine(e.Data);
						}
					};

				if (useBothErrorAndNormal)
				{
					process.ErrorDataReceived +=
						(sender, e) =>
						{
							if (e.Data != null)
							{
								//Console.WriteLine("ProcessExecuter.ExecuteProcess(): process.OutputDataReceived=\r\n" + e.Data);
								output.AppendLine(e.Data);
							}
						};
				}
				process.Exited +=
					(sender, e) => { tcs.SetResult(output.ToString()); };

				process.Start();
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();

				Debug.WriteLine("ProcessExecuter.ExecuteProcess() process started.");

				await tcs.Task;
				await Task.Delay(50);

				Debug.WriteLine("ProcessExecuter.ExecuteProcess() Final result=\r\n" + output.ToString());
				Debug.WriteLine("ProcessExecuter.ExecuteProcess() finish.");

				return output.ToString();
			}
		}
	}
}