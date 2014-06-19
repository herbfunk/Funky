using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Zeta.Common.Plugins;

namespace FunkyUpdater
{
    public class Plugin : IPlugin
    {
		internal static readonly log4net.ILog DBLog = Zeta.Common.Logger.GetLoggerInstanceForType();
		private static readonly string sDemonBuddyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

		#region IPlugin Members

		public string Author
		{
			get { return "HerbFunk"; }
		}

		public string Description
		{
			get { return "Updater for Funky Code!"; }
		}

		public Window DisplayWindow
		{
			get { return null; }
		}

		public string Name
		{
			get { return "FunkyUpdater"; }
		}

		public void OnDisabled()
		{

		}

		public void OnEnabled()
		{

		}

		public void OnInitialize()
		{

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


		public bool Equals(IPlugin other) { return (other.Name == Name) && (other.Version == Version); }

	}
}
