using System;
using Zeta;
using Zeta.Internals;
using Zeta.TreeSharp;
using System.Linq;

namespace FunkyBot
{
	 public partial class Funky
	 {
		  public static class D3Character
		  {
				private static UIElement SwitchHeroButton_;
				public static UIElement SwitchHeroButton
				{
					 get
					 {
						  if (SwitchHeroButton_==null||!SwitchHeroButton_.IsValid)
								SwitchHeroButton_=UIElement.FromHash(0xBE4E61ABD1DCDC79);

						  if (SwitchHeroButton_.IsValid&&SwitchHeroButton_.IsVisible&&SwitchHeroButton_.IsEnabled)
								return SwitchHeroButton_;
						  else
								return null;
					 }
				}

				private static UIElement CreateHeroButton_;
				public static UIElement CreateHeroButton
				{
					 get
					 {
						  if (CreateHeroButton_==null||!CreateHeroButton_.IsValid)
								CreateHeroButton_=UIElement.FromHash(0x744BC83D82918CE2);

						  if (CreateHeroButton_.IsValid&&CreateHeroButton_.IsVisible&&CreateHeroButton_.IsEnabled)
								return CreateHeroButton_;
						  else
								return null;
					 }
				}

				private static UIElement SelectHeroButton_;
				public static UIElement SelectHeroButton
				{
					 get
					 {
						  if (SelectHeroButton_==null||!SelectHeroButton_.IsValid)
								SelectHeroButton_=UIElement.FromHash(0x5D73E830BC87CE66);

						  if (SelectHeroButton_.IsValid&&SelectHeroButton_.IsVisible&&SelectHeroButton_.IsEnabled)
								return CreateHeroButton_;
						  else
								return null;
					 }
				}

				private static UIElement SelectHeroType(Zeta.Internals.Actors.ActorClass type)
				{
					 UIElement thisClassButton=null;
					 switch (type)
					 {
						  case Zeta.Internals.Actors.ActorClass.Barbarian:
								thisClassButton=UIElement.FromHash(0x98976D3F43BBF74);
								break;
						  case Zeta.Internals.Actors.ActorClass.DemonHunter:
								thisClassButton=UIElement.FromHash(0x98976D3F43BBF74);
								break;
						  case Zeta.Internals.Actors.ActorClass.Monk:
								thisClassButton=UIElement.FromHash(0x7733072C07DABF11);
								break;
						  case Zeta.Internals.Actors.ActorClass.WitchDoctor:
								thisClassButton=UIElement.FromHash(0x1A2DB1F47C26A8C2);
								break;
						  case Zeta.Internals.Actors.ActorClass.Wizard:
								thisClassButton=UIElement.FromHash(0xBC3AA6A915972065);
								break;
					 }

					 return thisClassButton;
				}
				
				private static UIElement HeroNameText_;
				public static UIElement HeroNameText
				{
					 get
					 {
						  if (HeroNameText_==null||!HeroNameText_.IsValid)
								HeroNameText_=UIElement.FromHash(0x8D2C771F09BC037F);

						  if (HeroNameText_.IsValid&&HeroNameText_.IsVisible&&HeroNameText_.IsEnabled)
								return HeroNameText_;
						  else
								return null;
					 }
				}

				private static UIElement CreateNewHeroButton_;
				public static UIElement CreateNewHeroButton
				{
					 get
					 {
						  if (CreateNewHeroButton_==null||!CreateNewHeroButton_.IsValid)
								CreateNewHeroButton_=UIElement.FromHash(0x28578F7B0F6384C6);

						  if (CreateNewHeroButton_.IsValid&&CreateNewHeroButton_.IsVisible&&CreateNewHeroButton_.IsEnabled)
								return CreateNewHeroButton_;
						  else
								return null;
					 }
				}

				private static DateTime LastActionTaken=DateTime.Today;
				private static bool SelectedClass=false;

				public static RunStatus CreateNewHero()
				{
					 if (DateTime.Now.Subtract(LastActionTaken).TotalMilliseconds>1000)
					 {
						  if (SwitchHeroButton!=null&&SwitchHeroButton.IsValid&&SwitchHeroButton.IsVisible)
						  {
								if (NewCharacterName==null)
								{
									 SwitchHeroButton.Click();
									 SwitchHeroButton_=null;
								}
								else if (ZetaDia.Service.CurrentHero.Name==NewCharacterName)
								{
									 //
									 Logger.Write(LogLevel.OutOfGame, "Successfully Created New Character");
									 return RunStatus.Success;
								}
						  }
						  else if (CreateHeroButton!=null&&CreateHeroButton.IsValid&&CreateHeroButton.IsVisible)
						  {
								CreateHeroButton.Click();
						  }
						  else if (HeroNameText!=null&&HeroNameText.IsValid&&HeroNameText.IsVisible)
						  {
								if (!SelectedClass)
								{
									 UIElement thisClassButton=SelectHeroType(Zeta.Internals.Actors.ActorClass.DemonHunter);
									 if (thisClassButton!=null&&thisClassButton.IsValid&&thisClassButton.IsEnabled&&thisClassButton.IsVisible)
									 {
										  thisClassButton.Click();
										  SelectedClass=true;
									 }
								}
								else
								{
									 if (NewCharacterName==null)
										  NewCharacterName=GenerateRandomText();

									 if (HeroNameText.TextObject.IsValid)
									 {
										   Logger.Write(LogLevel.OutOfGame, "Valid TextObject for character name UI");
									 }

									 if (!HeroNameText.HasText)
									 {
										  HeroNameText.SetText(NewCharacterName.Substring(0, 1));
									 }
									 else
									 {
										  if (HeroNameText.TextObject.Text!=NewCharacterName)
										  {
												HeroNameText.SetText(NewCharacterName.Substring(0, HeroNameText.TextObject.TextLength+1));
										  }
										  else if (CreateNewHeroButton!=null&&CreateNewHeroButton.IsVisible&&CreateNewHeroButton.IsEnabled)
										  {
												CreateNewHeroButton.Click();
										  }
									 }
								}
						  }

						  LastActionTaken=DateTime.Now;
					 }
					 return RunStatus.Running;
				}


				//barb: 0x98976D3F43BBF74
				//demonhunter: 0x98976D3F43BBF74
				//monk: 0x7733072C07DABF11
				//witchdoctor: 0x1A2DB1F47C26A8C2
				//wizard: 0xBC3AA6A915972065

				internal static string NewCharacterName=null;

				private static string GenerateRandomText()
				{
					 var chars="abcdefghijklmnopqrstuvwxyz";
					 var random=new Random();
					 var result=new string(
						  Enumerable.Repeat(chars, random.Next(5,8))
						  .Select(s => s[random.Next(s.Length)])
						  .ToArray());

					 Log("Generated Name "+result);
					 return result;
				}
		  }
	 }
}