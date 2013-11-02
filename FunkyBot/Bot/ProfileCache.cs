using System;
using System.Collections.Generic;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.CommonBot.Profile;
using Zeta.CommonBot.Settings;

namespace FunkyBot.Cache
{
	 public class ProfileCache
	 {
		  // A list of "useonceonly" tags that have been triggered this xml profile
		  internal static HashSet<int> hashUseOnceID=new HashSet<int>();
		  internal static Dictionary<int, int> dictUseOnceID=new Dictionary<int, int>();
		  // For the random ID tag
		  internal static Dictionary<int, int> dictRandomID=new Dictionary<int, int>();




		  private bool profileBehaviorIsOOCInteractive=false;
		  internal bool ProfileBehaviorIsOOCInteractive
		  {
				get { return profileBehaviorIsOOCInteractive; }
				set { profileBehaviorIsOOCInteractive=value; }
		  }

		  private ProfileBehavior currentProfileBehavior;
		  internal ProfileBehavior CurrentProfileBehavior
		  {
				get { return currentProfileBehavior; }
		  }

		  private DateTime LastProfileBehaviorCheck=DateTime.Today;
		  ///<summary>
		  ///Tracks Current Profile Behavior and sets IsRunningOOCBehavior depending on the current Type of behavior.
		  ///</summary>
		  internal void CheckCurrentProfileBehavior()
		  {
				if (DateTime.Now.Subtract(LastProfileBehaviorCheck).TotalMilliseconds>450)
				{
					 LastProfileBehaviorCheck=DateTime.Now;

					 if (currentProfileBehavior==null
						  ||ProfileManager.CurrentProfileBehavior!=null
						  &&ProfileManager.CurrentProfileBehavior.Behavior!=null
						  &&currentProfileBehavior.Behavior.Guid!=ProfileManager.CurrentProfileBehavior.Behavior.Guid)
					 {
						  currentProfileBehavior=ProfileManager.CurrentProfileBehavior;

						  if (ObjectCache.oocDBTags.Contains(currentProfileBehavior.GetType()))
						  {
							  ProfileBehaviorIsOOCInteractive = ObjectCache.InteractiveTags.Contains(currentProfileBehavior.GetType());
							  Logging.WriteDiagnostic("Current Profile Behavior has enabled OOC Behavior.");
							  IsRunningOOCBehavior = true;
							  OOCBehaviorStartVector = Bot.Character.Position;
						  }
						  else
						  {
							  IsRunningOOCBehavior = false;
							  OOCBehaviorStartVector = Vector3.Zero;
						  }
					 }

				}
		  }
		 internal bool IsRunningOOCBehavior { get; set; }

		 private Vector3 OOCbehaviorstartvector = Vector3.Zero;
		 internal Vector3 OOCBehaviorStartVector
		 {
			 get { return OOCbehaviorstartvector; }
			 set { OOCbehaviorstartvector = value; }
		 }


	 }
}

