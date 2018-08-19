using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit.Abstractions;

namespace APKViewer.XUnitTest
{
	public class TestBase : IDisposable
	{
		//https://github.com/Microsoft/vstest/issues/799

		protected readonly ITestOutputHelper _output;
		protected readonly TextWriter _originalOut;
		protected readonly TextWriter _textWriter;

		public TestBase(ITestOutputHelper output)
		{
			_output = output;
			_originalOut = Console.Out;
			_textWriter = new StringWriter();
			Console.SetOut(_textWriter);
		}

		public void Dispose()
		{
			_output.WriteLine(_textWriter.ToString());
			Console.SetOut(_originalOut);
		}
	}
}
