using System;
using System.Threading;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals;

namespace FunkyErrorClicker
{
	 public partial class FunkyErrorClicker
	 {
		  public static bool HadDisconnectError=false;
		  private static Thread ErrorClickerThread;
		  private static int DefaultSleepTime=2500;

		  internal void ErrorClickerWorker()
		  {
				do
				{
					 Thread.Sleep(DefaultSleepTime);

					 bool InGame=false;

					 try
					 {
						 using (ZetaDia.Memory.AcquireFrame())
						 {

							 InGame=ZetaDia.IsInGame;
							 
						 }
					 } catch (Exception)
					 {
						 DBLog.Debug("[ErrorClicker] Safely Handled IsInGame Exception!");
					 }

					 if (InGame)
					 {
						  try
						  {
							  using (ZetaDia.Memory.AcquireFrame())
							  {

								  if (ErrorDialog.IsVisible)
								  {
									  if (ErrorDialog.ErrorCode==-1)
										  HadDisconnectError=true;

									  DBLog.Debug("[ErrorClicker] Closing error " + ErrorDialog.ErrorCode.ToString());
									  ErrorDialog.Click();
								  }
								  //else if(BotMain.ExecutionStateAvailable)
								  //{
									  UIElement OkButton;
									  if (UIElement.IsValidElement(0xB4433DA3F648A992)&&(OkButton=UIElement.FromHash(0xB4433DA3F648A992))!=null)
									  {
										  if (OkButton.IsValid&&OkButton.IsVisible&&OkButton.IsEnabled)
										  {
											  DBLog.Info("[ErrorClicker] Closing unhandled error");
											  OkButton.Click();
										  }
									  }
								  //}
								  
							  }

						  } catch (Exception)
						  {

						  }
					 }




					 
				} while (true);
		  }

	 }
}