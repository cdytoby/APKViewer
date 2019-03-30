using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace APKViewer.WPFApp
{
	public class WindowsApkInstaller:IApkInstaller
	{
		public event Action<bool, string> installFinishedEvent;

		public async Task InstallApk(Uri fileUri)
		{
			ProcessStartInfo psi = new ProcessStartInfo()
			{
				FileName = ExternalToolBinPath.GetADBPath(),
				Arguments = " install " +fileUri.OriginalString
			};
			string processResult = await ProcessExecuter.ExecuteProcess(psi, false);

			installFinishedEvent?.Invoke(true, processResult);
		}
	}
}