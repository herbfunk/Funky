using System;
using System.Collections.Generic;
using Zeta.CommonBot;
using Zeta.Common;
using System.IO;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  public static int ScenceCheck;
		  public static DateTime LastProfileReload=DateTime.MinValue;

		  public static string StartProfile, LastProfile;
		  public static List<ProfileSet> ProfilesSets=new List<ProfileSet>();

		  private static void ResetProfileVars()
		  {
				ProfilesSets.Clear();
				StartProfile=null;
				LastProfile=null;
		  }

		  public class ProfileSet
		  {
				public bool random;
				public string[] Profiles;

				public ProfileSet(bool r, string p)
				{
					 this.random=r;
					 this.Profiles=p.Split(Char.Parse(","));
				}
		  }

		  public static void ReloadCurrentProfile()
		  {
				Logging.Write("Reloading current profile");
				ProfileManager.Load(ProfileManager.CurrentProfile.Path);

		  }

		  public static void ReloadStartingProfile()
		  {
				if (!String.IsNullOrEmpty(Zeta.CommonBot.ProfileManager.CurrentProfile.Name))
				{
					 string profileSearch=null;
					 string directoryPath=Path.GetDirectoryName(Zeta.CommonBot.ProfileManager.CurrentProfile.Path);
					 if (Directory.Exists(directoryPath))
					 {
						  foreach (string item in System.IO.Directory.GetFiles(directoryPath))
						  {
								if (item.ToLower().Contains("start"))
								{
									 profileSearch=item;
									 break;
								}
						  }
						  if (!String.IsNullOrEmpty(profileSearch))
						  {
								Logging.Write("Found a starting profile: "+profileSearch);
								ProfileManager.Load(profileSearch);
						  }
					 }
				}
		  }
	 }

}
