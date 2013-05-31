using System;
using System.Collections.Generic;
using Zeta.Common;
using System.IO;

namespace FunkyTrinity
{
    public partial class Funky
    {
        private void CleanLogs()
        {
            List<string> deleteList = new List<string>();
            if (string.IsNullOrEmpty(Funky.FolderPaths.sDemonBuddyPath))
            {
                Logging.Write("Failure to reconigze demon buddy path!");

            }
            else
            {
					 foreach (string file in System.IO.Directory.GetFiles(Funky.FolderPaths.sDemonBuddyPath+@"\Logs\"))
                {
                    DateTime curFileCreated = System.IO.Directory.GetCreationTime(file);
                    if (DateTime.Now.Subtract(curFileCreated).TotalHours >= 24)
                    {
                        deleteList.Add(file);
                    }
                }

                if (deleteList.Count > 0)
                {
                    foreach (string item in deleteList)
                    {
                        System.IO.File.Delete(item);
                    }
                    Logging.WriteDiagnostic("Total DB logs deleted " + deleteList.Count);
                }
            }

            string ItemRulesPath = @"\Plugins\FunkyTrinity\ItemRules\Log\Archive\";
            deleteList = new List<string>();
            try
            {
					 foreach (string file in System.IO.Directory.GetFiles(Funky.FolderPaths.sDemonBuddyPath+ItemRulesPath))
                {
                    DateTime curFileCreated = System.IO.Directory.GetCreationTime(file);
                    if (DateTime.Now.Subtract(curFileCreated).TotalHours >= 24)
                    {
                        deleteList.Add(file);
                    }
                }

                if (deleteList.Count > 0)
                {
                    foreach (string item in deleteList)
                    {
                        System.IO.File.Delete(item);
                    }
                    Logging.WriteDiagnostic("Total item rule logs deleted " + deleteList.Count);
                }

            }
            catch{Logging.WriteDiagnostic("Failure to clean log files @ path: " + ItemRulesPath);}
				
				string ProfileLogs=@"\Plugins\FunkyTrinity\Log\ProfileStats\";
				deleteList=new List<string>();
				try
				{
					 foreach (string file in System.IO.Directory.GetFiles(Funky.FolderPaths.sDemonBuddyPath+ProfileLogs))
					 {
						  DateTime curFileCreated=System.IO.Directory.GetCreationTime(file);
						  if (DateTime.Now.Subtract(curFileCreated).TotalDays>=1)
						  {
								deleteList.Add(file);
						  }
					 }

					 if (deleteList.Count>0)
					 {
						  foreach (string item in deleteList)
						  {
								System.IO.File.Delete(item);
						  }
						  Logging.WriteDiagnostic("Total item rule logs deleted "+deleteList.Count);
					 }

				} catch { Logging.WriteDiagnostic("Failure to clean log files @ path: "+ProfileLogs); }
		  
		  }
    }
}