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
			//"C:\Users\cdytoby\Downloads\Ticket69\DEVSTUDIO_live_ios-2020-04-17_16-17.ipa"

			string path = (@"C:\Users\cdytoby\Downloads\IPACollection\WhatsApp (v2.20.42 v2.20.42.1 unk PI DY FW iPhone4S 64bit os90 ok13)-user_hidden.bfi.ipa");
			Uri pathUri = new Uri(path);

			DefaultIPADecoder decoder = new DefaultIPADecoder();
			decoder.SetFilePath(pathUri);
			decoder.decodeProgressCallbackEvent += () =>
			{
				Console.WriteLine(decoder.GetDataModel().RawDumpBadging);
				Console.WriteLine(decoder.GetDataModel().AppName);
			};
			decoder.Decode().Wait();
		}

		[Fact]
		public void TestAABManifest()
		{
			string result = File.ReadAllText(@"D:\Download\aabplayground\dumpmanifest.xml");
			DesktopCMDAABUtil.ReadManifest(null, result);
		}

		[Fact]
		public void ParseOpenGL()
		{
			string numberA = "0x30000";
			string numberB = "0x00030001";
			string numberC = "0x00150009";
			string numberD = "00030000";

			string result = OtherUtil.OpenGLVersionParse(numberA);
			Console.WriteLine("resultA=" + result);
			Assert.Equal("3.0", result);
			result = OtherUtil.OpenGLVersionParse(numberB);
			Console.WriteLine("resultB=" + result);
			Assert.Equal("3.1", result);
			result = OtherUtil.OpenGLVersionParse(numberC);
			Console.WriteLine("resultC=" + result);
			Assert.Equal("21.9", result);
			result = OtherUtil.OpenGLVersionParse(numberD);
			Console.WriteLine("resultD=" + result);
			Assert.Equal("3.0", result);
		}
	}
}
