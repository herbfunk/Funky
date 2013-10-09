using System;

namespace FunkyBot.Movement.Clustering
{
	[Flags]
	public enum ClusterProperties
	{
		None=0,

		Weak=1,
		Strong=2,
		  
		Large=4,
		Small=8,
		  
		Elites=16,
		Boss=32,

		Fast=64,
		Ranged=128,

	}
}