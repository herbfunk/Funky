using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FunkyDebug;
using Zeta.Bot.Items;
using Zeta.Game.Internals.Actors;

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

		readonly string SEP = ";";

		//TextHighlighter highlighter;

		// dictonary for the item
		private static Dictionary<string, object> itemDic;




		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string getFullItem(CacheACDItem item)
		{
			fillDic(item);
			string result = "";

			// add stats            
			foreach (string key in itemDic.Keys)
			{
				object value;
				if (itemDic.TryGetValue(key, out value))
				{
					if (value is float && (float)value > 0)
						result += key.ToUpper() + ":" + ((float)value).ToString("0.00").Replace(".00", "") + SEP;
					else if (value is string && !String.IsNullOrEmpty((string)value))
						result += key.ToUpper() + ":" + value.ToString() + SEP;
					else if (value is bool)
						result += key.ToUpper() + ":" + value.ToString() + SEP;
					else if (value is int)
						result += key.ToUpper() + ":" + value.ToString() + SEP;
				}
			}
			return result;
		}





		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		private void fillDic(CacheACDItem item)
		{
			object result;
			itemDic = new Dictionary<string, object>();

			// return if no item available
			if (item == null)
			{
				// logOut("received an item with a null reference!", InterpreterAction.NULL, LogType.ERROR);
				return;
			}


			// add log unique key
			itemDic.Add("[KEY]", item.ThisDynamicID.ToString());

			// - BASETYPE ---------------------------------------------------------//
			itemDic.Add("[BASETYPE]", item.ItemBaseType.ToString());

			// - TYPE -------------------------------------------------------------//
			/// TODO remove this check if it isnt necessary anymore
			if (item.ItemType == ItemType.Unknown && item.Name.Contains("Plan"))
			{
				result = ItemType.CraftingPlan.ToString();
			}
			else result = item.ItemType.ToString();
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
			itemDic.Add("[AS%]", item.Stats.AttackSpeedPercent > 0 ? item.Stats.AttackSpeedPercent : item.Stats.AttackSpeedPercentBonus);
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
			//itemDic.Add("[FIREDMG%]", item.Stats.FireDamagePercent);
			//itemDic.Add("[LIGHTNINGDMG%]", item.Stats.LightningDamagePercent);
			//itemDic.Add("[COLDDMG%]", item.Stats.ColdDamagePercent);
			//itemDic.Add("[POISONDMG%]", item.Stats.PoisonDamagePercent);
			//itemDic.Add("[ARCANEDMG%]", item.Stats.ArcaneDamagePercent);
			//itemDic.Add("[HOLYDMG%]", item.Stats.HolyDamagePercent);
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
			itemDic.Add("[MAXSTATVIT]", new float[] { item.Stats.Strength, item.Stats.Intelligence, item.Stats.Dexterity }.Max() + item.Stats.Vitality);
			itemDic.Add("[STRVIT]", item.Stats.Strength + item.Stats.Vitality);
			itemDic.Add("[DEXVIT]", item.Stats.Dexterity + item.Stats.Vitality);
			itemDic.Add("[INTVIT]", item.Stats.Intelligence + item.Stats.Vitality);
			itemDic.Add("[MAXONERES]", new float[] { item.Stats.ResistArcane, item.Stats.ResistCold, item.Stats.ResistFire, item.Stats.ResistHoly, item.Stats.ResistLightning, item.Stats.ResistPhysical, item.Stats.ResistPoison }.Max());
			itemDic.Add("[TOTRES]", item.Stats.ResistArcane + item.Stats.ResistCold + item.Stats.ResistFire + item.Stats.ResistHoly + item.Stats.ResistLightning + item.Stats.ResistPhysical + item.Stats.ResistPoison + item.Stats.ResistAll);
			itemDic.Add("[DMGFACTOR]", item.Stats.AttackSpeedPercent + item.Stats.CritPercent * 2 + item.Stats.CritDamagePercent / 5 + (item.Stats.MinDamage + item.Stats.MaxDamage) / 20);
			itemDic.Add("[AVGDMG]", (item.Stats.MinDamage + item.Stats.MaxDamage) / 2);



			//itemDic.Add("[GAMEBALANCEID]", (float)item.GameBalanceId);
			//itemDic.Add("[DYNAMICID]", item.DynamicId);



			// end macro implementation
		}

	}

	#endregion Interpreter
}
