// These scripts are specific for UOErebor shard (http://uoerebor.cz/)
// and most likely they will not work on other shards. In any case you can
// use them as a tutorial how to write your very own scripts.

// You don't have to load all these scripts for all characters. 
// You can safely remove load of any script file you don't want to
// use. Removing scripts may make script (re)loading faster a little bit.
#load "UOErebor\colors.csx"
#load "UOErebor\Specs.csx"
#load "UOErebor\afk.csx"
#load "UOErebor\banking.csx"
#load "UOErebor\common.csx"
#load "UOErebor\cooking.csx"
#load "UOErebor\combattext.csx"
#load "UOErebor\craft.csx"
#load "UOErebor\doors.csx"
#load "UOErebor\eating.csx"
#load "UOErebor\healing.csx"
#load "UOErebor\hiding.csx"
#load "UOErebor\hpnotify.csx"
#load "UOErebor\items.csx"
#load "UOErebor\light.csx"
#load "UOErebor\looting.csx"
#load "UOErebor\magery.csx"
#load "UOErebor\meditation.csx"
#load "UOErebor\party.csx"
#load "UOErebor\pets.csx"
#load "UOErebor\questarrow.csx"
#load "UOErebor\selling.csx"
#load "UOErebor\sheepshaving.csx"
#load "UOErebor\sounds.csx"
#load "UOErebor\tailoring.csx"
#load "UOErebor\targeting.csx"
#load "UOErebor\travelstone.csx"
#load "UOErebor\walking.csx"
#load "UOErebor\watchdog.csx"
#load "UOErebor\tracker.csx"

using System;

// You can duplicate this file to have a specific configuration
// for each character.



// Feel free to remove all these comments and following section
// to make the file more readable and less chatty at startup.
// It is here just as a brief introduction to Infusion.
UO.Log("\nWrite ,help and press enter if you want get information about all available commands.\n" +
"Write ,edit and press enter to open the built-in script editor.\n\n" +
"Your character says text that you enter on command line and that doesn't start with a comma.\n\n" +
"In console you can use these keyboard shortcuts:\n" +
 " - esc to clear command line\n" +
 " - page up, page down, ctrl+home, ctrl+end, up, down to scroll the console content\n" +
 " - alt+up, alt+down to navigate command line history\n" +
 " - tab to autocomplete a command on the command line\n\n\n" +
    "For more information about Infusion take a look at " +
    "https://github.com/uoinfusion/Infusion/wiki."
);


// Id of container you want to use as loot target.
// If there is no container with such id in the game then
// the player puts the looted items into backpack (UO.Me.BackPack).
// You can use ,info command to get the id number.
Looting.LootContainerId = 0x40000000;

// Item specification of items you don't care about. It combines
// generally useles items (take a look to looting.csx file and search
// for UselessLoot) and items passed to Including method (Arrow, TightBoots, ...).
Looting.IgnoredLoot = Looting.UselessLoot.Including(
    Specs.Arrow, Specs.TightBoots, Specs.Furs, Specs.RawRibs, Specs.HorseShoes);
// If you want to loot items specified in UselessLoot you can modify
// looting.csx script or you can omit the Looting.UselessLoot. For example
// if you want to ignore just arrows and tight boots then you can use:
// Looting.IgnoredLoot = new[] { Specs.Arrow, Specs.TightBoots };


// You can use ,target-next ,target-prev and ,target-last command to use
// smarter targeting.
// Targeting script may turn on war mode. If you play characters like
// war, thief, ranger or mystic then you don't care and you can set
// Targeting.AutoTurnOffWarMode to false.
Targeting.AutoTurnOffWarMode = false;
// But if you play characters like mage, necro or cleric,
// you don't want to have war mode on - so it would be nice to turn
// war mode off after targeting a mobile.
// You may want to uncomment the next line:
// Targeting.AutoTurnOffWarMode = true;

// Sets targeting mode to Pvm, which means that you target
// monsters and player with red karma.
Targeting.Mode = TargetingModes.Pvm;

// If you want play Pvp you don't care about monsters. You
// can uncomment next line (or invoke ,target-pvp command) and you target
// players with red karma, no monsters:
// Targeting.Mode = TargetingModes.Pvp;

// If you want train Pvp with your friends, you want target all players
// event when they have good karma. If you uncomment next line (or invoke ,target-pvpfriend)
// you target players regardless their karma (no monsters):
// Targeting.Mode = TargetingModes.PvpFriend;


// Enable health (hit point) change notification by default. It means
// that you will see number stating current health above you and
// all other mobiles whenever their health changes.
// If you don't want to see the notification, then remove following line:
HitPointNotifier.Enable();
// or change it to:
// HitPointNotifier.Disable();

// This mode notifies about hit point changes of all mobiles on the screen.
// It displays the notification above specific mobile head.
HitPointNotifier.Mode = HitPointNotificationModes.AboveAllMobiles;

// You can configure color of hit point change notification text:
HitPointNotificationModes.AboveAllMobiles.EnemyColor = Colors.Red;
HitPointNotificationModes.AboveAllMobiles.FriendColor = Colors.LightBlue;
HitPointNotificationModes.AboveAllMobiles.PetsColor = Colors.Green;
HitPointNotificationModes.AboveAllMobiles.MyColor = Colors.Green;


// You can choose a light source. If you call Light.Check() and
// you need create or renew light, then script makes light using
// PreferredLightSource.
// If you want to write you own light source, you can take a look
// at LightSources class in UOErebor\light.csx how to do that.
Light.PreferredLightSource = LightSources.Torch; // uses torch to make light
//Light.PreferredLightSource = LightSources.Potion; // uses nightsight potion to make light


// Start quest arrow tracking. The script prints a message when
// a new quest arrow appears.
// You can call QuestArrow.Info() (or ,questarrow-info) to get information about
// an active quest arrow.
QuestArrow.Enable();
// If you want to stop quest arrow tracking, then you can remove previous line
// or uncomment the following line:
// QuestArrow.Stop();


// This script redirects messages from journal over your head, so you can
// notice them more easilly.
CombatText.Add("Kouzlo se nezdarilo.", Colors.Blue);
CombatText.Add("Nevidis na cil.", Colors.Blue);
CombatText.Enable();


Hiding.AlwaysWalkEnabled = true;
Hiding.AlwaysWalkDelayTime = TimeSpan.FromMilliseconds(1800);


// Game client ignores light level changes, so you are not bothered by dark in the night.
// You can set this to false if you want to enjoy darkness during night.
UO.ClientFilters.Light.Enable();

// Game client ignores weather changes, so you are not bothered by rain or snow animation.
// You can set this to true if you want to enjoy rain and snow.
UO.ClientFilters.Weather.Enable();

// You can select sounds that you don't want to hear. For example you can
// filter out sounds of your backyard (dog barking and horse neighing):
UO.ClientFilters.Sound.SetFilteredSounds(new[]
{
    Sounds.Horse1,Sounds.Horse2, Sounds.Horse3, Sounds.Horse4, Sounds.Horse5,
    Sounds.Dog1, Sounds.Dog2, Sounds.Dog3, Sounds.Dog4, Sounds.Dog5    
});
// Uncoment following line if you want to hear all sounds:
// UO.Configuration.FilteredSounds = null;
