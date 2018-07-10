using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace APKViewer
{
	public static class AndroidSDKData
	{
		public static Dictionary<int, (string, string)> defaultTableDict = new Dictionary<int, (string, string)>()
		{
			{1, ("1.0", "Base") },
			{2, ("1.1", "Base 1.1") },
			{3, ("1.5", "Cupcake") },
			{4, ("1.6", "Donut") },
			{5, ("2.0", "Eclair") },
			{6, ("2.0.1", "Eclair 0.1") },
			{7, ("2.1", "Eclair MR1") },
			{8, ("2.2", "Froyo") },
			{9, ("2.3", "Gingerbread") },
			{10, ("2.3.3", "Gingerbread MR1") },
			{11, ("3.0", "Honeycomb") },
			{12, ("3.1", "Honeycomb MR1") },
			{13, ("3.2", "Honeycomb MR2") },
			{14, ("4.0", "Ice Cream Sandwich") },
			{15, ("4.0.3", "Ice Cream Sandwich MR1") },
			{16, ("4.1", "Jelly Bean") },
			{17, ("4.2", "Jelly Bean MR1") },
			{18, ("4.3", "Jelly Bean MR2") },
			{19, ("4.4", "Kitkat") },
			{20, ("4.4W", "Kitkat Watch") },
			{21, ("5.0", "Lollipop") },
			{22, ("5.1", "Lollipop MR1") },
			{23, ("6.0", "Marshmallow") },
			{24, ("7.0", "Nougat") },
			{25, ("7.1", "Nougat MR1") },
			{26, ("8.0", "Oreo") },
			{27, ("8.1", "Oreo MR1") },
		};

		public const string FILE_ANDROIDTABLE = "AndroidVersionTable.json";
		private static Dictionary<int, (string, string)> tableDictionary = new Dictionary<int, (string, string)>();

		static AndroidSDKData()
		{
			string filePath = Path.Combine(Environment.CurrentDirectory, FILE_ANDROIDTABLE);
			if (File.Exists(filePath))
			{
				tableDictionary = JsonConvert.DeserializeObject<Dictionary<int, (string, string)>>(
					File.ReadAllText(filePath));
			}
			else
				tableDictionary = defaultTableDict;
		}

		public static string GetFullString(int sdkInt)
		{
			if (sdkInt <= 0)
				return string.Empty;
			if (!tableDictionary.ContainsKey(sdkInt))
				return "API " + sdkInt;
			return "API " + sdkInt + " Android " + tableDictionary[sdkInt].Item1 + " " + tableDictionary[sdkInt].Item2;
		}
	}
}
