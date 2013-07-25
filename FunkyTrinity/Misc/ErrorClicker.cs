using System;
using Zeta;
using Zeta.CommonBot;
using System.Threading;
using Zeta.Common;

namespace FunkyTrinity
{
    public partial class Funky
    {
		  private static bool HadDisconnectError=false;
		  internal static Thread ErrorClickerThread;
		  internal void ErrorClickerWorker()
		  {
				do
				{
					 bool InGame=false;
					 int SleepTime=2500;

					 try
					 {
						  InGame=ZetaDia.IsInGame;
					 } catch (Exception)
					 {
						  Logging.WriteDiagnostic("[ErrorClicker] Safely Handled IsInGame Exception!");
						  SleepTime=5000;
					 }

					 if (InGame)
					 {
						  try
						  {
								if (ErrorDialog.IsVisible)
								{
									 if (ErrorDialog.ErrorCode==-1)
										  HadDisconnectError=true;

									 Logging.Write("[ErrorClicker] Closing error "+ErrorDialog.ErrorCode.ToString());
									 ErrorDialog.Click();
								}
								else
								{
									 Zeta.Internals.UIElement OkButton;
									 if (Zeta.Internals.UIElement.IsValidElement(0xB4433DA3F648A992)&&(OkButton=Zeta.Internals.UIElement.FromHash(0xB4433DA3F648A992))!=null)
									 {
										  if (OkButton.IsValid&&OkButton.IsVisible&&OkButton.IsEnabled)
										  {
												Logging.Write("[ErrorClicker] Closing unhandled error");
												OkButton.Click();
										  }
									 }
								}

						  } catch (Exception ex)
						  {
								Logging.Write("[ErrorClicker] ERROR: "+ex.Message);
								Logging.WriteDiagnostic("[ErrorClicker] "+ex.StackTrace);
						  }


						  try
						  {
								if (ZetaDia.IsPlayingCutscene)
								{
									 Logging.WriteDiagnostic("[ErrorClicker] Attempting to Skip Cut Scene.");
									 ZetaDia.Me.SkipCutscene();
								}
						  } catch
						  {
								Logging.WriteDiagnostic("[ErrorClicker] Exception Caught: Attempting to Skip Cut Scene.");
						  }
					 }

					 Thread.Sleep(SleepTime);

				} while (true);
		  }

    }
}