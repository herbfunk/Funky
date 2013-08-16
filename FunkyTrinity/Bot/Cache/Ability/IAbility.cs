using Zeta.Internals.Actors;
using System;
namespace FunkyTrinity.ability
{
	 public interface IAbility
	 {
		  SNOPower Power { get; }
		  int RuneIndex { get; }
		
		 bool Equals(object obj);
		 int GetHashCode();
	 }
}