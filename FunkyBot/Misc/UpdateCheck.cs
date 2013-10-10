using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using FunkyBot;

namespace FunkyBot
{
	 public partial class Funky
	 {

		  void CheckUpdate()
		  {
				// in newVersion variable we will store the
				// version info from xml file
				Version newVersion=null;
				// and in this variable we will put the url we
				// would like to open so that the user can
				// download the new version
				// it can be a homepage or a direct
				// link to zip/exe file
				string url="";

				try
				{
					 string xmlURL="https://raw.github.com/herbfunk/Funky/master/FunkyBot/Version.xml";
					 using (XmlTextReader reader=new XmlTextReader(xmlURL))
					 {
						  // simply (and easily) skip the junk at the beginning
						  reader.MoveToContent();
						  // internal - as the XmlTextReader moves only
						  // forward, we save current xml element name
						  // in elementName variable. When we parse a
						  // text node, we refer to elementName to check
						  // what was the node name
						  string elementName="";
						  // we check if the xml starts with a proper
						  // "ourfancyapp" element node
						  if ((reader.NodeType==XmlNodeType.Element)&&
								(reader.Name=="FunkyTrinity"))
						  {
								while (reader.Read())
								{
									 // when we find an element node,
									 // we remember its name
									 if (reader.NodeType==XmlNodeType.Element)
										  elementName=reader.Name;
									 else
									 {
										  // for text nodes...
										  if ((reader.NodeType==XmlNodeType.Text)&&
												(reader.HasValue))
										  {
												// we check what the name of the node was
												switch (elementName)
												{
													 case "version":
														  // thats why we keep the version info
														  // in xxx.xxx.xxx.xxx format
														  // the Version class does the
														  // parsing for us
														  newVersion=new Version(reader.Value);
														  break;
													 case "url":
														  url=reader.Value;
														  break;
												}
										  }
									 }
								}
						  }
					 }


				} catch (Exception)
				{
				}
				finally
				{

				}

				// compare the versions
				if (this.Version.CompareTo(newVersion)<0)
				{
					 Zeta.Common.Logging.Write("New Version Available!");
					 Zeta.Common.Logging.Write("https://github.com/herbfunk/Funky/archive/master.zip");
				}
		  }

	 }
}
