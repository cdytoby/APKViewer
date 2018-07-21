using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace APKViewer
{
    public interface IApkDecoder
    {
		event Action decodeFinishedEvent;

		void SetApkFilePath(Uri fileUri);
		Task Decode();

		APKDataModel GetDataModel();
	}
}
