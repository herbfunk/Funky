using System.Linq;
using Zeta.Bot.Profile;
using Zeta.Bot.Profile.Composites;

namespace fBaseXtensions.XML
{
	public abstract class BaseWhileComplexNodeTag : ComplexNodeTag
	{
		public override bool IsDone
		{
			get
			{
				if (!GetConditionExec())
				{
					return true;
				}
				bool allChildrenDone = Body.All<ProfileBehavior>(p => p.IsDone);
				if (allChildrenDone)
				{
					//Reset children
					for (int i = 0; i < Body.Count; i++)
					{
						Body[i].ResetCachedDone();
					}
				}
				return false;
			}
		}

		public abstract bool GetConditionExec();

		public override void ResetCachedDone()
		{
			foreach (ProfileBehavior behavior in Body)
			{
				behavior.ResetCachedDone();
			}
		}
	}
}
