using Zeta.Internals.Actors;
using System;
namespace FunkyTrinity.ability
{
	 public interface IAbility
	 {
		  SNOPower Power { get; }
		  int RuneIndex { get; }
			AbilityConditions PreCastConditions { get; set; }
			ClusterConditions ClusterConditions { get; set; }
		 void SetupAbilityForUse();
		 void UsePower();
		 bool Equals(object obj);
		 int GetHashCode();
	 }
}