using System;

namespace FunkyTrinity.Ability
{
	 ///<summary>
	 ///Conditions used to determine if ability is capable of use.
	 ///</summary>
	 [Flags]
	 public enum AbilityPreCastFlags
	 {
			None=0,
			CheckEnergy=1,
			CheckExisitingBuff=2,
			CheckPetCount=4,
			CheckRecastTimer=8,
			CheckCanCast=16,
			CheckPlayerIncapacitated=32,
			CheckPlayerRooted=64,
	 }
}