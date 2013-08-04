using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.CommonBot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace FunkyTrinity.XMLTags
{
	[ComVisible(false)]
	[XmlElement("NextProfile")]
	public class NextProfileTag : ProfileBehavior
	{
		private bool town_=false;
		[XmlAttribute("returntotown")]
		[XmlAttribute("usetp")]
		public bool town
		{
			get
			{
				return town_;
			}
			set
			{
				town_=value;
			}
		}

		private bool m_IsDone=false;
		public override bool IsDone
		{
			get { return m_IsDone; }
		}

		private string nextProfile=null;
		private bool StartProfile=false;

		private bool CheckedProfile=false;
		private bool CheckProfile()
		{
			Random R=new Random(DateTime.Now.Millisecond);


			string sCurrentProfilePath=Path.GetDirectoryName(Zeta.CommonBot.ProfileManager.CurrentProfile.Path);
			FunkyTrinity.Funky.ProfileSet tmp_ProfileSet=null;

			//Check if all random profiles in first element are null and remove and update the list.
			if (FunkyTrinity.Funky.ProfilesSets!=null&&FunkyTrinity.Funky.ProfilesSets.Count>0)
			{
				tmp_ProfileSet=FunkyTrinity.Funky.ProfilesSets.First();
				var ProfileTest=tmp_ProfileSet.Profiles.Where(s => !string.IsNullOrEmpty(s));
				if (ProfileTest.Count()==0)
				{
					FunkyTrinity.Funky.ProfilesSets.RemoveAt(0);
					FunkyTrinity.Funky.ProfilesSets.TrimExcess();

					tmp_ProfileSet=null;
					if (FunkyTrinity.Funky.ProfilesSets.Count()>0)
					{
						tmp_ProfileSet=FunkyTrinity.Funky.ProfilesSets.FirstOrDefault();
						Logging.Write("[HerbFunk] Profile set completed. Next set is ready.");
					}
					else
					{
						Logging.Write("[HerbFunk] Profile set completed.. could not find another set to run!");
					}
				}
			}

			if (tmp_ProfileSet!=null)
			{
				if (tmp_ProfileSet.random)
				{//Random
					int TotalProfileCount=tmp_ProfileSet.Profiles.Count();
					do
					{
						int nextInt=R.Next(0, TotalProfileCount);
						if (!string.IsNullOrEmpty(tmp_ProfileSet.Profiles[nextInt]))
						{
							nextProfile=tmp_ProfileSet.Profiles[nextInt];
							tmp_ProfileSet.Profiles[nextInt]=null;
							break;
						}
					} while (true);
				}
				else
				{//Normal
					for (int i=0; i<tmp_ProfileSet.Profiles.Count(); i++)
					{
						if (!string.IsNullOrEmpty(tmp_ProfileSet.Profiles[i]))
						{
							nextProfile=tmp_ProfileSet.Profiles[i];
							tmp_ProfileSet.Profiles[i]=null;
							break;
						}
					}
				}
			}

			//Check if there is any random profile sets left.
			if (FunkyTrinity.Funky.ProfilesSets!=null&&FunkyTrinity.Funky.ProfilesSets.Count>0)
			{
				// And prepare a full string of the path, and the new .xml file name
				nextProfile=sCurrentProfilePath+@"\"+nextProfile;
			}
			else
			{
				//No random profile sets left, so now we check for a last profile if vaild or a starting profile if valid.
				if (String.IsNullOrEmpty(FunkyTrinity.Funky.LastProfile)&&!String.IsNullOrEmpty(FunkyTrinity.Funky.StartProfile))
				{
					nextProfile=sCurrentProfilePath+@"\"+FunkyTrinity.Funky.StartProfile;
					StartProfile=true;
				}
				else if (!String.IsNullOrEmpty(FunkyTrinity.Funky.LastProfile))
				{
					nextProfile=sCurrentProfilePath+@"\"+FunkyTrinity.Funky.LastProfile;
					FunkyTrinity.Funky.LastProfile=null;
				}
			}


			if (String.IsNullOrEmpty(nextProfile))
			{
				Logging.Write("Failed to load next profile, attempting to find Starting Profile..");
				if (!String.IsNullOrEmpty(sCurrentProfilePath))
				{
					string profileSearch=null;
					foreach (string item in System.IO.Directory.GetFiles(sCurrentProfilePath))
					{
						if (item.ToLower().Contains("start"))
						{
							profileSearch=item;
							break;
						}
					}
					if (!String.IsNullOrEmpty(profileSearch))
					{
						nextProfile=profileSearch;
						StartProfile=true;
						Logging.Write("Found a starting profile "+nextProfile+"\r\n"+" Will now restart game..");
					}

				}
			}
			CheckedProfile=true;
			return !String.IsNullOrEmpty(nextProfile);
		}

		private DateTime TimeSinceLoadedProfile=DateTime.Today;
		private RunStatus FinishBehavior()
		{
			if (DateTime.Now.Subtract(TimeSinceLoadedProfile).TotalSeconds<3)
				return RunStatus.Running;

			ProfileManager.Load(nextProfile);

			m_IsDone=true;

			if (StartProfile)
				ZetaDia.Service.Party.LeaveGame();

			return RunStatus.Success;
		}


		protected override Composite CreateBehavior()
		{


			return
				new Zeta.TreeSharp.PrioritySelector(
					new Zeta.TreeSharp.Decorator(ret => !CheckedProfile,
						new Zeta.TreeSharp.Action(ret => CheckProfile())),
					new Zeta.TreeSharp.Decorator(ret => ((StartProfile||town)&&Funky.FunkyTPOverlord(ret)),
						new Zeta.TreeSharp.Sequence(
							new Zeta.TreeSharp.Action(ret => Funky.FunkyTPBehavior(ret)),
							new Zeta.TreeSharp.Action(ret => Funky.ResetTPBehavior()))),
					new Zeta.TreeSharp.Sequence(
						new Zeta.TreeSharp.Action(ret => Logging.Write("[Funky] Loading next profile {0}", nextProfile.ToString())),
						new Zeta.TreeSharp.Action(ret => TimeSinceLoadedProfile=DateTime.Now),
						new Zeta.TreeSharp.Action(ret => FinishBehavior())));
		}

		public override void ResetCachedDone()
		{
			TimeSinceLoadedProfile=DateTime.Today;
			CheckedProfile=false;
			StartProfile=false;
			nextProfile=null;
			town_=false;
			m_IsDone=false;
			base.ResetCachedDone();
		}
	}
}