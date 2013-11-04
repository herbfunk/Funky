using FunkyBot.Game;
using Zeta;
using Zeta.Common;
using Zeta.Internals;
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
             UIElement ReviveAtLastCheckpointButton=null;
             try
             {
                 ReviveAtLastCheckpointButton = UIElements.ReviveAtLastCheckpointButton;
             }
             catch
             {
                 Logging.Write("Revive Button Exception Handled");
             }

             if (ReviveAtLastCheckpointButton != null)
             {
                 //TODO:: Add Safety Exit-Game for Broken Equipped Items 
                 if (Zeta.Internals.UIElements.ReviveAtLastCheckpointButton.IsVisible && Zeta.Internals.UIElements.ReviveAtLastCheckpointButton.IsEnabled)
                 {
                     Logging.Write("Clicking Revive Button!");
                     Zeta.Internals.UIElements.ReviveAtLastCheckpointButton.Click();
                     WaitingForRevive = true;
                     Bot.Character.OnHealthChanged += OnHealthChanged;
                     return RunStatus.Running;
                 }
             }
             else
             {
                 //No revive button?? lets check if we are alive..?
                 if (!ZetaDia.Me.IsDead)
                 {
                     //Don't wait for health change event..
                     WaitingForRevive = false;
                     Revived = false;

                     TotalStats.CurrentTrackingProfile.DeathCount++;
                     //Bot.BotStatistics.GameStats.CurrentGame.Deaths++;
                     //Bot.BotStatistics.ProfileStats.CurrentProfile.DeathCount++;
                     Bot.Character.OnHealthChanged -= OnHealthChanged;
                     return RunStatus.Success;
                 }
             }

			  if (WaitingForRevive)
			  {
				  if (Revived)
				  {
						WaitingForRevive=false;
						Revived=false;
                        TotalStats.CurrentTrackingProfile.DeathCount++;
                        //Bot.BotStatistics.GameStats.CurrentGame.Deaths++;
                        //Bot.BotStatistics.ProfileStats.CurrentProfile.DeathCount++;
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
			  if (newvalue>=1d)
					Revived=true;
		 }
	}
}
