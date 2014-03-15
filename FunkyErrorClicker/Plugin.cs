using System;
using System.Threading;
using System.Windows;
using Zeta.Bot;
using Zeta.Common.Plugins;

namespace FunkyErrorClicker
{
	 public partial class FunkyErrorClicker : IPlugin
	 {
		 internal static readonly log4net.ILog DBLog = Zeta.Common.Logger.GetLoggerInstanceForType();

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
				DBLog.Info("[Funky] Error Clicking Thread Created.");
		  }

		  public void OnPulse()
		  {

		  }

		  public void OnShutdown()
		  {

		  }

		  public Version Version
		  {
				get { return new Version(0, 0, 1); }
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
					 DBLog.Info("[Funky] Error Clicking Thread Started.");
				}
		  }
		  private void FunkyBotStop(IBot bot)
		  {
				ErrorClickerThread.Abort();
				DBLog.Info("[Funky] Error Clicking Thread Aborted.");
		  }
	 }
}
