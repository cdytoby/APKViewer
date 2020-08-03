using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace APKViewer.Utility
{
	public static class DesktopCMDAABUtil
	{
		private const string KEY_MANIFEST = "manifest";
		private const string KEY_SDK = "uses-sdk";
		private const string KEY_SCREENSIZE = "supports-screens";
		private const string KEY_FEATURE = "uses-feature";
		private const string KEY_PERMISSION = "uses-permission";
		private const string KEY_APPLICATION = "application";
		private const string KEY_RECEIVER = "receiver";
		private const string KEY_SERVICE = "service";

		private const string KEY_ATTRIBUTE_NAME = "android:name";
		private const string KEY_ATTRIBUTE_VALUE = "android:value";
		private const string KEY_ATTRIBUTE_MANIFEST_VERSIONCODE = "android:versionCode";
		private const string KEY_ATTRIBUTE_MANIFEST_VERSIONNAME = "android:versionName";
		private const string KEY_ATTRIBUTE_MANIFEST_PACKAGE = "package";
		private const string KEY_ATTRIBUTE_SDK_MIN = "android:minSdkVersion";
		private const string KEY_ATTRIBUTE_SDK_MAX = "android:targetSdkVersion";
		private const string KEY_ATTRIBUTE_FEATURE_REQUIRED = "android:required";
		private const string KEY_ATTRIBUTE_OPENGLVERSION = "android:glEsVersion";
		private const string KEY_ATTRIBUTE_APPLICATION_ICON = "android:icon";
		private const string KEY_ATTRIBUTE_APPLICATION_LABEL = "android:label";

		private const string KEY_DENSITY = "densities";
		private const string KEY_DENSITY_ANY = "supports-any-density";

		public static void ReadManifest(PackageDataModel targetModel, string manifestResult)
		{
			if (targetModel == null)
				targetModel = new PackageDataModel();

			try
			{
				XmlDocument targetDoc = new XmlDocument();
				targetDoc.LoadXml(manifestResult);

				ProcessNode(targetModel, targetDoc);
			}
			catch (Exception e)
			{
				Console.WriteLine("DesktopCMDAABUtil.ReadManifest() failed!");
			}
		}

		private static void ProcessNode(PackageDataModel targetModel, XmlNode currentNode)
		{
			Console.WriteLine("Node name:  " + currentNode.Name);
			switch (currentNode.Name)
			{
				case KEY_MANIFEST:
				case KEY_SDK:
					ProcessAttribute_Default(targetModel, currentNode.Attributes);
					break;
				case KEY_SCREENSIZE:
					ProcessAttribute_Screens(targetModel, currentNode.Attributes);
					break;
				case KEY_FEATURE:
					ProcessAttribute_Feature(targetModel, currentNode.Attributes);
					break;
				case KEY_PERMISSION:
					ProcessAttribute_Permission(targetModel, currentNode.Attributes);
					break;
				case KEY_APPLICATION:
					ProcessAttribute_Application(targetModel, currentNode.Attributes);
					break;
				default:
					break;
			}

			if (currentNode.HasChildNodes)
			{
				XmlNodeList nodeList = currentNode.ChildNodes;
				foreach (XmlNode node in nodeList)
				{
					ProcessNode(targetModel, node);
				}
			}
		}

		private static void ProcessAttribute_Default(PackageDataModel targetModel,
			XmlAttributeCollection attributeCollection)
		{
			if (attributeCollection == null)
				return;

			foreach (XmlAttribute currentAttribute in attributeCollection)
			{
				Console.WriteLine("Node attribute: " + currentAttribute.Name + " " + currentAttribute.Value);
				switch (currentAttribute.Name)
				{
					case KEY_ATTRIBUTE_MANIFEST_PACKAGE:
						targetModel.PackageName = currentAttribute.Value;
						break;
					case KEY_ATTRIBUTE_MANIFEST_VERSIONNAME:
						targetModel.VersionString = currentAttribute.Value;
						break;
					case KEY_ATTRIBUTE_MANIFEST_VERSIONCODE:
						targetModel.VersionCode = currentAttribute.Value;
						break;
					case KEY_ATTRIBUTE_SDK_MIN:
						targetModel.MinSDKCode = currentAttribute.Value;
						break;
					case KEY_ATTRIBUTE_SDK_MAX:
						targetModel.MaxSDKCode = currentAttribute.Value;
						break;

					default:
						break;
				}
			}
		}

		private static void ProcessAttribute_Screens(PackageDataModel targetModel,
			XmlAttributeCollection attributeCollection)
		{
			if (attributeCollection == null)
				return;

			foreach (XmlAttribute currentAttribute in attributeCollection)
			{
				if (currentAttribute.Value.Equals("true"))
				{
					Console.WriteLine("Node attribute localname:  " + currentAttribute.LocalName);
					if (currentAttribute.LocalName.EndsWith("Screens") ||
						currentAttribute.LocalName.EndsWith("Density"))
					{
						targetModel.ScreenSize.Add(
							currentAttribute.LocalName.Remove(currentAttribute.LocalName.Length - 7, 7));
					}
					else
					{
						targetModel.ScreenSize.Add(currentAttribute.LocalName);
					}
				}
			}

			Console.WriteLine("Node ScreenSize: " + string.Concat(targetModel.ScreenSize));
		}

		private static void ProcessAttribute_Feature(PackageDataModel targetModel,
			XmlAttributeCollection attributeCollection)
		{
			if (attributeCollection == null)
				return;

			string featureName = string.Empty;
			bool featureRequire = false;

			foreach (XmlAttribute currentAttribute in attributeCollection)
			{
				if (currentAttribute.Name.Equals(KEY_ATTRIBUTE_NAME))
				{
					featureName = currentAttribute.Value;
				}
				if (currentAttribute.Name.Equals(KEY_ATTRIBUTE_FEATURE_REQUIRED))
				{
					bool.TryParse(currentAttribute.Value, out featureRequire);
				}
				if (currentAttribute.Name.Equals(KEY_ATTRIBUTE_OPENGLVERSION))
				{
					targetModel.OpenGLVersion = StringConstant.FieldHead_OpenGL +
						OtherUtil.OpenGLVersionParse(currentAttribute.Value);
					Console.WriteLine("Node OpenGL: " + targetModel.OpenGLVersion);
					return;
				}
			}

			if (!string.IsNullOrEmpty(featureName))
			{
				Console.WriteLine("Node Feature: " + featureName + " " + featureRequire);
				if (featureRequire)
					targetModel.Feature_Require.Add(featureName);
				else
					targetModel.Feature_NotRequire.Add(featureName);
			}
		}

		private static void ProcessAttribute_Permission(PackageDataModel targetModel,
			XmlAttributeCollection attributeCollection)
		{
			if (attributeCollection == null)
				return;

			foreach (XmlAttribute currentAttribute in attributeCollection)
			{
				if (currentAttribute.Name.Equals(KEY_ATTRIBUTE_NAME))
				{
					Console.WriteLine("Node permission: " + currentAttribute.Value);
					targetModel.Permissions.Add(currentAttribute.Value);
				}
			}
		}

		private static void ProcessAttribute_Application(PackageDataModel targetModel,
			XmlAttributeCollection attributeCollection)
		{
			if (attributeCollection == null)
				return;

			foreach (XmlAttribute currentAttribute in attributeCollection)
			{
				Console.WriteLine("Node attribute: " + currentAttribute.Name + " " + currentAttribute.Value);
				switch (currentAttribute.Name)
				{
					case KEY_ATTRIBUTE_APPLICATION_ICON:
						targetModel.AppIconResourceEntry = currentAttribute.Value.Trim('@');
						break;
					case KEY_ATTRIBUTE_APPLICATION_LABEL:
						targetModel.AppNameResourceEntry = currentAttribute.Value.Trim('@');
						break;
					default:
						break;
				}
			}
		}

		//todo add ProcessResourceDump()

		public static void ReadAppName(PackageDataModel targetModel, string dumpResult)
		{
			if (string.IsNullOrWhiteSpace(dumpResult))
				return;

			string[] dumpResultLines = dumpResult.Split(new string[] { "\r\n" }, StringSplitOptions.None);
			foreach (string line in dumpResultLines)
			{
				if (string.IsNullOrWhiteSpace(line))
					continue;
				string trimedLine = line.Trim();
				if (trimedLine.StartsWith("Package"))
					continue;
				if (trimedLine.StartsWith("0x"))
					continue;

				string[] splitLine = trimedLine.Split('\"');
				if (splitLine.Length < 2)
					continue;

				string targetValue = splitLine[1];
				// Debug.WriteLine("Feature Test targetValue=" + targetValue);
				//todo multiple language AppName?
				if (trimedLine.Contains("default"))
				{
					targetModel.AppName = targetValue;
					targetModel.AppNameLangDict.Add("default", targetValue);
				}
			}
		}

		public static void ReadAppIconEntry(PackageDataModel targetModel, string dumpResult)
		{
			if (string.IsNullOrWhiteSpace(dumpResult))
				return;

			string[] dumpResultLines = dumpResult.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string line in dumpResultLines.Reverse())
			{
				if (string.IsNullOrWhiteSpace(line))
					continue;
				string trimedLine = line.Trim();
				if (trimedLine.StartsWith("Package"))
					continue;
				if (trimedLine.StartsWith("0x"))
					continue;

				string[] splitLine = trimedLine.Split(new[] { "[FILE]" }, StringSplitOptions.RemoveEmptyEntries);
				if (splitLine.Length < 2)
					continue;

				string targetValue = splitLine[1];

				targetModel.MaxIconZipEntry = "base/" + targetValue.Trim();
				break;

			}
		}
	}
}