using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Documents;
using fBaseXtensions.Game;
using fBaseXtensions.Helpers;
using fBaseXtensions.Settings;
using Zeta.Bot.Profile;
using Zeta.Bot.Settings;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Action = Zeta.TreeSharp.Action;

namespace fBaseXtensions.XML
{
	[ComVisible(false)]
    [XmlElement("UseItem")]
    public class UseItemTag : ProfileBehavior
	{
		[XmlAttribute("sno")]
		public int Sno { get; set; }

        private bool intWait = false;
		protected override Composite CreateBehavior()
		{
            return new PrioritySelector
            (
                new Decorator(ret => FunkyGame.GameIsInvalid,
                    new Action(ret => m_IsDone = true)),

                new Decorator(ret => !intWait,
                    new Sequence
                    (
                        new Sleep(2000),
                        new Action(ret => intWait = true)
                    )
                ),
                new Decorator(ret => !UIElements.InventoryWindow.IsVisible,
                    new Action(ret => UIManager.ToggleInventoryMenu())),

                //Update Item List
                new Decorator(ret => !updatedItemList,
                    new Sequence(
                        new Action(ret => UsableItemList=GetItemList()),
                        new Action(ret => updatedItemList=true))),

                new Decorator(ret => UsableItemList.Count == 0,
                    new Sequence
                    (
                        new Sleep(2000),
                        new Action(ret => m_IsDone = true)
                    )
                ),

                new Decorator(ret => UsableItemList.Count > 0,
                    new Sequence(
                        new Action(ret => AttemptUseItem(UsableItemList)),
                        new Action(ret => m_IsDone=true)))

            );
		}

        private bool updatedItemList = false;
	    private List<ACDItem> UsableItemList = new List<ACDItem>();
	    private List<ACDItem> GetItemList()
	    {
            List<ACDItem> returnList=new List<ACDItem>();

            foreach (ACDItem tempitem in ZetaDia.Me.Inventory.Backpack)
            {
                if (tempitem.BaseAddress != IntPtr.Zero && tempitem.ActorSNO==Sno)
                {
                    returnList.Add(tempitem);
                }
            }

	        return returnList;
	    }

	    private bool AttemptUseItem(List<ACDItem> items)
	    {
	        var item = items.FirstOrDefault();
	        if (item == null) return false;

            ZetaDia.Me.Inventory.UseItem(item.DynamicId);
	        return true;
	    }

		private bool m_IsDone;
		public override bool IsDone
		{
			get { return m_IsDone; }
		}
		public override void ResetCachedDone()
		{
			m_IsDone=false;
			base.ResetCachedDone();
		}
	}
}