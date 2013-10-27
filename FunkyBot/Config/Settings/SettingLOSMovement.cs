using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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


        public SettingLOSMovement()
        {
            EnableLOSMovementBehavior = true;
            AllowTreasureGoblin = true;
            AllowRareElites = true;
            AllowUniqueBoss = true;
            AllowRanged = true;
            AllowRareLootContainer = true;
            AllowSucideBomber = true;
            AllowSpawnerUnits = true;
        }
    }
}
