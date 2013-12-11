using Zeta;
using Zeta.Common;
using Zeta.Internals;
using Zeta.TreeSharp;
namespace FunkyBot
{
	public partial class EventHandlers
	{
		 private static bool WaitingForRevive;
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
                 if (UIElements.ReviveAtLastCheckpointButton.IsVisible && UIElements.ReviveAtLastCheckpointButton.IsEnabled)
                 {
                     Logging.Write("Clicking Revive Button!");
                     UIElements.ReviveAtLastCheckpointButton.Click();
                     WaitingForRevive = true;
                     Bot.Character.Data.OnHealthChanged += OnHealthChanged;
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

					 Bot.Game.CurrentGameStats.CurrentProfile.DeathCount++;
                     //Bot.BotStatistics.GameStats.CurrentGame.Deaths++;
                     //Bot.BotStatistics.ProfileStats.CurrentProfile.DeathCount++;
                     Bot.Character.Data.OnHealthChanged -= OnHealthChanged;
                     return RunStatus.Success;
                 }
             }

			  if (WaitingForRevive)
			  {
				  if (Revived)
				  {
						WaitingForRevive=false;
						Revived=false;
						Bot.Game.CurrentGameStats.CurrentProfile.DeathCount++;
                        //Bot.BotStatistics.GameStats.CurrentGame.Deaths++;
                        //Bot.BotStatistics.ProfileStats.CurrentProfile.DeathCount++;
				  }
				  else
				  {
						Bot.Character.Data.Update();
						return RunStatus.Running;
				  }
			  }

			  Bot.Character.Data.OnHealthChanged-=OnHealthChanged;
			  return RunStatus.Success;
		 }

		 private static bool Revived;
		 private static void OnHealthChanged(double oldvalue, double newvalue)
		 {
			  if (newvalue>=1d)
					Revived=true;
		 }
	}
}
