using System.IO;
using System.Xml.Serialization;

namespace FunkyBot.Config.Settings.Class
{
	public class SettingWizard
	{
		public bool bWaitForArchon { get; set; }
		public bool bKiteOnlyArchon { get; set; }
		public bool bCancelArchonRebuff { get; set; }
		public bool bTeleportIntoGrouping { get; set; }
		public bool bTeleportFleeWhenLowHP { get; set; }
		public bool SerpentSparker { get; set; }

		 public SettingWizard()
		 {
			 bTeleportIntoGrouping = false;
			 bTeleportFleeWhenLowHP = true;
			 bCancelArchonRebuff = false;
			 bWaitForArchon = false;
			 bKiteOnlyArchon = true;
			 SerpentSparker = false;
		 }
	}
}
