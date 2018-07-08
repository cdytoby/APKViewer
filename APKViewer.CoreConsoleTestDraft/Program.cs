using System;
using System.IO;

namespace APKViewer.CoreConsoleTestDraft
{
    class Program
    {
		private const string sourceFile = @"D:\Android\RawData.txt";

		static void Main(string[] args)
        {
			string allText= File.ReadAllText(sourceFile);

			DesktopCMDAAPTUtil.ReadBadging(null, allText);

		}
    }
}
