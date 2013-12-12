using FunkyBot.Player.Class;

namespace FunkyBot.Player
{
	public class Character
	{
		public Character()
		{
			Account = new Account();
			Data = new CharacterCache();
			Class = null;
		}
		///<summary>
		///Values of the Current Character
		///</summary>
		internal CharacterCache Data { get; set; }
		internal PlayerClass Class { get; set; }
		internal Account Account { get; set; }

		internal void Reset()
		{
			Data = new CharacterCache();
			Class = null;
		}

	}
}
