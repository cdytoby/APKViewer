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
		private const string KEY_TARGETOS = "DTPlatformVersion";
		private const string KEY_DEVICE_FAMILY = "UIDeviceFamily";
		private const string KEY_DEVICE_CAP = "UIRequiredDeviceCapabilities";

		private static readonly List<string> LIST_PERMISSION = new List<string>()
		{
			KEY_PERMISSION_CAMERA,
			KEY_PERMISSION_MICROPHONE,
			KEY_PERMISSION_BLUETOOTH_ALWAYS,
			KEY_PERMISSION_BLUETOOTH_PERI,
			KEY_PERMISSION_PHOTOLIB_READ,
			KEY_PERMISSION_PHOTOLIB_ADD
		};
		private const string KEY_PERMISSION_CAMERA = "NSCameraUsageDescription";
		private const string KEY_PERMISSION_MICROPHONE = "NSMicrophoneUsageDescription";
		private const string KEY_PERMISSION_BLUETOOTH_ALWAYS = "NSBluetoothAlwaysUsageDescription";
		private const string KEY_PERMISSION_BLUETOOTH_PERI = "NSBluetoothPeripheralUsageDescription";
		private const string KEY_PERMISSION_PHOTOLIB_READ = "NSPhotoLibraryUsageDescription";
		private const string KEY_PERMISSION_PHOTOLIB_ADD = "NSPhotoLibraryAddUsageDescription";

		public static void ReadNSData(PackageDataModel targetModel, NSDictionary nsData)
		{
			targetModel.AppName = nsData.ObjectForKey(KEY_APPNAME)?.ToString();
			targetModel.VersionString = nsData.ObjectForKey(KEY_VERSION_STRING)?.ToString();
			targetModel.VersionCode = nsData.ObjectForKey(KEY_VERSION)?.ToString();
			targetModel.PackageName = nsData.ObjectForKey(KEY_PACKAGENAME)?.ToString();

			//supports-screens <- UIDeviceFamily
			NSArray deviceFamilyArray = (NSArray)nsData.ObjectForKey(KEY_DEVICE_FAMILY);
			foreach (NSObject _obj in deviceFamilyArray)
			{
				int deviceFamilyValue = ((NSNumber)_obj).ToInt();
				switch (deviceFamilyValue)
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
			List<string> capList = new List<string>();
			NSObject capObj = nsData.ObjectForKey(KEY_DEVICE_CAP);
			if (capObj is NSArray)
			{
				NSArray deviceCapArray = (NSArray)capObj;
				List<NSObject> tempList = new List<NSObject>(deviceCapArray);
				capList.AddRange(tempList.ConvertAll(x => x.ToString()));
			}
			else if (capObj is NSDictionary)
			{
				NSDictionary deviceCapDict = (NSDictionary)capObj;
				capList.AddRange(deviceCapDict.Keys);
			}

			foreach (string objStr in capList)
			{
				if (objStr.Contains("arm"))
				{
					switch (objStr)
					{
						case "armv7":
							targetModel.Architecture.Add("armv7");
							break;
						case "arm64":
							targetModel.Architecture.Add("arm64");
							break;
					}
				}
				else if (objStr.Contains("opengl"))
				{
					targetModel.OpenGLVersion += objStr;
				}
				else
				{
					targetModel.Feature_Require.Add(objStr);
				}
			}


			//architecture

			//sdk
			targetModel.MinSDKCode = nsData.ObjectForKey(KEY_MINOS)?.ToString();
			targetModel.MaxSDKCode = nsData.ObjectForKey(KEY_TARGETOS)?.ToString();

			//dpi

			//permission
			foreach (string per in LIST_PERMISSION)
			{
				if (nsData.ContainsKey(per))
				{
					targetModel.Permissions.Add(per);
					targetModel.Permissions.Add("- " + nsData.ObjectForKey(per));
				}
			}

			//feature
		}
	}
}