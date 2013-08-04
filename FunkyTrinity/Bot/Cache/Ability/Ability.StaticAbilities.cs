using Zeta.Internals.Actors;

namespace FunkyTrinity.ability
{
	public partial class Ability
	{
		 //Default Abilities -- Used on new characters!

		 public static readonly Ability Instant_Melee_Attack=new Ability
		 {
				Range=8,
				Power=SNOPower.Weapon_Melee_Instant,
				Priority=AbilityPriority.None,
				UsageType=AbilityUseType.Target,
				WaitVars=new WaitLoops(0, 0, true),
		 };

		 public static readonly Ability Instant_Range_Attack=new Ability
		 {
				Range=25,
				Power=SNOPower.Weapon_Ranged_Instant,
				Priority=AbilityPriority.None,
				UsageType=AbilityUseType.Target,
				WaitVars=new WaitLoops(0, 0, true),
		 };

		 public static readonly Ability Projectile_Range_Attack=new Ability
		 {
				Range=25,
				Power=SNOPower.Weapon_Ranged_Projectile,
				Priority=AbilityPriority.None,
				UsageType=AbilityUseType.Target,
				WaitVars=new WaitLoops(0, 0, true),
		 };

		 public static readonly Ability Wand_Range_Attack=new Ability
		 {
				Range=25,
				Power=SNOPower.Weapon_Ranged_Wand,
				Priority=AbilityPriority.None,
				UsageType=AbilityUseType.Target,
				WaitVars=new WaitLoops(0, 0, true),
		 };

		 public static readonly Ability Cancel_Archon_Buff=new Ability
		 {
				UsageType=AbilityUseType.RemoveBuff,
				WaitVars=new WaitLoops(3, 3, true),
				Power=SNOPower.Wizard_Archon,
		 };
	}
}
