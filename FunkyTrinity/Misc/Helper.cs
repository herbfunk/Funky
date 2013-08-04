using System;
using FunkyTrinity.Enums;

namespace FunkyTrinity
{
	 public partial class Funky
	 {


			internal static ShrineTypes FindShrineType(int SNOID)
			{
				 switch (SNOID)
				 {
						case 176075:
							 return ShrineTypes.Enlightenment;
						case 176077:
							 return ShrineTypes.Frenzy;
						case 176074:
							 return ShrineTypes.Protection;
						case 176076:
							 return ShrineTypes.Fortune;
						case 260331:
							 return ShrineTypes.Fleeting;
						default:
							 return ShrineTypes.Empowered; //260330
				 }
			}








	 }
}