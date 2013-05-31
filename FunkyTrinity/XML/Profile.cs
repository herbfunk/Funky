using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Markup;
using Zeta;
using Zeta.Common;
using Zeta.Common.Plugins;
using Zeta.CommonBot;
using Zeta.CommonBot.Profile;
using Zeta.CommonBot.Profile.Composites;
using Zeta.Internals;
using Zeta.Internals.Actors;
using Zeta.Internals.Actors.Gizmos;
using Zeta.Internals.SNO;
using Zeta.Navigation;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  public static int ScenceCheck;
		  public static DateTime LastProfileReload=DateTime.MinValue;

		  internal static string StartProfile, LastProfile;
		  internal static List<ProfileSet> ProfilesSets=new List<ProfileSet>();

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
