using Zeta.Internals.Actors;
using System;
namespace FunkyTrinity.Ability
{
	 public interface IAbility
	 {
		  SNOPower Power { get; }
		  int RuneIndex { get; }

		 void Initialize();

		 bool Equals(object obj);
		 int GetHashCode();
	 }
}