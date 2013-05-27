using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Markup;
using Zeta.Common;
using System.Collections.Generic;
using Zeta;
using Zeta.CommonBot;
using Zeta.Navigation;
using Zeta.Internals.Actors;
using System.Threading;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  private static bool bSavingConfig=false;

		  // **********************************************************************************************
		  // *****              Arrange your stash by highest to lowest scoring items                 *****
		  // **********************************************************************************************
		  public class GilesStashSort
		  {
				public double dStashScore { get; set; }
				public int iStashOrPack { get; set; }
				public int iInventoryColumn { get; set; }
				public int iInventoryRow { get; set; }
				public int iDynamicID { get; set; }
				public bool bIsTwoSlot { get; set; }

				public GilesStashSort(double stashscore, int stashorpack, int icolumn, int irow, int dynamicid, bool twoslot)
				{
					 dStashScore=stashscore;
					 iStashOrPack=stashorpack;
					 iInventoryColumn=icolumn;
					 iInventoryRow=irow;
					 iDynamicID=dynamicid;
					 bIsTwoSlot=twoslot;
				}
		  }
		  #region SortStash
		  /*
		  private static void SortStash()
		  {
				// Try and update the player-data
				ZetaDia.Actors.Update();
				// Check we can get the player dynamic ID
				int iPlayerDynamicID=-1;
				try
				{
					 iPlayerDynamicID=ZetaDia.Me.CommonData.DynamicId;
				} catch
				{
					 Log("Failure getting your player data from DemonBuddy, abandoning the sort!");
					 return;
				}
				if (iPlayerDynamicID==-1)
				{
					 Log("Failure getting your player data, abandoning the sort!");
					 return;
				}

				// List used for all the sorting
				List<GilesStashSort> listSortMyStash=new List<GilesStashSort>();

				// Map out the backpack free slots
				for (int iRow=0; iRow<=5; iRow++)
					 for (int iColumn=0; iColumn<=9; iColumn++)
						  GilesBackpackSlotBlocked[iColumn, iRow]=false;
				foreach (ACDItem tempitem in ZetaDia.Me.Inventory.Backpack)
				{
					 int inventoryRow=tempitem.InventoryRow;
					 int inventoryColumn=tempitem.InventoryColumn;
					 // Mark this slot as not-free
					 GilesBackpackSlotBlocked[inventoryColumn, inventoryRow]=true;
					 // Try and reliably find out if this is a two slot item or not
					 GilesItemType tempItemType=DetermineItemType(tempitem.InternalName, tempitem.ItemType, tempitem.FollowerSpecialType);
					 if (DetermineIsTwoSlot(tempItemType)&&inventoryRow<5)
					 {
						  GilesBackpackSlotBlocked[inventoryColumn, inventoryRow+1]=true;
					 }
				}

				// Map out the stash free slots
				for (int iRow=0; iRow<=29; iRow++)
					 for (int iColumn=0; iColumn<=6; iColumn++)
						  GilesStashSlotBlocked[iColumn, iRow]=false;
				// Block off the entire of any "protected stash pages"
				foreach (int iProtPage in Zeta.CommonBot.Settings.CharacterSettings.Instance.ProtectedStashPages)
					 for (int iProtRow=0; iProtRow<=9; iProtRow++)
						  for (int iProtColumn=0; iProtColumn<=6; iProtColumn++)
								GilesStashSlotBlocked[iProtColumn, iProtRow+(iProtPage*10)]=true;
				// Remove rows we don't have
				for (int iRow=(ZetaDia.Me.NumSharedStashSlots/7); iRow<=29; iRow++)
					 for (int iColumn=0; iColumn<=6; iColumn++)
						  GilesStashSlotBlocked[iColumn, iRow]=true;

				// Map out all the items already in the stash and store their scores if appropriate
				foreach (ACDItem thisitem in ZetaDia.Me.Inventory.StashItems)
				{
					 int inventoryRow=thisitem.InventoryRow;
					 int inventoryColumn=thisitem.InventoryColumn;
					 // Mark this slot as not-free
					 GilesStashSlotBlocked[inventoryColumn, inventoryRow]=true;
					 // Try and reliably find out if this is a two slot item or not
					 GilesItemType tempItemType=DetermineItemType(thisitem.InternalName, thisitem.ItemType, thisitem.FollowerSpecialType);
					 bool bIsTwoSlot=DetermineIsTwoSlot(tempItemType);
					 if (bIsTwoSlot&&inventoryRow!=19&&inventoryRow!=9&&inventoryRow!=29)
					 {
						  GilesStashSlotBlocked[inventoryColumn, inventoryRow+1]=true;
					 }
					 else if (bIsTwoSlot&&(inventoryRow==19||inventoryRow==9||inventoryRow==29))
					 {
						  Log("WARNING: There was an error reading your stash, abandoning the process.");
						  Log("Always make sure you empty your backpack, open the stash, then RESTART DEMONBUDDY before sorting!");
						  return;
					 }
					 CacheACDItem thiscacheditem=new CacheACDItem(thisitem.InternalName, thisitem.Name, thisitem.Level, thisitem.ItemQualityLevel, thisitem.Gold, thisitem.GameBalanceId,
						  thisitem.DynamicId, thisitem.Stats.WeaponDamagePerSecond, thisitem.IsOneHand, thisitem.DyeType, thisitem.ItemType, thisitem.FollowerSpecialType,
						  thisitem.IsUnidentified, thisitem.ItemStackQuantity, thisitem.Stats, thisitem, thisitem.InventoryRow, thisitem.InventoryColumn, thisitem.IsPotion, thisitem.ACDGuid);
					 double iThisItemValue=ValueThisItem(thiscacheditem, tempItemType);
					 double iNeedScore=ScoreNeeded(tempItemType);
					 // Ignore stackable items
					 if (!DetermineIsStackable(tempItemType)&&tempItemType!=GilesItemType.StaffOfHerding)
					 {
						  listSortMyStash.Add(new GilesStashSort(((iThisItemValue/iNeedScore)*1000), 1, inventoryColumn, inventoryRow, thisitem.DynamicId, bIsTwoSlot));
					 }
				} // Loop through all stash items

				// Sort the items in the stash by their row number, lowest to highest
				listSortMyStash.Sort((p1, p2) => p1.iInventoryRow.CompareTo(p2.iInventoryRow));

				// Now move items into your backpack until full, then into the END of the stash
				Vector2 vFreeSlot;
				foreach (GilesStashSort thisstashsort in listSortMyStash)
				{
					 vFreeSlot=SortingFindLocationBackpack(thisstashsort.bIsTwoSlot);
					 int iStashOrPack=1;
					 if (vFreeSlot.X==-1||vFreeSlot.Y==-1)
					 {
						  vFreeSlot=SortingFindLocationStash(thisstashsort.bIsTwoSlot, true);
						  if (vFreeSlot.X==-1||vFreeSlot.Y==-1)
								continue;
						  iStashOrPack=2;
					 }
					 if (iStashOrPack==1)
					 {
						  ZetaDia.Me.Inventory.MoveItem(thisstashsort.iDynamicID, iPlayerDynamicID, InventorySlot.PlayerBackpack, (int)vFreeSlot.X, (int)vFreeSlot.Y);
						  GilesStashSlotBlocked[thisstashsort.iInventoryColumn, thisstashsort.iInventoryRow]=false;
						  if (thisstashsort.bIsTwoSlot)
								GilesStashSlotBlocked[thisstashsort.iInventoryColumn, thisstashsort.iInventoryRow+1]=false;
						  GilesBackpackSlotBlocked[(int)vFreeSlot.X, (int)vFreeSlot.Y]=true;
						  if (thisstashsort.bIsTwoSlot)
								GilesBackpackSlotBlocked[(int)vFreeSlot.X, (int)vFreeSlot.Y+1]=true;
						  thisstashsort.iInventoryColumn=(int)vFreeSlot.X;
						  thisstashsort.iInventoryRow=(int)vFreeSlot.Y;
						  thisstashsort.iStashOrPack=2;
					 }
					 else
					 {
						  ZetaDia.Me.Inventory.MoveItem(thisstashsort.iDynamicID, iPlayerDynamicID, InventorySlot.PlayerSharedStash, (int)vFreeSlot.X, (int)vFreeSlot.Y);
						  GilesStashSlotBlocked[thisstashsort.iInventoryColumn, thisstashsort.iInventoryRow]=false;
						  if (thisstashsort.bIsTwoSlot)
								GilesStashSlotBlocked[thisstashsort.iInventoryColumn, thisstashsort.iInventoryRow+1]=false;
						  GilesStashSlotBlocked[(int)vFreeSlot.X, (int)vFreeSlot.Y]=true;
						  if (thisstashsort.bIsTwoSlot)
								GilesStashSlotBlocked[(int)vFreeSlot.X, (int)vFreeSlot.Y+1]=true;
						  thisstashsort.iInventoryColumn=(int)vFreeSlot.X;
						  thisstashsort.iInventoryRow=(int)vFreeSlot.Y;
						  thisstashsort.iStashOrPack=1;
					 }
					 Thread.Sleep(150);
				}

				// Now sort the items by their score, highest to lowest
				listSortMyStash.Sort((p1, p2) => p1.dStashScore.CompareTo(p2.dStashScore));
				listSortMyStash.Reverse();

				// Now fill the stash in ordered-order
				foreach (GilesStashSort thisstashsort in listSortMyStash)
				{
					 vFreeSlot=SortingFindLocationStash(thisstashsort.bIsTwoSlot, false);
					 if (vFreeSlot.X==-1||vFreeSlot.Y==-1)
					 {
						  Log("Failure trying to put things back into stash, no stash slots free? Abandoning...");
						  return;
					 }
					 ZetaDia.Me.Inventory.MoveItem(thisstashsort.iDynamicID, iPlayerDynamicID, InventorySlot.PlayerSharedStash, (int)vFreeSlot.X, (int)vFreeSlot.Y);
					 if (thisstashsort.iStashOrPack==1)
					 {
						  GilesStashSlotBlocked[thisstashsort.iInventoryColumn, thisstashsort.iInventoryRow]=false;
						  if (thisstashsort.bIsTwoSlot)
								GilesStashSlotBlocked[thisstashsort.iInventoryColumn, thisstashsort.iInventoryRow+1]=false;
					 }
					 else
					 {
						  GilesBackpackSlotBlocked[thisstashsort.iInventoryColumn, thisstashsort.iInventoryRow]=false;
						  if (thisstashsort.bIsTwoSlot)
								GilesBackpackSlotBlocked[thisstashsort.iInventoryColumn, thisstashsort.iInventoryRow+1]=false;
					 }
					 GilesStashSlotBlocked[(int)vFreeSlot.X, (int)vFreeSlot.Y]=true;
					 if (thisstashsort.bIsTwoSlot)
						  GilesStashSlotBlocked[(int)vFreeSlot.X, (int)vFreeSlot.Y+1]=true;
					 thisstashsort.iStashOrPack=1;
					 thisstashsort.iInventoryRow=(int)vFreeSlot.Y;
					 thisstashsort.iInventoryColumn=(int)vFreeSlot.X;
					 Thread.Sleep(150);
				}

				Log("Stash sorted!");
		  } 
		  */
		  #endregion

		  // **********************************************************************************************
		  // *****                            Current plugin settings                                 *****
		  // **********************************************************************************************
		  public class GilesSettings
		  {
				// Performance stuff
				public bool bEnableTPS { get; set; }
				public double iTPSAmount { get; set; }

				// Enable Prowl
				public bool bEnableProwl { get; set; }
				public bool bEnableAndroid { get; set; }
				public double iNeedPointsToNotifyJewelry { get; set; }
				public double iNeedPointsToNotifyArmor { get; set; }
				public double iNeedPointsToNotifyWeapon { get; set; }
				// Log stuck points
				public bool bLogStucks { get; set; }
				// Enable unstucker
				public bool bEnableUnstucker { get; set; }
				public bool bEnableProfileReloading { get; set; }

				// Defaults For All Settings can be set here
				#region DefaultSettings
				public GilesSettings(bool enablecriticalmass=false,
						  bool enabletps=false, double tpsamount=10, bool logstucks=false, bool enableunstucker=true, bool selectiveww=false,
						  bool enableprowl=false, bool enableandroid=false,
						  double pointsnotifyj=28000, double pointsnotifya=30000, double pointsnotifyw=100000, int killlootdelay=800, int vaultdelay=400,
						  bool waitforwrath=true, bool goblinwrath=true, bool furydumpwrath=true, bool furydumpalways=false, int filterlegendary=1, bool profilereload=false,
						  bool monkinna=false, bool waitarchon=true, bool kitearchon=false, bool wrath90=false)
				{


					 // Advanced stuff
					 bEnableTPS=enabletps;
					 iTPSAmount=tpsamount;
					 bLogStucks=logstucks;
					 bEnableUnstucker=enableunstucker;
					 bEnableProfileReloading=profilereload;
					 // Enable prowl
					 bEnableProwl=enableprowl;
					 bEnableAndroid=enableandroid;
					 iNeedPointsToNotifyJewelry=pointsnotifyj;
					 iNeedPointsToNotifyArmor=pointsnotifya;
					 iNeedPointsToNotifyWeapon=pointsnotifyw;
				}
				#endregion
		  }


		  // **********************************************************************************************
		  // *****                               Save Configuration                                   *****
		  // **********************************************************************************************
		  private void SaveConfiguration()
		  {
				if (bSavingConfig) return;
				bSavingConfig=true;
				FileStream configStream=File.Open(FolderPaths.sTrinityConfigFile, FileMode.Create, FileAccess.Write, FileShare.Read);
				using (StreamWriter configWriter=new StreamWriter(configStream))
				{

					 
					 configWriter.WriteLine("LogStucks="+settings.bLogStucks.ToString());
					 configWriter.WriteLine("Unstucker="+settings.bEnableUnstucker.ToString());
					 configWriter.WriteLine("ProfileReloading="+settings.bEnableProfileReloading.ToString());

					 configWriter.WriteLine(settings.bEnableTPS?"TPSEnabled=true":"TPSEnabled=false");
					 configWriter.WriteLine("TPSAmount="+settings.iTPSAmount.ToString());

					 configWriter.WriteLine(settings.bEnableProwl?"EnableProwl=true":"EnableProwl=false");
					 configWriter.WriteLine(settings.bEnableAndroid?"EnableAndroid=true":"EnableAndroid=false");
					 configWriter.WriteLine("ProwlKey="+sProwlAPIKey);
					 configWriter.WriteLine("AndroidKey="+sAndroidAPIKey);
					 configWriter.WriteLine("JewelryNotify="+settings.iNeedPointsToNotifyJewelry.ToString());
					 configWriter.WriteLine("ArmorNotify="+settings.iNeedPointsToNotifyArmor.ToString());
					 configWriter.WriteLine("WeaponNotify="+settings.iNeedPointsToNotifyWeapon.ToString());

					 

				}
				//configStream.Close();
				bSavingConfig=false;
		  }


		  // **********************************************************************************************
		  // *****                               Load Configuration                                   *****
		  // **********************************************************************************************

		  private void LoadConfiguration()
		  {
				//Check for Config file
				if (!File.Exists(FolderPaths.sTrinityConfigFile))
				{
					 Log("No config file found, now creating a new config from defaults at: "+FolderPaths.sTrinityConfigFile);
					 SaveConfiguration();
					 return;
				}
				//Load File
				using (StreamReader configReader=new StreamReader(FolderPaths.sTrinityConfigFile))
				{
					 while (!configReader.EndOfStream)
					 {
						  string[] config=configReader.ReadLine().Split('=');
						  if (config!=null)
						  {
								switch (config[0])
								{
									 case "JewelryNotify":
										  settings.iNeedPointsToNotifyJewelry=Convert.ToDouble(config[1]);
										  break;
									 case "ArmorNotify":
										  settings.iNeedPointsToNotifyArmor=Convert.ToDouble(config[1]);
										  break;
									 case "WeaponNotify":
										  settings.iNeedPointsToNotifyWeapon=Convert.ToDouble(config[1]);
										  break;
									 case "TPSEnabled":
										  settings.bEnableTPS=Convert.ToBoolean(config[1]);
										  break;
									 case "TPSAmount":
										  settings.iTPSAmount=Convert.ToDouble(config[1]);
										  break;
									 case "LogStucks":
										  settings.bLogStucks=Convert.ToBoolean(config[1]);
										  break;
									 case "ProfileReloading":
										  settings.bEnableProfileReloading=Convert.ToBoolean(config[1]);
										  break;
									 case "Unstucker":
										  settings.bEnableUnstucker=Convert.ToBoolean(config[1]);
										  if (settings.bEnableUnstucker)
												Navigator.StuckHandler=new TrinityStuckHandler();
										  else
												Navigator.StuckHandler=new DefaultStuckHandler();
										  break;
									 case "EnableProwl":
										  settings.bEnableProwl=Convert.ToBoolean(config[1]);
										  break;
									 case "ProwlKey":
										  sProwlAPIKey=config[1];
										  break;
									 case "EnableAndroid":
										  settings.bEnableAndroid=Convert.ToBoolean(config[1]);
										  break;
									 case "AndroidKey":
										  sAndroidAPIKey=config[1];
										  break;
								}
						  }
					 }
					 //configReader.Close();
				}
				Logging.WriteDiagnostic("[Funky] Plugin Settings Loaded");
		  }



		  // ********************************************
		  // *********** CONFIG WINDOW REGION ***********
		  // ********************************************
		  #region configWindow

		  // First we create a variable that is of the "type" of the actual config window item - eg a "RadioButton" for each, well, radiobutton
		  // Later on we will "Link" these variables to the ACTUAL items within the XAML file, so we can do things with the XAML stuff
		  // I try to match the names of the variables here, with the "Name=" I give the item in the XAML - this isn't necessary, but makes things simpler
		  private Button saveButton, defaultButton, resetAdvanced, resetMobile;
		  private CheckBox checkTPS, checkLogStucks, checkUnstucker,  checkProwl, checkAndroid,
				checkProfileReload;
		  private Slider  slideTPS,
				slideNotifyWeapon, slideNotifyJewelry, slideNotifyArmor;
		  private TextBox JewelryNotifyText, ArmorNotifyText, WeaponNotifyText,
				textTPS, textProwlKey, textAndroidKey;
		  // This is needed by DB, is essentially the ACTUAL window object itself
		  private Window configWindow;

		  // This is what "creates" the window
		  public Window DisplayWindow
		  {
				get
				{
					 
					 // Check we can actually find the .xaml file first - if not, report an error
					 if (!File.Exists(FolderPaths.sTrinityPluginPath+"Trinity.xaml"))
						  Log("ERROR: Can't find \""+FolderPaths.sTrinityPluginPath+"Trinity.xaml\"");
					 try
					 {
						  if (configWindow==null)
						  {
								configWindow=new Window();
						  }
						  StreamReader xamlStream=new StreamReader(FolderPaths.sTrinityPluginPath+"Trinity.xaml");
						  DependencyObject xamlContent=XamlReader.Load(xamlStream.BaseStream) as DependencyObject;
						  configWindow.Content=xamlContent;

						  // I'm not going to comment everything below - it's all pretty similar
						  // Basically the concept is this:
						  // You take the variable you created above (30 lines up or so), and you use "FindLogicalNode" to sort of "link" the variable, to that object within the XAML file
						  // By using the "Name" tag as the way of finding it

						  // After assigning the variable to the actual node in the XAML, you then need to add event handlers - so we can do things when the user makes changes to those elements
						  // You can also alter settings and values of the nodes - eg the min-max values, the current value etc. - by using the variable we link

						  // Now - the huge list below is because I have so many damned config options of different types!
						  // Note that I do *NOT* have any events on text boxes - because I set all textboxes to uneditable/unchangeable - they are "read only"
						  // I simply use them to show the user what the slider-value is currently set to (so when the slider changes, my code updates the text box)

						  Button Funkysettings=LogicalTreeHelper.FindLogicalNode(xamlContent, "buttonFunkySettings") as Button;
						  Funkysettings.Click+=buttonFunkySettings_Click;

						  slideNotifyWeapon=LogicalTreeHelper.FindLogicalNode(xamlContent, "slideWeaponNotifyScore") as Slider;
						  slideNotifyWeapon.ValueChanged+=trackNotifyWeapons_Scroll;
						  slideNotifyWeapon.SmallChange=200;
						  slideNotifyWeapon.LargeChange=1000;
						  slideNotifyWeapon.TickFrequency=2000;
						  slideNotifyWeapon.IsSnapToTickEnabled=true;

						  slideNotifyJewelry=LogicalTreeHelper.FindLogicalNode(xamlContent, "slideJewelryNotifyScore") as Slider;
						  slideNotifyJewelry.ValueChanged+=trackNotifyJewelry_Scroll;
						  slideNotifyJewelry.SmallChange=100;
						  slideNotifyJewelry.LargeChange=500;
						  slideNotifyJewelry.TickFrequency=1000;
						  slideNotifyJewelry.IsSnapToTickEnabled=true;

						  slideNotifyArmor=LogicalTreeHelper.FindLogicalNode(xamlContent, "slideArmorNotifyScore") as Slider;
						  slideNotifyArmor.ValueChanged+=trackNotifyArmor_Scroll;
						  slideNotifyArmor.SmallChange=100;
						  slideNotifyArmor.LargeChange=500;
						  slideNotifyArmor.TickFrequency=1000;
						  slideNotifyArmor.IsSnapToTickEnabled=true;

						  JewelryNotifyText=LogicalTreeHelper.FindLogicalNode(xamlContent, "JewelryNotifyScore") as TextBox;
						  ArmorNotifyText=LogicalTreeHelper.FindLogicalNode(xamlContent, "ArmorNotifyScore") as TextBox;
						  WeaponNotifyText=LogicalTreeHelper.FindLogicalNode(xamlContent, "WeaponNotifyScore") as TextBox;


						  // prowl stuff
						  checkProwl=LogicalTreeHelper.FindLogicalNode(xamlContent, "checkProwl") as CheckBox;
						  checkProwl.Checked+=checkProwl_check;
						  checkProwl.Unchecked+=checkProwl_uncheck;
						  textProwlKey=LogicalTreeHelper.FindLogicalNode(xamlContent, "txtProwlAPI") as TextBox;
						  textProwlKey.TextChanged+=textProwl_change;

						  // android stuff
						  checkAndroid=LogicalTreeHelper.FindLogicalNode(xamlContent, "checkAndroid") as CheckBox;
						  checkAndroid.Checked+=checkAndroid_check;
						  checkAndroid.Unchecked+=checkAndroid_uncheck;
						  textAndroidKey=LogicalTreeHelper.FindLogicalNode(xamlContent, "txtAndroidAPI") as TextBox;
						  textAndroidKey.TextChanged+=textAndroid_change;

						  checkTPS=LogicalTreeHelper.FindLogicalNode(xamlContent, "checkTPS") as CheckBox;
						  checkTPS.Checked+=checkTPS_check;
						  checkTPS.Unchecked+=checkTPS_uncheck;
						  textTPS=LogicalTreeHelper.FindLogicalNode(xamlContent, "textTPS") as TextBox;
						  slideTPS=LogicalTreeHelper.FindLogicalNode(xamlContent, "slideTPS") as Slider;
						  slideTPS.ValueChanged+=trackTPS_Scroll;
						  slideTPS.SmallChange=1;
						  slideTPS.LargeChange=1;
						  slideTPS.TickFrequency=5;
						  slideTPS.IsSnapToTickEnabled=false;

						  checkLogStucks=LogicalTreeHelper.FindLogicalNode(xamlContent, "checkLogStucks") as CheckBox;
						  checkLogStucks.Checked+=checkLogStucks_check;
						  checkLogStucks.Unchecked+=checkLogStucks_uncheck;

						  checkUnstucker=LogicalTreeHelper.FindLogicalNode(xamlContent, "checkUnstucker") as CheckBox;
						  checkUnstucker.Checked+=checkUnstucker_check;
						  checkUnstucker.Unchecked+=checkUnstucker_uncheck;


						  checkProfileReload=LogicalTreeHelper.FindLogicalNode(xamlContent, "checkProfileReload") as CheckBox;
						  checkProfileReload.Checked+=checkProfileReload_check;
						  checkProfileReload.Unchecked+=checkProfileReload_uncheck;




						  // Finally the "defaults" button, and the save config button

						  defaultButton=LogicalTreeHelper.FindLogicalNode(xamlContent, "buttonDefaults") as Button;
						  defaultButton.Click+=buttonDefaults_Click;

						  resetAdvanced=LogicalTreeHelper.FindLogicalNode(xamlContent, "ResetAdvanced") as Button;
						  resetAdvanced.Click+=resetAdvanced_Click;

						  resetMobile=LogicalTreeHelper.FindLogicalNode(xamlContent, "ResetMobile") as Button;
						  resetMobile.Click+=resetMobile_Click;

						  saveButton=LogicalTreeHelper.FindLogicalNode(xamlContent, "buttonSave") as Button;
						  saveButton.Click+=buttonSave_Click;

						  UserControl mainControl=LogicalTreeHelper.FindLogicalNode(xamlContent, "mainControl") as UserControl;
						  // Set height and width and window title of main window
						  configWindow.Height=mainControl.Height+30;
						  configWindow.Width=mainControl.Width;
						  configWindow.Title="Giles Trinity";

						  // Event handling for the config window loading up/closing
						  configWindow.Loaded+=configWindow_Loaded;
						  configWindow.Closed+=configWindow_Closed;

						  // And finally put all of this content in effect
						  configWindow.Content=xamlContent;
					 } catch (XamlParseException ex)
					 {
						  // Log specific XAML exceptions that might have happened above
						  Log(ex.ToString());
					 } catch (Exception ex)
					 {
						  // Log any other issues
						  Log(ex.ToString());
					 }
					 return configWindow;
				}
		  }
		  // The below are all event handlers for all the window-elements within the config window
		  // WARNING: If you use code to alter the value of something that has an event attached...
		  // For example a slider - then your code automatically also fires the event for that slider
		  // I use "suppresseventchanges" to make sure that event code ONLY gets called from the USER changing values
		  // And NOT from my own code trying to change values
		  private static bool bSuppressEventChanges=false;

		  
		  private void checkAndroid_check(object sender, RoutedEventArgs e)
		  {
				if (bSuppressEventChanges)
					 return;
				settings.bEnableAndroid=true;
		  }
		  private void checkAndroid_uncheck(object sender, RoutedEventArgs e)
		  {
				if (bSuppressEventChanges)
					 return;
				settings.bEnableAndroid=false;
		  }
		  private void textAndroid_change(object sender, TextChangedEventArgs e)
		  {
				if (bSuppressEventChanges)
					 return;
				sAndroidAPIKey=textAndroidKey.Text;
		  }
		  private void checkProwl_check(object sender, RoutedEventArgs e)
		  {
				if (bSuppressEventChanges)
					 return;
				settings.bEnableProwl=true;
		  }
		  private void checkProwl_uncheck(object sender, RoutedEventArgs e)
		  {
				if (bSuppressEventChanges)
					 return;
				settings.bEnableProwl=false;
		  }
		  private void textProwl_change(object sender, TextChangedEventArgs e)
		  {
				if (bSuppressEventChanges)
					 return;
				sProwlAPIKey=textProwlKey.Text;
		  }


		  private void trackNotifyWeapons_Scroll(object sender, RoutedPropertyChangedEventArgs<double> e)
		  {
				if (bSuppressEventChanges)
					 return;
				slideNotifyWeapon.Value=Math.Round(slideNotifyWeapon.Value);
				settings.iNeedPointsToNotifyWeapon=slideNotifyWeapon.Value;
				WeaponNotifyText.Text=slideNotifyWeapon.Value.ToString();
		  }
		  private void trackNotifyArmor_Scroll(object sender, RoutedPropertyChangedEventArgs<double> e)
		  {
				if (bSuppressEventChanges)
					 return;
				slideNotifyArmor.Value=Math.Round(slideNotifyArmor.Value);
				settings.iNeedPointsToNotifyArmor=slideNotifyArmor.Value;
				ArmorNotifyText.Text=slideNotifyArmor.Value.ToString();
		  }
		  private void trackNotifyJewelry_Scroll(object sender, RoutedPropertyChangedEventArgs<double> e)
		  {
				if (bSuppressEventChanges)
					 return;
				slideNotifyJewelry.Value=Math.Round(slideNotifyJewelry.Value);
				settings.iNeedPointsToNotifyJewelry=slideNotifyJewelry.Value;
				JewelryNotifyText.Text=slideNotifyJewelry.Value.ToString();
		  }
		  private void checkTPS_check(object sender, RoutedEventArgs e)
		  {
				if (bSuppressEventChanges)
					 return;
				settings.bEnableTPS=true;
				BotMain.TicksPerSecond=(int)settings.iTPSAmount;
		  }
		  private void checkTPS_uncheck(object sender, RoutedEventArgs e)
		  {
				if (bSuppressEventChanges)
					 return;
				settings.bEnableTPS=false;
				BotMain.TicksPerSecond=10;
		  }
		  private void checkProfileReload_check(object sender, RoutedEventArgs e)
		  {
				if (bSuppressEventChanges)
					 return;
				settings.bEnableProfileReloading=true;
		  }
		  private void checkProfileReload_uncheck(object sender, RoutedEventArgs e)
		  {
				if (bSuppressEventChanges)
					 return;
				settings.bEnableProfileReloading=false;
		  }
		  private void checkUnstucker_check(object sender, RoutedEventArgs e)
		  {
				if (bSuppressEventChanges)
					 return;
				settings.bEnableUnstucker=true;
				Navigator.StuckHandler=new TrinityStuckHandler();
		  }
		  private void checkUnstucker_uncheck(object sender, RoutedEventArgs e)
		  {
				if (bSuppressEventChanges)
					 return;
				settings.bEnableUnstucker=false;
				Navigator.StuckHandler=new DefaultStuckHandler();
		  }
		  private void checkLogStucks_check(object sender, RoutedEventArgs e)
		  {
				if (bSuppressEventChanges)
					 return;
				settings.bLogStucks=true;
		  }
		  private void checkLogStucks_uncheck(object sender, RoutedEventArgs e)
		  {
				if (bSuppressEventChanges)
					 return;
				settings.bLogStucks=false;
		  }

		  private void trackTPS_Scroll(object sender, RoutedPropertyChangedEventArgs<double> e)
		  {
				if (bSuppressEventChanges)
					 return;
				slideTPS.Value=Math.Round(slideTPS.Value);
				textTPS.Text=slideTPS.Value.ToString();
				settings.iTPSAmount=Math.Round(slideTPS.Value);
				if (settings.bEnableTPS)
				{
					 BotMain.TicksPerSecond=(int)settings.iTPSAmount;
				}
		  }





		  // Event handler for the config window being closed
		  private void configWindow_Closed(object sender, EventArgs e)
		  {
				configWindow=null;
		  }
		  // Event handler for the config window loading, update all the window elements!
		  private void configWindow_Loaded(object sender, RoutedEventArgs e)
		  {
				settingsWindowResetValues();
		  }
		  private void buttonFunkySettings_Click(object sender, RoutedEventArgs e)
		  {
				buttonFunkySettingDB_Click(null, null);
				configWindow.Close();
		  }
		  // Button-clicked for saving the config
		  private void buttonSave_Click(object sender, RoutedEventArgs e)
		  {
				SaveConfiguration();
				configWindow.Close();
		  }
		  // Default button clicked
		  private void buttonDefaults_Click(object sender, RoutedEventArgs e)
		  {
				settings=new GilesSettings();
				settingsWindowResetValues();
		  }

		  private void resetAdvanced_Click(object sender, RoutedEventArgs e)
		  {
				GilesSettings tempsettings=new GilesSettings();
				settings.bEnableTPS=tempsettings.bEnableTPS;
				settings.iTPSAmount=tempsettings.iTPSAmount;
				settings.bLogStucks=tempsettings.bLogStucks;
				settings.bEnableUnstucker=tempsettings.bEnableUnstucker;
				settings.bEnableProfileReloading=tempsettings.bEnableProfileReloading;
				settingsWindowResetValues();
		  }
		  private void resetMobile_Click(object sender, RoutedEventArgs e)
		  {
				GilesSettings tempsettings=new GilesSettings();
				settings.bEnableProwl=tempsettings.bEnableProwl;
				settings.bEnableAndroid=tempsettings.bEnableAndroid;
				settings.iNeedPointsToNotifyJewelry=tempsettings.iNeedPointsToNotifyJewelry;
				settings.iNeedPointsToNotifyArmor=tempsettings.iNeedPointsToNotifyArmor;
				settings.iNeedPointsToNotifyWeapon=tempsettings.iNeedPointsToNotifyWeapon;
				settingsWindowResetValues();
		  }

		  // This function sets all of the window elements of the config window, to the current actual values held in the variables
		  private void settingsWindowResetValues()
		  {
				bSuppressEventChanges=true;


				slideNotifyWeapon.Value=Math.Round(settings.iNeedPointsToNotifyWeapon);
				WeaponNotifyText.Text=slideNotifyWeapon.Value.ToString();
				slideNotifyArmor.Value=Math.Round(settings.iNeedPointsToNotifyArmor);
				ArmorNotifyText.Text=slideNotifyArmor.Value.ToString();
				slideNotifyJewelry.Value=Math.Round(settings.iNeedPointsToNotifyJewelry);
				JewelryNotifyText.Text=slideNotifyJewelry.Value.ToString();


				textTPS.Text=settings.iTPSAmount.ToString();
				slideTPS.Value=settings.iTPSAmount;
				checkTPS.IsChecked=settings.bEnableTPS;
				checkLogStucks.IsChecked=settings.bLogStucks;
				checkProwl.IsChecked=settings.bEnableProwl;
				textProwlKey.Text=sProwlAPIKey;
				checkAndroid.IsChecked=settings.bEnableAndroid;
				textAndroidKey.Text=sAndroidAPIKey;
				checkUnstucker.IsChecked=settings.bEnableUnstucker;
				checkProfileReload.IsChecked=settings.bEnableProfileReloading;

				bSuppressEventChanges=false;
		  }
		  #endregion
		  // ***************************************************
		  // *********** END OF CONFIG WINDOW REGION ***********
		  // ***************************************************
	 }
}