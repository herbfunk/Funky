using System.IO;
using System.Xml.Serialization;

namespace FunkyBot.Config.Settings.Class
{
	public class SettingWitchDoctor
	{
		public bool ZunimassaFullSet { get; set; }
		public bool TallManFinger { get; set; }

		public SettingWitchDoctor()
		 {
			 ZunimassaFullSet = false;
		 }
	}
}
