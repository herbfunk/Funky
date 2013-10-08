using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Zeta.Common;
using Zeta.Common.Plugins;
using Zeta.CommonBot;

namespace FunkyErrorClicker
{
	 public partial class FunkyErrorClicker : IPlugin
	 {

		  #region IPlugin Members

		  public string Author
		  {
				get { return "HerbFunk"; }
		  }

		  public string Description
		  {
				get { return "Error Dialog Clicker!"; }
		  }

		  public Window DisplayWindow
		  {
				get { return null; }
		  }

		  public string Name
		  {
				get { return "FunkyErrorClicker"; }
		  }

		  public void OnDisabled()
		  {
				BotMain.OnStart-=FunkyBotStart;
				BotMain.OnStop-=FunkyBotStop;
		  }

		  public void OnEnabled()
		  {
				BotMain.OnStart+=FunkyBotStart;
				BotMain.OnStop+=FunkyBotStop;
		  }

		  public void OnInitialize()
		  {
				ErrorClickerThread=new Thread(ErrorClickerWorker);
				ErrorClickerThread.IsBackground=true;
				Logging.Write("[Funky] Error Clicking Thread Created.");
		  }

		  public void OnPulse()
		  {

		  }

		  public void OnShutdown()
		  {

		  }

		  public Version Version
		  {
				get { return new Version(0, 0, 0); }
		  }

		  #endregion

		  #region IEquatable<IPlugin> Members

		  public bool Equals(IPlugin other) { return (other.Name==Name)&&(other.Version==Version); }

		  #endregion

		  private void FunkyBotStart(IBot bot)
		  {
				if (!ErrorClickerThread.ThreadState.HasFlag(ThreadState.Running))
				{
					 ErrorClickerThread.Start();
					 Logging.Write("[Funky] Error Clicking Thread Started.");
				}
		  }
		  private void FunkyBotStop(IBot bot)
		  {
				ErrorClickerThread.Abort();
				Logging.Write("[Funky] Error Clicking Thread Aborted.");
		  }
	 }
}
