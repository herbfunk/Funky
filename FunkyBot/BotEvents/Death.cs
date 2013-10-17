using Zeta;
using Zeta.Common;
using Zeta.TreeSharp;
namespace FunkyBot
{
	public class Death
	{
		 public static bool DeathOverlord(object ret)
		 {
			 using (ZetaDia.Memory.AcquireFrame())
			 {
				 if (ZetaDia.Me.IsDead)
					 return true;
			 }

			  return false;
		 }

		 private static bool WaitingForRevive=false;
		 public static RunStatus DeathHandler(object ret)
		 {
			  //TODO:: Add Safety Exit-Game for Broken Equipped Items 
			  if (Zeta.Internals.UIElements.ReviveAtLastCheckpointButton.IsVisible&&Zeta.Internals.UIElements.ReviveAtLastCheckpointButton.IsEnabled)
			  {
					Logging.Write("Clicking Revive Button!");
					Zeta.Internals.UIElements.ReviveAtLastCheckpointButton.Click();
					WaitingForRevive=true;
					Bot.Character.OnHealthChanged+=OnHealthChanged;
					return RunStatus.Running;
			  }

			  if (WaitingForRevive)
			  {
				  if (Revived)
				  {
						WaitingForRevive=false;
						Revived=false;
						Bot.BotStatistics.GameStats.CurrentGame.Deaths++;
						Bot.BotStatistics.ProfileStats.CurrentProfile.DeathCount++;
				  }
				  else
				  {
						Bot.Character.Update();
						return RunStatus.Running;
				  }
			  }

			  Bot.Character.OnHealthChanged-=OnHealthChanged;
			  return RunStatus.Success;
		 }

		 private static bool Revived=false;
		 private static void OnHealthChanged(double oldvalue, double newvalue)
		 {
			  Logging.Write("Health Value Changed! {0} -- {1}", oldvalue, newvalue);
			  if (newvalue>=1d)
					Revived=true;
		 }
	}
}
