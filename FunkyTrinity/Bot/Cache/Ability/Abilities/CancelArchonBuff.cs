using System;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities
{
	public class CancelArchonBuff : Ability, IAbility
	{
		public CancelArchonBuff() : base()
		{
		}

		public override SNOPower Power
		{
			 get { return SNOPower.Wizard_Archon; }
		}

		protected override void Initialize()
		{
			ExecutionType = AbilityUseType.RemoveBuff;
			WaitVars = new WaitLoops(3, 3, true);
		}
	}
}
