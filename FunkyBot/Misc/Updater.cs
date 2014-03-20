using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Xml;

namespace FunkyBot.Misc
{
	internal static class Updater
	{
		private static readonly string GitHubUrl = "https://raw.github.com/herbfunk/Funky/master/FunkyBot";
		private static readonly string GitHubProjectUrl = "https://raw.github.com/herbfunk/Funky/master/FunkyBot/FunkyBot.csproj";
		private static readonly string GitHubChecksumUrl = "https://raw.githubusercontent.com/herbfunk/Funky/master/FunkyBot/checksum.xml";

		internal static bool UpdateAvailable()
		{
			Dictionary<string, string> GithubChecksumDict = GenerateDictionaryFromChecksumXML(GitHubChecksumUrl);
			Dictionary<string, string> LocalChecksumDict = GenerateDictionaryFromChecksumXML(FolderPaths.sTrinityPluginPath + @"\checksum.xml");

			List<string> FilesNeededUpdated = new List<string>();

			//GitHub Files (see if we are missing any!)
			foreach (var f in GithubChecksumDict)
			{
				if (f.Key.Contains("checksum")) continue;

				if (!LocalChecksumDict.ContainsKey(f.Key))
				{
					FilesNeededUpdated.Add(f.Key);
				}
			}

			//Local Files
			foreach (var f in LocalChecksumDict)
			{
				if (f.Key.Contains("checksum")) continue;

				if (GithubChecksumDict.ContainsKey(f.Key) && GithubChecksumDict[f.Key] != f.Value)
				{
					FilesNeededUpdated.Add(f.Key);
				}
			}

			Logger.DBLog.Info("Files Needed Updated: " + FilesNeededUpdated.Count);
			if (FilesNeededUpdated.Count>0)
			{
				MessageBoxResult result=MessageBox.Show(Application.Current.MainWindow,"Funky Bot Update Available", "Do you wish to update Funky Bot now?", MessageBoxButton.YesNo);
				if (result==MessageBoxResult.Yes)
				{
					foreach (var f in FilesNeededUpdated)
					{
						string FullDirectoryPath = Path.GetFullPath(FolderPaths.sTrinityPluginPath + f.Substring(0, f.LastIndexOf(Convert.ToChar(@"\"))));
						if (!Directory.Exists(FullDirectoryPath))
						{
							Directory.CreateDirectory(FullDirectoryPath);
						}

						string FullPath = Path.GetFullPath(FolderPaths.sTrinityPluginPath + f);
						// Create a new WebClient instance.
						using (WebClient myWebClient = new WebClient())
						{
							myWebClient.DownloadFile(GitHubUrl + f.Replace("//", "/"), FullPath);
						}
					}

					return true;
				}
			}

			
			return false;
		}

		//This returns all Files that are included in the plugin folder from GitHub.
		internal static List<string> ReturnGitHubContentFiles()
		{
			List<string> returnValues = new List<string>();

			try
			{
				using (XmlTextReader reader = new XmlTextReader(GitHubProjectUrl))
				{
					// simply (and easily) skip the junk at the beginning
					reader.MoveToContent();
					reader.ReadToDescendant("ItemGroup");


					if ((reader.NodeType == XmlNodeType.Element) &&
						  (reader.Name == "ItemGroup"))
					{
						while (reader.Read())
						{

							if ((reader.NodeType == XmlNodeType.Element) &&
								  (reader.IsStartElement()) &&
									reader.HasAttributes)
							{
								switch (reader.LocalName)
								{
									case "Content":
									case "None":
									case "Compile":
										string s = reader.GetAttribute(0);
										returnValues.Add(s);
										break;
								}
							}

						}
					}
				}


			}
			catch (Exception)
			{

			}

			return returnValues;
		}
		internal static Dictionary<string, string> ReturnGitHubChecksumDictionary()
		{
			Dictionary<string, string> returnValues = new Dictionary<string, string>();

			try
			{
				using (XmlTextReader reader = new XmlTextReader(GitHubProjectUrl))
				{
					// simply (and easily) skip the junk at the beginning
					reader.MoveToContent();
					//reader.ReadToDescendant("FileList");



					while (reader.Read())
					{
						if ((reader.NodeType == XmlNodeType.Element) &&
							  (reader.IsStartElement()) &&
								reader.HasAttributes)
						{
							string fileName = reader.GetAttribute(0);
							string fileHash = reader.GetAttribute(1);
							returnValues.Add(fileName, fileHash);
						}

					}
				}
			}
			catch (Exception)
			{

			}

			return returnValues;
		}

		internal static Dictionary<string, string> GenerateDictionaryFromChecksumXML(string path)
		{
			Dictionary<string, string> returnValues = new Dictionary<string, string>();

			try
			{
				using (XmlTextReader reader = new XmlTextReader(path))
				{
					// simply (and easily) skip the junk at the beginning
					reader.MoveToContent();
					//reader.ReadToDescendant("FileList");

					while (reader.Read())
					{
						if ((reader.NodeType == XmlNodeType.Element) &&
							  (reader.IsStartElement()) &&
								reader.HasAttributes)
						{
							string fileName = reader.GetAttribute(0);
							string fileHash = reader.GetAttribute(1);
							returnValues.Add(fileName, fileHash);
						}

					}
				}
			}
			catch (Exception)
			{

			}

			return returnValues;
		}

		//private static List<File> ReturnPluginFiles 
	}
}
