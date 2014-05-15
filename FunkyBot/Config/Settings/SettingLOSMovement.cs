namespace FunkyBot.Config.Settings
{
    public class SettingLOSMovement
    {
        public bool EnableLOSMovementBehavior { get; set; }
        public bool AllowTreasureGoblin { get; set; }
        public bool AllowRareElites { get; set; }
        public bool AllowUniqueBoss { get; set; }
        public bool AllowRanged { get; set; }
        public bool AllowSucideBomber { get; set; }
        public bool AllowSpawnerUnits { get; set; }
        public bool AllowRareLootContainer { get; set; }
		public bool AllowCursedChestShrines { get; set; }
		public bool AllowEventSwitches { get; set; }
		public int MaximumRange { get; set; }


        public SettingLOSMovement()
        {
            EnableLOSMovementBehavior = true;
            AllowTreasureGoblin = true;
            AllowRareElites = true;
            AllowUniqueBoss = true;
            AllowRanged = false;
			AllowRareLootContainer = true;
			AllowSucideBomber = false;
			AllowSpawnerUnits = false;
			AllowCursedChestShrines = true;
			AllowEventSwitches = true;
			MaximumRange = 500;
        }
    }
}
