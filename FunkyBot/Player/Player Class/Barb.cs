using System;
using FunkyBot.Player.HotBar.Skills;
using FunkyBot.Player.HotBar.Skills.Barb;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.Class
{

	internal class Barbarian : PlayerClass
	{
		public Barbarian()
		{
			Logger.DBLog.DebugFormat("[Funky] Using Barbarian Player Class");
		}
		//Base class for each individual class!
		public override ActorClass AC { get { return ActorClass.Barbarian; } }

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
		internal override HashSet<SNOAnim> KnockbackLandAnims
		{
			get
			{
				return knockbackanims;
			}
		}


		internal override bool ShouldGenerateNewZigZagPath()
		{
			return (DateTime.Now.Subtract(Bot.NavigationCache.lastChangedZigZag).TotalMilliseconds >= 2000f ||
					   (Bot.NavigationCache.vPositionLastZigZagCheck != Vector3.Zero && Bot.Character.Data.Position == Bot.NavigationCache.vPositionLastZigZagCheck && DateTime.Now.Subtract(Bot.NavigationCache.lastChangedZigZag).TotalMilliseconds >= 1200) ||
					   Vector3.Distance(Bot.Character.Data.Position, Bot.NavigationCache.vSideToSideTarget) <= 5f ||
					   Bot.Targeting.CurrentTarget != null && Bot.Targeting.CurrentTarget.AcdGuid.HasValue && Bot.Targeting.CurrentTarget.AcdGuid.Value != Bot.NavigationCache.iACDGUIDLastWhirlwind);
		}
		internal override void GenerateNewZigZagPath()
		{
			if (Bot.Targeting.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_30] >= 6 || Bot.Targeting.Environment.iElitesWithinRange[(int)RangeIntervals.Range_30] >= 3)
				Bot.NavigationCache.vSideToSideTarget = Bot.NavigationCache.FindZigZagTargetLocation(Bot.Targeting.CurrentTarget.Position, 25f, false, true);
			else
				Bot.NavigationCache.vSideToSideTarget = Bot.NavigationCache.FindZigZagTargetLocation(Bot.Targeting.CurrentTarget.Position, 25f);

			Bot.NavigationCache.iACDGUIDLastWhirlwind = Bot.Targeting.CurrentTarget.AcdGuid.HasValue ? Bot.Targeting.CurrentTarget.AcdGuid.Value : -1;
			Bot.NavigationCache.lastChangedZigZag = DateTime.Now;
		}

		internal override Skill CreateAbility(SNOPower Power)
		{
			BarbarianActiveSkills power = (BarbarianActiveSkills)Enum.ToObject(typeof(BarbarianActiveSkills), (int)Power);


			switch (power)
			{
				case BarbarianActiveSkills.Barbarian_AncientSpear:
					return new AncientSpear();
				case BarbarianActiveSkills.Barbarian_Rend:
					return new Rend();
				case BarbarianActiveSkills.Barbarian_Frenzy:
					return new Frenzy();
				case BarbarianActiveSkills.Barbarian_Sprint:
					return new Sprint();
				case BarbarianActiveSkills.Barbarian_BattleRage:
					return new Battlerage();
				case BarbarianActiveSkills.Barbarian_ThreateningShout:
					return new ThreateningShout();
				case BarbarianActiveSkills.Barbarian_Bash:
					return new Bash();
				case BarbarianActiveSkills.Barbarian_GroundStomp:
					return new GroundStomp();
				case BarbarianActiveSkills.Barbarian_IgnorePain:
					return new IgnorePain();
				case BarbarianActiveSkills.Barbarian_WrathOfTheBerserker:
					return new WrathOfTheBerserker();
				case BarbarianActiveSkills.Barbarian_HammerOfTheAncients:
					return new HammeroftheAncients();
				case BarbarianActiveSkills.Barbarian_CallOfTheAncients:
					return new CalloftheAncients();
				case BarbarianActiveSkills.Barbarian_Cleave:
					return new Cleave();
				case BarbarianActiveSkills.Barbarian_WarCry:
					return new Warcry();
				case BarbarianActiveSkills.Barbarian_SeismicSlam:
					return new SeismicSlam();
				case BarbarianActiveSkills.Barbarian_Leap:
					return new Leap();
				case BarbarianActiveSkills.Barbarian_WeaponThrow:
					return new WeaponThrow();
				case BarbarianActiveSkills.Barbarian_Whirlwind:
					return new Whirlwind();
				case BarbarianActiveSkills.Barbarian_FuriousCharge:
					return new FuriousCharge();
				case BarbarianActiveSkills.Barbarian_Earthquake:
					return new Earthquake();
				case BarbarianActiveSkills.Barbarian_Revenge:
					return new Revenge();
				case BarbarianActiveSkills.Barbarian_Overpower:
					return new Overpower();
				default:
					return DefaultAttack;
			}


		}

		enum BarbarianActiveSkills
		{
			Barbarian_AncientSpear = 377453,
			Barbarian_Rend = 70472,
			Barbarian_Frenzy = 78548,
			Barbarian_Sprint = 78551,
			Barbarian_BattleRage = 79076,
			Barbarian_ThreateningShout = 79077,
			Barbarian_Bash = 79242,
			Barbarian_GroundStomp = 79446,
			Barbarian_IgnorePain = 79528,
			Barbarian_WrathOfTheBerserker = 79607,
			Barbarian_HammerOfTheAncients = 80028,
			Barbarian_CallOfTheAncients = 80049,
			Barbarian_Cleave = 80263,
			Barbarian_WarCry = 375483,
			Barbarian_SeismicSlam = 86989,
			Barbarian_Leap = 93409,
			Barbarian_WeaponThrow = 377452,
			Barbarian_Whirlwind = 96296,
			Barbarian_FuriousCharge = 97435,
			Barbarian_Earthquake = 98878,
			Barbarian_Revenge = 109342,
			Barbarian_Overpower = 159169,
		}
		/*
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
		*/
	}

}