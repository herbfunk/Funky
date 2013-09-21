namespace FunkyTrinity.Avoidances
{
	 ///<summary>
	 ///Describes an individual avoidance properties
	 ///</summary>
	 public class AvoidanceValue
	 {
		  public AvoidanceType Type;
		  public double Health; //Minimum Health Percent
		  public double Radius; 
		  public int Weight; //How "Deadly"

		  public AvoidanceValue(AvoidanceType type, double hp, double radius, int weight)
		  {
				Type=type;
				Health=hp;
				Radius=radius;
				Weight=weight;
		  }
		  public AvoidanceValue()
		  {
				Type=AvoidanceType.None;
				Health=0.00d;
				Radius=0.00d;
				Weight=1;
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
