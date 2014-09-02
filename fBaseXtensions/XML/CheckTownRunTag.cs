using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fBaseXtensions.Game.Hero;
using Zeta.Bot.Logic;
using Zeta.Bot.Profile;
using Zeta.Bot.Settings;
using Zeta.Game;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Action = Zeta.TreeSharp.Action;

namespace fBaseXtensions.XML
{
	[XmlElement("CheckTownRun")]
	public class CheckTownRunTag : ProfileBehavior
	{
		[XmlAttribute("Durability")]
		public float MinimumDurability
		{
			get { return _minimumDurability; }
			set { _minimumDurability = value; }
		}
		private float _minimumDurability = CharacterSettings.Instance.RepairWhenDurabilityBelow;

		[XmlAttribute("Slots")]
		public int SlotsUnused
		{
			get { return _SlotsUnused; }
			set { _SlotsUnused = value; }
		}
		private int _SlotsUnused = 2;



		protected override Composite CreateBehavior()
		{
			return new Action(ret =>
			{
				if (!BrainBehavior.IsVendoring && (ZetaDia.Me.Inventory.NumFreeBackpackSlots<=SlotsUnused || Backpack.ShouldRepairItems(MinimumDurability)))
					BrainBehavior.ForceTownrun("Forcing Town Run!");

				m_IsDone = true;
			});
		}

		private bool m_IsDone;
		public override bool IsDone
		{
			get { return m_IsDone; }
		}

		

		public override void ResetCachedDone()
		{
			m_IsDone = false;
			base.ResetCachedDone();
		}
	}
}
