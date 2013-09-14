using System.IO;
using System.Xml.Serialization;

namespace FunkyTrinity.Settings
{
	public class SettingCluster
	{
		public double ClusterDistance { get; set; }
		public int ClusterMinimumUnitCount { get; set; }
		public bool EnableClusteringTargetLogic { get; set; }
		public bool IgnoreClusteringWhenLowHP { get; set; }
		public double IgnoreClusterLowHPValue { get; set; }

		public SettingCluster(bool enabled=true)
		{
			EnableClusteringTargetLogic=enabled;
			IgnoreClusteringWhenLowHP=true;
			ClusterDistance=7d;
			ClusterMinimumUnitCount=3;
			IgnoreClusterLowHPValue=0.55d;
		}
		public SettingCluster()
		{
			EnableClusteringTargetLogic=true;
			IgnoreClusteringWhenLowHP=true;
			ClusterDistance=10d;
			ClusterMinimumUnitCount=2;
			IgnoreClusterLowHPValue=0.55d;
		}

		private static string DefaultFilePath=Path.Combine(Funky.FolderPaths.sTrinityPluginPath, "Config", "Defaults", "Clustering_Default.xml");
		public static SettingCluster DeserializeFromXML()
		{
			 XmlSerializer deserializer=new XmlSerializer(typeof(SettingCluster));
			 TextReader textReader=new StreamReader(DefaultFilePath);
			 SettingCluster settings;
			 settings=(SettingCluster)deserializer.Deserialize(textReader);
			 textReader.Close();
			 return settings;
		}
		public static SettingCluster DeserializeFromXML(string Path)
		{
			XmlSerializer deserializer=new XmlSerializer(typeof(SettingCluster));
			TextReader textReader=new StreamReader(Path);
			SettingCluster settings;
			settings=(SettingCluster)deserializer.Deserialize(textReader);
			textReader.Close();
			return settings;
		}
	}
}