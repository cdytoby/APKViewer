using System;

namespace APKViewer.Utility
{
	public static class OtherUtil
	{
		public static string OpenGLVersionParse(string originalValue)
		{
			int hexValue = Convert.ToInt32(originalValue, 16);
			int versionMain = hexValue >> 0x00000010;
			int versionSub = hexValue & 0xFFFF;
			return versionMain + "." + versionSub;
		}
	}
}