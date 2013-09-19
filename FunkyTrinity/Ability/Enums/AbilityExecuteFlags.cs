using System;

namespace FunkyTrinity.Ability
{
	 ///<summary>
	 ///Describes how to use the ability (SNOPower)
	 ///</summary>
	 [Flags]
	 public enum AbilityExecuteFlags
	 {
			None=0,
			Buff=1,
			Location=2,
			Target=4,
			ClusterTarget=8,
			ClusterLocation=16,
			ZigZagPathing=32,
			Self=64,
			RemoveBuff=128,
			ClusterTargetNearest=256,
	 }
}