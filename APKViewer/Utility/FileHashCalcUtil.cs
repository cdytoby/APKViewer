using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace APKViewer.Utility
{
	public static class FileHashCalcUtil
	{
		public static void CalculateSHA1(Uri fileUri, APKDataModel dataModel)
		{
			using (SHA1 sha = SHA1.Create())
			{
				using (FileStream stream = File.OpenRead(fileUri.OriginalString))
				{
					dataModel.SHA1Hash = BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "");
				}
			}
		}
	}
}