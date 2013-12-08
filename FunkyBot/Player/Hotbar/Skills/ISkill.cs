using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills
{
	 public interface ISkill
	 {
		  SNOPower Power { get; }
		  int RuneIndex { get; }

		 void Initialize();

		 bool Equals(object obj);
		 int GetHashCode();
	 }
}