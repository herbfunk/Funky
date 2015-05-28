using fBaseXtensions.Game.Hero.Skills.Conditions;
using fBaseXtensions.Items.Enums;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Witchdoctor
{
    public class Sacrifice : Skill
    {
        public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Location | SkillExecutionFlags.ClusterLocation; } }

        /* Rune Index
         * 
         * 2 = Black Blood (3 sec Stun)
         * 4 = 35% resurect
         * 3 = Mana Return
         * 1 = All Dogs Explode
         * 0 = 5 sec damage buff
         * 
         */

        public override void Initialize()
        {
            Cooldown = 1000;

            WaitVars = new WaitLoops(1, 0, true);
            Cost = 10;
            Range = 51;

            Priority = SkillPriority.High;

            PreCast = new SkillPreCast
            {
                Flags = SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast,
            };
            PreCast.Criteria += skill => FunkyGame.Targeting.Cache.Environment.HeroPets.ZombieDogs>0;
            PreCast.CreatePrecastCriteria();

            if (RuneIndex == 2 || RuneIndex == 4 || RuneIndex==-1)
            {//normal explosions..

                SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None,-1,-1,0.99d,TargetProperties.Normal));

                ClusterConditions.Add(new SkillClusterConditions(12d, 45f, 3, true));
            }
            else if (RuneIndex == 3)
            {//mana return..

                SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None));
                ClusterConditions.Add(new SkillClusterConditions(12d, 45f, 3, true));
                FcriteriaCombat = (u) => FunkyGame.Hero.dCurrentEnergyPct<=0.30d;
            }
            else if (RuneIndex == 1)
            {//All Explode!

                SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, -1, -1, 0.99d, TargetProperties.Normal));
                ClusterConditions.Add(new SkillClusterConditions(12d, 50f, 5, true));
            }
            else if (RuneIndex == 0)
            {//Buff

                SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, -1, -1, 0.95d));

                ClusterConditions.Add(new SkillClusterConditions(12d, 45f, 3, true));
            }

        }


        public override SNOPower Power
        {
            get { return SNOPower.Witchdoctor_Sacrifice; }
        }
    }
}
