using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;

namespace FunkyBot.Game
{
	public class BountyCache
	{
		public Dictionary<int, QuestState> BountyQuestStates = new Dictionary<int, QuestState>();
		public Dictionary<int, BountyInfo> CurrentBounties = new Dictionary<int, BountyInfo>();
		public Dictionary<int,BountyMapMarker> CurrentBountyMapMarkers = new Dictionary<int,BountyMapMarker>();
		public BountyInfoCache ActiveBounty = null;

		private int currentBountyID = 0;
		///<summary>
		///Current SNO ID (set by BountyLoad tag)
		///</summary>
		public int CurrentBountyID
		{
			get { return currentBountyID; }
			set 
			{ 
				currentBountyID = value;
				Logger.DBLog.DebugFormat("Setting Current Bounty ID to {0}", value);
			}
		}


		///<summary>
		///Refreshes Bounty Info Cache
		///</summary>
		public void UpdateBounties()
		{
			
			CurrentBounties.Clear();
			foreach (var bounty in ZetaDia.ActInfo.Bounties)
			{
				CurrentBounties.Add(bounty.Info.QuestSNO, bounty);
			}
			RefreshBountyInfo();
		}
		///<summary>
		///Refreshes Current Bounty Quest States
		///</summary>
		public void RefreshBountyInfo()
		{
			BountyQuestStates.Clear();

			foreach (var bounty in CurrentBounties.Values)
			{
				BountyQuestStates.Add(bounty.Info.QuestSNO, bounty.Info.State);
			}
		}
		///<summary>
		///Refreshes Current Bounty Minimap Markers
		///</summary>
		public void RefreshBountyMapMarkers()
		{
			CurrentBountyMapMarkers.Clear();
			foreach (var m in ZetaDia.Minimap.Markers.OpenWorldMarkers)
			{
				var bmm=new BountyMapMarker(m.Position,m.DynamicWorldId, m.Id);
				CurrentBountyMapMarkers.Add(bmm.GetHashCode(), bmm);
			}
		}

		///<summary>
		///Sets Active Bounty
		///</summary>
		public void UpdateActiveBounty()
		{
			var activeBounty = new BountyInfoCache();
			if (ZetaDia.ActInfo.ActiveBounty != null)
			{
				activeBounty = new BountyInfoCache(ZetaDia.ActInfo.ActiveBounty);

				if ((ActiveBounty == null || ActiveBounty.QuestSNO != activeBounty.QuestSNO) && activeBounty.QuestSNO != 0)
				{
					ActiveBounty = activeBounty;
					Logger.DBLog.InfoFormat("Active Bounty Changed To {0}", ActiveBounty.QuestSNO);
				}
				else if (activeBounty.QuestSNO == 0)
				{//nullify when active bounty is nothing
					ActiveBounty = null;
				}
			}
			else
			{
				ActiveBounty=null;
			}
		}

		private DateTime lastUpdatedBountyInfo=DateTime.Today;
		public void RefreshBountyLevelChange()
		{
			if (DateTime.Now.CompareTo(lastUpdatedBountyInfo) > 0)
			{
				lastUpdatedBountyInfo = DateTime.Now.AddSeconds(2.5);
				Logger.DBLog.InfoFormat("Updating Bounty Info!");
				RefreshBountyInfo();
				UpdateActiveBounty();
				RefreshBountyMapMarkers();
			}
		}

		public void Reset()
		{
			BountyQuestStates.Clear();
			CurrentBounties.Clear();
			CurrentBountyMapMarkers.Clear();
			ActiveBounty = null;
		}

		public string DebugString()
		{
			string sBountyIDs = CurrentBounties.Aggregate("Current IDs: ", (current, cb) => current + (cb.Key + " , "));
			string sBountyStates = BountyQuestStates.Aggregate("States: ", (current, sBountyState) => current + (sBountyState.Key + " == " + sBountyState.Value.ToString()));
			string sBountyMapMarkers = CurrentBountyMapMarkers.Values.Aggregate("MapMarkers: ", (current, sBountyMapMarker) => current + ("ID: " + sBountyMapMarker.ID + " WorldID: " + sBountyMapMarker.WorldID + " Position: " + sBountyMapMarker.Position + " Distance: " + Bot.Character.Data.Position.Distance(sBountyMapMarker.Position) + "\r\n"));
			string sActiveBounty = ActiveBounty != null ? ActiveBounty.ToString() : "NONE!";

			return String.Format("{0}\r\n{1}\r\n{2}\r\n{3}",
				sActiveBounty, sBountyIDs, sBountyStates, sBountyMapMarkers);
		}

		public class BountyMapMarker
		{
			public Vector3 Position { get; set; }
			public int WorldID { get; set; }
			public int ID { get; set; }

			public BountyMapMarker(Vector3 pos, int worldid, int id)
			{
				Position = pos;
				WorldID = worldid;
				ID = id;
			}

			public float Distance
			{
				get { return Bot.Character.Data.Position.Distance(Position); }
			}

			public override int GetHashCode()
			{
				return WorldID * ID;
			}

			public override bool Equals(object obj)
			{
				// If parameter is null return false.
				if (obj == null)
				{
					return false;
				}

				// If parameter cannot be cast to Point return false.
				BountyMapMarker p = obj as BountyMapMarker;
				if ((System.Object)p == null)
				{
					return false;
				}

				// Return true if the fields match:
				return (WorldID == p.WorldID) && (ID == p.ID);
			}
		}

		public class BountyInfoCache
		{
			public Act Act { get; set; }
			public SNOLevelArea StartingLevelArea { get; set; }
			public QuestState State { get; set; }

			public readonly int QuestSNO;

			public readonly List<SNOLevelArea> LevelAreas;

			public BountyInfoCache(BountyInfo b)
			{
				try
				{
					Act = b.Act;
					StartingLevelArea = b.StartingLevelArea;
					State = b.State;
					LevelAreas = b.LevelAreas.ToList();
					QuestSNO = b.Info.QuestSNO;
				}
				catch (Exception ex)
				{
					Logger.Write(LogLevel.Cache, "Failure to create BountyInfoCache \r\n{0}", ex.Message);
				}
			}
			public BountyInfoCache()
			{
				Act= Act.Invalid;
				StartingLevelArea = SNOLevelArea.Uber_PortalWorld;
				State = QuestState.NotStarted;
				LevelAreas= new List<SNOLevelArea>();
				QuestSNO = -1;
			}

			public void UpdateState()
			{
				Bot.Game.Bounty.RefreshBountyInfo();
				if (Bot.Game.Bounty.CurrentBounties.ContainsKey(QuestSNO))
				{
					State = Bot.Game.Bounty.BountyQuestStates[QuestSNO];
				}
				else
				{
					Logger.DBLog.Info("Failure to Update Bounty QuestState!");
				}
			}

			public override string ToString()
			{
				string levelAreas = LevelAreas.Aggregate("", (current, levelArea) => current + ("\t" + levelArea + "\r\n"));
				return String.Format("QuestSNO: {0} Act: {1} StartArea: {2} State: {3} LevelAreas:\r\n {4}",
					QuestSNO, Act, StartingLevelArea, State, levelAreas);
			}
		}
	}
}
