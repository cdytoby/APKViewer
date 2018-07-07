using System;
using System.Collections.Generic;
using System.Text;

namespace APKViewer
{
    public interface IApkDecoder
    {
		event Action decodeFinished;

		void SetApkFilePath(Uri fileUri);
		void Decode();

		APKDataModel GetDataModel();
	}
}
