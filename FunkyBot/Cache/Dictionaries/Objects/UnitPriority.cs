namespace FunkyBot.Cache.Dictionaries.Objects
{
	public class UnitPriority
	{
		public int SNOId { get; set; }
		public int Value { get; set; }

		public UnitPriority()
		{
			SNOId = -1;
			Value = -1;
		}
		public UnitPriority(int snoId, int value)
		{
			SNOId = snoId;
			Value = value;
		}

		public override int GetHashCode()
		{
			return SNOId;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			var p = obj as UnitPriority;
			if (p == null)
				return false;
			return (SNOId == p.SNOId);
		}
	}
}