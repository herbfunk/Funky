using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using fBaseXtensions.Helpers;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.Bot.Profile.Common;

namespace fBaseXtensions.Game
{
	public class Profile
	{
		private HashSet<int> _profileBlacklist = new HashSet<int>();
		public HashSet<int> ProfileBlacklist
		{
			get { return _profileBlacklist; }
			set { _profileBlacklist = value; }
		}
		internal void UpdateProfileBlacklist()
		{
			//Refresh Profile Target Blacklist 
			_profileBlacklist.Clear();
			foreach (var item in ProfileManager.CurrentProfile.TargetBlacklists)
			{
				_profileBlacklist.Add(item.ActorId);
			}
		}

		[Flags]
		public enum ProfileBehaviorTypes
		{
			Unknown=0,
			ExploreDungeon=1,
			UseWaypoint=2,
			UseObject=4,
			UsePortal=8,
			TownPortal=16,
			TownRun=32,
			SetQuestMode=64,

			Interactive = UseWaypoint | UseObject | UsePortal,
			OutOfCombat = Interactive | TownPortal | TownRun,
		}

		private ProfileBehaviorTypes _currentProfileBehaviorType=ProfileBehaviorTypes.Unknown;
		public ProfileBehaviorTypes CurrentProfileBehaviorType
		{
			get { return _currentProfileBehaviorType; }
			set { _currentProfileBehaviorType = value; }
		}
		public bool ProfileBehaviorIsInteractive
		{
			get { return CurrentProfileBehaviorType.HasFlag(ProfileBehaviorTypes.Interactive); }
		}
		public bool ProfileBehaviorIsOutOfCombat
		{
			get { return CurrentProfileBehaviorType.HasFlag(ProfileBehaviorTypes.OutOfCombat); }
		}

		private ProfileBehavior currentProfileBehavior;
		public ProfileBehavior CurrentProfileBehavior
		{
			get { return currentProfileBehavior; }
		}



		public delegate void ProfileBehaviorChanged(ProfileBehaviorTypes newType);
		public event ProfileBehaviorChanged OnProfileBehaviorChange;

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
					CurrentProfileBehaviorType = GetProfileBehaviorType(profileTagType);
					if (OnProfileBehaviorChange != null)
						OnProfileBehaviorChange(CurrentProfileBehaviorType);
				}
			}
		}

		private ProfileBehaviorTypes GetProfileBehaviorType(Type behaviorType)
		{
			if (typeof(UseWaypointTag)==behaviorType)
				return ProfileBehaviorTypes.UseWaypoint;
			if (typeof(UseObjectTag)==behaviorType)
				return ProfileBehaviorTypes.UseObject;
			if (typeof(UseTownPortalTag)==behaviorType)
				return ProfileBehaviorTypes.TownPortal;
			if (typeof(UsePortalTag) == behaviorType)
				return ProfileBehaviorTypes.UsePortal;
			if (typeof(ExploreAreaTag) == behaviorType)
				return ProfileBehaviorTypes.ExploreDungeon;

			string profileTagTypeString = behaviorType.ToString();

			if (String.Equals(profileTagTypeString, QuestTools_SetQuestModeTag, StringComparison.InvariantCultureIgnoreCase))
				return ProfileBehaviorTypes.SetQuestMode;
			if (String.Equals(profileTagTypeString, QuestTools_ExploreDungeonTag, StringComparison.InvariantCultureIgnoreCase))
				return ProfileBehaviorTypes.ExploreDungeon;
			if (String.Equals(profileTagTypeString, QuestTools_TownPortalTag, StringComparison.InvariantCultureIgnoreCase))
				return ProfileBehaviorTypes.TownPortal;
			if (String.Equals(profileTagTypeString, QuestTools_TownRunTag, StringComparison.InvariantCultureIgnoreCase))
				return ProfileBehaviorTypes.TownRun;

			return ProfileBehaviorTypes.Unknown;
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
																	  //typeof (FunkyWaypoint),
																	};

		//Common Used Profile Tags that requires backtracking during combat sessions.
		private static readonly HashSet<Type> InteractiveTags = new HashSet<Type> 
																	{ 
																	  typeof(UseWaypointTag), 
																	  typeof(UseObjectTag),
																	  //typeof(Zeta.CommonBot.Profile.Common.UseTownPortalTag),
																	  typeof(UsePortalTag),
																	};

		private const string QuestTools_SetQuestModeTag = "QuestTools.ProfileTags.SetQuestingTag";
		private const string QuestTools_ExploreDungeonTag = "QuestTools.ProfileTags.ExploreDungeonTag";
		private const string QuestTools_TownPortalTag = "QuestTools.ProfileTags.TownPortalTag";
		private const string QuestTools_TownRunTag = "QuestTools.ProfileTags.TownRunTag";

		private static string[] oocDBTagsNames =
		{ 
			"QuestTools.ProfileTags.TownPortalTag",
			"QuestTools.ProfileTags.TownRunTag",
		};
		private static bool DoesNameExist(string name, string[] strings)
		{
			return (Array.BinarySearch(strings, name, StringComparer.InvariantCultureIgnoreCase) >= 0);  // Line B.
		}
	}
}
