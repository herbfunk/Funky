using System;
using Zeta;
using Zeta.CommonBot;
using System.Threading;
using Zeta.Common;
using Zeta.Internals;

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
						  Logging.WriteDiagnostic("[ErrorClicker] Safely Handled IsInGame Exception!");
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

									  Logging.Write("[ErrorClicker] Closing error "+ErrorDialog.ErrorCode.ToString());
									  ErrorDialog.Click();
								  }
								  else if(BotMain.ExecutionStateAvailable)
								  {
									  UIElement OkButton;
									  if (UIElement.IsValidElement(0xB4433DA3F648A992)&&(OkButton=UIElement.FromHash(0xB4433DA3F648A992))!=null)
									  {
										  if (OkButton.IsValid&&OkButton.IsVisible&&OkButton.IsEnabled)
										  {
											  Logging.Write("[ErrorClicker] Closing unhandled error");
											  OkButton.Click();
										  }
									  }
								  }
								  
							  }

						  } catch (Exception)
						  {

						  }
					 }




					 
				} while (true);
		  }

	 }
}