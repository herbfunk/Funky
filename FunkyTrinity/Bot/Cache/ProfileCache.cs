using System;
using System.Collections.Generic;
using Zeta.Common;
using Zeta.CommonBot.Profile;

namespace FunkyTrinity.Cache
{
	public class ProfileCache
	{
		 private ProfileBehavior currentProfileBehavior;
		 internal ProfileBehavior CurrentProfileBehavior
		 {
			  get { return currentProfileBehavior; }
			  set { LastProfileBehavior=currentProfileBehavior; currentProfileBehavior=value; LastProfileBehaviorChanged=DateTime.Now; }
		 }
		 internal ProfileBehavior LastProfileBehavior { get; set; }
		 internal DateTime LastProfileBehaviorChanged { get; set; }

		 private DateTime LastProfileBehaviorCheck=DateTime.Today;
		 ///<summary>
		 ///Tracks Current Profile Behavior and sets IsRunningOOCBehavior depending on the current Type of behavior.
		 ///</summary>
		 internal void CheckCurrentProfileBehavior()
		 {
			  if (DateTime.Now.Subtract(LastProfileBehaviorCheck).TotalMilliseconds>1000)
			  {
					LastProfileBehaviorCheck=DateTime.Now;

					if (currentProfileBehavior==null
						 ||Zeta.CommonBot.ProfileManager.CurrentProfileBehavior!=null
						 &&Zeta.CommonBot.ProfileManager.CurrentProfileBehavior.Behavior!=null
						 &&currentProfileBehavior.Behavior.Guid!=Zeta.CommonBot.ProfileManager.CurrentProfileBehavior.Behavior.Guid)
					{
						 currentProfileBehavior=Zeta.CommonBot.ProfileManager.CurrentProfileBehavior;

						 if (ObjectCache.oocDBTags.Contains(currentProfileBehavior.GetType()))
						 {
							  Logging.WriteDiagnostic("Current Profile Behavior has enabled OOC Behavior.");
							  Bot.Character.IsRunningOOCBehavior=true;
						 }
						 else
							  Bot.Character.IsRunningOOCBehavior=false;
					}
					
			  }
		 }


		 public int iTotalProfileRecycles { get; set; }
		 // Related to the profile reloaded when restarting games, to pick the FIRST profile.
		 // Also storing a list of all profiles, for experimental reasons/incase I want to use them down the line
		 public List<string> listProfilesLoaded=new List<string>();

		 private string slastProfileSeen=String.Empty;
		 public string LastProfileSeen
		 {
			  get { return slastProfileSeen; }
			  set { slastProfileSeen=value; }
		 }

		 private string firstProfileSeen=String.Empty;
		 public string FirstProfileSeen
		 {
			  get { return firstProfileSeen; }
			  set { firstProfileSeen=value; }
		 }

		 private DateTime lastProfileCheck { get; set; }
		 ///<summary>
		 ///Checks if current profile has changed and updates vars accordingly!
		 ///</summary>
		 public void CheckProfile()
		 {
			  if (DateTime.Now.Subtract(this.lastProfileCheck).TotalMilliseconds>1000)
			  {
					this.lastProfileCheck=DateTime.Now;
					string sThisProfile=Zeta.CommonBot.Settings.GlobalSettings.Instance.LastProfile;
					if (sThisProfile!=this.LastProfileSeen)
					{
						 //herbfunk stats
						 Bot.BotStatistics.ProfileStats.UpdateProfileChanged();

						 // See if we appear to have started a new game
						 if (!String.IsNullOrEmpty(this.FirstProfileSeen)&&sThisProfile==this.FirstProfileSeen)
						 {
							  this.iTotalProfileRecycles++;
							  if (this.iTotalProfileRecycles>Bot.Stats.iTotalJoinGames&&this.iTotalProfileRecycles>Bot.Stats.iTotalLeaveGames)
							  {
									Funky.Log("Reseting Game Data -- Total Profile Recycles exceedes join and leave count!");
									Funky.ResetGame();
							  }
						 }
						 this.listProfilesLoaded.Add(sThisProfile);
						 this.LastProfileSeen=sThisProfile;
						 if (String.IsNullOrEmpty(this.FirstProfileSeen))
							  this.FirstProfileSeen=sThisProfile;

						 //Refresh Profile Target Blacklist 
						 ObjectCache.hashProfileSNOTargetBlacklist=new HashSet<int>();
						 foreach (var item in Zeta.CommonBot.ProfileManager.CurrentProfile.TargetBlacklists)
						 {
							  ObjectCache.hashProfileSNOTargetBlacklist.Add(item.ActorId);
						 }
					}
			  }
		 }
	}
}
