using System.Xml.Serialization;
namespace FunkyBot.Cache.Enums
{

	 public enum AvoidanceType
	 {
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

		  [XmlEnum(Name="Dececrator")]
		  Dececrator,

		  [XmlEnum(Name = "DemonicForge")]
		  DemonicForge,

		  [XmlEnum(Name="DiabloMetor")]
		  DiabloMetor,
		  [XmlEnum(Name="DiabloPrison")]
		  DiabloPrison,

		  [XmlEnum(Name="Frozen")]
		  Frozen,

		  [XmlEnum(Name = "FrozenPulse")]
		  FrozenPulse,

		  [XmlEnum(Name="GrotesqueExplosion")]
		  GrotesqueExplosion,

		  [XmlEnum(Name="LacuniBomb")]
		  LacuniBomb,

		  [XmlEnum(Name="MageFirePool")]
		  MageFirePool,

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
		  [XmlEnum(Name="ShamanFireBall")]
		  ShamanFireBall,
		  [XmlEnum(Name="SuccubusProjectile")]
		  SuccubusProjectile,

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