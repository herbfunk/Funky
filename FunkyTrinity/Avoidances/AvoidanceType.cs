using System.Xml.Serialization;
namespace FunkyTrinity.Avoidances
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

		  [XmlEnum(Name="DiabloMetor")]
		  DiabloMetor,
		  [XmlEnum(Name="DiabloPrison")]
		  DiabloPrison,

		  [XmlEnum(Name="Frozen")]
		  Frozen,

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
		  [XmlEnum(Name="TreeSpore")]
		  TreeSpore,

		  [XmlEnum(Name="Wall")]
		  Wall,
		  [XmlEnum(Name="None")]
		  None,
	 }
}