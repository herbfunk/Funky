using System;

namespace FunkyBot.Player.HotBar.Skills
{
	 ///<summary>
	 ///
	 ///</summary>
	 [Flags]
	 public enum ConditionCriteraTypes
	 {
			None=0,
			Cluster=1,
			UnitsInRange=2,
			ElitesInRange=4,
			SingleTarget=8,

		  All=Cluster|UnitsInRange|ElitesInRange|SingleTarget,
	 }
}