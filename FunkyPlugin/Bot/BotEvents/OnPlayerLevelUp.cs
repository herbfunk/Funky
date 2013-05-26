using System;
using Zeta;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  internal static DateTime LastLevelUp=DateTime.MinValue;

		  public void OnPlayerLevelUp(object sender, EventArgs e)
		  {
				switch (Bot.Class.AC)
				{
					 case Zeta.Internals.Actors.ActorClass.Barbarian:
						  BarbarianOnLevelUp(sender, e);
						  break;
					 case Zeta.Internals.Actors.ActorClass.DemonHunter:
						  DemonHunterOnLevelUp(sender, e);
						  break;
					 case Zeta.Internals.Actors.ActorClass.Monk:
						  MonkOnLevelUp(sender, e);
						  break;
					 case Zeta.Internals.Actors.ActorClass.WitchDoctor:
						  WitchDoctorOnLevelUp(sender, e);
						  break;
					 case Zeta.Internals.Actors.ActorClass.Wizard:
						  WizardOnLevelUp(sender, e);
						  break;
				}

				LastLevelUp=DateTime.Now;
		  }
    }
}