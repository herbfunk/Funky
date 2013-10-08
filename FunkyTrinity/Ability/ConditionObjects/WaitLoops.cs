namespace FunkyBot.AbilityFunky
{
	 /// <summary> 
	 /// Describes Pre and Post Wait Loops for an Ability.
	 /// </summary> 
	 public struct WaitLoops
	 {
			public readonly int PreLoops;
			public readonly int PostLoops;
			public readonly bool Reusable;

			public WaitLoops(int BeforeLoops, int AfterLoops, bool Reuse)
			{
				 PreLoops=BeforeLoops;
				 PostLoops=AfterLoops;
				 Reusable=Reuse;
			}
	 }
}