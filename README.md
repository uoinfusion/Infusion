# Infusion

[![Build status](https://ci.appveyor.com/api/projects/status/jm3n5ix4avbbv0up?svg=true)](https://ci.appveyor.com/project/JakubLinhart/infusion)
[![Join the chat at Discord](https://img.shields.io/discord/522669579466440704.svg)](https://discord.gg/RYx8jzs)

Infusion is an Ultima Online assistant similar to Injection. Infusion focuses on powerful and easy scripting features.
You can write scripts in C# using either built in code editor that has source code highlighting, auto-complete and
[REPL](https://en.wikipedia.org/wiki/Read%E2%80%93eval%E2%80%93print_loop) (Read Eval Print Loop) which
makes getting started with script authoring much easier.

Infusion supports all clients - write your scripts once and run them on classic client or on CrossUO/OrionUO. You don't need to rewrite them if you want switch your client.

If you are not familiar with programming you can start using example scripts which covers many repetitive task in the game. They are designed to be easy to use and they guide you through a configuration process. You don't need to change any code to start using them.

You can use [Visual Studio](https://www.visualstudio.com/cs/thank-you-downloading-visual-studio/?sku=Community) or
[Visual Studio Code](https://code.visualstudio.com/) if you are a skilled developer and you want to enjoy their excellent refactoring and
debugging capabilities.

## Yoko Injection Scripts

Infusion can interpret Yoko Injection scripts. To get started, watch a video how to [push a button on a gump from Yoko Injection Script](https://www.youtube.com/watch?v=0x00bxTG8-c).

If you want make writing Yoko Injection scripts a lot easier, you can install [Injection script](https://marketplace.visualstudio.com/items?itemName=uoinfusion.injection-vscode) extension for [Visual Studio Code](https://code.visualstudio.com/).

Have you ever scratch your head because of "impossible" bugs? Make your live easier and debug your scripts directly from Infusion - watch a [video](https://www.youtube.com/watch?v=PUhZra2w0pI) how to do that.

If you find any missing feature or API, please [create an issue](https://github.com/uoinfusion/Infusion/issues) here on Github.

## Getting Started with C# Scripts

- Read [Getting Started](https://github.com/JakubLinhart/Infusion/wiki/Getting-started).
- If you know Injection, take a look to a brief [comparison with Infusion](https://github.com/uoinfusion/Infusion/wiki/Comparison-with-Injection).
- Look at [example scripts](https://github.com/uoinfusion/Infusion/tree/master/ExampleScripts).

You may take a look at some [videos](https://www.youtube.com/channel/UCfQMN3_FpX4wx1yQc1IGOIw):

- [Debugging scripts with Visual Studio 2017 Community edition](https://www.youtube.com/watch?v=X2hyImvCSHg)
- [Using Hotride/UOPatcher to enable FPS patch](https://www.youtube.com/watch?v=hagheyX6Odo)

Current C# API is in alpha release. It means that APIs are not stable yet and there may be some breaking changes.

## Client support

Infusion works with official clients and CrossUO/OrionUO/ClassicUO. Infusion supports encryption and is able to connect to OSI servers.

Please, create an issue if you find any bug, missing feature or server/client combination that doesn't work
for you.

## UOErebor

We develop, test and regularly use Infusion in game on highly customized shard [UOErebor](http://uoerebor.cz/) (Czech only). Which means that Infusion is pretty
stable on this shard and there is a lot of example scripts you can immediately start using:

- Display [status bar](ExampleScripts/UOErebor/party.csx) of your friends in an external window. These status bars never disappear so you always see how many HP your friends have.
- [Hiding](ExampleScripts/UOErebor/hidding.csx) with *always walk* feature to utilize stealth. 
- [Targeting](ExampleScripts/UOErebor/targeting.csx) red karma only, skipping own summon/pets.
- Fast and reliable [walking](ExampleScripts/UOErebor/walking.csx) from script.
- Read status of spell [chargers](ExampleScripts/UOErebor/chargers.csx) from script.
- Damage [notification](ExampleScripts/UOErebor/hpnotify.csx).
- Show [experience](ExampleScripts/UOErebor/explevel.csx) in game window title.
- [Open bank](ExampleScripts/UOErebor/banking.csx) using banker or house menu.
- Using [travel stones](ExampleScripts/UOErebor/travelstone.csx).
- Filtering sounds, overall light and weather effect.
- [Looting](ExampleScripts/UOErebor/looting.csx).
- [Chasing and shaving](ExampleScripts/UOErebor/sheepshaving.csx) sheep.
- Easier item manipulation (moving food/regs only, moving the same type).
- [Predefined names](ExampleScripts/UOErebor/Specs.csx) for many item types/color combinations (spell scrolls, food, regs, tools, resources, doors and many others).
- Popular UO client patches (like [FPS patch](https://www.youtube.com/watch?v=hagheyX6Odo)) using [UOPatcher](https://github.com/Hotride/UOPatcher).
