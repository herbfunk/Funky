using FunkyBot.Cache;

namespace FunkyBot.Movement.Clustering
{
	///<summary>
	///Describes a cluster -- tracks data to describe properties of the entire cluster.
	///</summary>
	public class ClusterInfo
	{
		private ClusterProperties properties;
		public ClusterProperties Properties
		{
			get { return properties; }
			set { properties=value; }
		}

		public ClusterInfo()
		{
			properties=ClusterProperties.None;
		}

		public void Merge(ClusterInfo other)
		{
			this.UnitCounter+=other.UnitCounter;
			this.WeakCounter+=other.WeakCounter;
			this.StrongCounter+=other.StrongCounter;
			this.ElitesCounter+=other.ElitesCounter;
			this.FastCounter+=other.FastCounter;
			this.RangedCounter+=other.RangedCounter;
			this.DOTDPSCounter+=other.DOTDPSCounter;
			this.BossCounter+=other.BossCounter;

			UpdateProperties();
		}


		//internal data
		protected int UnitCounter=0;

		protected int WeakCounter=0;
		protected int StrongCounter=0;

		protected int ElitesCounter=0;
		protected int BossCounter=0;

		protected int FastCounter=0;
		protected int RangedCounter=0;

		//
		protected int DOTDPSCounter=0;
		public double DotDPSRatio
		{
			get
			{
				return DOTDPSCounter/UnitCounter;
			}
		}


		public void Update(ref CacheUnit unit, bool refreshproperties=false)
		{
			UnitCounter++;

			if (unit.UnitMaxHitPointAverageWeight<0) WeakCounter++; else if (unit.UnitMaxHitPointAverageWeight>0) StrongCounter++;

			if (unit.IsEliteRareUnique) ElitesCounter++; else if (unit.IsBoss) BossCounter++;

			if (unit.IsFast) FastCounter++;
			if (unit.IsRanged) RangedCounter++;

			if (Bot.Combat.UsesDOTDPSAbility&&unit.HasDOTdps.HasValue&&unit.HasDOTdps.Value) DOTDPSCounter++;

			if (refreshproperties) UpdateProperties();
		}

		private void UpdateProperties()
		{
			properties=ClusterProperties.None;


			if (WeakCounter/UnitCounter>0.50d)
				properties|=ClusterProperties.Weak;
			else if (StrongCounter/UnitCounter>0.40d)
				properties|=ClusterProperties.Strong;

			if (FastCounter/UnitCounter>0.50d)
				properties|=ClusterProperties.Fast;
			if (RangedCounter/UnitCounter>0.50d)
				properties|=ClusterProperties.Ranged;
			if (ElitesCounter>0)
				properties|=ClusterProperties.Elites;
			if (BossCounter>0)
				properties|=ClusterProperties.Boss;

			if (UnitCounter<3)
				properties|=ClusterProperties.Small;
			else if (UnitCounter>4)
				properties|=ClusterProperties.Large;
		}

	}
}