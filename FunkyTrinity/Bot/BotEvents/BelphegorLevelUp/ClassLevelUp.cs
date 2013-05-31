using System;
using Zeta;
using Zeta.Common;
using Zeta.Internals.Actors;


namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  public static void BarbarianOnLevelUp(object sender, EventArgs e)
		  {
				if (ZetaDia.Me.ActorClass!=ActorClass.Barbarian)
					 return;

				int myLevel=ZetaDia.Me.Level;

				Logging.Write("Player leveled up, congrats! Your level is now: {0}",
								 myLevel
					 );

				#region Primarey Slot

				if (myLevel==18)
				{
					 Logging.Write("Add [R] Ravage to Cleave");
					 ZetaDia.Me.SetActiveSkill(SNOPower.Barbarian_Cleave, 2, 0);
				}
				else if (myLevel==9)
				{
					 Logging.Write("Add [R] Rupture to Cleave");
					 ZetaDia.Me.SetActiveSkill(SNOPower.Barbarian_Cleave, 1, 0);
				}
				else if (myLevel==3)
				{
					 Logging.Write("Equip Cleave");
					 ZetaDia.Me.SetActiveSkill(SNOPower.Barbarian_Cleave, -1, 0);
				}

				#endregion

				#region Secondary Slot

				if (myLevel==19)
				{
					 Logging.Write("Add [R] Blood Lust to Rend. ");
					 ZetaDia.Me.SetActiveSkill(SNOPower.Barbarian_Rend, 2, 1);
				}
				else if (myLevel==11)
				{
					 Logging.Write("Equip Rend with [R] Ravage in the place of Hammer of the Ancients.");
					 ZetaDia.Me.SetActiveSkill(SNOPower.Barbarian_Rend, 1, 1);
				}
				else if (myLevel==7)
				{
					 Logging.Write("Add [R] Rolling Thunder to Hammer of the Ancients.");
					 ZetaDia.Me.SetActiveSkill(SNOPower.Barbarian_HammerOfTheAncients, 1, 1);
				}
				else if (myLevel==2)
				{
					 Logging.Write("Setting Hammer of the Ancients");
					 ZetaDia.Me.SetActiveSkill(SNOPower.Barbarian_HammerOfTheAncients, -1, 1);
				}

				#endregion

				#region Defencive Slot

				if (myLevel==27)
				{
					 Logging.Write("Add [R] Battering Ram to Furious Charge. ");
					 ZetaDia.Me.SetActiveSkill(SNOPower.Barbarian_FuriousCharge, 1, 2);
				}
				else if (myLevel==21)
				{
					 Logging.Write("Equip Furious Charge in the place of Leap.");
					 ZetaDia.Me.SetActiveSkill(SNOPower.Barbarian_FuriousCharge, -1, 2);
				}
				else if (myLevel==14)
				{
					 Logging.Write("Add [R] Iron Impact to Leap.");
					 ZetaDia.Me.SetActiveSkill(SNOPower.Barbarian_Leap, -1, 2);
				}
				else if (myLevel==8)
				{
					 Logging.Write("Equip Leap in the place of Ground Stomp.");
					 ZetaDia.Me.SetActiveSkill(SNOPower.Barbarian_Leap, -1, 2);
				}
				else if (myLevel==4)
				{
					 Logging.Write("Setting Ground Stomp");
					 ZetaDia.Me.SetActiveSkill(SNOPower.Barbarian_GroundStomp, -1, 2);
				}

				#endregion

				#region Might Slot

				if (myLevel==26)
				{
					 Logging.Write("Add [R] Marauder's Rage to Battle Rage. ");
					 ZetaDia.Me.SetActiveSkill(SNOPower.Barbarian_BattleRage, 1, 3);
				}
				else if (myLevel==22)
				{
					 Logging.Write("Equip Battle Rage in the place of Threatening Shout.");
					 ZetaDia.Me.SetActiveSkill(SNOPower.Barbarian_BattleRage, -1, 3);
				}
				else if (myLevel==17)
				{
					 Logging.Write("Equip Threatening Shout in the place of Ground Stomp.");
					 ZetaDia.Me.SetActiveSkill(SNOPower.Barbarian_ThreateningShout, -1, 3);
				}
				else if (myLevel==12)
				{
					 Logging.Write("Add [R] Deafening Crash to Ground Stomp.");
					 ZetaDia.Me.SetActiveSkill(SNOPower.Barbarian_GroundStomp, 1, 3);
				}
				else if (myLevel==9)
				{
					 Logging.Write("Equip Ground Stomp.");
					 ZetaDia.Me.SetActiveSkill(SNOPower.Barbarian_GroundStomp, -1, 3);
				}

				#endregion

				#region Tactics Slot

				if (myLevel==19)
				{
					 Logging.Write("Add [R] Vengeance Is Mine to Revenge. ");
					 ZetaDia.Me.SetActiveSkill(SNOPower.Barbarian_Revenge, 1, 4);
				}
				if (myLevel==14)
				{
					 Logging.Write("Equip Revenge. ");
					 ZetaDia.Me.SetActiveSkill(SNOPower.Barbarian_Revenge, -1, 4);
				}

				#endregion

				#region Rage Slot

				if (myLevel==29)
				{
					 Logging.Write("Add [R] Storm of Steel to Overpower.");
					 ZetaDia.Me.SetActiveSkill(SNOPower.Barbarian_Overpower, 1, 5);
				}
				else if (myLevel==26)
				{
					 Logging.Write("Equip Overpower in the place of Earthquake.");
					 ZetaDia.Me.SetActiveSkill(SNOPower.Barbarian_Overpower, -1, 5);
				}
				else if (myLevel==24)
				{
					 Logging.Write("Add [R] Giant's Stride to Earthquake.");
					 ZetaDia.Me.SetActiveSkill(SNOPower.Barbarian_Earthquake, 1, 5);
				}
				else if (myLevel==19)
				{
					 Logging.Write("Equip Earthquake.");
					 ZetaDia.Me.SetActiveSkill(SNOPower.Barbarian_Earthquake, -1, 5);
				}

				#endregion

				#region Passive Skills

				if (myLevel==30)
				{
					 Logging.Write("Equip [P] Bloodthirst.");
					 ZetaDia.Me.SetTraits(SNOPower.Barbarian_Passive_WeaponsMaster,
												 SNOPower.Barbarian_Passive_InspiringPresence,
												 SNOPower.Barbarian_Passive_Bloodthirst);
				}
				if (myLevel==20)
				{
					 Logging.Write("Equip [P] Inspiring Presence.");
					 ZetaDia.Me.SetTraits(SNOPower.Barbarian_Passive_WeaponsMaster,
												 SNOPower.Barbarian_Passive_InspiringPresence);
				}
				else if (myLevel==16)
				{
					 Logging.Write("Equip [P] Weapons Master in the place of [P] Ruthless.");
					 ZetaDia.Me.SetTraits(SNOPower.Barbarian_Passive_WeaponsMaster);
				}
				else if (myLevel==10)
				{
					 Logging.Write("Equip [P] Ruthless.");
					 ZetaDia.Me.SetTraits(SNOPower.Barbarian_Passive_Ruthless);
				}

				#endregion
		  }
		  public static void DemonHunterOnLevelUp(object sender, EventArgs e)
		  {
				if (ZetaDia.Me.ActorClass!=ActorClass.DemonHunter)
					 return;

				int myLevel=ZetaDia.Me.Level;

				Logging.Write("Player leveled up, congrats! Your level is now: {0}",
								 myLevel
					 );

				#region Primary Slot

				if (myLevel==6)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.DemonHunter_HungeringArrow, 1, 0);
					 Logging.Write("Add [R] Puncturing Arrow to Hungering Arrow");
				}
				if (myLevel==14)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.DemonHunter_EvasiveFire, -1, 0);
					 Logging.Write("Setting Evasive Fire as Primary skill");
				}
				if (myLevel==21)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.DemonHunter_EvasiveFire, 1, 0);
					 Logging.Write("Setting [R] Shrapnel to Evasive Fire");
				}
				if (myLevel==34)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.DemonHunter_EvasiveFire, 3, 0);
					 Logging.Write("Setting [R] Covering Fire to Evasive Fire");
				}

				#endregion

				#region Secondary Slot

				if (myLevel==2)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.DemonHunter_Impale, -1, 1);
					 Logging.Write("Setting Impale as Secondary skill");
				}

				if (myLevel==22)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.DemonHunter_Multishot, -1, 1);
					 Logging.Write("Setting Multishot as Secondary skill");
				}

				if (myLevel==26)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.DemonHunter_Multishot, 1, 1);
					 Logging.Write("Setting [R] Fire at Will to Multishot");
				}

				#endregion

				#region Defensive Slot

				if (myLevel==4)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.DemonHunter_Caltrops, -1, 2);
					 Logging.Write("Setting Caltrops as Defensive skill");
				}

				if (myLevel==8)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.DemonHunter_SmokeScreen, -1, 2);
					 Logging.Write("Setting Smoke Screen as Defensive skill");
				}

				if (myLevel==14)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.DemonHunter_SmokeScreen, 1, 2);
					 Logging.Write("Setting [R] Displacement to Smoke Screen");
				}

				if (myLevel==33)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.DemonHunter_SmokeScreen, 3, 2);
					 Logging.Write("Setting [R] Breathe Deep to Smoke Screen");
				}

				#endregion

				#region Hunting Slot

				if (myLevel==9)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.DemonHunter_Vault, -1, 3);
					 Logging.Write("Setting Vault as Hunting skill");
				}
				if (myLevel==9)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.DemonHunter_Companion, -1, 3);
					 Logging.Write("Setting Companion as Hunting skill");
				}

				#endregion

				#region Device Slot

				if (myLevel==21)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.DemonHunter_MarkedForDeath, -1, 4);
					 Logging.Write("Setting Marked for Death as Device skill");
				}
				if (myLevel==27)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.DemonHunter_MarkedForDeath, 1, 4);
					 Logging.Write("Setting [R] Contagion to Marked for Death");
				}

				#endregion

				#region Archery Slot

				if (myLevel==16)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.DemonHunter_ShadowPower, -1, 5);
					 Logging.Write("Setting Shadow Power as Archery skill");
				}

				#endregion
		  }
		  public static void MonkOnLevelUp(object sender, EventArgs e)
		  {
				if (ZetaDia.Me.ActorClass!=ActorClass.Monk)
					 return;

				int myLevel=ZetaDia.Me.Level;

				Logging.Write("Player leveled up, congrats! Your level is now: {0}", myLevel);

				// Set Lashing tail kick once we reach level 2
				if (myLevel==2)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.Monk_LashingTailKick, -1, 1);
					 Logging.Write("Setting Lash Tail Kick as Secondary");
				}

				// Set Dead reach it's better then Fists of thunder imo.
				if (myLevel==3)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.Monk_DeadlyReach, -1, 0);
					 Logging.Write("Setting Deadly Reach as Primary");
				}

				// Make sure we set binding flash, useful spell in crowded situations!
				if (myLevel==4)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.Monk_BlindingFlash, -1, 2);
					 Logging.Write("Setting Binding Flash as Defensive");
				}

				// Binding flash is nice but being alive is even better!
				if (myLevel==8)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.Monk_BreathOfHeaven, -1, 2);
					 Logging.Write("Setting Breath of Heaven as Defensive");
				}

				// Make sure we set Dashing strike, very cool and useful spell great opener.
				if (myLevel==9)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.Monk_DashingStrike, -1, 3);
					 Logging.Write("Setting Dashing Strike as Techniques");
				}
		  }
		  public static void WitchDoctorOnLevelUp(object sender, EventArgs e)
		  {
				if (ZetaDia.Me.ActorClass!=ActorClass.WitchDoctor)
					 return;

				int myLevel=ZetaDia.Me.Level;

				Logging.Write("Player leveled up, congrats! Your level is now: {0}",
								 myLevel
					 );

				// Set Lashing tail kick once we reach level 2
				if (myLevel==2)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.Witchdoctor_GraspOfTheDead, -1, 1);
					 Logging.Write("Setting Grasp of the Dead as Secondary");
				}

				// Set Dead reach it's better then Fists of thunder imo.
				if (myLevel==3)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.Witchdoctor_CorpseSpider, -1, 0);
					 Logging.Write("Setting Grasp of the Dead as Secondary");
				}

				// Make sure we set binding flash, useful spell in crowded situations!
				if (myLevel==4)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.Witchdoctor_SummonZombieDog, -1, 2);
					 Logging.Write("Setting Summon Zombie Dogs as Defensive");
				}

				// Make sure we set Dashing strike, very cool and useful spell great opener.
				if (myLevel==9)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.Witchdoctor_SoulHarvest, -1, 3);
					 Logging.Write("Setting Sould Harvest as Terror");
				}

				if (myLevel==10)
				{
					 ZetaDia.Me.SetTraits(SNOPower.Witchdoctor_Passive_JungleFortitude);
				}
				if (myLevel==13)
				{
					 ZetaDia.Me.SetTraits(SNOPower.Witchdoctor_Passive_SpiritualAttunement);
				}
		  }
		  public static void WizardOnLevelUp(object sender, EventArgs e)
		  {
				if (ZetaDia.Me.ActorClass!=ActorClass.Wizard)
					 return;

				int myLevel=ZetaDia.Me.Level;

				Logging.Write("Player leveled up, congrats! Your level is now: {0}",
								 myLevel
					 );


				// ********** PRIMARY SLOT CHANGES **********
				// Set Shock Pulse as primary.
				if (myLevel==3)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.Wizard_ShockPulse, -1, 0);
					 Logging.Write("Setting Shock Pulse as Primary");
				}
				// Set Shock Pulse-Explosive bolts as primary.
				if (myLevel==9)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.Wizard_ShockPulse, 1, 0);
					 Logging.Write("Changing rune for Shock Pulse: \"Explosive Bolts\"");
				}
				// Set Electrocute as primary.
				if (myLevel==15)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.Wizard_Electrocute, -1, 0);
					 Logging.Write("Setting Electrocute as Primary");
				}
				// Set Electrocute-Chain lightning as primary.
				if (myLevel==22)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.Wizard_Electrocute, 1, 0);
					 Logging.Write("Changing rune for Electrocute: \"Chain Lightning\"");
				}

				// ********** SECONDARY SLOT CHANGES **********
				// Set Ray of Frost as secondary spell.
				if (myLevel==2)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.Wizard_RayOfFrost, -1, 1);
					 Logging.Write("Setting Ray of Frost as Secondary");
				}
				// Set arcane orb as secondary
				if (myLevel==5)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.Wizard_ArcaneOrb, -1, 1);
					 Logging.Write("Setting Arcane Orb as Secondary");
				}
				// Set arcane orb rune to "obliteration"
				if (myLevel==11)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.Wizard_ArcaneOrb, 1, 1);
					 Logging.Write("Changing rune for Arcane Orb: \"Obliteration\"");
				}
				// ********** SKILL SLOTS 1-4 **********
				// Set Frost Nova as slot 1
				if (myLevel==4)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.Wizard_FrostNova, -1, 2);
					 Logging.Write("Setting Frost Nova as slot 1");
				}
				// Set Diamond Skin as slot 1
				if (myLevel==8)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.Wizard_DiamondSkin, -1, 2);
					 Logging.Write("Setting Diamond Skin as slot 1");
				}
				// Level 9, slot 2 unlocked!
				// Set Wave of Force as slot 2
				if (myLevel==9)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.Wizard_WaveOfForce, -1, 3);
					 Logging.Write("Setting Wave of Force as slot 2");
				}
				// Level 14, slot 3 unlocked!
				// Set Diamond Skin-Crystal Shell as slot 1, Ice Armor as slot 3
				if (myLevel==14)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.Wizard_DiamondSkin, 1, 2);
					 Logging.Write("Changing rune for Diamond Skin: \"Crystal Shell\"");
					 ZetaDia.Me.SetActiveSkill(SNOPower.Wizard_IceArmor, -1, 4);
					 Logging.Write("Setting Ice Armor as slot 3");
				}
				// Set Wave of Force-Impactful Wave as slot 2
				if (myLevel==15)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.Wizard_WaveOfForce, 1, 3);
					 Logging.Write("Changing rune for Wave of Force: \"Impactful Wave\"");
				}
				// Level 19, slot 4 unlocked!
				// Set Explosive Blast as slot 4
				if (myLevel==19)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.Wizard_ExplosiveBlast, -1, 5);
					 Logging.Write("Setting Explosive Blast as slot 4");
				}
				// Set Ice Armor-Chilling Aura as slot 3, Hydra as slot 4
				if (myLevel==21)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.Wizard_IceArmor, 1, 4);
					 Logging.Write("Changing rune for Ice Armor: \"Chilling Aura\"");
					 ZetaDia.Me.SetActiveSkill(SNOPower.Wizard_Hydra, -1, 5);
					 Logging.Write("Setting Hydra as slot 4");
				}
				// Set Hydra-Arcane Hydra as slot 4
				if (myLevel==26)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.Wizard_Hydra, 1, 5);
					 Logging.Write("Changing rune for Hydra: \"Arcane Hydra\"");
				}
				// Set Energy Armor as slot 3
				if (myLevel==28)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.Wizard_EnergyArmor, -1, 4);
					 Logging.Write("Setting Energy Armor as slot 3");
				}
				// Set Energy Armor-Absorption as slot 3
				if (myLevel==32)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.Wizard_EnergyArmor, 1, 4);
					 Logging.Write("Changing rune for Energy Armor: \"Absorption\"");
				}
				// Set Hydra-Venom Hydra as slot 4
				if (myLevel==38)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.Wizard_Hydra, 3, 5);
					 Logging.Write("Changing rune for Hydra: \"Venom Hydra\"");
				}
				// Set Energy Armor-Pinpoint Barrier as slot 3
				if (myLevel==41)
				{
					 ZetaDia.Me.SetActiveSkill(SNOPower.Wizard_EnergyArmor, 2, 4);
					 Logging.Write("Changing rune for Energy Armor: \"Pinpoint Barrier\"");
				}

				// ********** PASSIVE SKILLS **********
				if (myLevel==10)
				{
					 // Blur - Decreases melee damage taken by 20%.
					 ZetaDia.Me.SetTraits(SNOPower.Wizard_Passive_Blur);
				}
				if (myLevel==20)
				{
					 // Blur - Decreases melee damage taken by 20%.
					 // Prodigy - 4 arcane power from signature casts
					 ZetaDia.Me.SetTraits(SNOPower.Wizard_Passive_Blur, SNOPower.Wizard_Passive_Prodigy);
				}
				if (myLevel==30)
				{
					 // Blur - Decreases melee damage taken by 20%.
					 // Prodigy - 4 arcane power from signature casts
					 // Astral Presence - +20 arcane power, +2 arcane regen
					 ZetaDia.Me.SetTraits(SNOPower.Wizard_Passive_Blur, SNOPower.Wizard_Passive_Prodigy,
												 SNOPower.Wizard_Passive_AstralPresence);
				}
		  }

    }
}