using System;
using System.Collections.Generic;
using System.Text;

namespace APKViewer.Utility
{
	public static class DesktopCMDAPKSignerUtil
	{
		private const char SPLITTER_MAINKEYVALUE = ':';
		private const string SIGNATURE_HEAD = "Verifies";

		public static void ReadAPKSignature(PackageDataModel targetModel, string signResult)
		{
			StringBuilder finalResult=new StringBuilder();
			if (targetModel == null)
				targetModel = new PackageDataModel();

			string[] textLines = signResult.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

			foreach(string line in textLines)
			{
				if(line.Trim().Equals(SIGNATURE_HEAD))
				{
					continue;
				}

				(string, string) splitResult = SplitKeyValue(line);

				if(splitResult.Item1.StartsWith("WARNING"))
				{
					continue;
				}

				finalResult.AppendLine(line);
			}

			targetModel.Signature = finalResult.ToString();
		}


		private static (string, string) SplitKeyValue(string lineText)
		{
			if (lineText.Contains(SPLITTER_MAINKEYVALUE.ToString()))
			{
				string[] splitResult = lineText.Split(new char[] { SPLITTER_MAINKEYVALUE }, 2);
				if (splitResult.Length == 2)
					return (splitResult[0], splitResult[1]);
			}

			return (lineText, string.Empty);
		}
	}
}
