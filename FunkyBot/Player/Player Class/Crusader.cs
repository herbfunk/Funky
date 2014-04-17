using System;
using System.Collections.Generic;
using FunkyBot.Player.HotBar.Skills;
using FunkyBot.Player.HotBar.Skills.Crusader;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.Class
{
	internal class Crusader : PlayerClass
	{
		public override ActorClass AC { get { return ActorClass.Crusader; } }

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

		internal override Skill CreateAbility(SNOPower Power)
		{
			CrusaderActiveSkills power = (CrusaderActiveSkills)Enum.ToObject(typeof(CrusaderActiveSkills), (int)Power);

			switch (power)
			{
				case CrusaderActiveSkills.Crusader_ShieldBash:
					return new ShieldBash();
				case CrusaderActiveSkills.Crusader_SweepAttack:
					return new SweepAttack();
				case CrusaderActiveSkills.Crusader_FallingSword:
					return new FallingSword();
				case CrusaderActiveSkills.Crusader_FistOfTheHeavens:
					return new FistOfTheHeavens();
				case CrusaderActiveSkills.Crusader_SteedCharge:
					return new SteedCharge();
				case CrusaderActiveSkills.Crusader_Condemn:
					return new Condemn();
				case CrusaderActiveSkills.Crusader_LawsOfJustice:
					return new LawsOfJustice();
				case CrusaderActiveSkills.Crusader_BlessedHammer:
					return new BlessedHammer();
				case CrusaderActiveSkills.Crusader_BlessedShield:
					return new BlessedShield();
				case CrusaderActiveSkills.Crusader_Judgment:
					return new Judgement();
				//case CrusaderActiveSkills.Crusader_CrushingResolve:
				//	return new Resolve();
				case CrusaderActiveSkills.Crusader_ShieldGlare:
					return new ShieldGlare();
				case CrusaderActiveSkills.Crusader_AkaratsChampion:
					return new AkaratsChampion();
				case CrusaderActiveSkills.Crusader_Consecration:
					return new Consecration();
				case CrusaderActiveSkills.Crusader_Bombardment:
					return new Bombardment();
				case CrusaderActiveSkills.Crusader_Punish:
					return new Punish();
				case CrusaderActiveSkills.Crusader_Smite:
					return new Smite();
				case CrusaderActiveSkills.Crusader_Slash:
					return new Slash();
				case CrusaderActiveSkills.Crusader_Provoke:
					return new Provoke();
				case CrusaderActiveSkills.Crusader_LawsOfHope:
					return new LawsOfHope();
				case CrusaderActiveSkills.Crusader_LawsOfValor:
					return new LawsOfValor();
				case CrusaderActiveSkills.Crusader_IronSkin:
					return new IronSkin();
				case CrusaderActiveSkills.Crusader_HeavensFury3:
					return new HeavensFury();
				case CrusaderActiveSkills.Crusader_Justice:
					return new Justice();
				case CrusaderActiveSkills.Crusader_Phalanx3:
					return new Phalanx();
				case CrusaderActiveSkills.Crusader_LawsOfHope2:
					return new LawsOfHope();
				case CrusaderActiveSkills.Crusader_LawsOfJustice2:
					return new LawsOfJustice();
				case CrusaderActiveSkills.Crusader_LawsOfValor2:
					return new LawsOfValor();
				default:
					return DefaultAttack;
			}
		}

		enum CrusaderActiveSkills
		{
			Crusader_ShieldBash = 353492,
			Crusader_SweepAttack = 239042,
			Crusader_FallingSword = 239137,
			Crusader_FistOfTheHeavens = 239218,
			Crusader_SteedCharge = 243853,
			Crusader_Condemn = 266627,
			Crusader_LawsOfJustice = 266722,
			Crusader_BlessedHammer = 266766,
			Crusader_BlessedShield = 266951,
			Crusader_Judgment = 267600,
			Crusader_CrushingResolve = 267818,
			Crusader_ShieldGlare = 268530,
			Crusader_AkaratsChampion = 269032,
			Crusader_Consecration = 273941,
			Crusader_Bombardment = 284876,
			Crusader_Punish = 285903,
			Crusader_Smite = 286510,
			Crusader_Slash = 289243,
			Crusader_Provoke = 290545,
			Crusader_LawsOfHope = 290912,
			Crusader_LawsOfValor = 290946,
			Crusader_LawsOfFate = 290960,
			Crusader_IronSkin = 291804,
			Crusader_HeavensFury3 = 316014,
			Crusader_Justice = 325216,
			Crusader_Phalanx3 = 330729,
			Crusader_LawsOfHope2 = 342279,
			Crusader_LawsOfJustice2 = 342280,
			Crusader_LawsOfValor2 = 342281,

		}

	}
}
