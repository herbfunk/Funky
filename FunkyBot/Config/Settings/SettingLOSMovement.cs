namespace FunkyBot.Settings
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
			MaximumRange = 500;
        }
    }
}
