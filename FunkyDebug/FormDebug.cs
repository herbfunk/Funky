using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Color = System.Drawing.Color;

namespace FunkyDebug
{
	public partial class FormDebug : Form
	{
		private static ItemWindow ItemWindowBrowser;

		public FormDebug()
		{
			InitializeComponent();
			//FontFamily fontFamily = new FontFamily("Arial");

			//Font f = new Font(fontFamily, 10, FontStyle.Regular);
			//Label entry = new Label
			//{
			//	AutoEllipsis = false,
			//	AutoSize = true,
			//	Dock = DockStyle.Top,
			//	BorderStyle = BorderStyle.FixedSingle,
			//	BackColor = Color.Black,
			//	ForeColor = Color.Green,
			//	Text = "Testing Item 11",
			//	Font = f,
			//};
			//entry.MouseEnter += entryMouseEnter;
			//entry.MouseLeave += entryMouseLeave;

			//this.panelCharacterInventory.Controls.Add(entry);
			//this.panelCharacterInventory.Controls.Add(entry);
			//this.panelCharacterInventory.Controls.Add(entry);
			//this.panelCharacterInventory.Controls.Add(entry);
			//this.Text = "test";
		}
		private void entryMouseEnter(object sender, EventArgs e)
		{

			Label entrySender = (Label)sender;

			CacheACDItem item;
			int itemACDGUID;
			try
			{
				itemACDGUID = Convert.ToInt32(entrySender.Name);
			}
			catch
			{
				return;
			}

			if (!cacheItems.ContainsKey(itemACDGUID))
				return;

			item = cacheItems[itemACDGUID];

			Point screenPoint = this.PointToScreen(entrySender.Location);//this.PointToClient(MousePosition);
			screenPoint.Offset(this.Width, 0);
			ItemWindowBrowser = new ItemWindow(item);
			ItemWindowBrowser.Location = screenPoint;
			ItemWindowBrowser.Show(this);
		}
		private void entryMouseLeave(object sender, EventArgs e)
		{
			ItemWindowBrowser.Dispose();
		}

		private void btnRefreshObjects_Click(object sender, EventArgs e)
		{
			if (BotMain.IsRunning)
			{
				return;
			}

			panelItems.Controls.Clear();
			panelMonsters.Controls.Clear();
			panelGizmos.Controls.Clear();

			try
			{
				using (ZetaDia.Memory.SaveCacheState())
				{
					ZetaDia.Memory.DisableCache();

					double iType = -1;

					ZetaDia.Actors.Update();
					var units = ZetaDia.Actors.GetActorsOfType<DiaUnit>(false, false)
						.Where(o => o.IsValid)
						.OrderBy(o => o.Distance);

					iType = DumpUnits(units, iType);


					//ZetaDia.Actors.Update();
					var items = ZetaDia.Actors.GetActorsOfType<DiaItem>(false, false)
						.Where(o => o.IsValid)
						.OrderBy(o => o.Distance);

					iType = DumpItems(items, iType);

					//ZetaDia.Actors.Update();
					var gizmos = ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true, false)
						.Where(o => o.IsValid)
						.OrderBy(o => o.Distance);

					iType = DumpGizmos(gizmos, iType);


					//DumpPlayerProperties();
				}
			}
			catch (Exception ex)
			{

			}
		}

		private double DumpUnits(IEnumerable<DiaUnit> units, double iType)
		{

			foreach (DiaUnit o in units)
			{
				if (!o.IsValid)
					continue;

				string attributesFound = "";

				foreach (ActorAttributeType aType in Enum.GetValues(typeof(ActorAttributeType)))
				{
					iType = GetAttribute(iType, o, aType);
					if (iType > 0)
					{
						attributesFound += aType.ToString() + "=" + iType.ToString() + ", ";
					}
				}
				try
				{
					Label entry = new Label
					{
						AutoEllipsis = true,
						AutoSize = true,
						Dock = DockStyle.Top,
						BorderStyle = BorderStyle.FixedSingle,
					};
					entry.DoubleClick += entryDoubleClick;
					entry.Text = String.Format("Unit ActorSNO: {0} Name: {1} Type: {2} Radius: {7:0.00} Position: {3} ({4}) Animation: {5} has Attributes: {6}\n",
											o.ActorSNO, o.Name, o.ActorInfo.GizmoType, o.Position, o.Position, o.CommonData.CurrentAnimation, attributesFound, o.CollisionSphere.Radius);

					panelMonsters.Controls.Add(entry);
					//listMonsters.Items.Add(entry);
				}
				catch { }

			}
			return iType;
		}
		private double DumpGizmos(IEnumerable<DiaGizmo> gizmos, double iType)
		{

			foreach (DiaGizmo o in gizmos)
			{
				if (!o.IsValid)
					continue;

				string attributesFound = "";

				foreach (ActorAttributeType aType in Enum.GetValues(typeof(ActorAttributeType)))
				{
					iType = GetAttribute(iType, o, aType);
					if (iType > 0)
					{
						attributesFound += aType.ToString() + "=" + iType.ToString() + ", ";
					}
				}

				//if (o.ActorInfo.GizmoType == Zeta.Game.Internals.SNO.GizmoType.ServerProp)
				//    continue;

				//if (o.ActorInfo.GizmoType == Zeta.Game.Internals.SNO.GizmoType.StartLocations)
				//    continue;

				if (o.ActorInfo.GizmoType == Zeta.Game.Internals.SNO.GizmoType.Trigger)
					continue;

				//if (o.ActorInfo.GizmoType == Zeta.Game.Internals.SNO.GizmoType.ProximityTriggered)
				//    continue;

				if (o.ActorInfo.GizmoType == Zeta.Game.Internals.SNO.GizmoType.Checkpoint)
					continue;
				try
				{
					Label entry = new Label
					{
						AutoEllipsis = true,
						AutoSize = true,
						Dock = DockStyle.Top,
						BorderStyle = BorderStyle.FixedSingle,
					};
					entry.DoubleClick += entryDoubleClick;
					entry.Text = String.Format("Gizmo ActorSNO: {0} Name: {1} Type: {2} Radius: {3:0.00} Position: {4} ({5}) Distance: {6:0} Animation: {7} AppearanceSNO: {8} has Attributes: {9}\n",
							o.ActorSNO, o.Name, o.ActorInfo.GizmoType, o.CollisionSphere.Radius, o.Position, o.Position, o.Distance, o.CommonData.CurrentAnimation, o.AppearanceSNO, attributesFound);

					panelGizmos.Controls.Add(entry);
				}
				catch { }
			}
			return iType;
		}
		private double DumpItems(IEnumerable<DiaItem> items, double iType)
		{

			foreach (DiaItem o in items)
			{
				if (!o.IsValid)
					continue;

				try
				{
					Label entry = new Label
					{
						AutoEllipsis = true,
						AutoSize = true,
						Dock = DockStyle.Top,
						BorderStyle = BorderStyle.FixedSingle,
					};
					entry.DoubleClick += entryDoubleClick;
					entry.Text = String.Format("Item ActorSNO: {0} ACDGuid: {30} DynamicID: {31} Name: {1} ItemType: {2} ItemBaseType: {3}\r\n" +
											"Position: {29}" +
											"IsArmor: {4} IsCrafted: {5} IsCraftingPage: {6} IsCraftingReagent: {7}\r\n" +
											"IsElite: {8} IsEquipped: {9} IsGem: {10} IsMiscItem: {11}\r\n" +
											"IsOneHand: {12} IsPotion: {13} IsRare: {14} IsTwoHand: {15}\r\n" +
											"IsTwoSquareItem: {16} IsUnidentified: {17} IsUnique: {18} IsVendorBought: {19}\r\n" +
											"Level: {20} RequiredLevel: {21} ItemLevelRequirementReduction: {22}" +
											"ItemQuality: {23} GemQuality: {24}" +
											"MaxStackCount: {25} MaxDurability: {26} NumSockets: {27}" +
											"Stats: {28}\n",
											o.ActorSNO, o.Name,
											o.CommonData.ItemType, o.CommonData.ItemBaseType,
											o.CommonData.IsArmor, o.CommonData.IsCrafted, o.CommonData.IsCraftingPage, o.CommonData.IsCraftingReagent,
											o.CommonData.IsElite, o.CommonData.IsEquipped, o.CommonData.IsGem, o.CommonData.IsMiscItem,
											o.CommonData.IsOneHand, o.CommonData.IsPotion, o.CommonData.IsRare, o.CommonData.IsTwoHand,
											o.CommonData.IsTwoSquareItem, o.CommonData.IsUnidentified, o.CommonData.IsUnique, o.CommonData.IsVendorBought,
											o.CommonData.Level, o.CommonData.RequiredLevel, o.CommonData.ItemLevelRequirementReduction,
											o.CommonData.ItemQualityLevel, o.CommonData.GemQuality,
											o.CommonData.MaxStackCount, o.CommonData.MaxDurability, o.CommonData.NumSockets,
											o.CommonData.Stats,
											o.Position, o.ACDGuid, o.CommonData.DynamicId);
					panelItems.Controls.Add(entry);
				}
				catch { }

			}
			return iType;
		}
		private static double GetAttribute(double iType, DiaObject o, ActorAttributeType aType)
		{
			try
			{
				iType = (double)o.CommonData.GetAttribute<ActorAttributeType>(aType);
			}
			catch
			{
				iType = -1;
			}

			return iType;
		}
		private static double GetAttribute(DiaObject o, ActorAttributeType aType)
		{
			try
			{
				return o.CommonData.GetAttribute<double>(aType);
			}
			catch
			{
				return (double)(-1);
			}
		}

		private void entryDoubleClick(object sender, EventArgs e)
		{
			Label entry = (Label)sender;
			Clipboard.SetText(entry.Text);
		}
		private void btnRefreshCharacter_Click(object sender, EventArgs e)
		{
			if (BotMain.IsRunning)
			{
				return;
			}

			panelCharacterStats.Controls.Clear();
			panelCharacterInventory.Controls.Clear();
			panelCharacterEquipped.Controls.Clear();

			try
			{
				using (ZetaDia.Memory.SaveCacheState())
				{
					ZetaDia.Memory.DisableCache();

					double iType = -1;

					ZetaDia.Actors.Update();

					DiaActivePlayer me = ZetaDia.Me;
					foreach (var buff in me.GetAllBuffs())
					{
						try
						{
							Label entry = new Label
							{
								AutoEllipsis = true,
								AutoSize = true,
								Dock = DockStyle.Top,
								BorderStyle = BorderStyle.FixedSingle,
							};
							entry.DoubleClick += entryDoubleClick;
							entry.Text = String.Format("Buff: {0} SNOID: {1} StackCount: {2} IsCancelable: {3}",
								buff.InternalName, buff.SNOId, buff.StackCount, buff.IsCancelable);
							panelCharacterStats.Controls.Add(entry);
						}
						catch (Exception)
						{

						}
					}
					foreach (var buff in me.GetAllDebuffs())
					{
						try
						{

							Label entry = new Label
							{
								AutoEllipsis = true,
								AutoSize = true,
								Dock = DockStyle.Top,
								BorderStyle = BorderStyle.FixedSingle,
							};
							entry.DoubleClick += entryDoubleClick;
							entry.Text = String.Format("Debuff: {0} SNOID: {1} StackCount: {2} IsCancelable: {3}",
								buff.InternalName, buff.SNOId, buff.StackCount, buff.IsCancelable);
							panelCharacterStats.Controls.Add(entry);

						}
						catch (Exception)
						{

						}
					}

					Label[] entrySkills = new Label[5];
					entrySkills[0] = new Label
					{
						AutoEllipsis = true,
						AutoSize = true,
						Dock = DockStyle.Top,
						BorderStyle = BorderStyle.FixedSingle,
						Text = ReturnSkillString(HotbarSlot.HotbarMouseLeft)
					};
					entrySkills[0].DoubleClick += entryDoubleClick;
					panelCharacterStats.Controls.Add(entrySkills[0]);

					entrySkills[1] = new Label
					{
						AutoEllipsis = true,
						AutoSize = true,
						Dock = DockStyle.Top,
						BorderStyle = BorderStyle.FixedSingle,
						Text = ReturnSkillString(HotbarSlot.HotbarMouseRight)
					};
					entrySkills[1].DoubleClick += entryDoubleClick;
					panelCharacterStats.Controls.Add(entrySkills[1]);

					entrySkills[2] = new Label
					{
						AutoEllipsis = true,
						AutoSize = true,
						Dock = DockStyle.Top,
						BorderStyle = BorderStyle.FixedSingle,
						Text = ReturnSkillString(HotbarSlot.HotbarSlot1)
					};
					entrySkills[2].DoubleClick += entryDoubleClick;
					panelCharacterStats.Controls.Add(entrySkills[2]);

					entrySkills[3] = new Label
					{
						AutoEllipsis = true,
						AutoSize = true,
						Dock = DockStyle.Top,
						BorderStyle = BorderStyle.FixedSingle,
						Text = ReturnSkillString(HotbarSlot.HotbarSlot2)
					};
					entrySkills[3].DoubleClick += entryDoubleClick;
					panelCharacterStats.Controls.Add(entrySkills[3]);

					entrySkills[4] = new Label
					{
						AutoEllipsis = true,
						AutoSize = true,
						Dock = DockStyle.Top,
						BorderStyle = BorderStyle.FixedSingle,
						Text = ReturnSkillString(HotbarSlot.HotbarSlot3)
					};
					entrySkills[4].DoubleClick += entryDoubleClick;
					panelCharacterStats.Controls.Add(entrySkills[4]);

					entrySkills[5] = new Label
					{
						AutoEllipsis = true,
						AutoSize = true,
						Dock = DockStyle.Top,
						BorderStyle = BorderStyle.FixedSingle,
						Text = ReturnSkillString(HotbarSlot.HotbarSlot4)
					};
					entrySkills[5].DoubleClick += entryDoubleClick;
					panelCharacterStats.Controls.Add(entrySkills[5]);

					//entry.DoubleClick += entryDoubleClick;
					//entry.Text = String.Format("Skill: {0}", skill);
					//panelCharacterStats.Controls.Add(entry);

					//String.Format("Armor: {0} ArmorBonusItem: {1} ArmorItemTotal: {2} ArmorItemSubTotal: {3}" +
					//			  "AttacksPerSecond: {4} AttacksPerSecondBase: {5}",
					//				me.Armor, me.ArmorBonusItem, me.ArmorItemTotal, me.ArmorItemSubTotal,
					//				me.AttacksPerSecond,me.AttacksPerSecondBase);
				}
			}
			catch (Exception ex)
			{

			}
		}

		private string ReturnSkillString(HotbarSlot slot)
		{
			try
			{

				return String.Format("Skill: {0} HotbarSlot: {1} RuneIndex: {2}",
								ZetaDia.CPlayer.GetPowerForSlot(slot), slot, ZetaDia.CPlayer.GetRuneIndexForSlot(slot));
			}
			catch (Exception)
			{
				return "None";
			}
		}

		private Dictionary<int, CacheACDItem> cacheItems = new Dictionary<int, CacheACDItem>();

		private void btnRefreshCharacterInventory_Click(object sender, EventArgs e)
		{
			if (BotMain.IsRunning)
			{
				return;
			}
			cacheItems.Clear();
			panelCharacterInventory.Controls.Clear();

			try
			{
				using (ZetaDia.Memory.SaveCacheState())
				{
					ZetaDia.Memory.DisableCache();
					ZetaDia.Actors.Update();

					#region Character Inventory Items
					foreach (var o in ZetaDia.Me.Inventory.Backpack)
					{

						Label entry = new Label
						{
							AutoEllipsis = true,
							AutoSize = true,
							Dock = DockStyle.Top,
							BorderStyle = BorderStyle.FixedSingle,
							BackColor = Color.Black,
						};
						entry.DoubleClick += entryDoubleClick;
						try
						{
							CacheACDItem newCacheAcdItem = new CacheACDItem(o);
							cacheItems.Add(o.ACDGuid, newCacheAcdItem);

							entry.Text = String.Format(newCacheAcdItem.Name + "({0})", newCacheAcdItem.InternalName);
							entry.ForeColor = newCacheAcdItem.ItemQualityLevel == ItemQuality.Legendary ? Color.Orange :
								newCacheAcdItem.ItemQualityLevel == ItemQuality.Inferior ? Color.Gray :
								newCacheAcdItem.ItemQualityLevel < ItemQuality.Magic1 ? Color.White :
								newCacheAcdItem.ItemQualityLevel <= ItemQuality.Magic3 ? Color.Blue :
								newCacheAcdItem.ItemQualityLevel <= ItemQuality.Rare6 ? Color.Yellow :
								Color.White;
							entry.Name = newCacheAcdItem.ACDGUID.ToString();
							entry.MouseEnter += entryMouseEnter;
							entry.MouseLeave += entryMouseLeave;

							panelCharacterInventory.Controls.Add(entry);
						}
						catch (Exception)
						{

						}

					}
					#endregion

					//#region Character Equipped Items
					//foreach (var o in ZetaDia.Me.Inventory.Equipped)
					//{

					//	Label entry = new Label
					//	{
					//		AutoEllipsis = true,
					//		AutoSize = true,
					//		Dock = DockStyle.Top,
					//		BorderStyle = BorderStyle.FixedSingle,
					//	};
					//	entry.DoubleClick += entryDoubleClick;
					//	try
					//	{
					//		entry.Text = String.Format("Item - Name: {1} Row: {32} Column: {33} ActorSNO: {0} ACDGuid: {30} DynamicID: {31}  ItemType: {2} ItemBaseType: {3}\r\n" +
					//								"Position: {29}" +
					//								"IsArmor: {4} IsCrafted: {5} IsCraftingPage: {6} IsCraftingReagent: {7}\r\n" +
					//								"IsElite: {8} IsEquipped: {9} IsGem: {10} IsMiscItem: {11}\r\n" +
					//								"IsOneHand: {12} IsPotion: {13} IsRare: {14} IsTwoHand: {15}\r\n" +
					//								"IsTwoSquareItem: {16} IsUnidentified: {17} IsUnique: {18} IsVendorBought: {19}\r\n" +
					//								"Level: {20} RequiredLevel: {21} ItemLevelRequirementReduction: {22} \r\n" +
					//								"ItemQuality: {23} GemQuality: {24} \r\n" +
					//								"MaxStackCount: {25} MaxDurability: {26} NumSockets: {27} \r\n" +
					//								"Stats: {28}\n",
					//								o.ActorSNO, o.Name,
					//								o.ItemType, o.ItemBaseType,
					//								o.IsArmor, o.IsCrafted, o.IsCraftingPage, o.IsCraftingReagent,
					//								o.IsElite, o.IsEquipped, o.IsGem, o.IsMiscItem,
					//								o.IsOneHand, o.IsPotion, o.IsRare, o.IsTwoHand,
					//								o.IsTwoSquareItem, o.IsUnidentified, o.IsUnique, o.IsVendorBought,
					//								o.Level, o.RequiredLevel, o.ItemLevelRequirementReduction,
					//								o.ItemQualityLevel, o.GemQuality,
					//								o.MaxStackCount, o.MaxDurability, o.NumSockets,
					//								o.Stats,
					//								o.Position, o.ACDGuid, o.DynamicId,
					//								o.InventoryRow, o.InventoryColumn);

					//	}
					//	catch (Exception ex)
					//	{
					//		entry.Text = ex.Message;
					//	}
					//	panelCharacterEquipped.Controls.Add(entry);
					//}
					//#endregion

				}
			}
			catch (Exception ex)
			{

			}
		}

		private void FormDebug_Load(object sender, EventArgs e)
		{


		}

		private static HashSet<UIElement> UiElements = new HashSet<UIElement> { 
			UIElements.BackgroundScreenPCButtonAchievements,
			UIElements.BackgroundScreenPCButtonInventory,
			UIElements.BackgroundScreenPCButtonMenu,
			UIElements.BackgroundScreenPCButtonQuests,
			UIElements.BackgroundScreenPCButtonRecall,
			UIElements.BackgroundScreenPCButtonSkills,
			UIElements.BnetAccountNameTextbox,
			UIElements.BnetAccountPasswordTextbox,
			UIElements.BnetLoginAuthenticatorInput,
			UIElements.ConfirmationDialog,
			UIElements.ConfirmationDialogCancelButton,
			UIElements.ConfirmationDialogOkButton,
			UIElements.ConversationDialogMain,
			UIElements.ConversationDialogMainButtonClose,
			UIElements.DeathMenuDialogMain,
			UIElements.FloatingBubbleText,
			UIElements.InventoryWindow,
			UIElements.LoginButton,
			UIElements.ReviveAtCorpseButton,
			UIElements.ReviveAtLastCheckpointButton,
			UIElements.ReviveInTownButton,
			UIElements.SalvageWindow,
			UIElements.ShopDialogRepairWindow,
			UIElements.StashDialogMainPageTab1,
			UIElements.StashDialogMainPageTab2,
			UIElements.StashDialogMainPageTab3,
			UIElements.StashDialogMainPageTab4,
			UIElements.StashWindow,
			UIElements.VendorWindow,
			UIElements.WaypointMap,
		};

		private void btn_dumpUIs_Click(object sender, EventArgs e)
		{
			if (BotMain.IsRunning)
			{
				return;
			}

			panel_UI.Controls.Clear();

			try
			{
				ZetaDia.Memory.ClearCache();

				using (ZetaDia.Memory.SaveCacheState())
				{
					ZetaDia.Memory.DisableCache();
					ZetaDia.Actors.Update();

					foreach (var uiElement in UiElements)
					{
						if (!uiElement.IsValid) continue;

						Label entry = new Label
						{
							AutoEllipsis = true,
							AutoSize = true,
							Dock = DockStyle.Top,
							BorderStyle = BorderStyle.FixedSingle,
							BackColor = Color.Black,
							ForeColor = Color.White,
						};

						entry.Text = string.Format("Name: {0} Visible: {1} Text: {2}", uiElement.Name, uiElement.IsVisible, uiElement.Text);
						panel_UI.Controls.Add(entry);
					}

				}
			}
			catch
			{

			}
		}


	}
}
