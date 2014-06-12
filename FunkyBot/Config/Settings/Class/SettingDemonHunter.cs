using System.IO;
using System.Xml.Serialization;

namespace FunkyBot.Config.Settings.Class
{
	public class SettingDemonHunter
	{
		public int iDHVaultMovementDelay { get; set; }
		public bool BombadiersRucksack { get; set; }
		public SettingDemonHunter()
		{
			iDHVaultMovementDelay = 400;
			BombadiersRucksack = false;
		}
	}
}
