using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Zeta.Common;
using Zeta.Common.Plugins;

namespace FunkyErrorClicker
{
    public partial class FunkyErrorClicker:IPlugin
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
				
		  }

		  public void OnEnabled()
		  {
				ErrorClickerThread.Start();
				Logging.Write("[Funky] Error Clicking Thread Started.");
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
				ErrorClickerThread.Abort();
				Logging.Write("[Funky] Error Clicking Thread Aborted.");
		  }

		  public Version Version
		  {
				get { return new Version(0, 0, 0); }
		  }

		  #endregion

		  #region IEquatable<IPlugin> Members

		  public bool Equals(IPlugin other) { return (other.Name==Name)&&(other.Version==Version); }

		  #endregion
	 }
}
