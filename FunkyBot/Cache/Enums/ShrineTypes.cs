using System;

namespace FunkyBot.Cache.Enums
{
	 [Flags]
	 public enum ShrineTypes
	 {
			None=0,
			Fleeting=1,
			Enlightenment=2,
			Frenzy=4,
			Fortune=8,
			Protection=16,
			Empowered=32,

			Channeling=64,
			Speed=128,
			Shield=256,
			Power=512,
		    Conduit=1024, 

		 Normal=Fleeting|Enlightenment|Frenzy|Fortune|Protection|Empowered,
		 Pylon=Channeling|Speed|Shield|Power|Conduit
	 }

}