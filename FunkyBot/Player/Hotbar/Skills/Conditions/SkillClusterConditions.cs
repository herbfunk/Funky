using System;
using FunkyBot.Movement.Clustering;

namespace FunkyBot.Player.HotBar.Skills.Conditions
{
	public class SkillClusterConditions : ClusterConditions
	{
		public SkillClusterConditions(double clusterRadius, float MaxDistanceFromBot, int MinimumUnitCount, bool IgnoreNonTargetableUnits, double DotDPSRatio=0.00, ClusterProperties clusterflags=ClusterProperties.None, float minDistance=0f, bool useRadiusDistance=false)
			:base(clusterRadius,MaxDistanceFromBot,MinimumUnitCount,IgnoreNonTargetableUnits,DotDPSRatio,clusterflags, minDistance, useRadiusDistance)
		{
			CreateCriteria();
		}

		public Func<bool> Criteria { get; set; } 

		private void CreateCriteria()
		{
			Criteria = () => Skill.CheckClusterConditions(this);
		}
	}
}
