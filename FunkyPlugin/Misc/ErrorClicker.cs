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
		  void ErrorClickerWorker()
		  {
				do
				{
					 if (ZetaDia.IsInGame)
					 {
						  try
						  {
								if (ErrorDialog.IsVisible)
								{
									 if (ErrorDialog.ErrorCode==-1)
										  HadDisconnectError=true;

									 Logging.Write("[ErrorClicker] Closing error "+ErrorDialog.ErrorCode);
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
									 ZetaDia.Me.SkipCutscene();
						  } catch
						  {

						  }
					 }
					 Thread.Sleep(2000);
				} while (true);
		  }

    }
}