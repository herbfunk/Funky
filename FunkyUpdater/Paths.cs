using System.IO;
using System.Reflection;

namespace FunkyUpdater
{
	public static class Paths
	{
		public static string DemonbuddyRootFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

		public static string FunkyBotRoutineFolder
		{
			get { return DemonbuddyRootFolder + @"\Routines\FunkyBot\"; }
		}

		public static string FunkyTownRunFolder
		{
			get { return DemonbuddyRootFolder + @"\Plugins\FunkyTownRun\"; }
		}

	}
}
