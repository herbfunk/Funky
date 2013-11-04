using System;
using System.Linq;
using FunkyBot.AbilityFunky;
using FunkyBot.AbilityFunky.Abilities;
using Zeta;
using Zeta.Internals.Actors;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.CommonBot;
using Zeta.Internals.SNO;
using FunkyBot.Cache;

namespace FunkyBot.Character
{

		  internal class Barbarian : Player
		  {

				//Base class for each individual class!
				public Barbarian()
					 : base()
				{
				}
				internal override ActorClass AC { get { return ActorClass.Barbarian; } }

				public override Ability DefaultAttack
				{
					 get { return new WeaponMeleeInsant(); }
				}

				public override bool IsMeleeClass
				{
					 get
					 {
						  return true;
					 }
				}
				private HashSet<SNOAnim> knockbackanims=new HashSet<SNOAnim>
				{
					 SNOAnim.Barbarian_Female_1HS_knockback_land_01, 
					 SNOAnim.Barbarian_Female_1HT_Knockback_Land, 
					 SNOAnim.Barbarian_Female_2HS_Knockback_Land_01, 
					 SNOAnim.Barbarian_Female_2HT_Knockback_Land_01, 
					 SNOAnim.Barbarian_Female_DW_Knockback_Land_01, 
					 SNOAnim.Barbarian_Female_HTH_knockback_land, 
					 SNOAnim.Barbarian_Female_STF_Knockback_Land_01,
					 SNOAnim.barbarian_male_1HS_Knockback_Land_01, 
					 SNOAnim.barbarian_male_1HT_knockback_land,
					 SNOAnim.barbarian_male_DW_Knockback_Land_01,
					 SNOAnim.barbarian_male_2HT_Knockback_Land_01,
					 SNOAnim.barbarian_male_STF_Knockback_Land_01,
					 SNOAnim.barbarian_male_2HS_Knockback_Land_01,
				};
				public override HashSet<SNOAnim> KnockbackLandAnims
				{
					 get
					 {
						  return this.knockbackanims;
					 }
				}


				public override bool ShouldGenerateNewZigZagPath()
				{
					 return (DateTime.Now.Subtract(Bot.NavigationCache.lastChangedZigZag).TotalMilliseconds>=2000f||
								(Bot.NavigationCache.vPositionLastZigZagCheck!=Vector3.Zero&&Bot.Character.Position==Bot.NavigationCache.vPositionLastZigZagCheck&&DateTime.Now.Subtract(Bot.NavigationCache.lastChangedZigZag).TotalMilliseconds>=1200)||
								Vector3.Distance(Bot.Character.Position, Bot.NavigationCache.vSideToSideTarget)<=5f||
								Bot.Targeting.CurrentTarget!=null&&Bot.Targeting.CurrentTarget.AcdGuid.HasValue&&Bot.Targeting.CurrentTarget.AcdGuid.Value!=Bot.NavigationCache.iACDGUIDLastWhirlwind);
				}
				public override void GenerateNewZigZagPath()
				{
					 if (Bot.Targeting.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_30]>=6||Bot.Targeting.Environment.iElitesWithinRange[(int)RangeIntervals.Range_30]>=3)
						  Bot.NavigationCache.vSideToSideTarget=Bot.NavigationCache.FindZigZagTargetLocation(Bot.Targeting.CurrentTarget.Position, 25f, false, true);
					 else
						  Bot.NavigationCache.vSideToSideTarget=Bot.NavigationCache.FindZigZagTargetLocation(Bot.Targeting.CurrentTarget.Position, 25f);

					 Bot.NavigationCache.iACDGUIDLastWhirlwind=Bot.Targeting.CurrentTarget.AcdGuid.HasValue?Bot.Targeting.CurrentTarget.AcdGuid.Value:-1;
					 Bot.NavigationCache.lastChangedZigZag=DateTime.Now;
				}

				public override void RecreateAbilities()
				{
					 base.RecreateAbilities();
				}

				public override Ability CreateAbility(SNOPower Power)
				{
					 BarbarianActiveSkills power=(BarbarianActiveSkills)Enum.ToObject(typeof(BarbarianActiveSkills), (int)Power);
					

					switch (power)
					{
						 case BarbarianActiveSkills.Barbarian_AncientSpear:
								return new AbilityFunky.Abilities.Barb.AncientSpear();
						 case BarbarianActiveSkills.Barbarian_Rend:
							return new AbilityFunky.Abilities.Barb.Rend();
						 case BarbarianActiveSkills.Barbarian_Frenzy:
								return new AbilityFunky.Abilities.Barb.Frenzy();
						 case BarbarianActiveSkills.Barbarian_Sprint:
								return new AbilityFunky.Abilities.Barb.Sprint();
						 case BarbarianActiveSkills.Barbarian_BattleRage:
								return new AbilityFunky.Abilities.Barb.Battlerage();
						 case BarbarianActiveSkills.Barbarian_ThreateningShout:
								return new AbilityFunky.Abilities.Barb.ThreateningShout();
						 case BarbarianActiveSkills.Barbarian_Bash:
								return new AbilityFunky.Abilities.Barb.Bash();
						 case BarbarianActiveSkills.Barbarian_GroundStomp:
								return new AbilityFunky.Abilities.Barb.GroundStomp();
						 case BarbarianActiveSkills.Barbarian_IgnorePain:
								return new AbilityFunky.Abilities.Barb.IgnorePain();
						 case BarbarianActiveSkills.Barbarian_WrathOfTheBerserker:
								return new AbilityFunky.Abilities.Barb.WrathOfTheBerserker();
						 case BarbarianActiveSkills.Barbarian_HammerOfTheAncients:
								return new AbilityFunky.Abilities.Barb.HammeroftheAncients();
						 case BarbarianActiveSkills.Barbarian_CallOfTheAncients:
								return new AbilityFunky.Abilities.Barb.CalloftheAncients();
						 case BarbarianActiveSkills.Barbarian_Cleave:
								return new AbilityFunky.Abilities.Barb.Cleave();
						 case BarbarianActiveSkills.Barbarian_WarCry:
								return new AbilityFunky.Abilities.Barb.Warcry();
						 case BarbarianActiveSkills.Barbarian_SeismicSlam:
								return new AbilityFunky.Abilities.Barb.SeismicSlam();
						 case BarbarianActiveSkills.Barbarian_Leap:
								return new AbilityFunky.Abilities.Barb.Leap();
						 case BarbarianActiveSkills.Barbarian_WeaponThrow:
								return new AbilityFunky.Abilities.Barb.WeaponThrow();
						 case BarbarianActiveSkills.Barbarian_Whirlwind:
								return new AbilityFunky.Abilities.Barb.Whirlwind();
						 case BarbarianActiveSkills.Barbarian_FuriousCharge:
								return new AbilityFunky.Abilities.Barb.FuriousCharge();
						 case BarbarianActiveSkills.Barbarian_Earthquake:
								return new AbilityFunky.Abilities.Barb.Earthquake();
						 case BarbarianActiveSkills.Barbarian_Revenge:
								return new AbilityFunky.Abilities.Barb.Revenge();
						 case BarbarianActiveSkills.Barbarian_Overpower:
								return new AbilityFunky.Abilities.Barb.Overpower();
						 default:
								return this.DefaultAttack;
					}


				}

				enum BarbarianActiveSkills
				{
					 Barbarian_AncientSpear=69979,
					 Barbarian_Rend=70472,
					 Barbarian_Frenzy=78548,
					 Barbarian_Sprint=78551,
					 Barbarian_BattleRage=79076,
					 Barbarian_ThreateningShout=79077,
					 Barbarian_Bash=79242,
					 Barbarian_GroundStomp=79446,
					 Barbarian_IgnorePain=79528,
					 Barbarian_WrathOfTheBerserker=79607,
					 Barbarian_HammerOfTheAncients=80028,
					 Barbarian_CallOfTheAncients=80049,
					 Barbarian_Cleave=80263,
					 Barbarian_WarCry=81612,
					 Barbarian_SeismicSlam=86989,
					 Barbarian_Leap=93409,
					 Barbarian_WeaponThrow=93885,
					 Barbarian_Whirlwind=96296,
					 Barbarian_FuriousCharge=97435,
					 Barbarian_Earthquake=98878,
					 Barbarian_Revenge=109342,
					 Barbarian_Overpower=159169,
				}
				enum BarbarianPassiveSkills
				{
					 Barbarian_Passive_BoonOfBulKathos=204603,
					 Barbarian_Passive_NoEscape=204725,
					 Barbarian_Passive_Brawler=205133,
					 Barbarian_Passive_Ruthless=205175,
					 Barbarian_Passive_BerserkerRage=205187,
					 Barbarian_Passive_PoundOfFlesh=205205,
					 Barbarian_Passive_Bloodthirst=205217,
					 Barbarian_Passive_Animosity=205228,
					 Barbarian_Passive_Unforgiving=205300,
					 Barbarian_Passive_Relentless=205398,
					 Barbarian_Passive_Superstition=205491,
					 Barbarian_Passive_InspiringPresence=205546,
					 Barbarian_Passive_Juggernaut=205707,
					 Barbarian_Passive_ToughAsNails=205848,
					 Barbarian_Passive_WeaponsMaster=206147,
				}
		  }
	 
}