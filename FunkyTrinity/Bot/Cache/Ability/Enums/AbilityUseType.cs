using System;

namespace FunkyTrinity.ability
{
	 ///<summary>
	 ///Describes how to use the Ability (SNOPower)
	 ///</summary>
	 [Flags]
	 public enum AbilityUseType
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
	 }
}