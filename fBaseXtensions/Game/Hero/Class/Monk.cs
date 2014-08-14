using System;
using System.Collections.Generic;
using System.Linq;
using fBaseXtensions.Game.Hero.Skills;
using fBaseXtensions.Game.Hero.Skills.SkillObjects;
using fBaseXtensions.Game.Hero.Skills.SkillObjects.Monk;
using fBaseXtensions.Items.Enums;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = fBaseXtensions.Helpers.Logger;

namespace fBaseXtensions.Game.Hero.Class
{

	internal class Monk : PlayerClass
	{
		//Base class for each individual class!
		public Monk()
		{
			//Monk Inna Full Set
			int InnaSetItemCount = Equipment.EquippedItems.Count(i => i.LegendaryItemType==LegendaryItemTypes.InnasMantra);
			if (InnaSetItemCount > 2 || InnaSetItemCount > 1 && Equipment.RingOfGrandeur)
			{
				Logger.DBLog.InfoFormat("Monk has inna set! (reduced sweeping wind cost)");
				FunkyBaseExtension.Settings.Monk.bMonkInnaSet = true;
			}
			else
				FunkyBaseExtension.Settings.Monk.bMonkInnaSet = false;

			int RotsSetItemCount = Equipment.EquippedItems.Count(i => i.LegendaryItemType == LegendaryItemTypes.RaimentofaThousandStorms);
			if (RotsSetItemCount>5 || RotsSetItemCount>4 && Equipment.RingOfGrandeur)
			{
				Logger.DBLog.InfoFormat("Monk has full Rainments of Thousand Storm Set!");
				FunkyBaseExtension.Settings.Monk.RainmentsOfThousandStormsFiveBonus = true;
			}
			else
				FunkyBaseExtension.Settings.Monk.RainmentsOfThousandStormsFiveBonus = false;

			//Combo Strike???
			if (Hotbar.PassivePowers.Contains(SNOPower.Monk_Passive_CombinationStrike))
			{
				Logger.DBLog.InfoFormat("Combination Strike Found!");
				FunkyBaseExtension.Settings.Monk.bMonkComboStrike = true;
				int TotalAbilities = Hotbar.HotbarSkills.Count(Skill => SpiritGeneratingAbilities.Contains(Skill.Power));
				FunkyBaseExtension.Settings.Monk.iMonkComboStrikeAbilities = TotalAbilities;
			}
			else
			{
				FunkyBaseExtension.Settings.Monk.bMonkComboStrike = false;
				FunkyBaseExtension.Settings.Monk.iMonkComboStrikeAbilities = 0;
			}

			Logger.DBLog.DebugFormat("[Funky] Using Monk Player Class");

		}
		public override ActorClass AC { get { return ActorClass.Monk; } }
		private readonly HashSet<SNOPower> SpiritGeneratingAbilities = new HashSet<SNOPower>
				{
					 SNOPower.Monk_FistsofThunder,
					 SNOPower.Monk_WayOfTheHundredFists,
					 SNOPower.Monk_DeadlyReach,
					 SNOPower.Monk_CripplingWave,
				};
		private readonly HashSet<SNOAnim> knockbackanims_Male = new HashSet<SNOAnim>
		{
					 SNOAnim.Monk_Male_HTH_knockback_land_01,
					 SNOAnim.Monk_Male_1HF_knockback_land_01,
					 SNOAnim.Monk_Male_DW_SF_knockback_land_01,
					 SNOAnim.Monk_Male_DW_FF_knockback_land_01,
					 SNOAnim.Monk_Male_1HS_knockback_land,
					 SNOAnim.Monk_Male_STF_knockback_land,
					 SNOAnim.Monk_Male_2HT_knockback_land,
		};
		private readonly HashSet<SNOAnim> knockbackanims_Female = new HashSet<SNOAnim>
		{
					 SNOAnim.Monk_Female_STF_knockback_land,
					 SNOAnim.Monk_Female_1HS_knockback_land,
					 SNOAnim.Monk_Female_2HT_knockback_land,
					 SNOAnim.Monk_Female_HTH_knockback_land_01,
					 SNOAnim.Monk_Female_DW_SS_knockback_land_01,
					 SNOAnim.Monk_Female_DW_SF_knockback_land_01,
					 SNOAnim.Monk_Female_DW_FF_knockback_land_01,
					 SNOAnim.Monk_Female_1HF_knockback_land_01,
		};

		internal override HashSet<SNOAnim> KnockbackLandAnims
		{
			get
			{
				return FunkyGame.Hero.SnoActor == SNOActor.Monk_Female ? knockbackanims_Female : knockbackanims_Male;
			}
		}
		internal override Skill DefaultAttack
		{
			get { return new WeaponMeleeInsant(); }
		}

		internal override int MainPetCount
		{
			get
			{
				return FunkyGame.Targeting.Cache.Environment.HeroPets.MysticAlly;
			}
		}
		internal override bool IsMeleeClass
		{
			get
			{
				return true;
			}
		}
		internal override bool ShouldGenerateNewZigZagPath()
		{
			return (DateTime.Now.Subtract(FunkyGame.Navigation.lastChangedZigZag).TotalMilliseconds >= 1500 ||
					 (FunkyGame.Navigation.vPositionLastZigZagCheck != Vector3.Zero && FunkyGame.Hero.Position == FunkyGame.Navigation.vPositionLastZigZagCheck && DateTime.Now.Subtract(FunkyGame.Navigation.lastChangedZigZag).TotalMilliseconds >= 200) ||
					 Vector3.Distance(FunkyGame.Hero.Position, FunkyGame.Navigation.vSideToSideTarget) <= 4f ||
					 FunkyGame.Targeting.Cache.CurrentTarget != null && FunkyGame.Targeting.Cache.CurrentTarget.AcdGuid.HasValue && FunkyGame.Targeting.Cache.CurrentTarget.AcdGuid.Value != FunkyGame.Navigation.iACDGUIDLastWhirlwind);
		}
		internal override void GenerateNewZigZagPath()
		{
			float fExtraDistance = FunkyGame.Targeting.Cache.CurrentTarget.CentreDistance <= 20f ? 5f : 1f;
			FunkyGame.Navigation.vSideToSideTarget = FunkyGame.Navigation.FindZigZagTargetLocation(FunkyGame.Targeting.Cache.CurrentTarget.Position, FunkyGame.Targeting.Cache.CurrentTarget.CentreDistance + fExtraDistance);
			// Resetting this to ensure the "no-spam" is reset since we changed our target location

			FunkyGame.Navigation.iACDGUIDLastWhirlwind = FunkyGame.Targeting.Cache.CurrentTarget.AcdGuid.HasValue ? FunkyGame.Targeting.Cache.CurrentTarget.AcdGuid.Value : -1;
			FunkyGame.Navigation.lastChangedZigZag = DateTime.Now;
		}


		internal override Skill CreateAbility(SNOPower Power)
		{
			MonkActiveSkills power = (MonkActiveSkills)Enum.ToObject(typeof(MonkActiveSkills), (int)Power);

			switch (power)
			{
				case MonkActiveSkills.Monk_BreathOfHeaven:
					return new BreathofHeaven();
				case MonkActiveSkills.Monk_MantraOfRetribution:
					return new MantraOfRetribution();
				case MonkActiveSkills.Monk_MantraOfHealing:
					return new MantraOfHealing();
				case MonkActiveSkills.Monk_MantraOfConviction:
					return new MantraOfConviction();
				case MonkActiveSkills.Monk_FistsofThunder:
					return new FistsofThunder();
				case MonkActiveSkills.Monk_DeadlyReach:
					return new DeadlyReach();
				case MonkActiveSkills.Monk_WaveOfLight:
					return new WaveOfLight();
				case MonkActiveSkills.Monk_SweepingWind:
					return new SweepingWind();
				case MonkActiveSkills.Monk_DashingStrike:
					return new DashingStrike();
				case MonkActiveSkills.Monk_Serenity:
					return new Serenity();
				case MonkActiveSkills.Monk_CripplingWave:
					return new CripplingWave();
				case MonkActiveSkills.Monk_SevenSidedStrike:
					return new SevenSidedStrike();
				case MonkActiveSkills.Monk_WayOfTheHundredFists:
					return new WayOfTheHundredFists();
				case MonkActiveSkills.Monk_InnerSanctuary:
					return new InnerSanctuary();
				case MonkActiveSkills.Monk_ExplodingPalm:
					return new ExplodingPalm();
				case MonkActiveSkills.Monk_LashingTailKick:
					return new LashingTailKick();
				case MonkActiveSkills.Monk_TempestRush:
					return new TempestRush();
				case MonkActiveSkills.Monk_MysticAlly:
					return new MysticAlly();
				case MonkActiveSkills.Monk_BlindingFlash:
					return new BlindingFlash();
				case MonkActiveSkills.Monk_MantraOfEvasion:
					return new MantraOfEvasion();
				case MonkActiveSkills.Monk_CycloneStrike:
					return new CycloneStrike();
				case MonkActiveSkills.Monk_Epiphany:
					return new Epiphany();
				default:
					return DefaultAttack;
			}
		}

		enum MonkActiveSkills
		{
			Monk_BreathOfHeaven = 69130,
			Monk_MantraOfRetribution = 375082,
			Monk_MantraOfHealing = 373143,
			Monk_MantraOfConviction = 375088,
			Monk_FistsofThunder = 95940,
			Monk_DeadlyReach = 96019,
			Monk_WaveOfLight = 96033,
			Monk_SweepingWind = 96090,
			Monk_DashingStrike = 312736,
			Monk_Serenity = 96215,
			Monk_CripplingWave = 96311,
			Monk_SevenSidedStrike = 96694,
			Monk_WayOfTheHundredFists = 97110,
			Monk_InnerSanctuary = 317076,
			Monk_ExplodingPalm = 97328,
			Monk_LashingTailKick = 111676,
			Monk_TempestRush = 121442,
			Monk_MysticAlly = 362102,
			Monk_BlindingFlash = 136954,
			Monk_MantraOfEvasion = 375049,
			Monk_CycloneStrike = 223473,
			Monk_Epiphany = 312307,
		}
		/*
						enum MonkPassiveSkills
						{
							 Monk_Passive_CombinationStrike=218415,
							 Monk_Passive_Resolve=211581,
							 Monk_Passive_TheGuardiansPath=209812,
							 Monk_Passive_Pacifism=209813,
							 Monk_Passive_SixthSense=209622,
							 Monk_Passive_SeizeTheInitiative=209628,
							 Monk_Passive_OneWithEverything=209656,
							 Monk_Passive_Transcendence=209250,
							 Monk_Passive_BeaconOfYtar=209104,
							 Monk_Passive_ExaltedSoul=209027,
							 Monk_Passive_FleetFooted=209029,
							 Monk_Passive_ChantOfResonance=156467,
							 Monk_Passive_NearDeathExperience=156484,
							 Monk_Passive_GuidingLight=156492,

						}
		*/

	}

}