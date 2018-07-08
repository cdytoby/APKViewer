using System;
using System.IO;
using System.IO.Compression;

namespace APKViewer.CoreConsoleTestDraft
{
	class Program
	{
		private const string sourceTxtFile = @"D:\Android\RawData.txt";
		private const string sourceAPKFile = @"D:\Android\YahooWeatherProvider.apk";
		private const string apkIconFile = @"res/mipmap-mdpi-v4/ic_launcher.png";

		static void Main(string[] args)
		{
			ZipArchive za = ZipFile.Open(sourceAPKFile, ZipArchiveMode.Read);
			foreach (ZipArchiveEntry ent in za.Entries)
			{
				Console.WriteLine(ent.FullName);
			}

			Console.WriteLine("Try get entry:");
			ZipArchiveEntry iconEntry = za.GetEntry(apkIconFile);
			Console.WriteLine(iconEntry.FullName);
			Stream s = iconEntry.Open();
			MemoryStream ms = new MemoryStream();
			s.CopyTo(ms);
			s.Dispose();
			Console.WriteLine(ms.ToArray());
			ms.Dispose();
		}
	}
}
