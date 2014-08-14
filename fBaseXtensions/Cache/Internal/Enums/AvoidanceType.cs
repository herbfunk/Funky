using System.Xml.Serialization;

namespace fBaseXtensions.Cache.Internal.Enums
{

	 public enum AvoidanceType
	 {
		  [XmlEnum(Name = "AdriaArcanePool")]
		  AdriaArcanePool,
		  [XmlEnum(Name = "AdriaBlood")]
		  AdriaBlood,

		  [XmlEnum(Name="ArcaneSentry")]
		  ArcaneSentry,

		  [XmlEnum(Name="AzmodanBodies")]
		  AzmodanBodies,
		  [XmlEnum(Name="AzmodanFireball")]
		  AzmodanFireball,
		  [XmlEnum(Name="AzmodanPool")]
		  AzmodanPool,

		  [XmlEnum(Name="BeeProjectile")]
		  BeeProjectile,

		  [XmlEnum(Name="BelialGround")]
		  BelialGround,

		  [XmlEnum(Name = "BloodspringSmall")]
		  BloodSpringSmall,
		  [XmlEnum(Name = "BloodspringMedium")]
		  BloodSpringMedium,
		  [XmlEnum(Name = "BloodspringLarge")]
		  BloodSpringLarge,

		  [XmlEnum(Name="Dececrator")]
		  Dececrator,

		  [XmlEnum(Name = "DemonicForge")]
		  DemonicForge,

		  [XmlEnum(Name = "DestroyerDrop")]
		  DestroyerDrop,

		  [XmlEnum(Name="DiabloMetor")]
		  DiabloMetor,
		  [XmlEnum(Name="DiabloPrison")]
		  DiabloPrison,

		  [XmlEnum(Name="Frozen")]
		  Frozen,

		  [XmlEnum(Name = "FrozenPulse")]
		  FrozenPulse,

		  [XmlEnum(Name = "GhomGasCloud")]
		  GhomGasCloud,

		  [XmlEnum(Name="GrotesqueExplosion")]
		  GrotesqueExplosion,

		  [XmlEnum(Name="LacuniBomb")]
		  LacuniBomb,

		  [XmlEnum(Name="MageFirePool")]
		  MageFirePool,

		  [XmlEnum(Name = "MalletLord")]
		  MalletLord,

		  [XmlEnum(Name = "MalthaelDeathFog")]
		  MalthaelDeathFog,
		  [XmlEnum(Name = "MalthaelDrainSoul")]
		  MalthaelDrainSoul,
		  [XmlEnum(Name = "MalthaelLightning")]
		  MalthaelLightning,

		  [XmlEnum(Name = "MeteorImpact")]
		  MeteorImpact,

		  [XmlEnum(Name="MoltenCore")]
		  MoltenCore,

		  [XmlEnum(Name="MoltenTrail")]
		  MoltenTrail,

		  [XmlEnum(Name = "OrbitProjectile")]
		  OrbitProjectile,

		  [XmlEnum(Name = "OrbitFocalPoint")]
		  OrbitFocalPoint,

		  [XmlEnum(Name="PlagueCloud")]
		  PlagueCloud,
		  [XmlEnum(Name="PlagueHand")]
		  PlagueHand,
		  [XmlEnum(Name="PoisonGas")]
		  PoisonGas,
		  [XmlEnum(Name = "RiftBossPoison")]
		  RiftPoison,
		  [XmlEnum(Name="ShamanFireBall")]
		  ShamanFireBall,
		  [XmlEnum(Name="SuccubusProjectile")]
		  SuccubusProjectile,
		  [XmlEnum(Name = "Teleport")]
		  Teleport,
		  [XmlEnum(Name = "Thunderstorm")]
		  Thunderstorm,

		  [XmlEnum(Name="TreeSpore")]
		  TreeSpore,

		  [XmlEnum(Name="WallOfFire")]
		  WallOfFire,

		  [XmlEnum(Name="Wall")]
		  Wall,
		  [XmlEnum(Name="None")]
		  None,
	 }
}