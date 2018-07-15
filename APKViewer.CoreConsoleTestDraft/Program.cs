using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace APKViewer.CoreConsoleTestDraft
{
	class Program
	{
		private const string AAPT = @"C:\CLIProgram\Android\AndroidSDK\build-tools\27.0.0\aapt.exe";
		private const string sourceTxtFile = @"D:\Android\RawData.txt";
		private const string sourceAPKFile = @"F:\Workspaces\VisualStudio\APKViewer\Release\Amaze File Manager_v3.2.2_apkpure.com.apk";
		private const string apkIconFile = @"res/mipmap-xxxhdpi-v4/ic_launcher.png";
		private const string jsonFile = "result.json";

		static void Main(string[] args)
		{
			Test4();
		}

		private static void Test4()
		{
			string numberA = "0x30000";
			string numberB = "0x00030000";
			string numberC = "0x00150015";
			string numberD = "00030000";
			int valueE = Convert.ToInt32(numberC, 16);

			var result = valueE >> 0x00000010;
			var result2 = valueE & 0xFFFF;

			Console.WriteLine("result=" + result + "." + result2);
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

		private static void Test2()
		{
			Uri targetFilePath = new Uri(sourceAPKFile);
			Process cmd = new Process();
			cmd.StartInfo.FileName = AAPT;
			cmd.StartInfo.Arguments = "d badging " + Uri.UnescapeDataString(targetFilePath.AbsolutePath);
			Console.WriteLine("WindowsAPKDecoder.Decode_Badging() Full CMD= " + cmd.StartInfo.Arguments);
		}

		private static void Test3()
		{
			string result = AndroidSDKData.RequestTableJsonString();
			Console.WriteLine(result);
			File.WriteAllText(Path.Combine(Environment.CurrentDirectory, jsonFile), result);
		}
	}
}
