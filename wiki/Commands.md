Commands are the way how to invoke a scripted action form game or from console.

## Registering commands

```CSharp
void SayHello()
{
    for (int i = 0; i < 10; i++)
    {
        UO.Say("Hello");
        UO.Wait(1000);
    }
}

UO.RegisterCommand("hello", SayHello);
```

If you type `,hello` in game window or on Infusion console, then player greets you 10 times in 10 seconds.

You cannot start one command twice.

## Managing commands

- type `,list` to get names of all running commands
- type `,terminate` to terminate all running commands
- type `,terminate hello` to terminate `hello` command

## Parametrizing commands

Sometimes you want to execute same command but in a little bit different way. For example you are a priest and you have a healing script. You want be able to start healing for each member of your party.

Probably most straight forward an intuitive approach is to register multiple commands:

```CSharp
ObjectId[] targetIds = new ObjectId[3];

void Healing(ObjectId targetId)
{
    var target = UO.Mobiles[targetId];

    if (target != null)
    {
        // you start healing by using bandages on UOErebor server
        UO.Use(Specs.Bandages);
        UO.WaitForTarget();
        UO.Target(target);
    }
    else
        UO.ClientPrint("Cannot see target")
}

UO.RegisterCommand("healing1", () => Healing(targetIds[0]))
UO.RegisterCommand("healing2", () => Healing(targetIds[1]))
UO.RegisterCommand("healing3", () => Healing(targetIds[2]))
```

The problem is that Infusion allows you to run `healing1` and `healing2` command at the same time. Most likely instead of healing both targets you end healing nobody.

Instead you can take advantage of *parametrized command*:

```CSharp
void HealingCommand(string parameters)
{
    int targetIndex = int.Parse()
    Healing(targetIds[targetIndex]);
}

void Healing(ObjectId targetId)
{
    var target = UO.Mobiles[targetId];

    if (target != null)
    {
        // you start healing by using bandages on UOErebor server
        UO.Use(Specs.Bandages);
        UO.WaitForTarget();
        UO.Target(target);
    }
    else
        UO.ClientPrint("Cannot see target")
}

UO.RegisterCommand("healing", HealingCommand);
```

Now, you have only one command registered and you can invoke the command by typing:

```Text
,healing 1
,healing 2
```

Infusion still allows only one `healing` command running at a time.

## Managing commands from script

To invoke a command without and with parameters:

```CSharp
    UO.CommandHandler.Invoke(",healing1")
    UO.CommandHandler.Invoke(",healing", "1")
```

`Invoke` waits until the invoked command finishes.

To terminate a command with a specific name:

```CSharp
    UO.CommandHandler.Terminate("healing");
```

To check if a command is already running

```CSharp
    if (UO.CommandHandler.IsRunning("healing"))
    {
        UO.Log("Healing is already running");
    }
```

## Background commands

Background commands are useful for scripts that should run all the time and you don't
want to terminate along with other scripts.

For example [UOErebor\questarrow.csx](https://github.com/uoinfusion/Infusion/blob/master/ExampleScripts/UOErebor/questarrow.csx)
writes coordinates of a quest location when you start a quest and it runs whole time you play.

Time to time you have to `,terminate` a command, especially when you
develop a new one. Usually you don't want to terminate commands like `questarrow`. If you would terminate `questarrow` command by typing `,terminate` then you would need to start
`,questarrow` command again or you would loose the comfort to see coordinates of a new quest
location. Now, imagine you have more commands like `questarrow` and you have to restart them
after each `,terminate`.

- `,terminate` doesn't terminate background commands
- you can terminate background command with `,terminate questarrow`
- you can register background command with `UO.RegisterBackgroundCommand`
- terminate all commands `,terminate-all` terminates also background commands
- `UO.CommandHandler.Invoke` doesn't wait until the invoked background command to be finishes
