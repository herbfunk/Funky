using System;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Action=Zeta.TreeSharp.Action;
using System.Diagnostics;


namespace FunkyBot.XMLTags
{
	[XmlElement("TrinityRandomWait")]
	 public class TrinityRandomWait : ProfileBehavior
	 {
		  private bool isDone;
		  private int minDelay;
		  private int maxDelay;
		  private int delay;
		  private string statusText;
		  private Stopwatch timer=new Stopwatch();

		  public override bool IsDone
		  {
				get { return isDone; }
		  }

		  protected override Composite CreateBehavior()
		  {
				var RandomWaitSequence=new Sequence(
					 new Action(ret => delay=new Random().Next(minDelay, maxDelay)),
					 new Action(ret => statusText=String.Format("[XML Tag] Trinity Random Wait - Taking a break for {0:3} seconds.", delay)),
					 new Action(ret => BotMain.StatusText=statusText),
					 new Action(ctx => DoRandomWait()),
					 new Action(ret => isDone=true)
				);

				return RandomWaitSequence;
		  }

		  private RunStatus DoRandomWait()
		  {
			  if (!timer.IsRunning)
				{
					 timer.Start();
					 return RunStatus.Running;
				}
			  if (timer.IsRunning&&timer.ElapsedMilliseconds<delay)
			  {
				  return RunStatus.Running;
			  }
			  timer.Reset();
			  return RunStatus.Success;
		  }


		[XmlAttribute("min")]
		  public int min
		  {
				get
				{
					 return minDelay;
				}
				set
				{
					 minDelay=value;
				}
		  }
		  [XmlAttribute("max")]
		  public int max
		  {
				get
				{
					 return maxDelay;
				}
				set
				{
					 maxDelay=value;
				}
		  }

		  public override void ResetCachedDone()
		  {
				isDone=false;
				base.ResetCachedDone();
		  }
	 }
}
