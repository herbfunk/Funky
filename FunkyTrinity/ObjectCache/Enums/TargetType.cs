using System;

namespace FunkyTrinity.Cache.Enums
{
	 ///<summary>
	 ///Used to describe the object type for target handling.
	 ///</summary>
	 [Flags]
	 public enum TargetType
	 {
		   None=0,
			Unit=1,
			Shrine=2,
			Interactable=4,
			Destructible=8,
			Barricade=16,
			Container=32,
			Item=64,
			Gold=128,
			Globe=256,
			Avoidance=512,
			Door=1024,
			LineOfSight=2048,

			All=Unit|Shrine|Interactable|Destructible|Barricade|Container|Item|Gold|Globe|Avoidance|Door,
			Gizmo=Shrine|Interactable|Destructible|Barricade|Container|Door,
			Interactables=Shrine|Interactable|Door|Container,
			ServerObjects=Unit|Interactables|Destructible|Barricade,
	 }
}