using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace CheckSum
{
	class Program
	{
		static void Main(string[] args)
		{
			List<string> files = getDirectoryFileList();

			foreach (var s in fileCheckSumDictionary)
			{
				Console.WriteLine(s.Key);
			}
			Console.WriteLine("Total Files: " + fileCheckSumDictionary.Count);
			WriteChecksumToXML();
			Console.WriteLine("Finished Writing Checksum to XML");
		}
		static Dictionary<string,string> fileCheckSumDictionary= new Dictionary<string, string>();

		public static readonly string DirName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

		static List<string> getDirectoryFileList()
		{
			DirectoryInfo dInfo = new DirectoryInfo(DirName);
			DirectoryInfo[] subdirs = dInfo.GetDirectories();

			List<string> FileList = new List<string>();

		
			AddFilesToDictionary(dInfo.GetFiles());
			foreach (var d in subdirs)
			{
				AddFilesToDictionary(d.GetFiles());

				foreach (var t1 in d.GetDirectories())
				{
					AddFilesToDictionary(t1.GetFiles());

					foreach (var t2 in t1.GetDirectories())
					{
						AddFilesToDictionary(t2.GetFiles());

						foreach (var t3 in t2.GetDirectories())
						{
							AddFilesToDictionary(t3.GetFiles());

							foreach (var t4 in t3.GetDirectories())
							{
								AddFilesToDictionary(t4.GetFiles());

								foreach (var t5 in t4.GetDirectories())
								{
									AddFilesToDictionary(t5.GetFiles());
								}
							}
						}
					}
				}

				//foreach (var f in d.GetFiles())
				//{
				//	FileList.Add(f.FullName);
				//}
			}
			return FileList;
		}

		internal static void AddFilesToDictionary(FileInfo[] files)
		{
			foreach (var fileInfo in files)
			{
				if (fileInfo.Extension==".dis" || fileInfo.Extension==".txt" || fileInfo.Extension==".log" ||
					fileInfo.Extension ==".exe" || fileInfo.Extension==".dll" || fileInfo.Extension==".csproj" || 
					fileInfo.Extension==".suo" || fileInfo.Extension==".sln" || fileInfo.Extension==".user"||
					fileInfo.Extension == ".pdb" || fileInfo.Extension == ".Cache" || fileInfo.Extension == ".cache"  || fileInfo.Extension == ".resources") continue;

				string md5Hash = GenerateMD5HashString(fileInfo.FullName);
				string fileName = fileInfo.FullName.Replace(DirName, "");
				fileCheckSumDictionary.Add(fileName, md5Hash);
			}
		}
		 
		internal static string GenerateMD5HashString(string file)
		{
			byte[] fileHash = ComputeHash(file);
			return BitConverter.ToString(fileHash).Replace("-", "").ToLower();
		}
		internal static byte[] ComputeHash(string filename)
		{
			using (var md5 = MD5.Create())
			{
				using (var stream = File.OpenRead(filename))
				{
					return md5.ComputeHash(stream);
				}
			}
		}

		internal static void WriteChecksumToXML()
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;

			using (XmlWriter writer = XmlWriter.Create(DirName + @"\checksum.xml", settings))
			{
				writer.WriteStartDocument();
				writer.WriteStartElement("FileList");

				foreach (var f in fileCheckSumDictionary)
				{
					writer.WriteStartElement("File");
					writer.WriteAttributeString("Name", f.Key);
					writer.WriteAttributeString("MD5", f.Value);
					writer.WriteEndElement();
				}

				writer.WriteEndElement();
				writer.WriteEndDocument();
			}
		}


	}
}
