using System;
using System.Globalization;
using System.Windows;
using FunkyBot.Cache.Enums;
using FunkyBot.Movement;
using Zeta.Common;

namespace FunkyBot.Cache.Objects
{


	public abstract class CacheObstacle : CacheObject
	{

		public virtual double PointRadius
		{
			get
			{
				return Radius / 2.5f;
			}
		}
		public override float Radius
		{
			get
			{
				return CollisionRadius.HasValue ? CollisionRadius.Value : base.Radius;
			}
		}
		public override float RadiusDistance
		{
			get
			{
				return Math.Max(0f, base.CentreDistance - Radius);
			}
		}

		public virtual void RefreshObject() { }

		public new string DebugString
		{
			get
			{
				string debugstring = base.DebugString + "\r\n";
				debugstring += "Type: " + Obstacletype.Value + "\r\n";
				return debugstring;
			}
		}

		public int BlacklistRefreshCounter { get; set; }

		private int RefreshRemovalCounter_;
		public int RefreshRemovalCounter
		{
			get { return RefreshRemovalCounter_; }
			set { RefreshRemovalCounter_ = value; }
		}

		///<summary>
		///Tests if this intersects with current bot position using CacheObject
		///</summary>
		public virtual bool TestIntersection(CacheObject OBJ, Vector3 BotPosition)
		{
			return MathEx.IntersectsPath(base.Position, Radius, BotPosition, BotMeleeVector);

		}
		///<summary>
		///Tests if this intersects between two vectors
		///</summary>
		public virtual bool TestIntersection(Vector3 V1, Vector3 V2, bool CollisonRadius = true)
		{
			return MathEx.IntersectsPath(base.Position, CollisonRadius ? Radius : base.Radius, V1, V2);
		}
		///<summary>
		///Tests if this intersects between two gridpoints
		///</summary>
		public virtual bool TestIntersection(GridPoint V1, GridPoint V2)
		{
			return GridPoint.LineIntersectsRect(V1, V2, AvoidanceRect);
			// return GilesIntersectsPath(base.Position, this.Radius, V1, V2);
		}
		//Special Method used inside cache collection
		public virtual bool PointInside(GridPoint Pos)
		{
			//return (Math.Min(rect_.TopLeft.X, rect_.BottomRight.X)>=Pos.X&&Math.Max(rect_.TopLeft.X, rect_.BottomRight.X)<=Pos.X&&
			//Math.Min(rect_.TopLeft.Y, rect_.BottomRight.Y)>=Pos.Y&&Math.Max(rect_.TopLeft.Y, rect_.BottomRight.Y)<=Pos.Y);
			return (GridPoint.GetDistanceBetweenPoints(PointPosition, Pos) - PointRadius <= 2.5f);
		}
		public virtual bool PointInside(Vector3 V3)
		{
			return base.Position.Distance(V3) <= Radius;
		}




		private Rect rect_;
		public virtual Rect AvoidanceRect
		{
			get
			{
				if (!rect_.Contains(PointPosition))
				{
					rect_ = new Rect(PointPosition, new Size(0, 0));
					rect_.Inflate(new Size(Radius, Radius));
				}
				return rect_;
			}
		}


		///<summary>
		///
		///</summary>
		public CacheObstacle(CacheObject fromObj)
			: base(fromObj)
		{
			if (!Obstacletype.HasValue)
				Obstacletype = ObstacleType.None;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			//Check for null and compare run-time types. 
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			return base.Equals((CacheObject)obj);
		}
	}



	///<summary>
	///Used for all avoidance objects
	///</summary>
	public class CacheAvoidance : CacheObstacle
	{
		public new string DebugString
		{
			get
			{
				string debugstring = base.DebugString + "\r\n";
				debugstring += "AvoidanceType: " + AvoidanceType + "\r\n";
				debugstring += Obstacletype.Value == ObstacleType.MovingAvoidance ? "Rotation " + Rotation.ToString(CultureInfo.InvariantCulture) + " Speed " + Speed.ToString(CultureInfo.InvariantCulture) + " " : "";
				return debugstring;
			}
		}
		public override double Weight
		{
			get
			{
				if (AvoidanceType.HasFlag(AvoidanceType.ArcaneSentry | AvoidanceType.Dececrator | AvoidanceType.MoltenCore | AvoidanceType.PlagueCloud))
				{
				}
				if (AvoidanceType == AvoidanceType.ArcaneSentry && base.CentreDistance < 30f)
				{
				}

				return base.Weight;
			}
			set
			{
				base.Weight = value;
			}
		}

		public override Vector3 Position
		{
			get
			{
				return base.Position;
			}
			set
			{
				base.Position = value;
				if (Obstacletype.Value == ObstacleType.MovingAvoidance)
				{
					//Update projecitle related
					ray_ = new Ray(value, ray_.Direction);
					ProjectileMaxRange = 100f - (Vector3.Distance(value, projectile_startPosition));

				}
			}
		}

		public override double PointRadius
		{
			get
			{
				return Radius / 2.5f;
			}
		}

		///<summary>
		///Returns avoidance type if any was set
		///</summary>
		public AvoidanceType AvoidanceType { get; set; }
		private AvoidanceValue AvoidanceValue = new AvoidanceValue();

		public override float Radius
		{
			get
			{
				float radius = (float)AvoidanceValue.Radius;

				//Modify radius during critical avoidance for arcane sentry.
				if (Bot.Character.Data.CriticalAvoidance && AvoidanceType == AvoidanceType.ArcaneSentry)
					radius = 25f;

				return radius;
			}
		}

		public override void RefreshObject()
		{
			if (AvoidanceCache.IgnoreAvoidance(AvoidanceType)) return;

			//Only update position of Movement Avoidances!
			if (IsProjectileAvoidance)
			{
				//Blacklisted updates
				if (BlacklistRefreshCounter > 0 &&
					 !CheckUpdateForProjectile)
				{
					BlacklistRefreshCounter--;
				}

				//If we need to avoid, than enable travel avoidance flag also.
				if (UpdateProjectileRayTest())
				{
					Bot.Targeting.Environment.TriggeringAvoidances.Add(this);
					Bot.Targeting.Environment.TriggeringAvoidanceRAGUIDs.Add(RAGUID);
					Bot.Targeting.TravellingAvoidance = true;
					Bot.Targeting.RequiresAvoidance = true;
				}
			}
			else
			{
				if (CentreDistance < 50f)
					Bot.Targeting.Environment.NearbyAvoidances.Add(RAGUID);

				if (Position.Distance(Bot.Character.Data.Position) <= Radius)
				{
					Bot.Targeting.Environment.TriggeringAvoidances.Add(this);
					Bot.Targeting.Environment.TriggeringAvoidanceRAGUIDs.Add(RAGUID);
					Bot.Targeting.RequiresAvoidance = true;
				}
			}
		}

		public bool CheckUpdateForProjectile
		{
			get
			{
				return Vector3.Distance(Bot.Character.Data.Position, Position) > Bot.Character.Data.fCharacterRadius;

			}
		}

		private bool projectileraytest_ = true;
		///<summary>
		///Check if this projectile will directly impact the bot.
		///</summary>
		public bool UpdateProjectileRayTest()
		{

			//Return last value during blacklist count
			if (BlacklistRefreshCounter > 0) return projectileraytest_;

			base.UpdatePosition();

			if (Bot.Settings.Avoidance.UseAdvancedProjectileTesting)
			{
				//Do fancy checks for this fixed projectile.
				if (Ray.Intersects(Bot.Character.Data.CharacterSphere).HasValue)
				{
					//Now we get the distance from us, divide it by the speed (which is also divided by 10 to normalize it) to get the total, this is than divided by our lastrefresh time, which gives us our loops before intersection.
					//Example: 35f away, 0.02f is speed, when divided is equal to 1750. Average Refresh is 150ms, so the loops would be ~11.6
					BlacklistRefreshCounter = (int)(Math.Round((Vector3.Distance(Position, Bot.Character.Data.Position) / (Speed / 10)) / 150));
					if (BlacklistRefreshCounter < 5 && BlacklistRefreshCounter > 1)
					{
						projectileraytest_ = true;
						return projectileraytest_;
					}
				}
				else
					projectileraytest_ = false;
			}
			else
			{
				Vector3 ProjectileEndPoint = MathEx.GetPointAt(Position, 40f, Rotation);
				projectileraytest_ = MathEx.IntersectsPath(Bot.Character.Data.Position, Bot.Character.Data.fCharacterRadius, Position, ProjectileEndPoint);
			}

			BlacklistRefreshCounter = 3;
			return projectileraytest_;
		}

		//private Vector2 directionV_;
		private Ray ray_;
		public Ray Ray
		{
			get
			{
				return ray_;
			}
		}
		public float Rotation { get; set; }
		public double Speed { get; set; }
		private Vector3 projectile_startPosition { get; set; }
		private float ProjectileMaxRange { get; set; }

		public bool ShouldAvoid
		{
			get
			{
				return !(AvoidanceCache.IgnoreAvoidance(AvoidanceType));
			}
		}

		public override bool TestIntersection(CacheObject OBJ, Vector3 BotPosition)
		{
			if (Obstacletype.Value == ObstacleType.MovingAvoidance)
			{
				Vector3 ProjectileEndPoint = MathEx.GetPointAt(Position, ProjectileMaxRange, Rotation);
				return GridPoint.LineIntersectsLine(BotPosition, Position, PointPosition, ProjectileEndPoint);
			}

			return MathEx.IntersectsPath(base.Position, Radius, BotPosition, OBJ.Position);
		}

		public override bool PointInside(GridPoint Pos)
		{
			return GridPoint.GetDistanceBetweenPoints(PointPosition, Pos) <= (PointRadius);
		}
		public override bool PointInside(Vector3 V3)
		{
			return base.Position.Distance(V3) <= Radius;
		}
		public override bool TestIntersection(Vector3 V1, Vector3 V2, bool CollisonRadius = true)
		{
			return MathEx.IntersectsPath(Position, CollisonRadius ? Radius : base.Radius, V1, V2);
		}

		public override Rect AvoidanceRect
		{
			get
			{
				if (Obstacletype.Value == ObstacleType.MovingAvoidance)
				{
					Vector3 thisEndVector = MathEx.GetPointAt(base.Position, ProjectileMaxRange, Rotation);
					return new Rect(PointPosition, (GridPoint)thisEndVector);
				}

				return base.AvoidanceRect;
			}
		}

		public CacheAvoidance(CacheObject parent, AvoidanceType avoidancetype)
			: base(parent)
		{
			AvoidanceType = avoidancetype;
			AvoidanceValue = Bot.Settings.Avoidance.Avoidances[(int)avoidancetype];

			//Special avoidances that require additional loops before removal (note: the loops are checked every 150ms, but obstacles are checked twice!)
			if (AvoidanceType.HasFlag(AvoidanceType.TreeSpore) && SNOID == 6578)
				RefreshRemovalCounter = 75;
			else if (AvoidanceType.HasFlag(AvoidanceType.GrotesqueExplosion))
				RefreshRemovalCounter = 25;
		}

		public CacheAvoidance(CacheObject parent, AvoidanceType type, Ray R, double speed)
			: base(parent)
		{
			AvoidanceType = type;
			AvoidanceValue = Bot.Settings.Avoidance.Avoidances[(int)type];
			ray_ = R;
			Speed = speed;
			projectile_startPosition = base.Position;
		}
	}


	///<summary>
	///Used for all non-avoidance objects (Monsters, Gizmos, and Misc Objects)
	///</summary>
	public class CacheServerObject : CacheObstacle
	{
		public CacheServerObject(CacheObject parent)
			: base(parent)
		{
		}

		public override void RefreshObject()
		{
			//Add nearby objects to our collection (used in navblock/obstaclecheck methods to reduce queries)
			if (CentreDistance < 25f)
				Bot.Targeting.Environment.NearbyObstacleObjects.Add(this);
		}
	}

}