namespace APKViewer.Utility
{
	public static class DesktopJavaUtil
	{
		public static bool javaTested { get; private set; }
		public static bool javaExist { get; private set; }

		private static bool JavaExist(string javaVersionResult)
		{
			return javaVersionResult.Contains("java version") || javaVersionResult.Contains("jdk version");
		}

		public static void SetJavaExist(string javaVersionResult)
		{
			javaExist = JavaExist(javaVersionResult);
			javaTested = true;
		}
	}
}