using System;
using System.Collections.Generic;
using System.Text;

namespace APKViewer.Localize
{
	public class LocalizeTextDataModel
	{
		public string LanguageName { get; set; }

		public string Head_AppName { get; set; }
		public string Head_PackageName { get; set; }
		public string Head_AppVersion { get; set; }
		public string Head_MinSDK { get; set; }
		public string Head_MaxSDK { get; set; }
		public string Head_ScreenSize { get; set; }
		public string Head_Densities { get; set; }
		public string Head_Architecture { get; set; }
		public string Head_Permission { get; set; }
		public string Head_Features { get; set; }
		public string Head_OpenGL { get; set; }
		public string Head_Signature { get; set; }
		public string Head_Hash_SHA1 { get; set; }
		public string Head_Raw_Badging { get; set; }
		public string Head_Raw_Signature { get; set; }

		public string Button_SearchPlayStore { get; set; }
		public string Button_ViewRawDump { get; set; }
		public string Button_InstallApk { get; set; }

		public string Field_Feature_Required { get; set; }
		public string Field_Feature_NotRequired { get; set; }

		public string Msg_DragDropHere { get; set; }
		public string Msg_JavaNotFound { get; set; }
	}
}
