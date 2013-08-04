using System.Runtime.InteropServices;
using Zeta.XmlEngine;

namespace FunkyTrinity.XMLTags
{
	[ComVisible(false)]
	[XmlElement("LoadProfileOnce")]
	public class LoadProfileOnce
	{
		[XmlAttribute("filename")]
		[XmlAttribute("Filename")]
		[XmlAttribute("FileName")]
		[XmlAttribute("fileName")]
		[XmlAttribute("profile")]
		public string FileName { get; set; }

		public LoadProfileOnce(string filename)
		{
			this.FileName=filename;
		}

		public LoadProfileOnce()
		{

		}

		public override string ToString()
		{
			return FileName;
		}
	}
}