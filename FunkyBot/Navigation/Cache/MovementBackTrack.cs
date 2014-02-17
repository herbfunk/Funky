using System.Collections.Generic;
using Zeta.Common;

namespace FunkyBot.Movement
{

	internal static class BackTrackCache
	{
		///<summary>
		///The maximum number of GPCs that will be added to cache during routine movement.
		///</summary>
		internal static int MovementGPRectMaxCount = 3;
		///<summary>
		///This sets whether or not to cache GPCs during routine movement.
		///</summary>
		internal static bool EnableBacktrackGPRcache = false;
		///<summary>
		///The minimum range required to cache a new GPC during routine movement.
		///</summary>
		internal static double MinimumRangeBetweenMovementGPRs = 20f;

		///<summary>
		///Cache of GPCs created during routine movement.
		///</summary>
		internal static List<GPRectangle> cacheMovementGPRs = new List<GPRectangle>();

		///<summary>
		///Creates new GPC at given Vector3 with expansion of 3 and adds it to the MovementGPCs list with any links found.
		///</summary>
		public static void StartNewGPR(Vector3 center)
		{//Creates a sample we could backtrack to if needed!

			cacheMovementGPRs.Add(new GPRectangle(center, 3));
			if (cacheMovementGPRs.Count > MovementGPRectMaxCount)
			{
				cacheMovementGPRs.RemoveAt(0);
				cacheMovementGPRs.TrimExcess();
			}
		}
	}

}