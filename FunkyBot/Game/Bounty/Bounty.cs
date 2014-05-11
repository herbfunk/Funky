﻿using System;
using System.Collections.Generic;
using System.Linq;
using FunkyBot.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.SNO;

namespace FunkyBot.Game.Bounty
{
	public class BountyCache
	{
		public Dictionary<int, QuestState> BountyQuestStates = new Dictionary<int, QuestState>();
		public Dictionary<int, BountyInfo> CurrentBounties = new Dictionary<int, BountyInfo>();
		public Dictionary<int, BountyMapMarker> CurrentBountyMapMarkers = new Dictionary<int, BountyMapMarker>();
		public BountyInfoCache ActiveBounty = null;

		public BountyQuestActCache CurrentActCache = new BountyQuestActCache();
		public BountyQuestCacheEntry CurrentBountyCacheEntry = null;
		private bool ShouldNavigateMinimapPoints = false;
		private int lastCheckedQuestSNO = -1;
		private Act CurrentAct = Act.Invalid;

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
		public void RefreshBountyInfo()
		{

			CurrentBounties.Clear();
			foreach (var bounty in ZetaDia.ActInfo.Bounties)
			{
				CurrentBounties.Add(bounty.Info.QuestSNO, bounty);
			}
			RefreshBountyQuestStates();
		}
		///<summary>
		///Refreshes Current Bounty Quest States
		///</summary>
		public void RefreshBountyQuestStates()
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
				var bmm = new BountyMapMarker(m.Position, m.DynamicWorldId, m.Id);
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
					Logger.Write(LogLevel.Bounty, "Active Bounty Changed To {0}", ActiveBounty.QuestSNO);
					//nullify Cache Entry then set it if Cache contains it.
					CurrentBountyCacheEntry = null;
					ShouldNavigateMinimapPoints = false;
				}
				else if (activeBounty.QuestSNO == 0)
				{//nullify when active bounty is nothing
					ActiveBounty = null;
				}
			}
			else
			{
				Logger.Write(LogLevel.Bounty, "Active Bounty Is Null!");
				
				//Unfinished:: Testing to match a possible ID..
				//foreach (var aq in ZetaDia.ActInfo.ActiveQuests)
				//{
				//	int questSNO = aq.QuestSNO;
				//	if (questSNO == 312429) continue;
				//	if (BountyQuestStates.ContainsKey(questSNO))
				//	{
				//		QuestState state = BountyQuestStates[questSNO];
				//		//int act = aq.QuestRecord.Act;

				//		Logger.Write(LogLevel.Bounty, "ID: {0} State: {1}", questSNO, state);
						 
				//		if (state == QuestState.InProgress)
				//		{

				//		}
				//	}
				//}
			}
		}

		
		public void RefreshBountyLevelChange()
		{
			//Logger.DBLog.InfoFormat("Updating Bounty Info!");

			//Do we have any bounties stored?.. if we do refresh states
			if (CurrentBounties.Count == 0) RefreshBountyInfo(); else RefreshBountyQuestStates();

			//If we are in town.. we don't do anything else! (Since the Active Bounty is no longer visible)
			if (Bot.Character.Data.bIsInTown)
			{
				//We could check that active bounty has been completed..
				if (ActiveBounty != null && BountyQuestStates.ContainsKey(ActiveBounty.QuestSNO) && BountyQuestStates[ActiveBounty.QuestSNO]==QuestState.Completed)
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

				#region Load Act Bounty Cache

				if (!ZetaDia.IsInTown)
				{
					if (ActiveBounty.Act != CurrentAct)
					{
						Logger.DBLog.Info(ActiveBounty.Act);

						CurrentAct = ActiveBounty.Act;
						switch (CurrentAct)
						{
							case Act.A1:
								CurrentActCache = BountyQuestActCache.DeserializeFromXML("Act1.xml");
								break;
							case Act.A2:
								break;
							case Act.A3:
								CurrentActCache = BountyQuestActCache.DeserializeFromXML("Act3.xml");
								break;
							case Act.A4:
								break;
							case Act.A5:
								break;
						}
					}
				}
				
				#endregion


				if (CurrentBountyCacheEntry == null)
				{//Attempt to Match a Cache Entry to the QuestSNO.

					if (lastCheckedQuestSNO != ActiveBounty.QuestSNO)
					{//Only attempt search once for the SNO..
 
						lastCheckedQuestSNO = ActiveBounty.QuestSNO;
						var allCacheBounties = CurrentActCache.AllBounties;
						foreach (var b in allCacheBounties.Where(c => c.QuestSNOs != null && c.QuestSNOs.Any(i => i == ActiveBounty.QuestSNO)))
						{
							CurrentBountyCacheEntry = b;
							break;
						}

					}
				}

				if (CurrentBountyCacheEntry == null) return;

				//We Modify the Bots Combat Behavior based on the quest Type and current Level Id..
				Logger.Write(LogLevel.Bounty, "Checking Bounty Type {0}", CurrentBountyCacheEntry.Type);

				if (CurrentBountyCacheEntry.Type==BountyQuestTypes.Clear)
				{
					if (CurrentBountyCacheEntry.EndingLevelAreaID == Bot.Character.Data.iCurrentLevelID)
					{
						Logger.Write(LogLevel.Bounty, "Bounty Level ID Match (Clear) -- Disabling Cluster Logic!");
						ProfileCache.ClusterSettingsTag = SettingCluster.DisabledClustering;
					}
					else
						ProfileCache.ClusterSettingsTag = Bot.Settings.Cluster;
				}

				if (CurrentBountyCacheEntry.Type == BountyQuestTypes.Kill)
				{
					int curLevelID=Bot.Character.Data.iCurrentLevelID;

					if (CurrentBountyCacheEntry.StartingLevelAreaID==curLevelID || CurrentBountyCacheEntry.EndingLevelAreaID==curLevelID ||
					    CurrentBountyCacheEntry.LevelAreaIDs!=null && CurrentBountyCacheEntry.LevelAreaIDs.Any(i => i==curLevelID))
					{
						Logger.Write(LogLevel.Bounty, "Bounty Level ID Match (Kill) -- Disabling Cluster Logic and Enabling Navigation of Points!");
						ProfileCache.ClusterSettingsTag = SettingCluster.DisabledClustering;
						ShouldNavigateMinimapPoints = true;
					}
					else
					{
						ProfileCache.ClusterSettingsTag = Bot.Settings.Cluster;
						ShouldNavigateMinimapPoints = false;
					}
				}

				if (CurrentBountyCacheEntry.Type== BountyQuestTypes.CursedEvent)
				{
					if (CurrentBountyCacheEntry.EndingLevelAreaID == Bot.Character.Data.iCurrentLevelID)
					{
						Logger.Write(LogLevel.Bounty, "Bounty Level ID Match (CursedEvent) -- Enabling Navigation of Points!");
						ShouldNavigateMinimapPoints = true;
						ProfileCache.QuestMode = true;
					}
					else
					{
						ShouldNavigateMinimapPoints = false;
						ProfileCache.QuestMode = false;
					}
				}

			}
		}

		
		///<summary>
		///Check used to validate active bounty (when it fails to update during level changes)
		///</summary>
		public void CheckActiveBounty()
		{
			//Check if active bounty is null.. and attempt to update again.
			if (ActiveBounty == null && !Bot.Character.Data.bIsInTown)
			{
				if (DateTime.Now.CompareTo(_lastAttemptedUpdateActiveBounty) > 0)
				{
					_lastAttemptedUpdateActiveBounty = DateTime.Now.AddSeconds(2.5);
					UpdateActiveBounty();
					if (ActiveBounty != null)
					{//No Longer Null.. Do Full Refresh!
						RefreshBountyLevelChange();
					}
				}
			}
		}
		private DateTime _lastAttemptedUpdateActiveBounty = DateTime.Today;

		public bool ShouldNavigatePointsOfInterest
		{
			get
			{
				return Bot.Game.AdventureMode && (Bot.Settings.AdventureMode.NavigatePointsOfInterest || ShouldNavigateMinimapPoints);
			}
		}

		public void Reset()
		{
			BountyQuestStates.Clear();
			CurrentBounties.Clear();
			CurrentBountyMapMarkers.Clear();
			ActiveBounty = null;
			CurrentBountyCacheEntry = null;
			ShouldNavigateMinimapPoints = false;
			CurrentAct = Act.Invalid;
			lastCheckedQuestSNO = -1;
			_lastAttemptedUpdateActiveBounty=DateTime.Today;
		}

		public string DebugString()
		{
			string sBountyIDs = CurrentBounties.Aggregate("Current IDs: ", (current, cb) => current + (cb.Key + " , "));
			string sBountyStates = BountyQuestStates.Aggregate("States: ", (current, sBountyState) => current + (sBountyState.Key + " == " + sBountyState.Value.ToString()));
			string sBountyMapMarkers = CurrentBountyMapMarkers.Values.Aggregate("MapMarkers: ", (current, sBountyMapMarker) => current + ("ID: " + sBountyMapMarker.ID + " WorldID: " + sBountyMapMarker.WorldID + " Position: " + sBountyMapMarker.Position + " Distance: " + Bot.Character.Data.Position.Distance(sBountyMapMarker.Position) + "\r\n"));
			string sActiveBounty = ActiveBounty != null ? ActiveBounty.ToString() : "NONE!";

			string sBountyActCache = String.Format("Kills: {0} Clear: {1} CursedEvent: {2}", CurrentActCache.KillBounties.Length, CurrentActCache.ClearBounties.Length, CurrentActCache.CursedEventBounties.Length);

			string sBountyCurrentCacheEntry = CurrentBountyCacheEntry == null ? "Null" :
								String.Format("Name {0} Type {1} StartingLevelID {2} EndingLevelID {3}",
												CurrentBountyCacheEntry.Name, CurrentBountyCacheEntry.Type, 
												CurrentBountyCacheEntry.StartingLevelAreaID, CurrentBountyCacheEntry.EndingLevelAreaID);

			return String.Format("{0}\r\n{1}\r\n{2}\r\n{3}\r\nBountyActCache: {4}\r\nBountyCacheEntry: {5}",
				sActiveBounty, sBountyIDs, sBountyStates, sBountyMapMarkers, sBountyActCache, sBountyCurrentCacheEntry);
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

					Act = (Act)quest.QuestRecord.Act;
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
				QuestSNO=questSNO;
				Act = act;
			}
			public BountyInfoCache()
			{
				Act = Act.Invalid;
				State = QuestState.NotStarted;
				QuestSNO = -1;
			}

			public void UpdateState()
			{
				Bot.Game.Bounty.RefreshBountyQuestStates();
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
				
				return String.Format("QuestSNO: {0} Act: {1} State: {2}",
					QuestSNO, Act, State);
			}
		}
	}
}