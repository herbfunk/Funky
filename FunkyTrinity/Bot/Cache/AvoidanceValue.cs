using FunkyTrinity.Enums;

namespace FunkyTrinity.Cache
{
	 public class AvoidanceValue
	 {
		  public AvoidanceType Type;
		  public double Health;
		  public double Radius;

		  public AvoidanceValue(AvoidanceType type, double hp, double radius)
		  {
				Type=type;
				Health=hp;
				Radius=radius;
		  }
		  public AvoidanceValue()
		  {
				Type=AvoidanceType.None;
				Health=0.00d;
				Radius=0.00d;
		  }

		  public override int GetHashCode()
		  {
				return (int)this.Type;
		  }
		  public override bool Equals(object obj)
		  {
				//Check for null and compare run-time types. 
				if (obj==null||this.GetType()!=obj.GetType())
				{
					 return false;
				}
				else
				{
					 AvoidanceValue p=(AvoidanceValue)obj;
					 return this.Type==p.Type;
				}
		  }
	 }
}
