using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace APKViewer
{
    public interface IFileDecoder
    {
		event Action decodeFinishedEvent;

		void SetFilePath(Uri fileUri);
		Task Decode();

		APKDataModel GetDataModel();
	}
}
