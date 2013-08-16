using Zeta.Internals.Actors;
using System;
namespace FunkyTrinity.ability
{
	 public interface IAbility
	 {
		  SNOPower Power { get; }
		  int RuneIndex { get; }

		 void Initialize();
		 void InitCriteria();

		 bool Equals(object obj);
		 int GetHashCode();
	 }
}