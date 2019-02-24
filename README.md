# Infusion

[![Join the chat at Discord](https://img.shields.io/discord/522669579466440704.svg)](https://discord.gg/RYx8jzs)

Infusion is an Ultima Online assistant similar to Injection. Infusion focuses on powerful and easy scripting features.
You can write scripts in C# using either built in code editor that has source code highlighting, auto-complete and
[REPL](https://en.wikipedia.org/wiki/Read%E2%80%93eval%E2%80%93print_loop) (Read Eval Print Loop) which
makes getting started with script authoring much easier.

If you are not familiar with programming you can start using example scripts which covers many repetitive task in the game. They are designed to be easy to use and they guide you through a configuration process. You don't need to change any code to start using them.

You can use [Visual Studio](https://www.visualstudio.com/cs/thank-you-downloading-visual-studio/?sku=Community) or
[Visual Studio Code](https://code.visualstudio.com/) if you are a skilled developer and you want to enjoy their excellent refactoring and
debugging capabilities.

## Getting Started

- Read [Getting Started](https://github.com/JakubLinhart/Infusion/wiki/Getting-started).
- If you know Injection, take a look to a brief [comparison with Infusion](https://github.com/uoinfusion/Infusion/wiki/Comparison-with-Injection).
- Look at [example scripts](https://github.com/uoinfusion/Infusion/tree/master/ExampleScripts).

You may take a look at some [videos](https://www.youtube.com/channel/UCfQMN3_FpX4wx1yQc1IGOIw):

- [Push a button on a gump via Yoko Injection Script](https://www.youtube.com/watch?v=0x00bxTG8-c)
- [Debugging scripts with Visual Studio 2017 Community edition](https://www.youtube.com/watch?v=X2hyImvCSHg)
- [Using Hotride/UOPatcher to enable FPS patch](https://www.youtube.com/watch?v=hagheyX6Odo)

If you want to write Yoko Injection scripts, you can install [Injection script](https://marketplace.visualstudio.com/items?itemName=uoinfusion.injection-vscode) extension for [Visual Studio Code](https://code.visualstudio.com/).

## Current Status

Although you should be to connect to any server with encrypted protocol and use Infusion with any Third Dawn and LBR clients
We test it by active game play on a prehistoric Sphere Server 0.99za and with 3.0.6m client.
We would like to extend support for other servers and clients if there is a demand for it.

Current status is alpha. It means that APIs are not stable yet and there may be some breaking changes. There can be many serious bugs and some critical functionality is still missing.

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

[![Join the chat at https://gitter.im/ultimaonlineinfusion/infusion](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/ultimaonlineinfusion/infusion-erebor)