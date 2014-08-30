using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Color = System.Drawing.Color;

namespace FunkyDebug
{
	public partial class FormDebug : Form
	{

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


		private void btnRefreshObjects_Click(object sender, EventArgs e)
		{
			if (BotMain.IsRunning)
			{
				return;
			}

			flowLayout_OutPut.Controls.Clear();


			try
			{
				using (ZetaDia.Memory.SaveCacheState())
				{
					ZetaDia.Memory.DisableCache();
					ZetaDia.Actors.Update();

					double iType = -1;

					if (tabControl_Objects.SelectedTab.Text == "Monsters")
					{
						var units = ZetaDia.Actors.GetActorsOfType<DiaUnit>(false, false)
							.Where(o => o.IsValid)
							.OrderBy(o => o.Distance);

						iType = DumpUnits(units, iType);
					}

					if (tabControl_Objects.SelectedTab.Text == "Items")
					{
						var items = ZetaDia.Actors.GetActorsOfType<DiaItem>(false, false)
							.Where(o => o.IsValid)
							.OrderBy(o => o.Distance);

						iType = DumpItems(items, iType);
					}

					if (tabControl_Objects.SelectedTab.Text == "Gizmos")
					{
						var gizmos = ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true, false)
							.Where(o => o.IsValid)
							.OrderBy(o => o.Distance);

						iType = DumpGizmos(gizmos, iType);
					}
				}
			}
			catch (Exception ex)
			{

			}

			flowLayout_OutPut.Focus();
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

					flowLayout_OutPut.Controls.Add(new UserControlDebugEntry(String.Format("Unit ActorSNO: {0} Name: {1} MonsterName: {8} Type: {2} Radius: {7:0.00} Position: {3} ({4}) Animation: {5} has Attributes: {6}\n",
											o.ActorSNO, o.Name, o.ActorInfo.GizmoType, o.Position, o.Position, o.CommonData.CurrentAnimation, attributesFound, o.CollisionSphere.Radius, o.CommonData.MonsterInfo.Name)));
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

					flowLayout_OutPut.Controls.Add(new UserControlDebugEntry(String.Format("Gizmo ActorSNO: {0} Name: {1} Type: {2} Radius: {3:0.00} Position: {4} ({5}) Distance: {6:0} Animation: {7} AppearanceSNO: {8} has Attributes: {9}\n",
							o.ActorSNO, o.Name, o.ActorInfo.GizmoType, o.CollisionSphere.Radius, o.Position, o.Position, o.Distance, o.CommonData.CurrentAnimation, o.AppearanceSNO, attributesFound)));

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
					flowLayout_OutPut.Controls.Add(new UserControlDebugEntry(String.Format("Item ActorSNO: {0} ACDGuid: {30} DynamicID: {31} Name: {1} ItemType: {2} ItemBaseType: {3}\r\n" +
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
											o.Position, o.ACDGuid, o.CommonData.DynamicId)));
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


			Clipboard.SetText(entrySender.Text);
		}
		private void btnRefreshCharacter_Click(object sender, EventArgs e)
		{
			if (BotMain.IsRunning)
			{
				return;
			}

			flowLayout_OutPut.Controls.Clear();

			try
			{
				using (ZetaDia.Memory.SaveCacheState())
				{
					ZetaDia.Memory.DisableCache();
					ZetaDia.Actors.Update();

					DiaActivePlayer me = ZetaDia.Me;
					foreach (var buff in me.GetAllBuffs())
					{
						try
						{
							flowLayout_OutPut.Controls.Add(new UserControlDebugEntry(String.Format("Buff: {0} SNOID: {1} StackCount: {2} IsCancelable: {3}",
								buff.InternalName, buff.SNOId, buff.StackCount, buff.IsCancelable)));
						}
						catch (Exception)
						{

						}
					}
					foreach (var buff in me.GetAllDebuffs())
					{
						try
						{

							flowLayout_OutPut.Controls.Add(new UserControlDebugEntry(String.Format("Debuff: {0} SNOID: {1} StackCount: {2} IsCancelable: {3}",
								buff.InternalName, buff.SNOId, buff.StackCount, buff.IsCancelable)));


						}
						catch (Exception)
						{

						}
					}

					var hotbarslots = Enum.GetValues(typeof(HotbarSlot));
					foreach (HotbarSlot hotbarslot in hotbarslots)
					{
						if (hotbarslot == HotbarSlot.Invalid) continue;
						flowLayout_OutPut.Controls.Add(new UserControlDebugEntry(ReturnSkillString(hotbarslot)));
					}

					try
					{

						SNOPower power = SNOPower.DrinkHealthPotion;
						PowerManager.CanCastFlags castflags;
						bool cancast = Zeta.Bot.PowerManager.CanCast(power, out castflags);
						string potionString = String.Format("Skill: {0} HotbarSlot: {1} RuneIndex: {2}\r\n" +
											 "CanCast: {3} CanCastFlags: {4}",
										power, HotbarSlot.Invalid, -1, cancast, castflags);
						flowLayout_OutPut.Controls.Add(new UserControlDebugEntry(potionString));
					}
					catch (Exception)
					{

					}

					foreach (var snoPower in ZetaDia.CPlayer.PassiveSkills)
					{
						string s = String.Format("Passive Skill {0}", snoPower.ToString());
						flowLayout_OutPut.Controls.Add(new UserControlDebugEntry(s));
					}


				}
			}
			catch (Exception ex)
			{

			}

			flowLayout_OutPut.Focus();
		}

		private string ReturnSkillString(HotbarSlot slot)
		{
			try
			{
				SNOPower power=ZetaDia.CPlayer.GetPowerForSlot(slot);
				PowerManager.CanCastFlags castflags;
				bool cancast=Zeta.Bot.PowerManager.CanCast(power, out castflags);
				return String.Format("Skill: {0} HotbarSlot: {1} RuneIndex: {2}\r\n" +
				                     "CanCast: {3} CanCastFlags: {4}",
								power, slot, ZetaDia.CPlayer.GetRuneIndexForSlot(slot), cancast, castflags);
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
			flowLayout_OutPut.Controls.Clear();

			try
			{
				using (ZetaDia.Memory.SaveCacheState())
				{
					ZetaDia.Memory.DisableCache();
					ZetaDia.Actors.Update();

					#region Character Inventory Items
					foreach (var o in ZetaDia.Me.Inventory.Backpack)
					{


						try
						{
							flowLayout_OutPut.Controls.Add(new UserControlDebugEntry(ReturnItemString(o)));
						}
						catch (Exception)
						{

						}

					}
					#endregion



				}
			}
			catch (Exception ex)
			{

			}

			flowLayout_OutPut.Focus();
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

			flowLayout_OutPut.Controls.Clear();

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
						flowLayout_OutPut.Controls.Add(entry);
					}

				}
			}
			catch
			{

			}

			flowLayout_OutPut.Focus();
		}

		private string ReturnItemString(ACDItem o)
		{
			return String.Format("Item - Name: {1} (InternalName: {35})\r\n" +
									"Row: {32} Column: {33}\r\n" +
									"ActorSNO: {0} ACDGuid: {30} DynamicID: {31} BalanceID: {34}\r\n" +
									"ItemType: {2} ItemBaseType: {3}\r\n" +
									"Position: {29}" +
									"IsArmor: {4} IsCrafted: {5} IsCraftingPage: {6} IsCraftingReagent: {7}\r\n" +
									"IsElite: {8} IsEquipped: {9} IsGem: {10} IsMiscItem: {11}\r\n" +
									"IsOneHand: {12} IsPotion: {13} IsRare: {14} IsTwoHand: {15}\r\n" +
									"IsTwoSquareItem: {16} IsUnidentified: {17} IsUnique: {18} IsVendorBought: {19}\r\n" +
									"Level: {20} RequiredLevel: {21} ItemLevelRequirementReduction: {22} \r\n" +
									"ItemQuality: {23} GemQuality: {24} \r\n" +
									"MaxStackCount: {25} MaxDurability: {26} NumSockets: {27} \r\n" +
									"Stats: {28}\n",
									o.ActorSNO, o.Name,
									o.ItemType, o.ItemBaseType,
									o.IsArmor, o.IsCrafted, o.IsCraftingPage, o.IsCraftingReagent,
									o.IsElite, o.IsEquipped, o.IsGem, o.IsMiscItem,
									o.IsOneHand, o.IsPotion, o.IsRare, o.IsTwoHand,
									o.IsTwoSquareItem, o.IsUnidentified, o.IsUnique, o.IsVendorBought,
									o.Level, o.RequiredLevel, o.ItemLevelRequirementReduction,
									o.ItemQualityLevel, o.GemQuality,
									o.MaxStackCount, o.MaxDurability, o.NumSockets,
									o.Stats,
									o.Position, o.ACDGuid, o.DynamicId,
									o.InventoryRow, o.InventoryColumn,
									o.GameBalanceId, o.InternalName);
		}


		private void btnRefreshCharacterEquipped_Click(object sender, EventArgs e)
		{
			if (BotMain.IsRunning) return;


			flowLayout_OutPut.Controls.Clear();

			try
			{
				using (ZetaDia.Memory.SaveCacheState())
				{
					ZetaDia.Memory.DisableCache();
					ZetaDia.Actors.Update();

					#region Character Inventory Items
					foreach (var o in ZetaDia.Me.Inventory.Equipped)
					{
						try
						{
							flowLayout_OutPut.Controls.Add(new UserControlDebugEntry(ReturnItemString(o)));
						}
						catch (Exception)
						{

						}

					}
					#endregion
				}
			}
			catch (Exception ex)
			{

			}

			flowLayout_OutPut.Focus();
		}

		private void flowLayout_OutPut_MouseEnter(object sender, EventArgs e)
		{
			flowLayout_OutPut.Focus();
			
		}

		private void btn_DumpOpenWorldMarkers_Click(object sender, EventArgs e)
		{
			if (BotMain.IsRunning)
			{
				return;
			}

			flowLayout_OutPut.Controls.Clear();

			try
			{
				ZetaDia.Memory.ClearCache();

				using (ZetaDia.Memory.SaveCacheState())
				{
					ZetaDia.Memory.DisableCache();
					ZetaDia.Actors.Update();

					foreach (var mmm in ZetaDia.Minimap.Markers.OpenWorldMarkers)
					{
						var entry = new UserControlDebugEntry(ReturnMinimapMarkerString(mmm));
						flowLayout_OutPut.Controls.Add(entry);
					}
				}
			}
			catch
			{

			}

			flowLayout_OutPut.Focus();
		}

		private string ReturnMinimapMarkerString(MinimapMarker mmm)
		{
			return String.Format("ID: {0} DynamicWorldId: {1} NameHash: {2} Position: {3}\r\n" +
								 "IsPointOfInterest:{4} IsPortalEntrance:{5} IsPortalExit:{6} IsWaypoint:{7}",
				mmm.Id, mmm.DynamicWorldId, String.Format("{0:X}", mmm.NameHash), mmm.Position,
				mmm.IsPointOfInterest,mmm.IsPortalEntrance,mmm.IsPortalExit,mmm.IsWaypoint);
		}

		private void btn_DumpCurrentWorldMarkers_Click(object sender, EventArgs e)
		{
			if (BotMain.IsRunning)
			{
				return;
			}

			flowLayout_OutPut.Controls.Clear();

			try
			{
				ZetaDia.Memory.ClearCache();

				using (ZetaDia.Memory.SaveCacheState())
				{
					ZetaDia.Memory.DisableCache();
					ZetaDia.Actors.Update();

					foreach (var mmm in ZetaDia.Minimap.Markers.CurrentWorldMarkers)
					{
						var entry = new UserControlDebugEntry(ReturnMinimapMarkerString(mmm));
						flowLayout_OutPut.Controls.Add(entry);
					}
				}
			}
			catch
			{

			}

			flowLayout_OutPut.Focus();
		}

		private void btn_DumpNormalMarkers_Click(object sender, EventArgs e)
		{
			if (BotMain.IsRunning)
			{
				return;
			}

			flowLayout_OutPut.Controls.Clear();

			try
			{
				ZetaDia.Memory.ClearCache();

				using (ZetaDia.Memory.SaveCacheState())
				{
					ZetaDia.Memory.DisableCache();
					ZetaDia.Actors.Update();

					foreach (var mmm in ZetaDia.Minimap.Markers.NormalMarkers)
					{
						var entry = new UserControlDebugEntry(ReturnMinimapMarkerString(mmm));
						flowLayout_OutPut.Controls.Add(entry);
					}
				}
			}
			catch
			{

			}

			flowLayout_OutPut.Focus();
		}

		private string ReturnQuestInfoString(QuestInfo q)
		{
			return String.Format("Type: {0} QuestSNO: {1} State: {2} Step: {3} LevelArea: {4}\r\n" +
								 "BonusCount: {5} CreationTick: {6} KillCount: {7} QuestMeter: {8}",
				q.QuestType,q.QuestSNO, q.State,q.QuestStep,q.LevelArea,
				q.BonusCount,q.CreationTick,q.KillCount,q.QuestMeter);
		}

		private void btn_DumpBounties_Click(object sender, EventArgs e)
		{
			if (BotMain.IsRunning)
			{
				return;
			}

			flowLayout_OutPut.Controls.Clear();

			try
			{
				ZetaDia.Memory.ClearCache();

				using (ZetaDia.Memory.SaveCacheState())
				{
					ZetaDia.Memory.DisableCache();
					ZetaDia.Actors.Update();

					foreach (var q in ZetaDia.ActInfo.Bounties)
					{

						var entry = new UserControlDebugEntry(ReturnQuestInfoString(q.Info));
						flowLayout_OutPut.Controls.Add(entry);
					}
				}
			}
			catch
			{

			}

			flowLayout_OutPut.Focus();
		}

		private void btn_DumpQuests_Click(object sender, EventArgs e)
		{
			if (BotMain.IsRunning)
			{
				return;
			}

			flowLayout_OutPut.Controls.Clear();

			try
			{
				ZetaDia.Memory.ClearCache();

				using (ZetaDia.Memory.SaveCacheState())
				{
					ZetaDia.Memory.DisableCache();
					ZetaDia.Actors.Update();

					foreach (var q in ZetaDia.ActInfo.AllQuests)
					{

						var entry = new UserControlDebugEntry(ReturnQuestInfoString(q));
						flowLayout_OutPut.Controls.Add(entry);
					}
				}
			}
			catch
			{

			}

			flowLayout_OutPut.Focus();
		}

		private bool Raycast_UsingPlayerLocation = true;
		private void radioButton_FromOtherLocation_CheckedChanged(object sender, EventArgs e)
		{
			RadioButton radioButton = (RadioButton)sender;
			bool ischecked=radioButton.Checked;
			panel_Raycast_FromLocation.Enabled = ischecked;
			Raycast_UsingPlayerLocation = !ischecked;
		}

		private void txtbox_Raycast_X_TextChanged(object sender, EventArgs e)
		{
			TextBox txtbox = (TextBox)sender;
			string text=txtbox.Text;

			//x="392.958" y="400.0356" z="0.4476572" (new Vector3(392.958f,400.0356f,0.4476572f))
			if (text.Contains("new Vector3"))
			{
				string[] textArray = text.Split(Convert.ToChar(" "));
				float x, y, z;
				x = ReturnNumberFromString(textArray[0]);
				y = ReturnNumberFromString(textArray[1]);
				z = ReturnNumberFromString(textArray[2]);
				if (Convert.ToString(txtbox.Tag)=="from")
				{
					txtbox_Raycast_X.Text = x.ToString();
					txtbox_Raycast_Y.Text = y.ToString();
					txtbox_Raycast_Z.Text = z.ToString();
					return;
				}
				else
				{
					txtbox_RaycastTo_X.Text = x.ToString();
					txtbox_RaycastTo_Y.Text = y.ToString();
					txtbox_RaycastTo_Z.Text = z.ToString();
					return;
				}
			}


			bool invalidcharfound = false;
			foreach (var c in text.ToCharArray())
			{
				if (!Char.IsDigit(c) && !c.Equals(Convert.ToChar(".")))
				{
					invalidcharfound = true;
					break;
				}
			}

			if (invalidcharfound)
				txtbox.Text = String.Empty;
		}
		private float ReturnNumberFromString(string s)
		{
			try
			{
				int startIndex = s.IndexOf(Convert.ToChar("=")) + 2;
				int length = (s.Length - 1) - startIndex;
				return Convert.ToSingle(s.Substring(startIndex, length));
			}
			catch (Exception ex)
			{

			}

			return 0;
		}
		private static DefaultNavigationProvider NP
		{
			get
			{
				return Navigator.GetNavigationProviderAs<DefaultNavigationProvider>();
			}
		}
		private void btn_navagation_Click(object sender, EventArgs e)
		{
			if (BotMain.IsRunning)
			{
				return;
			}

			ZetaDia.Memory.ClearCache();
			ZetaDia.Actors.Update();
			Navigator.SearchGridProvider.Update();
			NP.Clear();

			flowLayout_OutPut.Controls.Clear();

			Vector3 ToVector3;
			ToVector3 = ConvertTextToVector3(txtbox_RaycastTo_X.Text, txtbox_RaycastTo_Y.Text, txtbox_RaycastTo_Z.Text);
			if (ToVector3 == Vector3.Zero) return;

			bool bCanPathWithinDistance = NP.CanPathWithinDistance(ToVector3);
			var canpathwithinDistance = new UserControlDebugEntry(String.Format("CanPathWithinDistance {0}", bCanPathWithinDistance));
			flowLayout_OutPut.Controls.Add(canpathwithinDistance);

			bool bCanFullyClientPathTo=NP.CanFullyClientPathTo(ToVector3);
			var CanFullyClientPathTo = new UserControlDebugEntry(String.Format("CanFullyClientPathTo {0}", bCanFullyClientPathTo));
			flowLayout_OutPut.Controls.Add(CanFullyClientPathTo);

		}

		private void btn_RayCast_Click(object sender, EventArgs e)
		{
			if (BotMain.IsRunning)
			{
				return;
			}

			ZetaDia.Memory.ClearCache();
			ZetaDia.Actors.Update();
			Navigator.SearchGridProvider.Update();
			

			flowLayout_OutPut.Controls.Clear();


			float FromX, FromY, FromZ, ToX, ToY, ToZ;
			Vector3 FromVector3, ToVector3;

			try
			{
				if (!Raycast_UsingPlayerLocation)
				{
					FromVector3 = ConvertTextToVector3(txtbox_Raycast_X.Text, txtbox_Raycast_Y.Text, txtbox_Raycast_Z.Text);
					if (FromVector3 == Vector3.Zero) return;
				}
				else
				{
					FromVector3 = ZetaDia.Me.Position;
				}

				ToVector3 = ConvertTextToVector3(txtbox_RaycastTo_X.Text, txtbox_RaycastTo_Y.Text, txtbox_RaycastTo_Z.Text);
				if (ToVector3 == Vector3.Zero) return;
			}
			catch (Exception ex)
			{
				var exceptionEntry = new UserControlDebugEntry(String.Format("{0}\r\n{1}",ex.Message,ex.StackTrace));
				flowLayout_OutPut.Controls.Add(exceptionEntry);
				return;
			}

			bool raycast=Navigator.Raycast(FromVector3, ToVector3);
			var entry = new UserControlDebugEntry(String.Format("Navigator Raycast={0} ({1} to {2}", raycast, FromVector3, ToVector3));
			flowLayout_OutPut.Controls.Add(entry);

			raycast = ZetaDia.Physics.Raycast(FromVector3, ToVector3, NavCellFlags.AllowWalk);
			entry = new UserControlDebugEntry(String.Format("Physics Raycast={0} ({1} to {2}", raycast, FromVector3, ToVector3));
			flowLayout_OutPut.Controls.Add(entry);

			Vector2 outHitPoint;
			raycast=Navigator.SearchGridProvider.Raycast(FromVector3.ToVector2(), ToVector3.ToVector2(), out outHitPoint);
			entry = new UserControlDebugEntry(String.Format("SearchGridProvider Raycast={0} HitPoint: ({1}) ({2} to {3}", raycast, outHitPoint, FromVector3, ToVector3));
			flowLayout_OutPut.Controls.Add(entry);
		}

		private Vector3 ConvertTextToVector3(string x, string y, string z)
		{
			float FromX, FromY, FromZ;
			Vector3 FromVector3 = Vector3.Zero;
			try
			{
				FromX = Convert.ToSingle(x);
				FromY = Convert.ToSingle(y);
				FromZ = Convert.ToSingle(z);
				FromVector3 = new Vector3(FromX, FromY, FromZ);
			}
			catch (Exception ex)
			{
				var exceptionEntry = new UserControlDebugEntry(String.Format("{0}\r\n{1}", ex.Message, ex.StackTrace));
				flowLayout_OutPut.Controls.Add(exceptionEntry);

			}

			return FromVector3;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			/*
			 *     <DebugEntry>
					<SNOID>6348</SNOID>
				<Name>twoHandedSword_norm_base_flippy_02-9088</Name>
				<ActorType>Item</ActorType>
				<GizmoType>None</GizmoType>
				 <TargetType>Item</TargetType>
			</DebugEntry>
			 */

			//new ItemEntry(5457, PluginDroppedItemTypes.Spear,"Spear_norm_base_flippy_01"),

		
			string s = Clipboard.GetText();
			if (s.Contains("<DebugEntry>"))
			{

				string[] splitStrings = s.Split(Convert.ToChar(Environment.NewLine));
				string snoID = splitStrings.FirstOrDefault(str => str.StartsWith("<SNOID>"));
				if (snoID == null) return;
				snoID=ExtractDataFromXMLTag(snoID);
				if (snoID == String.Empty) return;

				string returnString = "new ItemEntry(" + snoID + ", PluginDroppedItemTypes.Spear";

				string name = splitStrings.FirstOrDefault(str => str.StartsWith("<Name>"));
				if (name!=null)
				{
					name = ExtractDataFromXMLTag(name);
					if (name!=String.Empty)
						returnString = returnString + @", """ + name + @"""),";
				}

				Clipboard.SetText(returnString);
			}
			
		}

		private string ExtractDataFromXMLTag(string tag)
		{
			try
			{

				int startIndex = tag.IndexOf(Convert.ToChar(">"));
				int endIndex = tag.LastIndexOf(Convert.ToChar("<"));
				return tag.Substring(startIndex, (endIndex - startIndex));

			}
			catch
			{

			}

			return String.Empty;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			if (textBox_UI_Hash.Text == String.Empty) return;
			ulong hash;
			try
			{
				//var text_Hash = string.Format("0x{0:X}", textBox_UI_Hash.Text);
				hash = Convert.ToUInt64(textBox_UI_Hash.Text);
			}
			catch (Exception)
			{
				return;
			}

			var uie = UIElement.FromHash(hash);
			if (uie.IsValid && uie.IsVisible && uie.IsEnabled)
			{
				uie.Click();
			}
		}

		

		


	}
}
