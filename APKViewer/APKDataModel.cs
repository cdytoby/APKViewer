using System;
using System.Collections.Generic;
using System.Text;

namespace APKViewer
{
    public class APKDataModel
    {
		public string AppName { get; set; }
		public string PackageName { get; set; }
		public string VersionString { get; set; }
		public int VersionCode { get; set; }
		public int MinSDKCode { get; set; }
		public int MaxSDKCode { get; set; }
		public List<string> Permissions { get; set; }
		public string SHA1Hash { get; set; }
		public string maxIconZipEntry { get; set; }
		public byte[] maxIconContent { get; set; }

		public string RawDumpBadging { get; set; }
	}
}
