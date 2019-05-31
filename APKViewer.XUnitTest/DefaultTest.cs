using APKViewer.Utility;
using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace APKViewer.XUnitTest
{
	public class DefaultTest : TestBase
	{
		public DefaultTest(ITestOutputHelper output) : base(output)
		{
		}

		//Path.GetExtension

		[Fact]
		public void Test()
		{
			//"D:\Download\aabplayground\CAI_dev_android-2019-04-23_14-31.apk"

			string result = Path.GetExtension(@"D:\Download\aabplayground\CAI_dev_android-2019-04-23_14-31.apk");
			Console.WriteLine("result=" + result);
		}

		[Fact]
		public void TestAABManifest()
		{
			string result= File.ReadAllText(@"D:\Download\aabplayground\dumpmanifest.xml");
			DesktopCMDAABUtil.ReadManifest(null, result);
		}

		[Fact]
		public void ParseOpenGL()
		{
			string numberA = "0x30000";
			string numberB = "0x00030001";
			string numberC = "0x00150009";
			string numberD = "00030000";

			string result = DesktopCMDAAPTUtil.OpenGLVersionParse(numberA);
			Console.WriteLine("resultA=" + result);
			Assert.Equal("3.0", result);
			result = DesktopCMDAAPTUtil.OpenGLVersionParse(numberB);
			Console.WriteLine("resultB=" + result);
			Assert.Equal("3.1", result);
			result = DesktopCMDAAPTUtil.OpenGLVersionParse(numberC);
			Console.WriteLine("resultC=" + result);
			Assert.Equal("21.9", result);
			result = DesktopCMDAAPTUtil.OpenGLVersionParse(numberD);
			Console.WriteLine("resultD=" + result);
			Assert.Equal("3.0", result);
		}
	}
}
