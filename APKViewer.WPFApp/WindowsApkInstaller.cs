using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace APKViewer.WPFApp
{
	public class WindowsApkInstaller: IApkInstaller
	{
		public event Action<bool, string> installFinishedEvent;

		public async Task InstallApk(Uri fileUri)
		{
			ProcessStartInfo psi = new ProcessStartInfo()
			{
				FileName = ExternalToolBinPath.GetADBPath(),
				Arguments = " install -r \"" + fileUri.OriginalString + "\""
			};
			string processResult = await ProcessExecuter.ExecuteProcess(psi, true);

			string[] resultArray = processResult?.Split(
				new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

			if (resultArray != null)
				processResult = resultArray[resultArray.Length - 1];

			installFinishedEvent?.Invoke(true, processResult);
		}
	}
}