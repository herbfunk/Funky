using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeta.Bot.Profile;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace fBaseXtensions.XML
{
	[XmlElement("HasKeystone")]
	public class HasKeystoneTag : BaseIfComplexNodeTag
	{
		public enum KeystoneType
		{
			None,
			Fragment,
			Trial,
			Tiered
		}
		[XmlAttribute("Keystone")]
		public KeystoneType Type { get; set; }

		public enum ItemSource
		{
			Backpack,
			Stash
		}

		[XmlAttribute("Itemsource")]
		[XmlAttribute("itemsource")]
		[XmlAttribute("ItemSource")]
		public ItemSource Itemsource
		{
			get { return _itemsource; }
			set { _itemsource = value; }
		}
		private ItemSource _itemsource = ItemSource.Backpack;

		[XmlAttribute("Not")]
		[XmlAttribute("not")]
		[XmlAttribute("NOT")]
		public bool NOT { get; set; }

		protected override Composite CreateBehavior()
		{
			return
			 new Decorator(ret => !IsDone,
				 new PrioritySelector(
					 base.GetNodes().Select(b => b.Behavior).ToArray()
				 )
			 );
		}

		public override bool GetConditionExec()
		{
			if (Itemsource == ItemSource.Stash)
			{
				foreach (ACDItem tempitem in ZetaDia.Me.Inventory.StashItems)
				{
					if (tempitem.BaseAddress != IntPtr.Zero && tempitem.ItemType==ItemType.KeystoneFragment)
					{
						int tieredLevel=tempitem.TieredLootRunKeyLevel;
						if (Type == KeystoneType.Fragment)
						{
							if (tieredLevel == -1)
								return !NOT;
							
							continue;
						}

						if (Type == KeystoneType.Trial)
						{
							if (tieredLevel == 0)
								return !NOT;

							continue;
						}

						if (Type == KeystoneType.Tiered)
						{
							if (tieredLevel > 0 && tieredLevel<=FunkyBaseExtension.Settings.AdventureMode.MaximumTieredRiftKeyAllowed)
								return !NOT;
						}
					}
				}
			}
			else
			{
				foreach (ACDItem tempitem in ZetaDia.Me.Inventory.Backpack)
				{
					if (tempitem.BaseAddress != IntPtr.Zero && tempitem.ItemType == ItemType.KeystoneFragment)
					{
						int tieredLevel = tempitem.TieredLootRunKeyLevel;
						if (Type == KeystoneType.Fragment)
						{
							if (tieredLevel == -1)
								return !NOT;

							continue;
						}

						if (Type == KeystoneType.Trial)
						{
							if (tieredLevel == 0)
								return !NOT;

							continue;
						}

						if (Type == KeystoneType.Tiered)
						{
							if (tieredLevel > 0 && tieredLevel <= FunkyBaseExtension.Settings.AdventureMode.MaximumTieredRiftKeyAllowed)
								return !NOT;
						}
					}
				}
			}

			return NOT;
		}
	}
}
