﻿using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using APKViewer.Localize;
using Newtonsoft.Json;

namespace APKViewer.CoreConsoleTestDraft
{
	class Program
	{
		private const string WorkingDirectory = @"D:\Android";
		private const string localizationPath = @"F:\Workspaces\VisualStudio\APKViewer\OtherAssets\Local";
		private const string sourceTxtFile = @"D:\Android\RawData.txt";
		private const string sourceAPKFile = @"F:\Workspaces\VisualStudio\APKViewer\Release\Amaze File Manager_v3.2.2_apkpure.com.apk";
		private const string apkIconFile = @"res/mipmap-xxxhdpi-v4/ic_launcher.png";
		private const string jsonFile = "result.json";

		static void Main(string[] args)
		{
		}

		private static void Test6()
		{
			Console.WriteLine("Result=" + System.Globalization.CultureInfo.CurrentCulture.Name);
		}
		

		private static void Test1()
		{
			using (ZipArchive za = ZipFile.Open(sourceAPKFile, ZipArchiveMode.Read))
			{
				Console.WriteLine("Feature Test try to get entry");
				ZipArchiveEntry iconEntry = za.GetEntry("res/mipmap-xxxhdpi-v4/ic_launcher.png");
				Console.WriteLine(iconEntry.FullName);
				Console.WriteLine("Feature Test try entry got");

				using (Stream s = iconEntry.Open())
				using (MemoryStream ms = new MemoryStream())
				{
					Console.WriteLine("Feature Test ready to copy s");
					s.CopyTo(ms);
					Console.WriteLine("Feature Test copy finished");
				}
			}
		}
		
	}
}
