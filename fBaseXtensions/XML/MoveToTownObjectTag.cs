using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using fBaseXtensions.Behaviors;
using fBaseXtensions.Game;
using fBaseXtensions.Helpers;
using Zeta.Bot.Navigation;
using Zeta.Bot.Profile;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Action = Zeta.TreeSharp.Action;

namespace fBaseXtensions.XML
{
	[XmlElement("MoveToTownObject")]
	public class MoveToTownObjectTag: ProfileBehavior
	{
		public enum TownObjects
		{
			None=0,
			Stash,
			Sell,
			Salvage,
			Gamble,
			Idenify,
			NephalemObelisk,
			NephalemNPC
		}

		[XmlAttribute("Type")]
		public TownObjects TownObject { get { return _townobject; } set { _townobject = value; } }
		private TownObjects _townobject= TownObjects.None;


		public override void OnStart()
		{
			Helpers.Logger.DBLog.InfoFormat("Type {0}", TownObject.ToString());
		}

		protected override Composite CreateBehavior()
		{
			
			return new PrioritySelector
			(
				new Decorator(ret => FunkyGame.GameIsInvalid,
					new Action(ret => m_IsDone=true)),

				
				//Return To Town
				new Decorator(ret => TownPortalBehavior.FunkyTPOverlord(null),
					new Action(ret => TownPortalBehavior.FunkyTPBehavior(null))),

				//Setup our Vector and SNO
				new Decorator(ret => !initalizedTownVars,
					new Action(ret => InitalizeTownVariables())),

				

				new Decorator(ret => MovementVector==Vector3.Zero || ObjectSNO==-1,
					new Action(ret => m_IsDone=true)),

				//Movement
				new Decorator(ret => !UpdateObject() || ZetaDia.Me.Position.Distance(MovementVector)>10f || !Object.InLineOfSight,
					new Action(ret => Navigator.MoveTo(MovementVector))),

				//Interaction
				new Decorator(ret => !DialogIsVisible(),
					new Action(ret => Object.Interact())),

				new Decorator(ret => DialogIsVisible(),
					new Action(ret => m_IsDone=true))
			);
		}


		private bool initalizedTownVars = false;
		private void InitalizeTownVariables()
		{
			Act CurrentAct = Act.Invalid;

			if (FunkyGame.AdventureMode)
				CurrentAct = GameCache.FindActByTownLevelAreaID(ZetaDia.CurrentLevelAreaId);
			else
				CurrentAct = ZetaDia.CurrentAct;

			switch (TownObject)
			{
				case TownObjects.Stash:
					MovementVector = GameCache.ReturnTownRunMovementVector(GameCache.TownRunBehavior.Stash, CurrentAct);
					ObjectSNO = GameCache.ReturnTownRunObjectSNO(GameCache.TownRunBehavior.Stash, CurrentAct);
					break;
				case TownObjects.Sell:
					MovementVector = GameCache.ReturnTownRunMovementVector(GameCache.TownRunBehavior.Sell, CurrentAct);
					ObjectSNO = GameCache.ReturnTownRunObjectSNO(GameCache.TownRunBehavior.Sell, CurrentAct);
					break;
				case TownObjects.Salvage:
					MovementVector = GameCache.ReturnTownRunMovementVector(GameCache.TownRunBehavior.Salvage, CurrentAct);
					ObjectSNO = GameCache.ReturnTownRunObjectSNO(GameCache.TownRunBehavior.Salvage, CurrentAct);
					break;
				case TownObjects.Gamble:
					MovementVector = GameCache.ReturnTownRunMovementVector(GameCache.TownRunBehavior.Gamble, CurrentAct);
					ObjectSNO = GameCache.ReturnTownRunObjectSNO(GameCache.TownRunBehavior.Gamble, CurrentAct);
					break;
				case TownObjects.Idenify:
					MovementVector = GameCache.ReturnTownRunMovementVector(GameCache.TownRunBehavior.Idenify, CurrentAct);
					ObjectSNO = GameCache.ReturnTownRunObjectSNO(GameCache.TownRunBehavior.Idenify, CurrentAct);
					break;
				case TownObjects.NephalemObelisk:
					MovementVector = GameCache.ReturnTownRunMovementVector(GameCache.TownRunBehavior.NephalemObelisk, CurrentAct);
					ObjectSNO = GameCache.ReturnTownRunObjectSNO(GameCache.TownRunBehavior.NephalemObelisk, CurrentAct);
					break;
				case TownObjects.NephalemNPC:
					MovementVector = GameCache.ReturnTownRunMovementVector(GameCache.TownRunBehavior.NephalemNPC, CurrentAct);
					ObjectSNO = GameCache.ReturnTownRunObjectSNO(GameCache.TownRunBehavior.NephalemNPC, CurrentAct);
					break;
			}

			//Navigator.Clear();

			Helpers.Logger.DBLog.InfoFormat("Initalized Variables Vector {0} Sno {1}", MovementVector, ObjectSNO);

			initalizedTownVars = true;
		}

		private bool attemptedInteraction = false;
		private bool DialogIsVisible()
		{
			 UIElement uie=null;

			switch (TownObject)
			{
				case TownObjects.Stash:
					uie = Game.GameCache.ReturnTownRunObjectDialogElement(GameCache.TownRunBehavior.Stash);
					break;
				case TownObjects.Sell:
					uie = Game.GameCache.ReturnTownRunObjectDialogElement(GameCache.TownRunBehavior.Sell);
					break;
				case TownObjects.Salvage:
					uie = Game.GameCache.ReturnTownRunObjectDialogElement(GameCache.TownRunBehavior.Salvage);
					break;
				case TownObjects.Gamble:
					uie = Game.GameCache.ReturnTownRunObjectDialogElement(GameCache.TownRunBehavior.Gamble);
					break;
				case TownObjects.NephalemObelisk:
					uie = Game.GameCache.ReturnTownRunObjectDialogElement(GameCache.TownRunBehavior.NephalemObelisk);
					break;
				case TownObjects.NephalemNPC:
					uie = UI.Game.Conversation_Dialog_Main;
					break;
			}

			if (uie == null)
			{
				if (!attemptedInteraction)
				{
					attemptedInteraction = true;
					return false;
				}
				return true;
			}
				

			return UI.ValidateUIElement(uie);
		}

		private Vector3 MovementVector = Vector3.Zero;
		private int ObjectSNO = -1;
		private DiaObject Object = null;

		private bool UpdateObject()
		{
			Object=ZetaDia.Actors.GetActorsOfType<DiaObject>(true).FirstOrDefault(obj => obj.ActorSNO == ObjectSNO);
			return Object != null;
		}

		public override void ResetCachedDone()
		{
			m_IsDone=false;
			base.ResetCachedDone();
		}

		private bool m_IsDone;
		public override bool IsDone
		{
			get { return m_IsDone; }
		}
	}
}