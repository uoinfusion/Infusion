// You can use all commands registered in following files:
#load "UOErebor\Specs.csx"
#load "UOErebor\hidding.csx"
#load "UOErebor\hpnotify.csx"
#load "UOErebor\looting.csx"
#load "UOErebor\targeting.csx"
#load "UOErebor\watchdog.csx"

// These scripts are specific for UOErebor shard (http://uoerebor.cz/)
// and most likely they will not work at you shard. But you can use them
// as a tutorial how to write your very own scripts. 

// You can duplicate this file and have a specific configuration
// for each character.

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
// If you want to loot even items specified in UselessLoot you can modify
// looting.csx script or you can omit the Looting.UselessLoot. For example
// if you want just to ignore arrows and tight boots then you can use:
// Looting.IgnoredLoot = new[] { Specs.Arrow, Specs.TightBoots };

// Enable health (hit point) change notification by default. It means
// that you will see number stating current health above you and
// all other mobiles whenever their health changes.
// If you don't want to see the notification, then remove following line:
HitPointNotifier.Enable();
// or change it to:
// HitPointNotifier.Disable();

// Feel free to remove all these comments and following section
// to make the file more readable and less chatty at startup.
// It is here just as an introduction to Infusion scripting.
UO.Log("\nWrite ,help and press enter if you want get information about all available commands.\n" +
"Write ,edit and press enter to open the built-in script editor.\n\n" +
"In console you can use these keyboard shortcuts:\n" +
 " - esc to clear command line\n" +
 " - page up, page down, ctrl+home, ctrl+end, up, down to scroll the console content\n" +
 " - alt+up, alt+down to navigate command line history\n" +
 " - tab to autocomplete a command on the command line\n\n\n" +
    "For more information about Infusion take a look at " +
    "https://github.com/uoinfusion/Infusion/wiki."
);
