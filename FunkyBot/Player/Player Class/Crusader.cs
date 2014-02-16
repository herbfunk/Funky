using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FunkyBot.Player.HotBar.Skills;
using Zeta;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.Class
{
	internal class Crusader : PlayerClass
	{
		public override ActorClass AC { get { return ActorClass.Invalid; } }

		internal override Skill DefaultAttack
		{
			get { return new WeaponMeleeInsant(); }
		}

		internal override bool IsMeleeClass
		{
			get
			{
				return true;
			}
		}

		private readonly HashSet<SNOAnim> knockbackanims = new HashSet<SNOAnim>
				{
					 SNOAnim.Invalid,
				};
		internal override HashSet<SNOAnim> KnockbackLandAnims
		{
			get
			{
				return knockbackanims;
			}
		}

		//internal override Skill CreateAbility(SNOPower Power)
		//{
		//	//WitchDoctorActiveSkills power = (WitchDoctorActiveSkills)Enum.ToObject(typeof(WitchDoctorActiveSkills), (int)Power);

		//	//switch (power)
		//	//{
		//	//	default:
		//	//		return DefaultAttack;
		//	//}
		//}

	}
}
