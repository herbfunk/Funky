using System.IO;
using System.Xml.Serialization;
using FunkyBot.Cache.Avoidance;

namespace FunkyBot.Config.Settings
{
	public class SettingAvoidance
	{
		 public bool AttemptAvoidanceMovements { get; set; }
		 public bool UseAdvancedProjectileTesting { get; set; }
         public int FailureRetryMilliseconds { get; set; }

		 [XmlArray]
		 public AvoidanceValue[] Avoidances { get { return avoidances; } set { avoidances=value; } }

		 private AvoidanceValue[] avoidances;

		 public SettingAvoidance()
		 {
			  AttemptAvoidanceMovements=true;
			  UseAdvancedProjectileTesting=false;
			  Avoidances=AvoidanceCache.AvoidancesDefault;
              FailureRetryMilliseconds = 2000;
		 }
	}
}
