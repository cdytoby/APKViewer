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
		private const string KEY_DEVICE_FAMILY = "UIDeviceFamily";
		private const string KEY_DEVICE_CAP = "UIRequiredDeviceCapabilities";

		public static void ReadNSData(PackageDataModel targetModel, NSDictionary nsData)
		{
			targetModel.AppName = nsData.ObjectForKey(KEY_APPNAME)?.ToString();
			targetModel.VersionString = nsData.ObjectForKey(KEY_VERSION_STRING)?.ToString();
			targetModel.VersionCode = nsData.ObjectForKey(KEY_VERSION)?.ToString();
			targetModel.PackageName = nsData.ObjectForKey(KEY_PACKAGENAME)?.ToString();

			//supports-screens <- UIDeviceFamily
			NSArray deviceFamilyArray = (NSArray)nsData.ObjectForKey(KEY_DEVICE_FAMILY);
			foreach(NSObject _obj in deviceFamilyArray)
			{
				int deviceFamilyValue = ((NSNumber)_obj).ToInt();
				switch(deviceFamilyValue)
				{
					case 1:
						targetModel.ScreenSize.Add("iPhone");
						break;
					case 2:
						targetModel.ScreenSize.Add("iPad");
						break;
					case 3:
						targetModel.ScreenSize.Add("AppleTV");
						break;
					case 4:
						targetModel.ScreenSize.Add("AppleWatch");
						break;
				}
			}

			//Device Capability
			//https://developer.apple.com/documentation/bundleresources/information_property_list/uirequireddevicecapabilities?language=objc
			// some ipa use dictionary for this
			NSArray deviceCapArray = (NSArray)nsData.ObjectForKey(KEY_DEVICE_CAP);
			foreach(NSObject _obj in deviceCapArray)
			{
				string objStr = _obj.ToString();
				switch(objStr)
				{
					case "armv7":
						targetModel.Architecture.Add("armv7");
						break;
					case "arm64":
						targetModel.Architecture.Add("arm64");
						break;
				}
			}


			//architecture


			//sdk
			//dpi
			//architecture
			//permission
			//feature
		}
	}
}
