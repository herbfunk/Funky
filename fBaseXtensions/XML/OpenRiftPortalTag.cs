using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fBaseXtensions.Game;
using Zeta.Bot.Profile;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Action = Zeta.TreeSharp.Action;

namespace fBaseXtensions.XML
{
	[XmlElement("OpenRiftPortal")]
	public class OpenRiftPortalTag : ProfileBehavior
	{
		public enum KeystoneType
		{
			Fragment,
			Trial,
			Tiered
		}
		[XmlAttribute("Keystone")]
		public KeystoneType KeyType { get; set; }

		[XmlAttribute("KeystoneHighest")]
		public bool KeyStoneHighest
		{
			get { return _keyStoneHighest; }
			set { _keyStoneHighest = value; }
		}
		private bool _keyStoneHighest = false;

		protected override Composite CreateBehavior()
		{
			return new PrioritySelector
			(
				new Decorator(ret => FunkyGame.GameIsInvalid || !ZetaDia.IsInTown,
					new Action( ret => m_IsDone=true)),

				new Decorator(ret => !Game.UI.ValidateUIElement(Game.UI.Game.NeaphlemObeliskDialog),
					new Action(ret => m_IsDone = true)),

				new Decorator(ret => SendOpenPortal(),
					new Action( ret => m_IsDone=true))
			);

		}

		private ACDItem GetKeystoneItem()
		{
			var items = ZetaDia.Me.Inventory.Backpack;
			if (KeyType == KeystoneType.Tiered)
			{
				if (!KeyStoneHighest)
					items = items.OrderBy(i => i.TieredLootRunKeyLevel);
				else
					items = items.OrderByDescending(i => i.TieredLootRunKeyLevel);
			}

			foreach (var tempitem in items)
			{
				if (tempitem.BaseAddress != IntPtr.Zero && tempitem.ItemType == ItemType.KeystoneFragment)
				{
					int tieredLevel = tempitem.TieredLootRunKeyLevel;
					if (KeyType == KeystoneType.Fragment)
					{
						if (tieredLevel == -1)
							return tempitem;

						continue;
					}

					if (KeyType == KeystoneType.Trial)
					{
						if (tieredLevel == 0)
							return tempitem;

						continue;
					}

					if (KeyType == KeystoneType.Tiered)
					{
						if (tieredLevel > 0)
							return tempitem;
					}
				}
			}

			return null;
		}
		private bool SendOpenPortal()
		{
			var keystone = GetKeystoneItem();
			if (keystone != null)
			{
				ZetaDia.Me.OpenRift(keystone);
			}

			return true;
		}

		private bool m_IsDone;
		public override bool IsDone
		{
			get { return m_IsDone; }
		}
	}
}
