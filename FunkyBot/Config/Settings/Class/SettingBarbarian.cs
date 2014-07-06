namespace FunkyBot.Config.Settings.Class
{
	public class SettingBarbarian
	{
		public bool bSelectiveWhirlwind { get; set; }
		public bool bWaitForWrath { get; set; }
		public bool bGoblinWrath { get; set; }
		public bool bFuryDumpWrath { get; set; }
		public bool bFuryDumpAlways { get; set; }
		public bool bBarbUseWOTBAlways { get; set; }

		public bool FullEarthSet { get; set; }

		public SettingBarbarian()
		{
			bBarbUseWOTBAlways = false;
			bSelectiveWhirlwind = false;
			bWaitForWrath = false;
			bGoblinWrath = false;
			bFuryDumpWrath = false;
			bFuryDumpAlways = false;
			FullEarthSet = false;
		}
	}
}
