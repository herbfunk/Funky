using System;

namespace FunkyTrinity.ability
{
	 ///<summary>
	 ///Describes how to use the Ability (SNOPower)
	 ///</summary>
	 [Flags]
	 public enum PowerExecutionTypes
	 {
			None=0,
			Buff=1, //no location or target raguid
			Location=2,
			Target=4,
			ClusterTarget=8,
			ClusterLocation=16,
			ZigZagPathing=32,
			Self=64, //bot location, no target raguid
			RemoveBuff=128,
			ClusterTargetNearest=256,
	 }
}