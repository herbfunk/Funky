using System;
using System.Linq;
using FunkyTrinity.Cache;

namespace FunkyTrinity.Targeting.Behaviors
{
	 public class TB_Refresh : TargetBehavior
	 {
		  public TB_Refresh() : base() { }



		  public override void Initialize()
		  {
				base.Test=(ref CacheObject obj) =>
				{
					

					 //Check avoidance requirement still valid
					 if (FunkyTrinity.Bot.Combat.RequiresAvoidance)
					 {
						  if (FunkyTrinity.Bot.Combat.TriggeringAvoidances.Count==0)
						  {
								if (!FunkyTrinity.Bot.SettingsFunky.EnableFleeingBehavior||FunkyTrinity.Bot.Character.dCurrentHealthPct>0.25d)
									 FunkyTrinity.Bot.Combat.RequiresAvoidance=false;
						  }
						  else
						  {
								if (Bot.NavigationCache.IsMoving)
									 Bot.Combat.RequiresAvoidance=false;
						  }
					 }

					 FunkyTrinity.Bot.ValidObjects=ObjectCache.Objects.Values.Where(o => o.ObjectIsValidForTargeting).ToList();

					 return false;
				};
		  }
	 }
}
