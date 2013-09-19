using System.Collections.Generic;
using Zeta.Internals.Actors;
using Zeta.Internals.SNO;
namespace FunkyTrinity.Ability
{
	 ///<summary>
	 ///Conditions used in LOS checking
	 ///</summary>
	public class LOSConditions
	{
		 public bool RequiresRayCast { get; set; }
		 public bool RequiresServerObjectIntersection { get; set; }
		 public NavCellFlags NavCellFlags { get; set; }

		 public LOSConditions(Dictionary<SNOPower, ability> abilities, bool raycast=true)
		 {
			  //check the abilities
			  bool objectIntersection=false;
			  bool projectileTest=false;
			  bool walkTest=false;
			  foreach (var ability in abilities.Values)
			  {
					if (ability.IsRanged)
					{
						 projectileTest=true;
						 if (ability.IsProjectile) objectIntersection=true;
					}
					else if (ability.Range>0)
						 walkTest=true;
			  }


			  RequiresRayCast=raycast;
			  NavCellFlags=NavCellFlags.None;
			  if (projectileTest) NavCellFlags|=NavCellFlags.AllowProjectile;
			  if (walkTest) NavCellFlags|=NavCellFlags.AllowWalk;
			  RequiresServerObjectIntersection=objectIntersection;
		 }
	}
}
