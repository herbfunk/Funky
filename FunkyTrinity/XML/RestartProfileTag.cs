using System;
using System.Runtime.InteropServices;
using Zeta;
using Zeta.CommonBot;
using Zeta.CommonBot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace FunkyTrinity.XMLTags
{
	[ComVisible(false)]
	[XmlElement("RestartProfile")]
	public class RestartProfileTag : ProfileBehavior
	{
		private bool m_IsDone=false;
		public override bool IsDone
		{
			get { return m_IsDone; }
		}

		protected override Composite CreateBehavior()
		{
			string ReloadPath=null;

			return
				new Zeta.TreeSharp.PrioritySelector(
					new Zeta.TreeSharp.Decorator(ret => (DateTime.Now.Subtract(FunkyTrinity.Funky.LastProfileReload).TotalSeconds<5)==true,
						new Zeta.TreeSharp.Action(ret => m_IsDone=true)
						),
					new Zeta.TreeSharp.Decorator(ret => ZetaDia.IsInGame&&ZetaDia.Me.IsValid,
						new Zeta.TreeSharp.Sequence(
							new Zeta.TreeSharp.Action(ret => ReloadPath=Zeta.CommonBot.ProfileManager.CurrentProfile.Path
								),
							new Zeta.TreeSharp.Action(ret => ProfileManager.Load(ReloadPath)
								),
							new Zeta.TreeSharp.Action(ret => m_IsDone=true)
							)
						));
		}
	}
}