using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using APKViewer.Localize;
using APKViewer.Utility;

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
