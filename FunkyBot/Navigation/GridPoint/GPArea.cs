using System;
using System.Collections.Generic;
using System.Linq;
using Zeta.Common;

namespace FunkyBot.Movement
{

	//Redesign how we use the GPCache by creating it into a instance which contains the entrie area we conduct a search in.


	public class GPArea
	{
		//ToDo: Track object IDs so we can add them to the appropriate GPRect to use for updating.

		//GPArea -- a collection of GPRects which are connected, describes the entire location, holds each point indexed for easier access
		private List<GPRectangle> gridpointrectangles_;
		internal bool AllGPRectsFailed = false;
		internal GPRectangle centerGPRect;
		internal GPRectangle lastUsedGPRect = null;

		public GPArea(Vector3 startingLocation)
		{
			//Creation and Cache base
			centerGPRect = new GPRectangle(startingLocation, 5);
			GPRectangle centerClone = centerGPRect.Clone();

			//Get all valid points (besides current point) from our current location GPR
			GridPoint[] SearchPoints = centerGPRect.Points.Keys.Where(gp => !gp.Ignored).ToArray();
			gridpointrectangles_ = new List<GPRectangle>();
			if (SearchPoints.Length > 1)
			{
				Vector3 LastSearchVector3 = Bot.NavigationCache.LastSearchVector;
				// LastSearchVector3.Normalize();

				//we should check our surrounding points to see if we can even move into any of them first!
				for (int i = 1; i < SearchPoints.Length - 1; i++)
				{
					GridPoint curGP = SearchPoints[i];
					Vector3 thisV3 = (Vector3)curGP;
					//thisV3.Normalize();

					//Its a valid point for direction testing!
					float DirectionDegrees = Navigation.FindDirection(LastSearchVector3, thisV3);
					DirectionPoint P = new DirectionPoint((Vector3)curGP, DirectionDegrees, 125f);

					if (P.Range > 5f)
					{
						gridpointrectangles_.Add(new GPRectangle(P, centerGPRect));
					}
				}
				gridpointrectangles_.Add(centerClone);
				gridpointrectangles_ = gridpointrectangles_.OrderByDescending(gpr => gpr.Count).ToList();
			}
		}

		public bool GridPointContained(GridPoint point)
		{
			return centerGPRect.Contains(point);
		}
		//Blacklisted points used by movement behaviors
		internal List<GridPoint> BlacklistedGridpoints = new List<GridPoint>();
		private bool BlacklistedPoint;
		internal void BlacklistLastSafespot()
		{
			if (lastUsedGPRect != null)
			{
				BlacklistedGridpoints.Add(lastUsedGPRect.LastFoundSafeSpot);
				BlacklistedPoint = true;
			}

		}

		///<summary>
		///Searches for a safespot!
		///</summary>
		public Vector3 AttemptFindSafeSpot(Vector3 CurrentPosition, Vector3 LOS, PointCheckingFlags Flags)
		{
			if (AllGPRectsFailed && Bot.NavigationCache.CurrentGPArea.BlacklistedGridpoints.Count > 0)
			{
				//Reset all blacklist to retry again.
				AllGPRectsFailed = false;
				//Clear Blacklisted
				Bot.NavigationCache.CurrentGPArea.BlacklistedGridpoints.Clear();
			}


			Vector3 safespot;
			//Check if we actually created any surrounding GPCs..
			if (gridpointrectangles_.Count > 0)
			{
				iterateGPRectsSafeSpot(CurrentPosition, out safespot, LOS, Flags);
				//If still failed to find a safe spot.. set the timer before we try again.
				if (safespot == Vector3.Zero)
				{
					Logging.WriteVerbose("All GPCs failed to find a valid location to move!");

					AllGPRectsFailed = true;
					return safespot;
				}
				//Cache it and set timer
				Bot.NavigationCache.lastFoundSafeSpot = DateTime.Now;
				Bot.NavigationCache.vlastSafeSpot = safespot;
			}
			//Logging.WriteVerbose("Safespot location {0} distance from {1} is {2}", safespot.ToString(), LastSearchVector.ToString(), safespot.Distance2D(LastSearchVector));
			return Bot.NavigationCache.vlastSafeSpot;
		}

		private int lastGPRectIndexUsed;
		private void iterateGPRectsSafeSpot(Vector3 CurrentPosition, out Vector3 safespot, Vector3 LOS, PointCheckingFlags Flags)
		{
			//blacklisted a point?.. we advance to next index!
			if (BlacklistedPoint)
			{
				lastGPRectIndexUsed++;
				BlacklistedPoint = false;
			}

			GPRectangle PositionRect = GetGPRectContainingPoint(CurrentPosition);
			if (PositionRect != null) PositionRect.UpdateObjectCount();

			GPQuadrant PositionQuadrant = PositionRect.GetQuadrantContainingPoint(CurrentPosition);
			double CompareWeight = PositionQuadrant != null ? PositionQuadrant.ThisWeight : PositionRect != null ? PositionRect.Weight : 0;

			safespot = Vector3.Zero;
			for (int i = lastGPRectIndexUsed; i < gridpointrectangles_.Count - 1; i++)
			{
				GPRectangle item = gridpointrectangles_[i];

				item.UpdateObjectCount(AllGPRectsFailed);

				if (item.TryFindSafeSpot(CurrentPosition, out safespot, LOS, Flags, BlacklistedGridpoints, AllGPRectsFailed, CompareWeight))
				{
					lastUsedGPRect = gridpointrectangles_[i];
					return;
				}
			}
			lastGPRectIndexUsed = 0;
		}
		private GPRectangle GetGPRectContainingPoint(GridPoint Point)
		{
			foreach (var item in gridpointrectangles_)
			{
				if (item.Contains(Point))
					return item;
			}

			return null;
		}
	}


}