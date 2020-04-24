using System;
using System.Collections.Generic;
using System.Text;
using Claunia.PropertyList;

namespace APKViewer.Utility
{
	public static class PListCilUtil
	{
		private const string KEY_APPNAME = "CFBundleDisplayName";
		private const string KEY_PACKAGENAME = "CFBundleIdentifier";

		private const string KEY_VERSION_STRING = "CFBundleShortVersionString";
		private const string KEY_VERSION = "CFBundleVersion";

		private const string KEY_MINOS = "MinimumOSVersion";

		public static void ReadNSData(PackageDataModel targetModel, NSDictionary nsData)
		{
			targetModel.AppName = nsData.ObjectForKey(KEY_APPNAME)?.ToString();
			targetModel.VersionString = nsData.ObjectForKey(KEY_VERSION_STRING)?.ToString();
			targetModel.VersionCode = nsData.ObjectForKey(KEY_VERSION)?.ToString();
			targetModel.PackageName = nsData.ObjectForKey(KEY_PACKAGENAME)?.ToString();
		}
	}
}
