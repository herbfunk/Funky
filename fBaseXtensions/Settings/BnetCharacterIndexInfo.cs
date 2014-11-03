using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using fBaseXtensions.Game;
using fBaseXtensions.Helpers;
using Zeta.Game;

namespace fBaseXtensions.Settings
{
    public class BnetCharacterIndexInfo
    {

        public List<BnetCharacterEntry> Characters { get; set; }

        public BnetCharacterIndexInfo()
        {
            Characters = new List<BnetCharacterEntry>();
        }
        internal static string BnetCharacterInfoSettingsPath
        {
            get
            {
                return Path.Combine(FolderPaths.sFunkySettingsPath,"HeroIndexInfo.xml");

            }
        }
        public static void SerializeToXML(BnetCharacterIndexInfo settings, string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BnetCharacterIndexInfo));
            TextWriter textWriter = new StreamWriter(path);
            serializer.Serialize(textWriter, settings);
            textWriter.Close();
        }


        public static BnetCharacterIndexInfo DeserializeFromXML()
        {
            return DeserializeFromXML(BnetCharacterInfoSettingsPath);
        }
        public static BnetCharacterIndexInfo DeserializeFromXML(string path)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(BnetCharacterIndexInfo));
            TextReader textReader = new StreamReader(path);
            BnetCharacterIndexInfo settings;
            settings = (BnetCharacterIndexInfo)deserializer.Deserialize(textReader);
            textReader.Close();
            return settings;
        }

        public class BnetCharacterEntry
        {
            public int Index { get; set; }
            public string Name { get; set; }
            public ActorClass Class { get; set; }

            public BnetCharacterEntry(int index, string name, ActorClass actorclass)
            {
                Index = index;
                Name = name;
                Class = actorclass;
            }

            public BnetCharacterEntry()
            {
                Index = -1;
                Name = String.Empty;
                Class= ActorClass.Invalid;
            }
        }
    }
}
