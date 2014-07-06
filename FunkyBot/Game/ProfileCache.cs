using System;
using System.Linq;
using System.Collections.Generic;
using FunkyBot.Cache.Objects;
using FunkyBot.Config.Settings;
using FunkyBot.XMLTags;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.Bot.Profile.Common;
using Zeta.Common;
using Logger = FunkyBot.Misc.Logger;
using LogLevel = FunkyBot.Misc.LogLevel;

namespace FunkyBot.Game
{
	public class ProfileCache
	{
		private static SettingCluster clusterSettingsTag=new SettingCluster();
		internal static SettingCluster ClusterSettingsTag
		{
			get { return clusterSettingsTag; }
			set { clusterSettingsTag = value; }
		}

		private static SettingLOSMovement losSettingsTag = new SettingLOSMovement();
		internal static SettingLOSMovement LOSSettingsTag
		{
			get { return losSettingsTag; }
			set { losSettingsTag = value; }
		}

		private static bool _adventuremode = false;
		internal static bool AdventureMode
		{
			get
			{
				return _adventuremode;
			}
			set
			{
				_adventuremode = value;
			}
		}

		internal static HashSet<int> LineOfSightSNOIds = new HashSet<int>(); 

		internal static bool QuestMode { get; set; }

		internal bool PreformingInteractiveBehavior
		{
			get
			{
				return IsRunningOOCBehavior && ProfileBehaviorIsOOCInteractive && InteractableCachedObject != null;
			}
		}

		internal Dictionary<int, CacheObject> InteractableObjectCache = new Dictionary<int, CacheObject>();
		internal bool ProfileBehaviorIsOOCInteractive { get; set; }

		internal CacheObject InteractableCachedObject = null;

		private ProfileBehavior currentProfileBehavior;
		internal ProfileBehavior CurrentProfileBehavior
		{
			get { return currentProfileBehavior; }
		}

		internal bool ExploreDungeonTag { get; set; }

		private DateTime LastProfileBehaviorCheck = DateTime.Today;
		///<summary>
		///Tracks Current Profile Behavior and sets IsRunningOOCBehavior depending on the current Type of behavior.
		///</summary>
		internal void CheckCurrentProfileBehavior()
		{
			if (DateTime.Now.Subtract(LastProfileBehaviorCheck).TotalMilliseconds > 250)
			{
				LastProfileBehaviorCheck = DateTime.Now;

				if ((currentProfileBehavior == null && ProfileManager.CurrentProfileBehavior != null && ProfileManager.CurrentProfileBehavior.Behavior != null)
					 || (ProfileManager.CurrentProfileBehavior != null && ProfileManager.CurrentProfileBehavior.Behavior != null && currentProfileBehavior != null && currentProfileBehavior.Behavior.Guid != ProfileManager.CurrentProfileBehavior.Behavior.Guid))
				{
					currentProfileBehavior = ProfileManager.CurrentProfileBehavior;
					Logger.Write(LogLevel.Event, "Profile Behavior Changed To {0}", currentProfileBehavior.GetType().ToString());

					Type profileTagType = currentProfileBehavior.GetType();
					string profileTagTypeString = profileTagType.ToString();
					//Logger.DBLog.Info(profileTagTypeString);

					if (oocDBTags.Contains(profileTagType) || DoesNameExist(profileTagTypeString, oocDBTagsNames))
					{
						if (InteractiveTags.Contains(profileTagType))
						{
							ProfileBehaviorIsOOCInteractive = true;
							Logger.DBLog.DebugFormat("Interactable Profile Tag!");

							InteractableCachedObject = GetInteractiveCachedObject(currentProfileBehavior);
							if (InteractableCachedObject != null)
								Logger.DBLog.DebugFormat("Found Cached Interactable Server Object");
						}
						else
						{
							ProfileBehaviorIsOOCInteractive = false;
							InteractableCachedObject = null;
						}

						Logger.DBLog.DebugFormat("Current Profile Behavior has enabled OOC Behavior.");
						IsRunningOOCBehavior = true;
					}
					else if (String.Equals(profileTagTypeString,QuestingTag, StringComparison.InvariantCultureIgnoreCase))
					{
						Logger.DBLog.DebugFormat("Current Profile Behavior has enabled QuestMode.");
						QuestMode = true;
					}
					else if ((profileTagType==(typeof(ExploreAreaTag)))
							||(String.Equals(profileTagTypeString, ExploreTag, StringComparison.InvariantCultureIgnoreCase)))
					{
						Logger.DBLog.DebugFormat("Current Profile Behavior Is Explore Dungeon Tag!");
						ExploreDungeonTag = true;
					}
					else
					{
						ExploreDungeonTag = false;
						ProfileBehaviorIsOOCInteractive = false;
						InteractableCachedObject = null;
						IsRunningOOCBehavior = false;
					}
				}

			}
		}
		internal bool IsRunningOOCBehavior { get; set; }



		internal static CacheObject GetInteractiveCachedObject(ProfileBehavior tag)
		{
			Type TagType = tag.GetType();
			if (InteractiveTags.Contains(TagType))
			{
				if (TagType == typeof(UseWaypointTag))
				{
					UseWaypointTag tagWP = (UseWaypointTag)tag;
					var WaypointObjects = Bot.Game.Profile.InteractableObjectCache.Values.Where(obj => obj.SNOID == 6442);
					foreach (CacheObject item in WaypointObjects)
					{
						if (item.Position.Distance(tagWP.Position) < 100f)
						{
							//Found matching waypoint object!
							return item;
						}
					}
				}
				else if (TagType == typeof(UseObjectTag))
				{
					UseObjectTag tagUseObj = (UseObjectTag)tag;
					if (tagUseObj.ActorId > 0)
					{//Using SNOID..
						var Objects = Bot.Game.Profile.InteractableObjectCache.Values.Where(obj => obj.SNOID == tagUseObj.ActorId);
						foreach (CacheObject item in Objects.OrderBy(obj => obj.Position.Distance(Bot.Character.Data.Position)))
						{
							//Found matching object!
							return item;
						}

					}
					else
					{//use position to match object
						Vector3 tagPosition = tagUseObj.Position;
						var Objects = Bot.Game.Profile.InteractableObjectCache.Values.Where(obj => obj.Position.Distance(tagPosition) <= 100f);
						foreach (CacheObject item in Objects)
						{
							//Found matching object!
							return item;
						}
					}
				}
				else if (TagType == typeof(UsePortalTag))
				{
					UsePortalTag tagUsePortal = (UsePortalTag)tag;
					if (tagUsePortal.ActorId > 0)
					{//Using SNOID..
						var Objects = Bot.Game.Profile.InteractableObjectCache.Values.Where(obj => obj.SNOID == tagUsePortal.ActorId);
						foreach (CacheObject item in Objects.OrderBy(obj => obj.Position.Distance(Bot.Character.Data.Position)))
						{
							//Found matching object!
							return item;
						}

					}
					else
					{//use position to match object
						Vector3 tagPosition = tagUsePortal.Position;
						var Objects = Bot.Game.Profile.InteractableObjectCache.Values.Where(obj => obj.Position.Distance(tagPosition) <= 100f);
						foreach (CacheObject item in Objects.OrderBy(obj => obj.Position.Distance(Bot.Character.Data.Position)))
						{
							//Found matching object!
							return item;
						}
					}
				}
			}

			return null;
		}

		//Common Used Profile Tags that should be considered Out-Of-Combat Behavior.
		private static readonly HashSet<Type> oocDBTags = new HashSet<Type> 
																	{ 
																	  typeof(UseWaypointTag), 
																	  typeof(UseObjectTag),
																	  typeof(UseTownPortalTag),
																	  //typeof(Zeta.CommonBot.Profile.Common.WaitTimerTag),
																	  //typeof (TrinityTownPortal),
																	  //typeof(QuestTools.ProfileTags.TownPortalTag),
																	  //typeof(QuestTools.ProfileTags.TownRunTag),
																	  typeof (FunkyWaypoint),
																	};

		//Common Used Profile Tags that requires backtracking during combat sessions.
		private static readonly HashSet<Type> InteractiveTags = new HashSet<Type> 
																	{ 
																	  typeof(UseWaypointTag), 
																	  typeof(UseObjectTag),
																	  //typeof(Zeta.CommonBot.Profile.Common.UseTownPortalTag),
																	  typeof(UsePortalTag),
																	};

		private const string QuestingTag = "QuestTools.ProfileTags.SetQuestingTag";
		private const string ExploreTag = "QuestTools.ProfileTags.ExploreDungeonTag";
		private static string[] oocDBTagsNames =
		{ 
			"QuestTools.ProfileTags.TownPortalTag",
			"QuestTools.ProfileTags.TownRunTag",
		};
		private static bool DoesNameExist(string name, string[] strings)
		{
			return (Array.BinarySearch(strings, name, StringComparer.InvariantCultureIgnoreCase) >= 0);  // Line B.
		}


		public ProfileCache()
		{
			ProfileBehaviorIsOOCInteractive = false;
		}
	}
}

