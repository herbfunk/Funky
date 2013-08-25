using System;
using System.Linq;
using FunkyTrinity.Cache;

namespace FunkyTrinity
{
	 public class TLA_Refresh : TargetLogicAction
	 {
		  public override void Initialize()
		  {
				base.Test=(ref CacheObject obj) =>
				{
					 FunkyTrinity.Bot.ValidObjects=ObjectCache.Objects.Values.Where(o => o.ObjectIsValidForTargeting).ToList();

					 //Check avoidance requirement still valid
					 if (FunkyTrinity.Bot.Combat.RequiresAvoidance)
					 {
						  if (FunkyTrinity.Bot.Combat.TriggeringAvoidances.Count==0)
						  {
								if (!FunkyTrinity.Bot.SettingsFunky.EnableFleeingBehavior||FunkyTrinity.Bot.Character.dCurrentHealthPct>0.25d)
									 FunkyTrinity.Bot.Combat.RequiresAvoidance=false;
						  }
					 }
					 return false;
				};
		  }
	 }
}
