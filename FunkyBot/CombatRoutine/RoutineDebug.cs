using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.IO;
using Demonbuddy;
using Zeta.Common;
using Zeta.Common.Compiler;
using Zeta.Common.Plugins;
using Zeta.CommonBot;
using Zeta.CommonBot.Logic;
using Zeta.Internals.Actors;
using System.Collections.Generic;
using Zeta;
using System.Windows;
using System.Reflection;

namespace GilesBlankCombatRoutine
{
	 public class FunkyDebug
	 {
		  private static Label lblDebug_DumpUnits, lblDebug_OpenLog, lblDebug_DumpUnitAttributes, lblDebug_DumpObjects, lblDebug_FunkyLog;
		  private static MenuItem menuItem_Debug, menuItem_Debug_Units;

		  public static void initDebugLabels(out SplitButton btn)
		  {
				btn=new SplitButton
				{
					 Width=125,
					 Height=20,
					 HorizontalAlignment=HorizontalAlignment.Left,
					 VerticalAlignment=VerticalAlignment.Top,
					 Margin=new Thickness(425, 10, 0, 0),
					 IsEnabled=true,
					 Content="Funky",
					 Name="Funky",
				};
				btn.Click+=lblFunky_Click;

				lblDebug_OpenLog=new Label
				{
					 Content="Open DB LogFile",
					 Width=100,
					 Height=25,
					 HorizontalAlignment=HorizontalAlignment.Stretch,
				};
				lblDebug_OpenLog.MouseDown+=lblDebug_OpenDBLog;

				lblDebug_FunkyLog=new Label
				{
					 Content="Open Funky LogFile",
					 Width=100,
					 Height=25,
					 HorizontalAlignment=HorizontalAlignment.Stretch,
				};
				lblDebug_FunkyLog.MouseDown+=lblDebug_OpenFunkyLog;

				Label OpenTrinityFolder=new Label
				{
					 Content="Open Funky Folder",
					 Width=100,
					 Height=25,
					 HorizontalAlignment=HorizontalAlignment.Stretch,

				};
				OpenTrinityFolder.MouseDown+=lblDebug_OpenTrinityFolder;

				Label Recompile=new Label
				{
					 Content="Recompile Funky",
					 Width=100,
					 Height=25,
					 HorizontalAlignment= HorizontalAlignment.Stretch,

				};
				Recompile.MouseDown+=lblCompile_Click;

				lblDebug_DumpUnits=new Label
				{
					 Content="Simple",
					 Width=100,
					 Height=25
				};
				lblDebug_DumpUnits.MouseDown+=lblDebug_DumpUnits_Click;

				lblDebug_DumpUnitAttributes=new Label
				{
					 Content="with Attributes",
					 Width=100,
					 Height=25
				};
				lblDebug_DumpUnitAttributes.MouseDown+=lblDebug_DumpUnitsAttributes_Click;

				menuItem_Debug_Units=new MenuItem
				{
					 Header="Dump Units",
					 Width=100,
					 Height=25
				};
				menuItem_Debug_Units.Items.Add(lblDebug_DumpUnits);
				menuItem_Debug_Units.Items.Add(lblDebug_DumpUnitAttributes);

				menuItem_Debug=new MenuItem
				{
					 Header="Debuging",
					 Width=125
				};
				menuItem_Debug.Items.Add(lblDebug_OpenLog);
				menuItem_Debug.Items.Add(lblDebug_FunkyLog);
				menuItem_Debug.Items.Add(OpenTrinityFolder);
				menuItem_Debug.Items.Add(Recompile);
				btn.ButtonMenuItemsSource.Add(menuItem_Debug);

				MenuItem menuItem_DumpInfo=new MenuItem
				{
					 Header="Info Dumping",
					 Width=125
				};
				menuItem_DumpInfo.Items.Add(lblDebug_DumpObjects);
				menuItem_DumpInfo.Items.Add(menuItem_Debug_Units);
				//btn.ButtonMenuItemsSource.Add(menuItem_DumpInfo);

		  }
		  //private static GridPointAreaCache.GPMap map=null;
		  static void lblFunky_Click(object sender, EventArgs e)
		  {
			  try
			  {
					BotMain.CurrentBot.ConfigWindow.Show();
			  }
			  catch
			  {

			  }
				
		  }
		  static void lblCompile_Click(object sender, EventArgs e)
		  {
				RecompilePlugins();
				//ZetaDia.Actors.Update();
				//Bot.Character_.Data.IsSurrounded();
		  }

		  private static void RecompilePlugins()
		  {
				if (BotMain.IsRunning)
				{
					 BotMain.Stop(false, "Recompiling Plugin!");
					 while (BotMain.BotThread.IsAlive)
						  Thread.Sleep(0);
				}

				var EnabledPlugins=PluginManager.GetEnabledPlugins().ToArray();

				PluginManager.ShutdownAllPlugins();

				Logging.WriteDiagnostic("Removing Funky from plugins");
				while (PluginManager.Plugins.Any(p => p.Plugin.Name=="Funky"))
				{
					 PluginManager.Plugins.Remove(PluginManager.Plugins.First(p => p.Plugin.Name=="Funky"));
				}

				Logging.WriteDiagnostic("Clearing all treehooks");
				TreeHooks.Instance.ClearAll();

				Logging.WriteDiagnostic("Disposing of current bot");
				BotMain.CurrentBot.Dispose();

				Logging.WriteDiagnostic("Removing old Assemblies");
				CodeCompiler.DeleteOldAssemblies();

				string sDemonBuddyPath=Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
				string sTrinityPluginPath=sDemonBuddyPath+@"\Plugins\FunkyBot\";

				CodeCompiler FunkyCode=new CodeCompiler(sTrinityPluginPath);
				FunkyCode.ParseFilesForCompilerOptions();
				Logging.WriteDiagnostic("Recompiling Funky Plugin");
				FunkyCode.Compile();
				Logging.WriteDiagnostic(FunkyCode.CompiledToLocation);



				TreeHooks.Instance.ClearAll();
				BrainBehavior.CreateBrain();

				Logging.WriteDiagnostic("Reloading Plugins");
				PluginManager.ReloadAllPlugins(sDemonBuddyPath+@"\Plugins\");

				Logging.WriteDiagnostic("Enabling Plugins");
				PluginManager.SetEnabledPlugins(EnabledPlugins);
		  }
		  static void lblDebug_OpenDBLog(object sender, EventArgs e)
		  {
				try
				{
					 Process.Start(Logging.LogFilePath);

				} catch (Exception)
				{

				}
		  }
		  static void lblDebug_OpenFunkyLog(object sender, EventArgs e)
		  {
				//string sDemonBuddyPath=Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
				
				FileInfo demonbuddyLogFolder=new FileInfo(Logging.LogFilePath);
				if (!demonbuddyLogFolder.Directory.GetFiles().Any())
					 return;

				var newestfile=demonbuddyLogFolder.Directory.GetFiles().Where(f=>f.Name.Contains("FunkyLog")).OrderByDescending(file => file.LastWriteTime).First();
				try
				{
					 Process.Start(newestfile.FullName);
				} catch (Exception)
				{

				}
		  }

		  static void lblDebug_OpenTrinityFolder(object sender, EventArgs e)
		  {
				string sDemonBuddyPath=Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

				Process.Start(sDemonBuddyPath+@"\Plugins\FunkyBot\");
		  }

		  static void lblDebug_DumpUnits_Click(object sender, EventArgs e)
		  {
				if (!ZetaDia.IsInGame)
					 return;


				if (BotMain.IsRunning)
				{
					 Logging.Write("Stop the bot before dumping!");
					 return;
				}


				int iType=-1;
				ZetaDia.Actors.Update();
				var units=ZetaDia.Actors.GetActorsOfType<DiaUnit>(false, false)
					  .Where(o => o.IsValid&&o.ActorSNO>0)
					  .OrderBy(o => o.Distance);

				iType=DumpUnits(units, iType);
		  }

		  static void lblDebug_DumpUnitsAttributes_Click(object sender, EventArgs e)
		  {
				if (!ZetaDia.IsInGame)
					 return;


				if (BotMain.IsRunning)
				{
					 Logging.Write("Stop the bot before dumping!");
					 return;
				}


				int iType=-1;
				ZetaDia.Actors.Update();
				var units=ZetaDia.Actors.GetActorsOfType<DiaUnit>(false, false)
					  .Where(o => o.IsValid&&o.ActorSNO>0)
					  .OrderBy(o => o.Distance);

				iType=DumpUnitsAttributes(units, iType);
		  }


		  private static int DumpUnits(IEnumerable<DiaUnit> units, int iType)
		  {
				Logging.Write("[QuestTools] Units found: {0}", units.Count());
				foreach (DiaUnit o in units)
				{
					 if (!o.IsValid)
						  continue;

					 string attributesFound="";

					 foreach (ActorAttributeType aType in Enum.GetValues(typeof(ActorAttributeType)))
					 {
						  iType=GetAttribute(iType, o, aType);
						  if (iType>0)
						  {
								attributesFound+=aType.ToString()+"="+iType.ToString()+", ";
						  }
					 }
					 //Logging.Write("[QuestTools] Unit ActorSNO: {0} Name: {1} Type: {2} Position: {3} ({4}) has Attributes: {5}\n",
					 // o.ActorSNO, o.Name, o.ActorInfo.GizmoType, getProfilePosition(o.Position), o.Position.ToString(), attributesFound);

					 Logging.Write("[Debug] Unit SNO: {0} Name: {1} Type: {2} Position {3} Distance {4} Radius {5}",
							o.ActorSNO, o.Name, o.ActorType.ToString(), o.Position.ToString(), o.Distance, o.CollisionSphere.Radius);
				}
				return iType;
		  }
		  private static int DumpUnitsAttributes(IEnumerable<DiaUnit> units, int iType)
		  {
				Logging.Write("[Debug] Units found: {0}", units.Count());
				foreach (DiaUnit o in units)
				{
					 if (!o.IsValid)
						  continue;

					 string attributesFound="";

					 foreach (ActorAttributeType aType in Enum.GetValues(typeof(ActorAttributeType)))
					 {
						  iType=GetAttribute(iType, o, aType);
						  if (iType>0)
						  {
								attributesFound+=aType.ToString()+"="+iType.ToString()+", ";
						  }
					 }
					 Logging.Write("[Debug] Unit ActorSNO: {0} Name: {1} Type: {2} Position: {3} ({4}) has Attributes: {5}\n",
					  o.ActorSNO, o.Name, o.ActorInfo.GizmoType, getProfilePosition(o.Position), o.Position.ToString(), attributesFound);
				}
				return iType;
		  }

		  private static int GetAttribute(int iType, DiaObject o, ActorAttributeType aType)
		  {
				try
				{
					 iType=(int)o.CommonData.GetAttribute<ActorAttributeType>(aType);
				} catch
				{
					 iType=-1;
				}

				return iType;
		  }

		  //private static int GetItemStat(int iType, ACDItem item, Zeta.Internals.ItemStats.Stat stat)
		  //{
		  //	 try
		  //	 {
		  //		  iType=(int)item.Stats.GetStat<Zeta.Internals.ItemStats.Stat>(stat);
		  //	 } catch
		  //	 {
		  //		  iType=-1;
		  //	 }

		  //	 return iType;
		  //}

		  private static string getProfilePosition(Vector3 pos)
		  {
				return String.Format("x=\"{0:0}\" y=\"{1:0}\" z=\"{2:0}\" ", pos.X, pos.Y, pos.Z);
		  }
		  //private static string getSimplePosition(Vector3 pos)
		  //{
		  //	 return String.Format("{0:0}, {1:0}, {2:0},", pos.X, pos.Y, pos.Z);
		  //}
	 }
}