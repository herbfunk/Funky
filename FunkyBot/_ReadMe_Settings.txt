!It is IMPORTANT to setup the bot via Settings!

This plugin uses the following settings:

Demonbuddy - Bot Settings.
Funky Settings (can be opened by clicking the Funky Button, found on the main tab*).
Plugin Settings (can be opened by using FunkyTrinity config, found on the plugins tab (or by clicking the button found in the misc tab in the Funky Settings)).



Note:	Townrun Behavior Hooks can be disabled by editing the Treehooks.xml File inside the plugin folder.
		Funky Settings are unique to each individual character.
		Funky Settings cannot be opened via Funky Button when Demonbuddy is launched via Relogger!
			-However you can still access it from the plugin settings -> Funky Settings
	 



Funky Settings:
	-Avoidances
		-Advanced Projectile Testing will use Ray Intersection against Sphere. (CPU-Intense!)
		-Setting an avoidance health percent to zero will ignore it.
		-All avoidances can be ignored by unchecking Attempt Avoidance Movements.
		-Radius may need to be tweaked for best "avoidance" movement.
		-Health value is the minimum value before avoidance is triggered.
	-Combat:
		-Globe, Potion and Heathwell percents are minimum health before any action will be taken.
	-Targeting:
		-Unit exception settings will set qualified units as "special" -- and also excludes them from clustering logic.
	-Grouping:
		-Is triggered when currently in combat -- and distant clusters that are valid for targeting meet the requirements.
		-This will engage the distant cluster with the intent to make a bigger group.
	-Class
		-Settings vary depending on which class is currently active.
	-General:
		-OOCIdenifyItems will preform out of combat IDing of items when the minimum # of unid items is met.
		-OOC Minimum is the minimum unidified item count before OOC IDing behavior will start.
		-BuyPotions will buy upto 99 potions when under.
		-EnableWaitAfterContainer uses the KillDelayTime to wait after successfully using a container.
		-ExtendedRangeRepChest will allow upto 75f instead of obeying the normal container range.
		-Use Out Of Combat Movements enables abilities that increase speed and is used during non-combat movements.
		-EnableCoffeeBreaks will execute an AFK after completion of a game and the duration since the last break has surpassed the minmum time.
		-Min/Max BreakTimes are used to create a range of time during each AFK break.
		-Minimum time before break -- This is in hours (Example 1 == One Hour, 1.5 == One and a half hours).
	-Item:
		-ItemRules takes priority when used, Giles/DB Weighting option is secondary.
	-Targeting:
		-Clustering (See Bottom of TextFile for details!)
		-Range:
			-Range is used to determine if the object should be considered as current target by checking distance.
			-These values are added to the profile combat or loot value (Combat only for units).
		-General:
			-Delay after combat sets a time that will enforce a wait after combat finishes.
			-Extend Combat Range will temporary extend all kill ranges by the value set.
		



ItemRules Funky Extensions:
	Funky adds two additional rule sets that are used during town runs.
	*salvage
		-Determines which items should be salvaged.	
	*UnidStash
		-Determines which items should be stashed prior to idenification. (Rules are similar to pickup)




Clustering Settings
	-Distance determines how close units are to one another, I.E. how compact/tight the cluster will be.
	-Unit Count determines which clusters are valid for target interaction.
	-Health before disabling logic is used as a safety feature, which will disable the logic and enable all normal combat.

How does the clustering logic work?
	Clusters are computed after refreshing grabs a valid list of objects. 
	During this stage units specificly are added to a list which determines if it is valid for cluster computing.
	Clusters are computed using the settings and the final result determines if which units are valid for targeting.
	
Of course there are many exceptions to the rule...
	Above average units will not be ignored if valid regardless if it is clustered. (Unless Completely ignored by setting)
	Out-of-Combat behaviors, such as town portaling, will no longer obey the rule.
	If an unit is determined to be blocking the bot, then that unit no longer obeys the rule.
	Units with low hp and setting to kill these units will make the unit ignore the rule.
	Bot HP that is lower than the setting value and setting enabled will force all units to ignore the rule.

Some examples of how to use it.
	Example: ignore majority of small packs but engage in massive groups.
		-Using 5+ unit count with a 6 distance cluster size will ignore most units until compacted.
		-Recommended to keep the HP value for disable at least 50% / 0.50
	Example: steady pace but not allowing to become overwhelmed.
		-Using 2+ unit count with a 8 distance cluster size will keep a safe pace.