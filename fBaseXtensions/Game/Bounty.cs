using System;
using System.Collections.Generic;
using System.Linq;
using fBaseXtensions.Cache;
using fBaseXtensions.Cache.External.Enums;
using fBaseXtensions.Cache.External.Objects;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Logger = fBaseXtensions.Helpers.Logger;
using LogLevel = fBaseXtensions.Helpers.LogLevel;

namespace fBaseXtensions.Game
{
	public class BountyCache
	{
		public static readonly int ADVENTUREMODE_QUESTID = 312429;
		public static readonly int ADVENTUREMODE_RIFTID = 337492;
		public static readonly int ADVENTUREMODE_GREATERRIFT_TRIAL = 405695;
		internal static readonly Vector3 RiftTrial_StartPosition = new Vector3(279.566f, 277.9756f, -11.48438f);
		/*
		 Greater Rift Trial
		GizmoType: Portal Name: X1_OpenWorld_Tiered_Rifts_Challenge_Portal-30347 ActorSNO: 408511
		Type: Event QuestSNO: 405695
		Step 13: Count Down
		Step 1: Killing
		Step 9: Finished
		 * 
		 WorldID: 405684
		 LevelAreaID: 405915
		 p1_TieredRift_Challenge
		 */

		public delegate void BountyQuestStateChange(int QuestSno, QuestState newState);
		public event BountyQuestStateChange OnBountyQuestStateChanged;

		public Dictionary<int, QuestState> BountyQuestStates = new Dictionary<int, QuestState>();
		public Dictionary<int, BountyInfo> CurrentBounties = new Dictionary<int, BountyInfo>();
		public Dictionary<int, BountyMapMarker> CurrentBountyMapMarkers = new Dictionary<int, BountyMapMarker>();
		public Dictionary<int, QuestInfoCache> ActiveQuests = new Dictionary<int, QuestInfoCache>();

		public BountyInfoCache ActiveBounty = null;

		/// <summary>
		/// Bounty Cache Loaded from File
		/// </summary>
		//public BountyQuestActCache CurrentActCache = new BountyQuestActCache();

		/// <summary>
		/// Cache Entry that contains info about the bounty.
		/// </summary>
		public BountyDataCollection.BountyQuestCacheEntry CurrentBountyCacheEntry = null;

		private int lastCheckedQuestSNO = -1;
		private Act CurrentAct = Act.Invalid;

		public static bool GreaterRiftIsActiveQuest
		{
			get
			{
				return FunkyGame.Bounty.ActiveBounty!=null &&
					FunkyGame.Bounty.ActiveBounty.QuestSNO==ADVENTUREMODE_RIFTID &&
					FunkyGame.Bounty.ActiveBounty.State == QuestState.InProgress &&
					FunkyGame.Bounty.ActiveQuests.ContainsKey(ADVENTUREMODE_RIFTID) &&
					(FunkyGame.Bounty.ActiveQuests[ADVENTUREMODE_RIFTID].Step == 13 ||
					FunkyGame.Bounty.ActiveQuests[ADVENTUREMODE_RIFTID].Step == 16||
					FunkyGame.Bounty.ActiveQuests[ADVENTUREMODE_RIFTID].Step == 34);
			}
		}
		public static bool RiftTrialIsActiveQuest
		{
			get
			{
				//if (FunkyGame.Bounty.ActiveQuests.ContainsKey(ADVENTUREMODE_GREATERRIFT_TRIAL))
				//	FunkyGame.Bounty.ActiveQuests[ADVENTUREMODE_GREATERRIFT_TRIAL].Refresh();

				return FunkyGame.Bounty.ActiveBounty != null &&
						FunkyGame.Bounty.ActiveBounty.QuestSNO == ADVENTUREMODE_GREATERRIFT_TRIAL &&
						FunkyGame.Bounty.ActiveBounty.State== QuestState.InProgress;
			}
		}


		///<summary>
		///Refreshes Bounty Info Cache
		///</summary>
		public void RefreshBountyInfo()
		{

			CurrentBounties.Clear();
			using (ZetaDia.Memory.AcquireFrame())
			{
				foreach (var bounty in ZetaDia.ActInfo.Bounties)
				{
					CurrentBounties.Add(bounty.Info.QuestSNO, bounty);
				}
			}
			BountyQuestStates.Clear();
			RefreshBountyQuestStates();
		}
		///<summary>
		///Refreshes Current Bounty Quest States
		///</summary>
		public void RefreshBountyQuestStates()
		{
			foreach (var bounty in CurrentBounties.Values)
			{
				var bountySNO = bounty.Info.QuestSNO;
				var bountyState = bounty.Info.State;

				if (!BountyQuestStates.ContainsKey(bountySNO))
				{
					BountyQuestStates.Add(bountySNO, bountyState);
				}
				else if (BountyQuestStates[bountySNO] != bountyState)
				{
					//Update State
					BountyQuestStates[bountySNO] = bountyState;

					//Check State?
					if (bountyState == QuestState.Completed)
					{
						FunkyGame.CurrentGameStats.CurrentProfile.BountiesCompleted++;
						//Bot.Game.CurrentGameStats.CurrentProfile.BountiesCompleted++;
					}

					//Raise Event
					if (OnBountyQuestStateChanged != null)
					{
						OnBountyQuestStateChanged(bountySNO, bountyState);
					}
				}
			}
		}

		public void RefreshActiveQuests()
		{
			try
			{
				var CurrentListOfSNOS = ActiveQuests.Keys.ToList();
				List<int> newActiveQuestSNOS = new List<int>();

				using (ZetaDia.Memory.AcquireFrame())
				{
					foreach (var aq in ZetaDia.ActInfo.ActiveQuests)
					{
						int sno = aq.QuestSNO;
						newActiveQuestSNOS.Add(sno);

						//Filter Adventure Mode and Bounty IDs
						if (sno == ADVENTUREMODE_QUESTID) continue;
						if (BountyQuestStates.ContainsKey(sno)) continue;
						//Ignore entries we already added
						if (ActiveQuests.ContainsKey(sno))
						{
							ActiveQuests[sno].Refresh();
							continue;
						}
							

						var newEntry = new QuestInfoCache(aq);
						ActiveQuests.Add(sno, newEntry);
					}
				}

				var removalQuests = CurrentListOfSNOS.Where(i => !newActiveQuestSNOS.Contains(i)).ToList();
				foreach (var sno in removalQuests)
				{
					ActiveQuests.Remove(sno);
				}

			}
			catch (Exception ex)
			{

			}

		}

		///<summary>
		///Refreshes Current Bounty Minimap Markers
		///</summary>
		public void RefreshBountyMapMarkers()
		{
			CurrentBountyMapMarkers.Clear();
			using (ZetaDia.Memory.AcquireFrame())
			{
				foreach (var m in ZetaDia.Minimap.Markers.OpenWorldMarkers)
				{
					var bmm = new BountyMapMarker(m.Position, m.DynamicWorldId, m.Id);
					CurrentBountyMapMarkers.Add(bmm.GetHashCode(), bmm);
				}
			}
		}

		public void RefreshRiftMapMarkers()
		{
			CurrentBountyMapMarkers.Clear();
			int exitHash = GetRiftWorldExitHash(FunkyGame.Hero.CurrentWorldID);
			if (exitHash != -1)
			{
				using (ZetaDia.Memory.AcquireFrame())
				{
					foreach (var m in ZetaDia.Minimap.Markers.CurrentWorldMarkers)
					{
						if (m.NameHash == exitHash)
						{
							var bmm = new BountyMapMarker(m.Position, m.DynamicWorldId, m.Id);
							CurrentBountyMapMarkers.Add(bmm.GetHashCode(), bmm);
						}
					}
				}
			}
		}

		///<summary>
		///Sets Active Bounty
		///</summary>
		public void UpdateActiveBounty()
		{
			var activeBounty = new BountyInfoCache();

			BountyInfo ZetaActiveBounty=null;
			using(ZetaDia.Memory.AcquireFrame())
			{
				ZetaActiveBounty = ZetaDia.ActInfo.ActiveBounty;
			}

			if (ZetaActiveBounty != null)
			{
				activeBounty = new BountyInfoCache(ZetaDia.ActInfo.ActiveBounty);


				if ((ActiveBounty == null || ActiveBounty.QuestSNO != activeBounty.QuestSNO) && activeBounty.QuestSNO != 0)
				{
					ActiveBounty = activeBounty;
					Logger.Write(LogLevel.Bounty, "Active Bounty Changed To {0}", ActiveBounty.QuestSNO);
					//nullify Cache Entry then set it if Cache contains it.
					CurrentBountyCacheEntry = null;
				}
				else if (activeBounty.QuestSNO == 0)
				{//nullify when active bounty is nothing
					ActiveBounty = null;
				}

				return;
			}

			RefreshActiveQuests();

			if (ActiveQuests.ContainsKey(ADVENTUREMODE_RIFTID) && ActiveQuests[ADVENTUREMODE_RIFTID].State!= QuestState.NotStarted)
			{
				if (ActiveBounty!=null && ActiveBounty.QuestSNO == ADVENTUREMODE_RIFTID)
				{
					((QuestInfoCache)ActiveBounty).Refresh();
				}
				else
				{
					ActiveBounty = ActiveQuests[ADVENTUREMODE_RIFTID];
					Logger.Write(LogLevel.Bounty, "Active Bounty Changed To Rifting");
				}
			}
			else if(ActiveQuests.ContainsKey(ADVENTUREMODE_GREATERRIFT_TRIAL) && ActiveQuests[ADVENTUREMODE_GREATERRIFT_TRIAL].State!= QuestState.NotStarted)
			{
				if (ActiveBounty != null && ActiveBounty.QuestSNO == ADVENTUREMODE_GREATERRIFT_TRIAL)
				{
					((QuestInfoCache)ActiveBounty).Refresh();
				}
				else
				{
					ActiveBounty = ActiveQuests[ADVENTUREMODE_GREATERRIFT_TRIAL];
					Logger.Write(LogLevel.Bounty, "Active Bounty Changed To Rift Trial");
				}
			}
			else
			{
				Logger.Write(LogLevel.Bounty, "Active Bounty Is Null!");
			}
		}

		public void RefreshLevelChanged()
		{
			if (!IsInRiftWorld)
			{
				RefreshBountyLevelChange();
			}
			else
			{
				RefreshRiftLevelChange();
			}
		}
		private void RefreshBountyLevelChange()
		{
			//Logger.DBLog.InfoFormat("Updating Bounty Info!");

			//Do we have any bounties stored?.. if we do refresh states
			if (CurrentBounties.Count == 0) RefreshBountyInfo(); else RefreshBountyQuestStates();

			//If we are in town.. we don't do anything else! (Since the Active Bounty is no longer visible)
			if (FunkyGame.Hero.bIsInTown)
			{
				//We could check that active bounty has been completed..
				if (ActiveBounty != null && BountyQuestStates.ContainsKey(ActiveBounty.QuestSNO) && BountyQuestStates[ActiveBounty.QuestSNO] == QuestState.Completed)
				{
					Logger.Write(LogLevel.Bounty, "ActiveBounty Quest State is Completed!");
					ActiveBounty = null;
				}

				return;
			}


			//Do we have an active bounty set.. lets try to invalidate it.
			if (ActiveBounty == null)
			{
				UpdateActiveBounty();
			}
			else if (!BountyQuestStates.ContainsKey(ActiveBounty.QuestSNO))
			{
				Logger.Write(LogLevel.Bounty, "ActiveBounty is not contained within BountyQuestStates Cache!");
				UpdateActiveBounty();
			}
			else if (BountyQuestStates[ActiveBounty.QuestSNO] == QuestState.Completed)
			{
				Logger.Write(LogLevel.Bounty, "ActiveBounty Quest State is Completed!");
				ActiveBounty = null;
				UpdateActiveBounty();
			}

			//Refresh any Map Markers we could use for navigation..
			RefreshBountyMapMarkers();


			//Is ActiveBounty valid still?
			if (ActiveBounty != null)
			{
				//Load Act Bounty Cache
				if (!ZetaDia.IsInTown && ActiveBounty.Act != CurrentAct)
				{
					CurrentAct = ActiveBounty.Act;
				}


				if (CurrentBountyCacheEntry == null)
				{//Attempt to Match a Cache Entry to the QuestSNO.

					if (lastCheckedQuestSNO != ActiveBounty.QuestSNO)
					{//Only attempt search once for the SNO..

						lastCheckedQuestSNO = ActiveBounty.QuestSNO;
						//var allCacheBounties = CurrentActCache.AllBounties;

						BountyDataCollection.BountyActCollection bountyActCache=null;
						switch (CurrentAct)
						{
							case Act.A1:
								bountyActCache = TheCache.ObjectIDCache.BountyEntries.ActOne;
								break;
							case Act.A2:
								bountyActCache = TheCache.ObjectIDCache.BountyEntries.ActTwo;
								break;
							case Act.A3:
								bountyActCache = TheCache.ObjectIDCache.BountyEntries.ActThree;
								break;
							case Act.A4:
								bountyActCache = TheCache.ObjectIDCache.BountyEntries.ActFour;
								break;
							case Act.A5:
								bountyActCache = TheCache.ObjectIDCache.BountyEntries.ActFive;
								break;
						}
						if (bountyActCache == null) return;

						foreach (var b in bountyActCache.AllBounties.Where(c => c.QuestSNOs != null && c.QuestSNOs.Any(i => i == ActiveBounty.QuestSNO)))
						{
							CurrentBountyCacheEntry = b;
							break;
						}

					}
				}
			}
		}
		private void RefreshRiftLevelChange()
		{
			if (ActiveBounty == null)
			{
				UpdateActiveBounty();
			}

			if (ActiveBounty != null)
			{
				int _step = ((QuestInfoCache)ActiveBounty).Step;
				ActiveBounty.Refresh();

				int curStep = ((QuestInfoCache)ActiveBounty).Step;
				if (_step!=curStep)
				{
					Logger.Write(LogLevel.Bounty, "Active Rift Step Changed From {0} To {1}", _step, curStep);

					//Raise Event
					if (OnBountyQuestStateChanged != null)
						OnBountyQuestStateChanged(ActiveBounty.QuestSNO, ActiveBounty.State);
					
				}

				//Killing..
				if (curStep == 1 || curStep==3 || curStep==13 || curStep == 16)
				{
					RefreshRiftMapMarkers();
				}
			}
		}


		
		
		public delegate void OnActiveBountyChanged();
		public event OnActiveBountyChanged ActiveBountyChanged;

		///<summary>
		///Check used to validate active bounty (when it fails to update during level changes)
		///</summary>
		public void CheckActiveBounty()
		{
			//Check if active bounty is null.. and attempt to update again.
			if (ActiveBounty == null && !FunkyGame.Hero.bIsInTown)
			{
				if (DateTime.Now.CompareTo(_lastAttemptedUpdateActiveBounty) > 0)
				{
					_lastAttemptedUpdateActiveBounty = DateTime.Now.AddSeconds(2.5);
					UpdateActiveBounty();
					if (ActiveBounty != null)
					{//No Longer Null.. Do Full Refresh!
						RefreshLevelChanged();
						if (ActiveBountyChanged != null)
							ActiveBountyChanged();
					}
				}
			}
			else if (ActiveBounty != null && (!FunkyGame.Hero.bIsInTown || ActiveBounty.QuestSNO == ADVENTUREMODE_GREATERRIFT_TRIAL || ActiveBounty.QuestSNO == ADVENTUREMODE_RIFTID))
			{
				if (ActiveBounty.QuestSNO == ADVENTUREMODE_RIFTID || ActiveBounty.QuestSNO == ADVENTUREMODE_GREATERRIFT_TRIAL)
				{
					if (DateTime.Now.CompareTo(_lastAttemptedUpdateActiveRift) > 0)
					{
						//Refresh every 10 seconds!
						_lastAttemptedUpdateActiveRift = DateTime.Now.AddSeconds(10);
						RefreshRiftLevelChange();
						if (ActiveBountyChanged != null)
							ActiveBountyChanged();
					}
				}
				else if(CurrentBountyCacheEntry!=null && CurrentBountyCacheEntry.Type== BountyTypes.CursedEvent && DateTime.Now.Subtract(FunkyGame.Targeting.Cache.lastSeenCursedShrine).TotalMilliseconds <= (10000))
				{
					if (DateTime.Now.CompareTo(_lastAttemptedUpdateActiveBounty) > 0)
					{
						_lastAttemptedUpdateActiveBounty = DateTime.Now.AddSeconds(2.5);
						int activebountysno = ActiveBounty.QuestSNO;
						RefreshLevelChanged();
						if ((ActiveBounty == null || ActiveBounty.QuestSNO != activebountysno) && ActiveBountyChanged != null)
							ActiveBountyChanged();
					}
				}
			}
		}
		private DateTime _lastAttemptedUpdateActiveBounty = DateTime.Today;
		private DateTime _lastAttemptedUpdateActiveRift = DateTime.Today;

		public void Reset()
		{
			BountyQuestStates.Clear();
			CurrentBounties.Clear();
			ActiveQuests.Clear();
			CurrentBountyMapMarkers.Clear();
			ActiveBounty = null;
			CurrentBountyCacheEntry = null;
			CurrentAct = Act.Invalid;
			lastCheckedQuestSNO = -1;
			_lastAttemptedUpdateActiveBounty = DateTime.Today;
		}



		public bool IsInRiftWorld
		{
			get
			{

				return TheCache.riftWorldIds.Contains(FunkyGame.Hero.CurrentWorldID);
			}
		}

		public int GetRiftWorldExitHash(int worldID)
		{
			switch (worldID)
			{
				case 288454:
					return 1938876094;
				case 288685:
					return 1938876095;
				case 288687:
					return 1938876096;
				case 288798:
					return 1938876097;
				case 288800:
					return 1938876098;
				case 288802:
					return 1938876099;
				case 288804:
					return 1938876100;
				case 288810:
					return 1938876101;
				case 288814:
					return 1938876102;
			}

			return -1;
		}


		public string DebugString()
		{
			string sBountyIDs = CurrentBounties.Aggregate("Current IDs: ", (current, cb) => current + (cb.Key + " , "));
			string sBountyStates = BountyQuestStates.Aggregate("States: ", (current, sBountyState) => current + (sBountyState.Key + " == " + sBountyState.Value.ToString()));
			string sBountyMapMarkers = CurrentBountyMapMarkers.Values.Aggregate("MapMarkers: ", (current, sBountyMapMarker) => current + ("ID: " + sBountyMapMarker.ID + " WorldID: " + sBountyMapMarker.WorldID + " Position: " + sBountyMapMarker.Position + " Distance: " + FunkyGame.Hero.Position.Distance(sBountyMapMarker.Position) + "\r\n"));
			string sActiveBounty = ActiveBounty != null ? ActiveBounty.ToString() : "NONE!";

			//string sBountyActCache = String.Format("Kills: {0} Clear: {1} CursedEvent: {2}", CurrentActCache.KillBounties.Count, CurrentActCache.ClearBounties.Count, CurrentActCache.CursedEventBounties.Count);

			string sBountyCurrentCacheEntry = CurrentBountyCacheEntry == null ? "Null" :
								String.Format("Name {0} Type {1} StartingLevelID {2} EndingLevelID {3}",
												CurrentBountyCacheEntry.Name, CurrentBountyCacheEntry.Type,
												CurrentBountyCacheEntry.StartingLevelAreaID, CurrentBountyCacheEntry.EndingLevelAreaID);

			string sActiveQuests = ActiveQuests.Count == 0 ? "" :
										ActiveQuests.Aggregate("Active Quest: ", (current, q) => current +
											(String.Format("ID: {4} Step: {0} questMeter: {1} killCount: {2} bonusCount: {3}\r\n",
															q.Value.Step, q.Value.QuestMeter, q.Value.KillCount, q.Value.BonusCount, q.Key)));

			return String.Format("{0}\r\n{1}\r\n{2}\r\n{3}\r\nBountyCacheEntry: {4}\r\n{5}\r\nGreaterRiftIsActiveQuest: {6} RiftTrialIsActiveQuest: {7}",
				sActiveBounty, sBountyIDs, sBountyStates, sBountyMapMarkers, sBountyCurrentCacheEntry, sActiveQuests, GreaterRiftIsActiveQuest, RiftTrialIsActiveQuest);
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
				get { return FunkyGame.Hero.Position.Distance(Position); }
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
				if (p == null)
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

			public QuestState State { get; set; }

			public readonly int QuestSNO;



			public BountyInfoCache(BountyInfo b)
			{
				try
				{
					Act = b.Act;
					State = b.State;
					QuestSNO = b.Info.QuestSNO;

				}
				catch (Exception ex)
				{
					Logger.DBLog.DebugFormat("Failure to create BountyInfoCache \r\n{0}\r\n{1}", ex.Message, ex.StackTrace);
				}
			}
			public BountyInfoCache(QuestInfo quest)
			{
				try
				{
					//Act = (Act)quest.QuestRecord.Act;
					Act = Act.Invalid;
					State = quest.State;
					QuestSNO = quest.QuestSNO;

				}
				catch (Exception ex)
				{
					Logger.DBLog.DebugFormat("Failure to create BountyInfoCache \r\n{0}\r\n{1}", ex.Message, ex.StackTrace);
				}
			}
			public BountyInfoCache(int questSNO, Act act)
			{
				QuestSNO = questSNO;
				Act = act;
			}
			public BountyInfoCache()
			{
				Act = Act.Invalid;
				State = QuestState.NotStarted;
				QuestSNO = -1;
			}

			public virtual void Refresh()
			{
				FunkyGame.Bounty.RefreshBountyQuestStates();
				if (FunkyGame.Bounty.CurrentBounties.ContainsKey(QuestSNO))
				{
					State = FunkyGame.Bounty.BountyQuestStates[QuestSNO];
				}
				else
				{
					Logger.DBLog.Info("Failure to Update Bounty QuestState!");
				}
			}

			public override string ToString()
			{

				return String.Format("QuestSNO: {0} Act: {1} State: {2}",
					QuestSNO, Act, State);
			}
		}

		public class QuestInfoCache : BountyInfoCache
		{
			public int Step { get; set; }
			public int KillCount { get; set; }
			public float QuestMeter { get; set; }
			public int BonusCount { get; set; }
			public int CreationTick { get; set; }

			public QuestInfoCache(QuestInfo info)
				: base(info)
			{
				Step = info.QuestStep;
				KillCount = info.KillCount;
				QuestMeter = info.QuestMeter;
				BonusCount = info.BonusCount;
				CreationTick = info.CreationTick;
			}

			public override void Refresh()
			{
				try
				{
					using (ZetaDia.Memory.AcquireFrame())
					{
						foreach (var quest in ZetaDia.ActInfo.ActiveQuests)
						{
							if (quest.QuestSNO == QuestSNO)
							{
								Step = quest.QuestStep;
								KillCount = quest.KillCount;
								QuestMeter = quest.QuestMeter;
								BonusCount = quest.BonusCount;
								CreationTick = quest.CreationTick;
								return;
							}
						}
					}
				}
				catch (Exception ex)
				{
					Logger.Write(LogLevel.Bounty, "Safely hanlded updating quest info cache for entry {0}", QuestSNO);
				}
			}

			public override string ToString()
			{
				string s = String.Format("{0}\r\nStep {1} KillCount {2} QuestMeter {3} BonusCount {4} CreationTick {5}",
					base.ToString(), Step, KillCount, QuestMeter, BonusCount, CreationTick);

				return s;
			}

		}
	}
}
