using System;
using System.Linq;
using Zeta;
using Zeta.Internals.Actors;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.CommonBot;
using Zeta.Internals.SNO;
using FunkyTrinity.Enums;
using FunkyTrinity.ability;
using FunkyTrinity.ability.Abilities;
using FunkyTrinity.ability.Abilities.Monk;

namespace FunkyTrinity
{

		  internal class Monk : Player
		  {
				 enum MonkActiveSkills
				 {
						Monk_BreathOfHeaven=69130,
						Monk_MantraOfRetribution=69484,
						Monk_MantraOfHealing=69490,
						Monk_MantraOfConviction=95572,
						Monk_FistsofThunder=95940,
						Monk_DeadlyReach=96019,
						Monk_WaveOfLight=96033,
						Monk_SweepingWind=96090,
						Monk_DashingStrike=96203,
						Monk_Serenity=96215,
						Monk_CripplingWave=96311,
						Monk_SevenSidedStrike=96694,
						Monk_WayOfTheHundredFists=97110,
						Monk_InnerSanctuary=97222,
						Monk_ExplodingPalm=97328,
						Monk_LashingTailKick=111676,
						Monk_TempestRush=121442,
						Monk_MysticAlly=123208,
						Monk_BlindingFlash=136954,
						Monk_MantraOfEvasion=192405,
						Monk_CycloneStrike=223473,
				 }
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

				//Base class for each individual class!
				public Monk(ActorClass a)
					 : base(a)
				{

				}
				public override Ability DefaultAttack
				{
					 get { return new WeaponMeleeInsant(); }
				}

				public override int MainPetCount
				{
					 get
					 {
						  return Bot.Character.PetData.MysticAlly;
					 }
				}
				public override bool IsMeleeClass
				{
					 get
					 {
						  return true;
					 }
				}
				public override bool ShouldGenerateNewZigZagPath()
				{
					 return (DateTime.Now.Subtract(Bot.Combat.lastChangedZigZag).TotalMilliseconds>=1500||
							  (Bot.Combat.vPositionLastZigZagCheck!=Vector3.Zero&&Bot.Character.Position==Bot.Combat.vPositionLastZigZagCheck&&DateTime.Now.Subtract(Bot.Combat.lastChangedZigZag).TotalMilliseconds>=200)||
							  Vector3.Distance(Bot.Character.Position, Bot.Combat.vSideToSideTarget)<=4f||
							  Bot.Target.CurrentTarget!=null&&Bot.Target.CurrentTarget.AcdGuid.HasValue&&Bot.Target.CurrentTarget.AcdGuid.Value!=Bot.Combat.iACDGUIDLastWhirlwind);
				}
				public override void GenerateNewZigZagPath()
				{
					 float fExtraDistance=Bot.Target.CurrentTarget.CentreDistance<=20f?5f:1f;
					 Bot.Combat.vSideToSideTarget=Bot.NavigationCache.FindZigZagTargetLocation(Bot.Target.CurrentTarget.Position, Bot.Target.CurrentTarget.CentreDistance+fExtraDistance);
					 // Resetting this to ensure the "no-spam" is reset since we changed our target location
					 Bot.Combat.powerLastSnoPowerUsed=SNOPower.None;
					 Bot.Combat.iACDGUIDLastWhirlwind=Bot.Target.CurrentTarget.AcdGuid.HasValue?Bot.Target.CurrentTarget.AcdGuid.Value:-1;
					 Bot.Combat.lastChangedZigZag=DateTime.Now;
				}

				private bool HasMantraBuff()
				{
					if (Bot.Class.HotbarPowers.Contains(SNOPower.Monk_MantraOfConviction))
					{
						 return Bot.Class.HasBuff(SNOPower.Monk_MantraOfConviction);
					}
					else if (Bot.Class.HotbarPowers.Contains(SNOPower.Monk_MantraOfEvasion))
					{
						 return Bot.Class.HasBuff(SNOPower.Monk_MantraOfEvasion);
					}
					else if (Bot.Class.HotbarPowers.Contains(SNOPower.Monk_MantraOfHealing))
					{
						 return Bot.Class.HasBuff(SNOPower.Monk_MantraOfHealing);
					}
					else if (Bot.Class.HotbarPowers.Contains(SNOPower.Monk_MantraOfRetribution))
					{
						 return Bot.Class.HasBuff(SNOPower.Monk_MantraOfRetribution);
					}

					 //No Mantra on Hotbar.. we return true since we cannot cast it.
					return true;
				}

				public override void RecreateAbilities()
				{
					 Abilities=new Dictionary<SNOPower, Ability>();

					 //Create the abilities
					 foreach (var item in HotbarPowers)
					 {
							Abilities.Add(item, this.CreateAbility(item));
					 }

					 //No default rage generation ability.. then we add the Instant Melee Ability.
					 if (!HotbarContainsAPrimaryAbility())
					 {
							Ability defaultAbility=this.DefaultAttack;
							Abilities.Add(defaultAbility.Power, defaultAbility);
							RuneIndexCache.Add(defaultAbility.Power, -1);
					 }

					 //Sort Abilities
					 SortedAbilities=Abilities.Values.OrderByDescending(a => a.Priority).ThenBy(a => a.Range).ToList();
					 
					 //Update LOS conditions
					 base.UpdateLOSConditions();

				}

				public override Ability CreateAbility(SNOPower Power)
				{
				  MonkActiveSkills power=(MonkActiveSkills)Enum.ToObject(typeof(MonkActiveSkills), (int)Power);
					
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
							default:
								 return this.DefaultAttack;
					 }
				}

				
				private bool IsHobbling
				{
					 get
					 {
						  Bot.Character.UpdateAnimationState(false);
						  return Bot.Character.CurrentSNOAnim.HasFlag(SNOAnim.Monk_Female_Hobble_Run|SNOAnim.Monk_Male_HTH_Hobble_Run);
					 }
				}
		  }
	 
}