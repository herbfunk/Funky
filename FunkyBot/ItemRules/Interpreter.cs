using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using FunkyBot.Cache;
using Zeta.Internals.Actors;
using Zeta.Common;
using GilesTrinity.ItemRules.Core;
using Zeta;
using Zeta.CommonBot.Items;
using Zeta.CommonBot;

namespace FunkyBot
{
	 #region Interpreter

	 /// <summary>
	 /// +---------------------------------------------------------------------------+
	 /// | _______ __                     ______         __                   ______ 
	 /// ||_     _|  |_.-----.--------.  |   __ \.--.--.|  |.-----.-----.    |__    |
	 /// | _|   |_|   _|  -__|        |  |      <|  |  ||  ||  -__|__ --|    |    __|
	 /// ||_______|____|_____|__|__|__|  |___|__||_____||__||_____|_____|    |______|
	 /// |+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ 
	 /// +---------------------------------------------------------------------------+
	 /// | - Created by darkfriend77
	 /// +---------------------------------------------------------------------------+
	 /// </summary>
	 public class Interpreter
	 {
		  public enum TrinityItemQuality
		  {
				Unknown=-1,
				Common=0,
				Magic=1,
				Rare=2,
				Legendary=3,
				Set=4
		  }

		  // enumerations
		  public enum LogType
		  {
				LOG,
				DEBUG,
				ERROR
		  };

		  public enum InterpreterAction
		  {
				PICKUP,
				IGNORE,
				KEEP,
				TRASH,
				SCORE,
				SALVAGE,
				NULL,
		  };

		  // final variables
		  readonly string logPath=@"Plugins\FunkyBot\ItemRules\Log\";
		  readonly string itemrulesPath=@"Plugins\FunkyBot\ItemRules\";
		  
		  //readonly string configFile="config.dis";
		  readonly string pickupFile="pickup.dis";
		  readonly string salvageFile="salvage.dis";
		  readonly string unidFile="UnidStash.dis";

		  //readonly string townrunFile="salvage.dis";
		  readonly string version="2.2.1.5";
		  readonly string translationFile="translation.dis";
		  readonly string logFile="ItemRules.log";
		  readonly string transFixFile="TranslationFix.log";
		  readonly string assign="->", SEP=";";
		  readonly Regex macroPattern=new Regex(@"(@[A-Z]+)[ ]*:=[ ]*(.+)", RegexOptions.Compiled);

		  TrinityItemQuality logPickQuality, logKeepQuality;

		  // objects
		  ArrayList ruleSet, pickUpRuleSet, salvageRuleSet, unidKeepRuleSet;
		  TextWriter log;
		  Scanner scanner;
		  Parser parser;
		  //TextHighlighter highlighter;

		  // dictonary for the item
		  private static Dictionary<string, object> itemDic;

		  // dictonary for the translation
		  private Dictionary<string, string> nameToBalanceId;

		  // dictonary for the use of macros
		  private Dictionary<string, string> macroDic;

		  /// <summary>
		  /// 
		  /// </summary>
		  public Interpreter()
		  {
				// initialize parser objects
				scanner=new Scanner();
				parser=new Parser(scanner);
				//highlighter = new TextHighlighter(richTextBox, scanner, parser);

				// read configuration file and item files now
				readConfiguration();
				Logging.Write(" _______________________________________");
				Logging.Write(" ___-|: Darkfriend's Item Rules 2 :|-___");
				Logging.Write(" ___________________Rel.-v {0}_______", version);
		  }

		  public void reset()
		  {
				string actualLog=Path.Combine(logPath, logFile);
				string archivePath=Path.Combine(logPath, "Archive");
				string archiveLog=Path.Combine(archivePath, DateTime.Now.ToString("ddMMyyyyHHmmss")+"_log.txt");

				if (!File.Exists(actualLog))
					 return;

				if (!Directory.Exists(archivePath))
					 Directory.CreateDirectory(archivePath);

				File.Copy(actualLog, archiveLog, true);

				File.Delete(actualLog);
		  }

		  public bool reloadFromUI()
		  {
				readConfiguration();
				return false;
		  }

		  /// <summary>
		  /// Loads (or re-loads) the ItemRules configuration from settings and .dis entries
		  /// </summary>
		  public void readConfiguration()
		  {
				reset();

				// initialize or reset ruleSet array
				ruleSet=new ArrayList();
				pickUpRuleSet=new ArrayList();
				salvageRuleSet=new ArrayList();
				unidKeepRuleSet=new ArrayList();

				// instantiating our macro dictonary
				macroDic=new Dictionary<string, string>();

				// use giles setting
				if (Bot.Settings.ItemRules.ItemRuleDebug)
					 Logging.Write("ItemRules is running in debug mode!", logPickQuality);
				Logging.Write("ItemRules is using the {0} rule set.", Bot.Settings.ItemRules.ItemRuleType.ToString().ToLower());
				logPickQuality=getTrinityItemQualityFromString(Bot.Settings.ItemRules.ItemRuleLogPickup.ToString());
				Logging.Write("PICKLOG = {0} ", logPickQuality);
				logKeepQuality=getTrinityItemQualityFromString(Bot.Settings.ItemRules.ItemRuleLogKeep.ToString());
				Logging.Write("KEEPLOG = {0} ", logKeepQuality);

				string rulesPath;
				if (Bot.Settings.ItemRules.ItemRuleType.Equals("Custom"))
				{
					 rulesPath=Path.GetFullPath(Bot.Settings.ItemRules.ItemRuleCustomPath);
				}
				else
					 rulesPath=Path.Combine(itemrulesPath, "Rules", Bot.Settings.ItemRules.ItemRuleType.ToString().ToLower());

				Logging.Write("RULEPATH = {0} ", rulesPath);

				// fill translation dictionary
				nameToBalanceId=new Dictionary<string, string>();
				StreamReader streamReader=new StreamReader(Path.Combine(itemrulesPath, "Rules", translationFile));
				string str;
				while ((str=streamReader.ReadLine())!=null)
				{
					 string[] strArrray=str.Split(';');
					 nameToBalanceId[strArrray[1].Replace(" ", "")]=strArrray[0];
				}
				//DbHelper.Log(TrinityLogLevel.Normal, LogCategory.UserInformation, "... loaded: {0} ITEMID translations", nameToBalanceId.Count);

				// parse pickup file
				pickUpRuleSet=readLinesToArray(new StreamReader(Path.Combine(rulesPath, pickupFile)), pickUpRuleSet);
				Logging.Write("... loaded: {0} Pickup rules", pickUpRuleSet.Count);

				//parse savlage file
				salvageRuleSet=readLinesToArray(new StreamReader(Path.Combine(itemrulesPath, "Rules", salvageFile)), salvageRuleSet);
				Logging.Write("... loaded: {0} Salvage rules", salvageRuleSet.Count);

				//parse unid keep file
				unidKeepRuleSet=readLinesToArray(new StreamReader(Path.Combine(itemrulesPath, "Rules", unidFile)), unidKeepRuleSet);
				Logging.Write("... loaded: {0} Unid Keep rules", unidKeepRuleSet.Count);

				// parse all item files
				foreach (TrinityItemQuality itemQuality in Enum.GetValues(typeof(TrinityItemQuality)))
				{
					 string fileName=itemQuality.ToString().ToLower()+".dis";
					 string filePath=Path.Combine(rulesPath, fileName);
					 int oldValue=ruleSet.Count;
					 if (File.Exists(filePath))
					 {
						  ruleSet=readLinesToArray(new StreamReader(filePath), ruleSet);
						  Logging.Write("... loaded: {0} {1} rules", (ruleSet.Count-oldValue), itemQuality.ToString());
					 }
				}

				Logging.Write("... loaded: {0} Macros", macroDic.Count);
				Logging.Write("ItemRules loaded a total of {0} {1} rules!", ruleSet.Count, Bot.Settings.ItemRules.ItemRuleType.ToString());
		  }

		  /// <summary>
		  /// 
		  /// </summary>
		  /// <param name="streamReader"></param>
		  /// <param name="array"></param>
		  /// <returns></returns>
		  private ArrayList readLinesToArray(StreamReader streamReader, ArrayList array)
		  {
				string str="";
				Match match;
				while ((str=streamReader.ReadLine())!=null)
				{
					 str=str.Split(new string[] { "//" }, StringSplitOptions.None)[0]
						  .Replace(" ", "")
						  .Replace("\t", "");

					 if (str.Length==0)
						  continue;

					 // - start macro transformation
					 match=macroPattern.Match(str);

					 if (match.Success)
					 {
						  //DbHelper.Log(TrinityLogLevel.Normal, LogCategory.UserInformation, " macro added: {0} := {1}", match.Groups[1].Value, match.Groups[2].Value);
						  macroDic.Add(match.Groups[1].Value, match.Groups[2].Value);
						  continue;
					 }
					 // - stop macro transformation

					 // do simple translation with name to itemid
					 if (Bot.Settings.ItemRules.ItemRuleUseItemIDs&&str.Contains("[NAME]"))
					 {
						  bool foundTranslation=false;
						  foreach (string key in nameToBalanceId.Keys.ToList())
						  {
								key.Replace(" ", "").Replace("\t", "");
								if (str.Contains(key))
								{
									 str=str.Replace(key, nameToBalanceId[key]).Replace("[NAME]", "[ITEMID]");
									 foundTranslation=true;
									 break;
								}
						  }
						  if (!foundTranslation&&Bot.Settings.ItemRules.ItemRuleDebug)
								Logging.Write("No translation found for rule: {0}", str);
					 }

					 array.Add(str);
				}
				return array;
		  }

		  /// <summary>
		  /// 
		  /// </summary>
		  /// <param name="item"></param>
		  /// <returns></returns>
		  internal InterpreterAction checkPickUpItem(CacheItem item, ItemEvaluationType evaluationType)
		  {
				fillPickupDic(item);

				return checkItem(evaluationType);
		  }

		  /// <summary>
		  /// 
		  /// </summary>
		  /// <param name="item"></param>
		  /// <returns></returns>
		  internal InterpreterAction checkItem(ACDItem item, ItemEvaluationType evaluationType)
		  {
				fillDic(item);

				return checkItem(evaluationType);
		  }

		  /// <summary>
		  /// 
		  /// </summary>
		  /// <param name="item"></param>
		  /// <returns></returns>
		  public InterpreterAction checkSalvageItem(ACDItem item)
		  {
				fillTownDic(item);

				InterpreterAction action=InterpreterAction.NULL;

				string validRule="";

				ArrayList rules;
				rules=salvageRuleSet;

				foreach (string str in rules)
				{
					 ParseErrors parseErrors=null;

					 // default configuration for positive rules is pickup and keep
					 InterpreterAction tempAction=InterpreterAction.SALVAGE;
					 

					 string[] strings=str.Split(new string[] { assign }, StringSplitOptions.None);
					 if (strings.Count()>1)
						  tempAction=getInterpreterAction(strings[1]);

					 try
					 {
						  if (evaluate(strings[0], out parseErrors))
						  {
								validRule=str;
								action=tempAction;
								if (parseErrors.Count>0)
									 logOut("Have errors with out a catch!"
										  +SEP+"last use rule: "+str
										  +SEP+getParseErrors(parseErrors)
										  +SEP+getFullItem(), tempAction, LogType.DEBUG);
								break;
						  }
					 } catch (Exception e)
					 {
						  logOut(e.Message
								+SEP+"last use rule: "+str
								+SEP+getParseErrors(parseErrors)
								+SEP+getFullItem(), tempAction, LogType.ERROR);
					 }
				}

				//logOut(ItemEvaluationType.Salvage, validRule, action);

				return action;
		  }
		  /// <summary>
		  /// 
		  /// </summary>
		  /// <param name="item"></param>
		  /// <returns></returns>
		  public InterpreterAction checkUnidStashItem(ACDItem item)
		  {
				fillDic(item);

				InterpreterAction action=InterpreterAction.NULL;

				string validRule="";

				ArrayList rules;
				rules=unidKeepRuleSet;

				foreach (string str in rules)
				{
					 ParseErrors parseErrors=null;

					 // default configuration for positive rules is pickup and keep
					 InterpreterAction tempAction=InterpreterAction.KEEP;


					 string[] strings=str.Split(new string[] { assign }, StringSplitOptions.None);
					 if (strings.Count()>1)
						  tempAction=getInterpreterAction(strings[1]);

					 try
					 {
						  if (evaluate(strings[0], out parseErrors))
						  {
								validRule=str;
								action=tempAction;
								if (parseErrors.Count>0)
									 logOut("Have errors with out a catch!"
										  +SEP+"last use rule: "+str
										  +SEP+getParseErrors(parseErrors)
										  +SEP+getFullItem(), tempAction, LogType.DEBUG);
								break;
						  }
					 } catch (Exception e)
					 {
						  logOut(e.Message
								+SEP+"last use rule: "+str
								+SEP+getParseErrors(parseErrors)
								+SEP+getFullItem(), tempAction, LogType.ERROR);
					 }
				}

				//logOut(ItemEvaluationType.Salvage, validRule, action);

				return action;
		  }
		  /// <summary>
		  /// 
		  /// </summary>
		  /// <param name="item"></param>
		  /// <returns></returns>
		  public InterpreterAction checkItem(ItemEvaluationType evaluationType)
		  {

				InterpreterAction action=InterpreterAction.NULL;

				string validRule="";

				ArrayList rules;
				if (evaluationType==ItemEvaluationType.PickUp) rules=pickUpRuleSet;
				else rules=ruleSet;

				foreach (string str in rules)
				{
					 ParseErrors parseErrors=null;

					 // default configuration for positive rules is pickup and keep
					 InterpreterAction tempAction;
					 if (evaluationType==ItemEvaluationType.PickUp) tempAction=InterpreterAction.PICKUP;
					 else tempAction=InterpreterAction.KEEP;

					 string[] strings=str.Split(new string[] { assign }, StringSplitOptions.None);
					 if (strings.Count()>1)
						  tempAction=getInterpreterAction(strings[1]);

					 try
					 {
						  if (evaluate(strings[0], out parseErrors))
						  {
								validRule=str;
								action=tempAction;
								if (parseErrors.Count>0)
									 logOut("Have errors with out a catch!"
										  +SEP+"last use rule: "+str
										  +SEP+getParseErrors(parseErrors)
										  +SEP+getFullItem(), InterpreterAction.NULL, LogType.ERROR);
								break;
						  }
					 } catch (Exception e)
					 {
						  logOut(e.Message
								+SEP+"last use rule: "+str
								+SEP+getParseErrors(parseErrors)
								+SEP+getFullItem(), InterpreterAction.NULL, LogType.ERROR);
					 }
				}

				logOut(evaluationType, validRule, action);

				return action;
		  }

		  /// <summary>
		  /// 
		  /// </summary>
		  /// <param name="pickUp"></param>
		  /// <param name="validRule"></param>
		  /// <param name="action"></param>
		  private void logOut(ItemEvaluationType evaluationType, string validRule, InterpreterAction action)
		  {
				// return if we have a evaluationtype sell or salvage
				if (evaluationType==ItemEvaluationType.Salvage||evaluationType==ItemEvaluationType.Sell)
					 return;

				string logString=getFullItem()+validRule;

				TrinityItemQuality quality=getTrinityItemQualityFromString(itemDic["[QUALITY]"]);

				switch (action)
				{
					 case InterpreterAction.PICKUP:
						  if (quality>=logPickQuality)
								logOut(logString, action, LogType.LOG);
						  break;
					 case InterpreterAction.IGNORE:
						  if (quality>=logPickQuality)
								logOut(logString, action, LogType.LOG);
						  break;
					 case InterpreterAction.KEEP:
						  if (quality>=logKeepQuality)
								logOut(logString, action, LogType.LOG);
						  break;
					 case InterpreterAction.TRASH:
						  if (quality>=logKeepQuality)
								logOut(logString, action, LogType.LOG);
						  break;
					 case InterpreterAction.SCORE:
						  if (quality>=logKeepQuality)
								logOut(logString, action, LogType.LOG);
						  break;
					 case InterpreterAction.SALVAGE:
						  if (quality>=logKeepQuality)
								logOut(logString, action, LogType.LOG);
						  break;
					 case InterpreterAction.NULL:
						  if (quality>=logPickQuality)
								logOut(logString, action, LogType.LOG);
						  break;
				}
		  }

		  // todo use an enumeration value
		  /// <summary>
		  /// 
		  /// </summary>
		  /// <param name="quality"></param>
		  /// <returns></returns>
		  private TrinityItemQuality getTrinityItemQualityFromString(object quality)
		  {
				TrinityItemQuality trinityItemQuality;
				if (Enum.TryParse<TrinityItemQuality>(quality.ToString(), true, out trinityItemQuality))
					 return trinityItemQuality;
				else
					 return TrinityItemQuality.Common;
		  }


		  /// <summary>
		  /// 
		  /// </summary>
		  /// <param name="str"></param>
		  /// <param name="item"></param>
		  /// <param name="parseErrors"></param>
		  /// <returns></returns>
		  private bool evaluate(string str, out ParseErrors parseErrors)
		  {
				bool result=false;
				GilesTrinity.ItemRules.Core.ParseTree tree=parser.Parse(str);
				parseErrors=tree.Errors;
				object obj=tree.Eval(null);

				if (!Boolean.TryParse(obj.ToString(), out result))
					 tree.Errors.Add(new ParseError("TryParse Boolean failed!", 101, 0, 0, 0, 0));

				return result;
		  }

		  /// <summary>
		  /// 
		  /// </summary>
		  /// <param name="str"></param>
		  /// <param name="parseErrors"></param>
		  /// <returns></returns>
		  private object evaluateExpr(string str, out ParseErrors parseErrors)
		  {
				GilesTrinity.ItemRules.Core.ParseTree tree=parser.Parse(str);
				parseErrors=tree.Errors;
				return tree.Eval(null);

		  }

		  /// <summary>
		  /// 
		  /// </summary>
		  /// <param name="str"></param>
		  /// <returns></returns>
		  private InterpreterAction getInterpreterAction(string str)
		  {
				foreach (InterpreterAction action in Enum.GetValues(typeof(InterpreterAction)))
					 if (str.IndexOf(action.ToString())!=-1)
						  return action;
				return InterpreterAction.NULL;
		  }

		  /// <summary>
		  /// 
		  /// </summary>
		  /// <param name="str"></param>
		  /// <param name="logType"></param>
		  public void logOut(string str, InterpreterAction action, LogType logType)
		  {
				// no debugging when flag set false
				if (logType==LogType.DEBUG&&!Bot.Settings.ItemRules.ItemRuleDebug)
					 return;

				//if (!GilesTrinity.Settings.Advanced.ItemRulesLogs)
				//return;

				log=new StreamWriter(Path.Combine(logPath, logFile), true);
				log.WriteLine(DateTime.Now.ToString("yyyyMMddHHmmssffff")+".Hero"+SEP+logType+SEP+action+SEP+str);
				log.Close();
		  }

		  /// <summary>
		  /// 
		  /// </summary>
		  /// <param name="parseErrors"></param>
		  /// <returns></returns>
		  private string getParseErrors(ParseErrors parseErrors)
		  {
				if (parseErrors==null) return null;
				string result="tree.Errors = "+parseErrors.Count()+SEP;
				foreach (ParseError parseError in parseErrors)
					 result+="ParseError( "+parseError.Code+"): "+parseError.Message+SEP;
				return result;
		  }

		  /// <summary>
		  /// 
		  /// </summary>
		  /// <returns></returns>
		  public string getFullItem()
		  {
				string result="";

				// add stats            
				foreach (string key in itemDic.Keys)
				{
					 object value;
					 if (itemDic.TryGetValue(key, out value))
					 {
						  if (value is float&&(float)value>0)
								result+=key.ToUpper()+":"+((float)value).ToString("0.00").Replace(".00", "")+SEP;
						  else if (value is string&&!String.IsNullOrEmpty((string)value))
								result+=key.ToUpper()+":"+value.ToString()+SEP;
						  else if (value is bool)
								result+=key.ToUpper()+":"+value.ToString()+SEP;
						  else if (value is int)
								result+=key.ToUpper()+":"+value.ToString()+SEP;
					 }
				}
				return result;
		  }

		  /// <summary>
		  /// 
		  /// </summary>
		  /// <param name="key"></param>
		  /// <returns></returns>
		  public static bool getVariableValue(string key, out object obj)
		  {

				string[] strArray=key.Split('.');

				if (Interpreter.itemDic.TryGetValue(strArray[0], out obj)&&strArray.Count()>1)
				{
					 switch (strArray[1])
					 {
						  case "dual":
								if (obj is float&&(float)obj>0)
									 obj=(float)1;
								break;
						  case "max":
								object itemType, twoHand;
								double result;
								if (obj is float
									 &&Interpreter.itemDic.TryGetValue("[TYPE]", out itemType)
									 &&Interpreter.itemDic.TryGetValue("[TWOHAND]", out twoHand)
									 &&MaxStats.maxItemStats.TryGetValue(itemType.ToString()+twoHand.ToString()+strArray[0], out result)
									 &&result>0)
									 obj=(float)obj/(float)result;
								else
									 obj=(float)0;
								break;
					 }
				}

				return (obj!=null);
		  }

		  private void fillTownDic(ACDItem item)
		  {
				itemDic=new Dictionary<string, object>();

				// return if no item available
				if (item==null)
				{
					 logOut("received an item with a null reference!", InterpreterAction.NULL, LogType.ERROR);
					 return;
				}

				// add log unique key
				itemDic.Add("[KEY]", item.DynamicId.ToString());

				itemDic.Add("[QUALITY]", Regex.Replace(item.ItemQualityLevel.ToString(), @"[\d-]", string.Empty));

				itemDic.Add("[LEVEL]", (float)item.Level);

				itemDic.Add("[REQLEVEL]", (float)item.RequiredLevel);

				itemDic.Add("[REDUCEDLEVEL]", (float)item.ItemLevelRequirementReduction);
		  }

		  /// <summary>
		  /// 
		  /// </summary>
		  /// <param name="name"></param>
		  /// <param name="level"></param>
		  /// <param name="itemQuality"></param>
		  /// <param name="itemBaseType"></param>
		  /// <param name="itemType"></param>
		  /// <param name="isOneHand"></param>
		  /// <param name="isTwoHand"></param>
		  /// <param name="gameBalanceId"></param>
		  private void fillPickupDic(CacheItem item)
		  {
				object result;
				itemDic=new Dictionary<string, object>();

				// add log unique key
				itemDic.Add("[KEY]", item.DynamicID.ToString());

				// - BASETYPE ---------------------------------------------------------//
				itemDic.Add("[BASETYPE]", item.BalanceData.thisItemBaseType.ToString());

				// - TYPE -------------------------------------------------------------//
				/// TODO remove this check if it isnt necessary anymore
				if (item.BalanceData.thisItemType==ItemType.Unknown&&(item.InternalName.Contains("Plan")||item.InternalName.Contains("Design")))
				{
					 Logging.Write("There are still buggy itemType infos for craftingPlan around {0} has itemType = {1}", item.InternalName, item.BalanceData.thisItemType);
					 result=ItemType.CraftingPlan.ToString();
				}
				else result=item.BalanceData.thisItemType.ToString();
				itemDic.Add("[TYPE]", result);

				// - QUALITY -------------------------------------------------------//
				itemDic.Add("[QUALITY]", Regex.Replace(item.Itemquality.ToString(), @"[\d-]", string.Empty));
				itemDic.Add("[D3QUALITY]", item.Itemquality.ToString());

				// - ROLL ----------------------------------------------------------//
				float roll;
				if (float.TryParse(Regex.Replace(item.Itemquality.ToString(), @"[^\d]", string.Empty), out roll))
					 itemDic.Add("[ROLL]", roll);
				else
					 itemDic.Add("[ROLL]", 0);

				// - NAME -------------------------------------------------------------//
				itemDic.Add("[NAME]", item.InternalName.ToString().Replace(" ", ""));

				// - LEVEL ------------------------------------------------------------//
				itemDic.Add("[LEVEL]", (float)item.BalanceData.iThisItemLevel);
				itemDic.Add("[ONEHAND]", item.BalanceData.bThisOneHand);
				itemDic.Add("[TWOHAND]", item.BalanceData.bThisTwoHand);
				itemDic.Add("[UNIDENT]", (bool)true);
				itemDic.Add("[INTNAME]", item.InternalName);
				itemDic.Add("[ITEMID]", item.BalanceID.ToString());
		  }

		  /// <summary>
		  /// 
		  /// </summary>
		  /// <param name="item"></param>
		  private void fillDic(ACDItem item)
		  {
				object result;
				itemDic=new Dictionary<string, object>();

				// return if no item available
				if (item==null)
				{
					 logOut("received an item with a null reference!", InterpreterAction.NULL, LogType.ERROR);
					 return;
				}

				// check for missing translations
				if (Bot.Settings.ItemRules.ItemRuleDebug&&item.ItemQualityLevel==ItemQuality.Legendary)
					 checkItemForMissingTranslation(item);

				// add log unique key
				itemDic.Add("[KEY]", item.DynamicId.ToString());

				// - BASETYPE ---------------------------------------------------------//
				itemDic.Add("[BASETYPE]", item.ItemBaseType.ToString());

				// - TYPE -------------------------------------------------------------//
				/// TODO remove this check if it isnt necessary anymore
				if (item.ItemType==ItemType.Unknown&&item.Name.Contains("Plan"))
				{
					 Logging.Write("There are still buggy itemType infos for craftingPlan around {0} has itemType = {1}", item.Name, item.ItemType);
					 result=ItemType.CraftingPlan.ToString();
				}
				else result=item.ItemType.ToString();
				itemDic.Add("[TYPE]", result);

				// - QUALITY -------------------------------------------------------//
				itemDic.Add("[QUALITY]", Regex.Replace(item.ItemQualityLevel.ToString(), @"[\d-]", string.Empty));
				itemDic.Add("[D3QUALITY]", item.ItemQualityLevel.ToString());

				// - ROLL ----------------------------------------------------------//
				float roll;
				if (float.TryParse(Regex.Replace(item.ItemQualityLevel.ToString(), @"[^\d]", string.Empty), out roll))
					 itemDic.Add("[ROLL]", roll);
				else
					 itemDic.Add("[ROLL]", 0);

				// - NAME -------------------------------------------------------------//
				itemDic.Add("[NAME]", item.Name.ToString().Replace(" ", ""));

				// - LEVEL ------------------------------------------------------------//
				itemDic.Add("[LEVEL]", (float)item.Level);
				itemDic.Add("[ONEHAND]", item.IsOneHand);
				itemDic.Add("[TWOHAND]", item.IsTwoHand);
				itemDic.Add("[UNIDENT]", item.IsUnidentified);
				itemDic.Add("[INTNAME]", item.InternalName);
				itemDic.Add("[ITEMID]", item.GameBalanceId.ToString());

				// if there are no stats return
				//if (item.Stats == null) return;

				itemDic.Add("[STR]", item.Stats.Strength);
				itemDic.Add("[DEX]", item.Stats.Dexterity);
				itemDic.Add("[INT]", item.Stats.Intelligence);
				itemDic.Add("[VIT]", item.Stats.Vitality);
				itemDic.Add("[AS%]", item.Stats.AttackSpeedPercent>0?item.Stats.AttackSpeedPercent:item.Stats.AttackSpeedPercentBonus);
				itemDic.Add("[MS%]", item.Stats.MovementSpeed);
				itemDic.Add("[LIFE%]", item.Stats.LifePercent);
				itemDic.Add("[LS%]", item.Stats.LifeSteal);
				itemDic.Add("[LOH]", item.Stats.LifeOnHit);
				itemDic.Add("[LOK]", item.Stats.LifeOnKill);
				itemDic.Add("[REGEN]", item.Stats.HealthPerSecond);
				itemDic.Add("[GLOBEBONUS]", item.Stats.HealthGlobeBonus);
				itemDic.Add("[DPS]", item.Stats.WeaponDamagePerSecond);
				itemDic.Add("[WEAPAS]", item.Stats.WeaponAttacksPerSecond);
				itemDic.Add("[WEAPDMGTYPE]", item.Stats.WeaponDamageType.ToString());
				itemDic.Add("[WEAPMAXDMG]", item.Stats.WeaponMaxDamage);
				itemDic.Add("[WEAPMINDMG]", item.Stats.WeaponMinDamage);
				itemDic.Add("[CRIT%]", item.Stats.CritPercent);
				itemDic.Add("[CRITDMG%]", item.Stats.CritDamagePercent);
				itemDic.Add("[BLOCK%]", item.Stats.BlockChanceBonus);
				itemDic.Add("[MINDMG]", item.Stats.MinDamage);
				itemDic.Add("[MAXDMG]", item.Stats.MaxDamage);
				itemDic.Add("[ALLRES]", item.Stats.ResistAll);
				itemDic.Add("[RESPHYSICAL]", item.Stats.ResistPhysical);
				itemDic.Add("[RESFIRE]", item.Stats.ResistFire);
				itemDic.Add("[RESLIGHTNING]", item.Stats.ResistLightning);
				itemDic.Add("[RESHOLY]", item.Stats.ResistHoly);
				itemDic.Add("[RESARCANE]", item.Stats.ResistArcane);
				itemDic.Add("[RESCOLD]", item.Stats.ResistCold);
				itemDic.Add("[RESPOISON]", item.Stats.ResistPoison);
				itemDic.Add("[FIREDMG%]", item.Stats.FireDamagePercent);
				itemDic.Add("[LIGHTNINGDMG%]", item.Stats.LightningDamagePercent);
				itemDic.Add("[COLDDMG%]", item.Stats.ColdDamagePercent);
				itemDic.Add("[POISONDMG%]", item.Stats.PoisonDamagePercent);
				itemDic.Add("[ARCANEDMG%]", item.Stats.ArcaneDamagePercent);
				itemDic.Add("[HOLYDMG%]", item.Stats.HolyDamagePercent);
				itemDic.Add("[ARMOR]", item.Stats.Armor);
				itemDic.Add("[ARMORBONUS]", item.Stats.ArmorBonus);
				itemDic.Add("[ARMORTOT]", item.Stats.ArmorTotal);
				itemDic.Add("[GF%]", item.Stats.GoldFind);
				itemDic.Add("[MF%]", item.Stats.MagicFind);
				itemDic.Add("[PICKRAD]", item.Stats.PickUpRadius);
				itemDic.Add("[SOCKETS]", (float)item.Stats.Sockets);
				itemDic.Add("[THORNS]", item.Stats.Thorns);
				itemDic.Add("[DMGREDPHYSICAL]", item.Stats.DamageReductionPhysicalPercent);
				itemDic.Add("[MAXARCPOWER]", item.Stats.MaxArcanePower);
				itemDic.Add("[HEALTHSPIRIT]", item.Stats.HealthPerSpiritSpent);
				itemDic.Add("[MAXSPIRIT]", item.Stats.MaxSpirit);
				itemDic.Add("[SPIRITREG]", item.Stats.SpiritRegen);
				itemDic.Add("[ARCONCRIT]", item.Stats.ArcaneOnCrit);
				itemDic.Add("[MAXFURY]", item.Stats.MaxFury);
				itemDic.Add("[MAXDISCIP]", item.Stats.MaxDiscipline);
				itemDic.Add("[HATREDREG]", item.Stats.HatredRegen);
				itemDic.Add("[MAXMANA]", item.Stats.MaxMana);
				itemDic.Add("[MANAREG]", item.Stats.ManaRegen);

				// - NEW STATS ADDED --------------------------------------------------//
				itemDic.Add("[LEVELRED]", (float)item.Stats.ItemLevelRequirementReduction);
				itemDic.Add("[TOTBLOCK%]", item.Stats.BlockChance);
				itemDic.Add("[DMGVSELITE%]", item.Stats.DamagePercentBonusVsElites);
				itemDic.Add("[DMGREDELITE%]", item.Stats.DamagePercentReductionFromElites);
				itemDic.Add("[EXPBONUS]", item.Stats.ExperienceBonus);
				itemDic.Add("[REQLEVEL]", (float)item.Stats.RequiredLevel);
				itemDic.Add("[WEAPDMG%]", item.Stats.WeaponDamagePercent);

				itemDic.Add("[MAXSTAT]", new float[] { item.Stats.Strength, item.Stats.Intelligence, item.Stats.Dexterity }.Max());
				itemDic.Add("[MAXSTATVIT]", new float[] { item.Stats.Strength, item.Stats.Intelligence, item.Stats.Dexterity }.Max()+item.Stats.Vitality);
				itemDic.Add("[STRVIT]", item.Stats.Strength+item.Stats.Vitality);
				itemDic.Add("[DEXVIT]", item.Stats.Dexterity+item.Stats.Vitality);
				itemDic.Add("[INTVIT]", item.Stats.Intelligence+item.Stats.Vitality);
				itemDic.Add("[MAXONERES]", new float[] { item.Stats.ResistArcane, item.Stats.ResistCold, item.Stats.ResistFire, item.Stats.ResistHoly, item.Stats.ResistLightning, item.Stats.ResistPhysical, item.Stats.ResistPoison }.Max());
				itemDic.Add("[TOTRES]", item.Stats.ResistArcane+item.Stats.ResistCold+item.Stats.ResistFire+item.Stats.ResistHoly+item.Stats.ResistLightning+item.Stats.ResistPhysical+item.Stats.ResistPoison+item.Stats.ResistAll);
				itemDic.Add("[DMGFACTOR]", item.Stats.AttackSpeedPercent+item.Stats.CritPercent*2+item.Stats.CritDamagePercent/5+(item.Stats.MinDamage+item.Stats.MaxDamage)/20);
				itemDic.Add("[AVGDMG]", (item.Stats.MinDamage+item.Stats.MaxDamage)/2);

				float offstats=0;
				//if (new float[] { item.Stats.Strength, item.Stats.Intelligence, item.Stats.Dexterity }.Max() > 0)
				//    offstats += 1;
				if (item.Stats.CritPercent>0)
					 offstats+=1;
				if (item.Stats.CritDamagePercent>0)
					 offstats+=1;
				if (item.Stats.AttackSpeedPercent>0)
					 offstats+=1;
				if (item.Stats.MinDamage+item.Stats.MaxDamage>0)
					 offstats+=1;
				itemDic.Add("[OFFSTATS]", offstats);

				float defstats=0;
				//if (item.Stats.Vitality > 0)
				defstats+=1;
				if (item.Stats.ResistAll>0)
					 defstats+=1;
				if (item.Stats.ArmorBonus>0)
					 defstats+=1;
				if (item.Stats.BlockChance>0)
					 defstats+=1;
				if (item.Stats.LifePercent>0)
					 defstats+=1;
				//if (item.Stats.HealthPerSecond > 0)
				//    defstats += 1;
				itemDic.Add("[DEFSTATS]", defstats);
				itemDic.Add("[WEIGHTS]", WeightSet.CurrentWeightSet.EvaluateItem(item));

				//itemDic.Add("[GAMEBALANCEID]", (float)item.GameBalanceId);
				//itemDic.Add("[DYNAMICID]", item.DynamicId);

				// starting on macro implementation here
				foreach (string key in macroDic.Keys)
				{
					 ParseErrors parseErrors=null;
					 string expr=macroDic[key];
					 try
					 {
						  object exprValue=evaluateExpr(expr, out parseErrors);
						  itemDic.Add("["+key+"]", exprValue);
					 } catch (Exception e)
					 {
						  logOut(e.Message
								+SEP+"last use rule: "+expr
								+SEP+getParseErrors(parseErrors)
								+SEP+getFullItem(), InterpreterAction.NULL, LogType.ERROR);
					 }
				}

				// end macro implementation
		  }

		  private void checkItemForMissingTranslation(ACDItem item)
		  {
				string balanceIDstr;
				if (!nameToBalanceId.TryGetValue(item.Name.Replace(" ", ""), out balanceIDstr)&&!nameToBalanceId.ContainsValue(item.GameBalanceId.ToString()))
				{
					 Logging.Write("Translation: Missing: "+item.GameBalanceId.ToString()+";"+item.Name+" (ID is missing report)");
					 // not found missing name
					 StreamWriter transFix=new StreamWriter(Path.Combine(logPath, transFixFile), true);
					 transFix.WriteLine("Missing: "+item.GameBalanceId.ToString()+";"+item.Name);
					 transFix.Close();
				}
				else if (balanceIDstr!=item.GameBalanceId.ToString())
				{
					 Logging.Write("Translation: Wrong("+balanceIDstr+"): "+item.GameBalanceId.ToString()+";"+item.Name);
					 // wrong reference
					 StreamWriter transFix=new StreamWriter(Path.Combine(logPath, transFixFile), true);
					 transFix.WriteLine("Wrong("+balanceIDstr+"): "+item.GameBalanceId.ToString()+";"+item.Name);
					 transFix.Close();
				}
		  }
	 }

	 #endregion Interpreter
}
