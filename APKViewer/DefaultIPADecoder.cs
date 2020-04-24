using System;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Claunia.PropertyList;
using APKViewer.Localize;
using APKViewer.Utility;

namespace APKViewer
{
	public class DefaultIPADecoder : IFileDecoder
	{
		private Uri targetFilePath;
		private PackageDataModel dataModel;

		public event Action decodeProgressCallbackEvent;

		public void SetFilePath(Uri fileUri)
		{
			targetFilePath = fileUri;
		}

		public async Task Decode()
		{
			//https://github.com/claunia/plist-cil
			//https://developer.apple.com/library/archive/documentation/General/Reference/InfoPlistKeyReference/Articles/AboutInformationPropertyListFiles.html
			Console.WriteLine("DefaultIPADecoder.Decode() decode start.");

			dataModel = new PackageDataModel();

			Console.WriteLine("DefaultIPADecoder.Decode() Extract info.plist start.");

			using (ZipArchive zipObject = ZipFile.Open(targetFilePath.OriginalString, ZipArchiveMode.Read, Encoding.UTF8))
			{
				IEnumerable<ZipArchiveEntry> listEntry = zipObject.Entries.Where(x => x.Name.Equals("Info.plist"));
				Console.WriteLine("DefaultIPADecoder.Decode() all plist entries: ");
				foreach (ZipArchiveEntry zae in listEntry)
				{
					Console.WriteLine(zae.FullName);
				}
				Console.WriteLine("DefaultIPADecoder.Decode() all plist ends here");
				ZipArchiveEntry targetEntry = null;
				foreach (ZipArchiveEntry zae in listEntry)
				{
					if (Path.GetDirectoryName(zae.FullName).Trim('/').EndsWith(".app"))
					{
						targetEntry = zae;
						Console.WriteLine("DefaultIPADecoder.Decode() selected entry: " + zae.FullName);
						break;
					}
				}

				if (targetEntry != null)
				{
					NSDictionary rootDoc = null;
					using (Stream sourceStream = targetEntry.Open())
					using (MemoryStream byteStream = new MemoryStream())
					{
						await sourceStream.CopyToAsync(byteStream);
						rootDoc = (NSDictionary)PropertyListParser.Parse(byteStream.ToArray());
						dataModel.RawDumpBadging = rootDoc.ToXmlPropertyList();
					}

					if (rootDoc != null)
					{
						PListCilUtil.ReadNSData(dataModel, rootDoc);
					}
				}
			}

			decodeProgressCallbackEvent?.Invoke();
		}

		public PackageDataModel GetDataModel()
		{
			return dataModel;
		}

	}
}
