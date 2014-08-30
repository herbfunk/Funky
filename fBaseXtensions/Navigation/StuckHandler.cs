using Zeta.Bot.Navigation;
using Zeta.Common;

namespace fBaseXtensions.Navigation
{
	public class PluginStuckHandler : IStuckHandler
	{
		public Vector3 GetUnstuckPos()
		{
			return Vector3.Zero;
		}

		public bool IsStuck
		{
			get
			{
				return false;
			}
		}
	}
}
