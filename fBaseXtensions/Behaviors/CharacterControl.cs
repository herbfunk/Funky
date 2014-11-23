using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero;
using fBaseXtensions.Helpers;
using fBaseXtensions.Settings;
using Zeta.Bot;
using Zeta.Bot.Settings;
using Zeta.Game;
using Zeta.Game.Internals.Service;
using Zeta.TreeSharp;

namespace fBaseXtensions.Behaviors
{
    public static class CharacterControl
    {
        internal static GameDifficulty OrginalGameDifficultySetting;
        internal static bool GameDifficultyChanged = false;

        
        //Monitor Setting Changes from Demonbuddy -- so if difficulty changes we can modify the orignal variable.
        internal static void OnDBCharacterSettingPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "GameDifficulty")
            {
                if (!GameDifficultyChanged) //only when we are not changing it from code
                    OrginalGameDifficultySetting = CharacterSettings.Instance.GameDifficulty;
            }
        }

        private static BnetCharacterIndexInfo _heroindexinfo = new BnetCharacterIndexInfo();

        public static BnetCharacterIndexInfo HeroIndexInfo
        {
            get
            {
                if (_heroindexinfo.Characters.Count == 0)
                {
                    if (File.Exists(BnetCharacterIndexInfo.BnetCharacterInfoSettingsPath))
                    {
                        _heroindexinfo = BnetCharacterIndexInfo.DeserializeFromXML();
                    }
                }
                return _heroindexinfo;
            }
        }

        /// <summary>
        /// If we are engaging a character switch to preform a town run with.
        /// </summary>
        public static bool GamblingCharacterSwitch = false;

        //For the combat handler!
        public static bool AltHeroGamblingEnabled = false;
        //To switch back after finished!
        internal static bool GamblingCharacterSwitchToMain = false;

        private static List<int> _mainHeroIndexes = new List<int>(); 
        public static RunStatus GamblingCharacterSwitchBehavior()
        {
            if (!_delayer.Test(5d))
                return RunStatus.Running;

            if (GamblingCharacterSwitchToMain)
            {
                BotMain.StatusText = "[Funky] Switching to Main Hero";
                ZetaDia.Memory.ClearCache();
                HeroInfo curheroinfo = new HeroInfo(ZetaDia.Service.Hero);

                if (!curheroinfo.Equals(MainHeroInfo))
                {//Need To Switch back to main hero!

                    Logger.DBLog.DebugFormat("Current Hero Info Mismatched -- Current Hero {0}\r\nMain Hero{1}",
                        curheroinfo.ToString(), MainHeroInfo.ToString());

                    if (_mainHeroIndexes.Count == 0)
                    {//Get a list of possible heroes using our index list (matching hero class and name)
                        foreach (var c in HeroIndexInfo.Characters.Where(c => c.Class == MainHeroInfo.Class && c.Name == MainHeroInfo.Name))
                        {
                            _mainHeroIndexes.Add(c.Index);
                        }
                    }
                    else
                    {//Switch and remove index from list
                        int mainheroIndex = _mainHeroIndexes.First();
                        Logger.DBLog.InfoFormat("[Funky] Switching to Main Hero -- Index {0}", mainheroIndex);
                        ZetaDia.Service.GameAccount.SwitchHero(mainheroIndex);
                        _mainHeroIndexes.RemoveAt(0);
                    }
                }
                else
                {//Finished! Reset the variables.
                    FunkyGame.ShouldRefreshAccountDetails = true;
                    PluginSettings.LoadSettings();
                    MainHeroInfo = null;
                    AltHeroInfo = null;
                    GamblingCharacterSwitch = false;
                    GamblingCharacterSwitchToMain = false;
                    _mainHeroIndexes.Clear();
                    return RunStatus.Success;
                }

                return RunStatus.Running;
            }

            BotMain.StatusText = "[Funky] Switching to Alt Hero";

            //Update Main Hero Info and Switch to Alt Hero!
            if (MainHeroInfo == null)
                return CharacterSwitch();

            //Update Alt Hero Info and Start Adventure Mode Game!
            if (AltHeroInfo == null)
                return UpdateAltHero();


            //Finished for now.. lets load the new game and let combat control take over!
            FunkyGame.ShouldRefreshAccountDetails = true;
            GamblingCharacterSwitch = false;
            AltHeroGamblingEnabled = true;
            return RunStatus.Success;
        }


        private static int _startingBloodShardCount = -1;
        private static bool _forcedTownRun = false;
        internal static RunStatus GamblingCharacterCombatHandler()
        {
            if (_startingBloodShardCount == -1)
                _startingBloodShardCount = Backpack.GetBloodShardCount();
            

            if (!Zeta.Bot.Logic.BrainBehavior.IsVendoring)
            {
                if (_startingBloodShardCount != Backpack.GetBloodShardCount() || _forcedTownRun)
                {
                    //Finished!
                    Logger.DBLog.InfoFormat("[Funky] Finished Alternative Gambling!");
                    ExitGameBehavior.ShouldExitGame = true;
                    AltHeroGamblingEnabled = false;
                    GamblingCharacterSwitchToMain = true;
                    GamblingCharacterSwitch = true;
                    _startingBloodShardCount = -1;
                    _forcedTownRun = false;
                    _delayer.Reset();
                    return RunStatus.Success;
                }
                else
                {
                    Zeta.Bot.Logic.BrainBehavior.ForceTownrun();
                    _forcedTownRun = true;
                    return RunStatus.Running;
                }
            }
            else
            {
                return RunStatus.Success;
            }
        }


        private static Delayer _delayer = new Delayer();

        private static HeroInfo MainHeroInfo;
        private static string _lastProfilePath;
        private static RunStatus CharacterSwitch()
        {
            BotMain.StatusText = "[Funky] Hero Switch *Switching Heros*";
            if (FunkyBaseExtension.Settings.General.AltHeroIndex < 0)
            {
                Logger.DBLog.InfoFormat("Hero Index Info not setup!");
                BotMain.Stop();
                return RunStatus.Success;
            }

            if (HeroIndexInfo.Characters.Count == 0)
            {
                Logger.DBLog.InfoFormat("Hero Index Info not setup!");
                BotMain.Stop();
                return RunStatus.Success;
            }

            if (MainHeroInfo == null)
            {
                ZetaDia.Memory.ClearCache();
                MainHeroInfo = new HeroInfo(ZetaDia.Service.Hero);
            }

            _lastProfilePath = ProfileManager.CurrentProfile.Path;
            Logger.DBLog.InfoFormat("Switching to Hero Index {0}", FunkyBaseExtension.Settings.General.AltHeroIndex);
            ZetaDia.Service.GameAccount.SwitchHero(FunkyBaseExtension.Settings.General.AltHeroIndex);
            return RunStatus.Running;
        }


        private static HeroInfo AltHeroInfo;

        private static RunStatus UpdateAltHero()
        {
            BotMain.StatusText = "[Funky] Hero Switch *Refreshing Alt Info*";
            if (AltHeroInfo == null)
            {
                ZetaDia.Memory.ClearCache();
                AltHeroInfo = new HeroInfo(ZetaDia.Service.Hero);
                return RunStatus.Running;
            }

            if (AltHeroInfo.Equals(MainHeroInfo))
            {
                //Switch Failed or Values were incorrect!
                Logger.DBLog.InfoFormat("[Funky] Hero Switch (Incorrect Hero Info!)");
                AltHeroInfo = null;
                return RunStatus.Success;
            }

            return RunStatus.Running;
        }

        internal static void ResetVars()
        {
            MainHeroInfo = null;
            AltHeroInfo = null;
            AltHeroGamblingEnabled = false;
            GamblingCharacterSwitch = false;
            GamblingCharacterSwitchToMain = false;
            _mainHeroIndexes.Clear();
        }

        /// <summary>
        /// Used as a temporary wrapper of hero info
        /// </summary>
        public class HeroInfo
        {
            public string Name { get; set; }
            public int Level { get; set; }
            public int ParagonLevel { get; set; }
            public int QuestSNO { get; set; }
            public int QuestStep { get; set; }
            public ActorClass Class { get; set; }
            public TimeSpan TimePlayed { get; set; }


            public HeroInfo(BnetHero hero)
            {
                Name = hero.Name;
                Level = hero.Level;
                ParagonLevel = hero.ParagonLevel;
                QuestSNO = hero.QuestSNO;
                QuestStep = hero.QuestStep;
                Class = hero.Class;
                TimePlayed = hero.TimePlayed;
            }

            public HeroInfo()
            {
                Name = String.Empty;
                Level = -1;
                ParagonLevel = -1;
                QuestSNO = -1;
                QuestStep = -1;
                Class = ActorClass.Invalid;
                TimePlayed= new TimeSpan(0,0,0,0);
            }

            public override string ToString()
            {
                return String.Format("Hero: Name {0} Level {1} Paragon {2} Class {3} QuestSNO {4} Step {5} TimePlayed {6}",
                                            Name,Level,ParagonLevel,Class,QuestSNO,QuestStep,TimePlayed);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                //Check for null and compare run-time types. 
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }
                else
                {
                    HeroInfo p = (HeroInfo)obj;
                    return 
                        (Name == p.Name) && 
                        (Level == p.Level) && 
                        (Class == p.Class) &&
                        (ParagonLevel == p.ParagonLevel);
                        //&& (TimePlayed == p.TimePlayed);
                }
            }
        }
    }
}
