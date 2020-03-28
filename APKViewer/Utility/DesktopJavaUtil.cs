using System.Diagnostics;
using System.Threading.Tasks;

namespace APKViewer.Utility
{
	public static class DesktopJavaUtil
	{
		public static bool javaTested { get; private set; }
		public static bool javaExist { get; private set; }

		private static bool JavaExist(string javaVersionResult)
		{
			return javaVersionResult.Contains("java version") || javaVersionResult.Contains("jdk version");
		}

		public static void SetJavaExist(string javaVersionResult)
		{
			javaExist = JavaExist(javaVersionResult);
			javaTested = true;
		}

		public static async Task TestJavaExist()
		{
			string processResult = string.Empty;

			if (!javaTested)
			{
				// java -jar apksigner.jar verify --verbose --print-certs FDroid.apk
				// java -version

				ProcessStartInfo psiJavaVersion = new ProcessStartInfo()
				{
					FileName = "cmd.exe",
					Arguments = "/c java -version"
				};
				processResult = await ProcessExecuter.ExecuteProcess(psiJavaVersion, true);
				SetJavaExist(processResult);
			}
		}
	}
}