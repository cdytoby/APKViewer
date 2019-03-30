using System;
using System.Threading.Tasks;

namespace APKViewer
{
	public interface IApkInstaller
	{
		event Action<bool, string> installFinishedEvent;

		Task InstallApk(Uri fileUri);
	}
}