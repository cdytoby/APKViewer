using System;
using System.Collections.Generic;
using System.Text;

namespace APKViewer
{
    public class APKDataModel
    {
		public string FileExtension { get; set; }
		public string AppName { get; set; }
		public Dictionary<string, string> AppNameLangDict { get; set; }
		public string PackageName { get; set; }
		public string VersionString { get; set; }
		public int VersionCode { get; set; }
		public int MinSDKCode { get; set; }
		public int MaxSDKCode { get; set; }
		public List<string> ScreenSize { get; set; }
		public List<string> Densities { get; set; }
		public List<string> Permissions { get; set; }
		public List<string> Feature_Require { get; set; }
		public List<string> Feature_NotRequire { get; set; }
		public string OpenGLVersion { get; set; }
		public string Signature { get; set; }
		public string SHA1Hash { get; set; }
		public string MaxIconZipEntry { get; set; }
		public byte[] MaxIconContent { get; set; }

		public string RawDumpBadging { get; set; }
		public string RawDumpSignature { get; set; }
	}
}
