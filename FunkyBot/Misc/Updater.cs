using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace FunkyBot.Misc
{
	internal static class Updater
	{
		private static readonly string GitHubUrl = "https://raw.github.com/herbfunk/Funky/master/";
		private static readonly string GitHubProjectUrl = "https://raw.github.com/herbfunk/Funky/master/FunkyBot/FunkyBot.csproj";

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

		internal static bool Test(string file)
		{
			string localFileString = ReadFileContent(System.IO.File.OpenRead(FolderPaths.sTrinityPluginPath + file));
			WebClient client = new WebClient();
			string gitHubFileString=ReadFileContent(client.OpenRead(GitHubUrl + file));
			bool result = String.Equals(localFileString, gitHubFileString, StringComparison.Ordinal);
			return result;
		}

		private static String ReadFileContent(Stream S)
		{
			if (S != null)
			{
				StreamReader reader = new StreamReader(S);
				return reader.ReadToEnd();
			}

			return String.Empty;
		}

		//private static List<File> ReturnPluginFiles 
	}
}
