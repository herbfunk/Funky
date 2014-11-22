using System;
using fBaseXtensions.Game.Hero.Skills.Conditions;
using fBaseXtensions.Items.Enums;
using fBaseXtensions.Navigation.Clustering;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Barbarian
{
    public class Earthquake : Skill
    {
        public override SNOPower Power { get { return SNOPower.Barbarian_Earthquake; } }


        public override double Cooldown { get { return 60000; } }

        private readonly WaitLoops _waitVars = new WaitLoops(0, 4, true);
        public override WaitLoops WaitVars { get { return _waitVars; } }

        public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

        public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

        public override void Initialize()
        {
            Priority = SkillPriority.High;
            Range = 16;
            PreCast = new SkillPreCast((SkillPrecastFlags.CheckCanCast | SkillPrecastFlags.CheckPlayerIncapacitated));

            ClusterConditions.Add(new SkillClusterConditions(6d, Range, 3, true, useRadiusDistance: true, clusterflags: ClusterProperties.Strong));
            
            //Might of the earth 2 set bonus reduces the cooldown!
            if (Equipment.CheckLegendaryItemCount(LegendaryItemTypes.MightOfTheEarth, 2))
                ClusterConditions.Add(new SkillClusterConditions(6d, Range, 5, true, useRadiusDistance: true));
            else
                ClusterConditions.Add(new SkillClusterConditions(6d, Range, 7, true, useRadiusDistance: true));

            SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, Range, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));
        }

    }
}
