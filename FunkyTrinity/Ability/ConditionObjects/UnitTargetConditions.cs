

namespace FunkyTrinity.Ability
{
	 ///<summary>
	 ///Describes a condition for a single unit
	 ///</summary>
	 public class UnitTargetConditions
	 {
			public UnitTargetConditions(TargetProperties trueconditionalFlags, int MinimumDistance=-1, double MinimumHealthPercent=0d, TargetProperties falseConditionalFlags=TargetProperties.None)
			{
				 TrueConditionFlags=trueconditionalFlags;
				 FalseConditionFlags=falseConditionalFlags;
				 Distance=MinimumDistance;
				 HealthPercent=MinimumHealthPercent;
			}

			//Default Constructor
			public UnitTargetConditions()
			{
				 TrueConditionFlags=TargetProperties.None;
				 FalseConditionFlags=TargetProperties.None;
				 Distance=-1;
				 HealthPercent=0d;
			}

			public TargetProperties TrueConditionFlags { get; set; }
			public TargetProperties FalseConditionFlags { get; set; }
			public int Distance { get; set; }
			public double HealthPercent { get; set; }
	 }
}