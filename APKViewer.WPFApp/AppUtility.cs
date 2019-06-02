using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace APKViewer.WPFApp
{
	public static class AppUtility
	{
		public static string GetCurrentExePath()
		{
			return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
		}

	}
}
