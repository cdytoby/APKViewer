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
		public string Permission { get; set; }
		public string Hash { get; set; }

		public string rawDumpBadging { get; set; }
	}
}
