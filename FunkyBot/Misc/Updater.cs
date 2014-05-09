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
		private static readonly string GitHubUrl = "https://raw.githubusercontent.com/herbfunk/Funky/master/FunkyBot";
		private static readonly string GitHubProjectUrl = "https://raw.github.com/herbfunk/Funky/master/FunkyBot/FunkyBot.csproj";
		private static readonly string GitHubChecksumUrl = "https://raw.githubusercontent.com/herbfunk/Funky/master/FunkyBot/checksum.xml";

		internal static bool UpdateAvailable()
		{
			Dictionary<string, string> GithubChecksumDict = GenerateDictionaryFromChecksumXML(GitHubChecksumUrl);
			Dictionary<string, string> LocalChecksumDict = GenerateDictionaryFromChecksumXML(FolderPaths.PluginPath + @"\checksum.xml");
			List<string> GithubFileList = ReturnGitHubContentFiles();

			List<string> FilesNeededUpdated = new List<string>();

			//find any new or missing files..
			foreach (var f in GithubFileList)
			{
				if (!LocalChecksumDict.ContainsKey(f))
				{
					FilesNeededUpdated.Add(f);
				}
				else if (!GithubChecksumDict.ContainsKey(f))
				{
					Logger.DBLog.InfoFormat("Github Checksum failed for file {0}", f);
				}
				else if (GithubChecksumDict[f] != LocalChecksumDict[f])
				{
					FilesNeededUpdated.Add(f);
				}

			}

			if (FilesNeededUpdated.Count == 1 && FilesNeededUpdated[0].Contains("CheckSum")) return false;

			Logger.DBLog.InfoFormat("Files Needed Updated: " + FilesNeededUpdated.Count);
		
			if (FilesNeededUpdated.Count > 0)
			{
				MessageBoxResult result = MessageBox.Show(Application.Current.MainWindow, "Funkybot Update Available!", "Do you wish to update files now?", MessageBoxButton.YesNo);
				if (result == MessageBoxResult.Yes)
				{
					foreach (var f in FilesNeededUpdated)
					{
						Logger.DBLog.InfoFormat("File: " + f);
						string FullDirectoryPath = Path.GetFullPath(FolderPaths.PluginPath + f.Substring(0, f.LastIndexOf(Convert.ToChar("/"))));
						if (!Directory.Exists(FullDirectoryPath))
						{
							Directory.CreateDirectory(FullDirectoryPath);
							Logger.DBLog.InfoFormat("Creating new dictionary {0}", FullDirectoryPath);
						}

						string FullPath = Path.GetFullPath(FolderPaths.PluginPath + f);
						string GitHubUrlFullPath = GitHubUrl + f;

						// Create a new WebClient instance.
						using (WebClient myWebClient = new WebClient())
						{
							try
							{
								myWebClient.DownloadFile(GitHubUrlFullPath, FullPath);
								Logger.DBLog.InfoFormat("Downloaded file {0} to location {1}", GitHubUrlFullPath, FullPath);
							}
							catch (Exception ex)
							{
								Logger.DBLog.InfoFormat("Error updating file {0} at location {1}", f, GitHubUrlFullPath);
							}
						}
					}

					return true;
				}
			}


			return false;
		}
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
										string s = "/" + reader.GetAttribute(0);
										returnValues.Add(s.Replace(@"\", "/"));
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
							returnValues.Add(fileName.Replace(@"\", "/"), fileHash);
						}

					}
				}
			}
			catch (Exception)
			{

			}

			return returnValues;
		}
	}
}
