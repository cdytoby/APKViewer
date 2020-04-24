using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace APKViewer
{
    public interface IFileDecoder
    {
		event Action decodeProgressCallbackEvent;

		void SetFilePath(Uri fileUri);
		Task Decode();

		PackageDataModel GetDataModel();
	}
}
