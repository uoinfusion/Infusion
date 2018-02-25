## Cheat sheet

| | Injection | Infusion
|--- |--- | ---|
| define number variable       | `var i = 0`                             | `var i = 0;` |
| define number array          | `dim a[10]`                             | `var a = new int[10]` |
| define string array          | `dim s[10]`                             | `var s = new string[10]` |
| assign to array              | `s[0] = 'some text'`                    | `s[0] = "some text";` |
| if                           | `if i == 0 then ... else ... endif`     | `if (i == 0) { ... } else { ... }` |
| while                        | `while i < 10 ... wend`                 | `while (i < 10) { ... }` |
| repeat                       | `repeat ... until i < 10`               | `do { } while (i < 10);` |
| define function              | `sub func1() ... end sub`               | `static void func1() { ... }` |
| call function                | `func1()`                               | `func1();` |
| return value from function   | `return 1;`                             | `return 1;` |
| Terminate all functions      | `UO.exec("terminate all")`              | `UO.CommandHadler.TerminateAll();` |
| Terminate function func1     | `UO.Exec("terminate func1")`            | `UO.CommandHandler.Terminate(("func1");` |
| wait                         | `wait(1000)`                            | `UO.Wait(1000);` |
| print                        | `UO.Print('text')`                      | `UO.ClientPrint("text");` |
| say text using keyboard      | `UO.Say('text')`                        | no equivalent, see ServerPrint|
| send text directly to server | `UO.ServerPrint('text')`                | `UO.Say("text");` |
| use skill                    | `UO.UseSkill("Meditation")`             | `UO.UseSkill(Skill.Meditation)`|
| cast spell                   | `UO.Cast("Night Sight")`                | `UO.CastSpell(Spell.NightSight)` |
| Use type (crook)             | `UO.UseType("0x13F5")`                  | `UO.Use(Specs.Crook);` |
| get current player health    | `UO.GetHP()`                            | `UO.Me.CurrentHealth` |
| get current player health    | `UO.Life`                               | `UO.Me.CurrentHealth` |
| get max player health        | `UO.GetMaxHP()`                         | `UO.Me.MaxHealth`     |
| get current player mana      | `UO.Mana`                               | `UO.Me.CurrentMana`   |
| get player's location        | `UO.GetX(), UO.GetY(), UO.GetZ()`       | `UO.Me.Location`    |
| journal contains something   | `if UO.InJournal('some text') then`     | `if (UO.Journal.Contains("some text"))` |
| delete journal               | `UO.deletejournal()`                    | `UO.Journal.Delete()` |
| press key                    | `UO.Press(33)`                          | `UO.ClientWindow.PressKey(KeyCode.PageUp);` |

## Configuring what to display in game client window's title

*Injection*

[[injection-title.jpg|Put information to the game window's title]]

*Infusion*

Put this code to your initial script:

```CSharp
Title.Text = () =>
    $"Diblik - Weight: {UO.Me.Weight}, Hp: {UO.Me.CurrentHealth:###}/{UO.Me.MaxHealth:###}, "+
    $"Mana: {UO.Me.CurrentMana}/{UO.Me.MaxMana}, " +
    $"Armor: {UO.Me.Armor}, " +
    $"Weight: {UO.Me.Weight}, " +
    $"GP: {UO.Me.Gold}, " +
    $"BM: {Title.Amount(Specs.BloodMoss)}, " +
    $"BP: {Title.Amount(Specs.BlackPearl)}, " +
    $"GA: {Title.Amount(Specs.Garlic)}, " +
    $"GS: {Title.Amount(Specs.Ginseng)}, " +
    $"MR: {Title.Amount(Specs.Mandrake)}, " +
    $"NS: {Title.Amount(Specs.Nightshade)}, " +
    $"SA: {Title.Amount(Specs.SulfurousAsh)}, " +
    $"SS: {Title.Amount(Specs.SpidersSilk)}, " +
    $"Pos: {UO.Me.Location} ";
Title.Enable();
```

And include `UOErebor\title.csx` file at the beginning of your initial script:

```CSharp
#load "UOErebor\title.csx"
```

## Filtering light and weather

*Injection*

[[injection-filters.jpg|Filters]]

*Infusion*

```CSharp
UO.ClientFilters.Light.Enable();
UO.ClientFilters.Weather.Enable();
```

## Targeting specific object

*Injection*

```Injection
UO.WaitTargetObject(tamingTarget)
uo.UseSkill("Animal Taming")

UO.WaitTargetObject("self")
UO.Cast("Night Sight");
```

*Infusion*

```CSharp
UO.UseSkill(Skill.AnimalTaming);
UO.WaitForTarget();
UO.Target(tamingTarget);

UO.CastSpell(Spell.NightSight);
UO.WaitForTarget();
UO.Target(UO.Me);
```

## Ask player for an item

*Injection*

```Injection
UO.Exec("addobject myitem")
While UO.Targeting()
    Wait(100)
wend
```

*Infusion*

```CSharp
var myItem = UO.AskForItem();
var myMobile = UO.AskForMobile();
```

## SetArm, Arm

*Injection*

```Injection
UO.exec("setarm 'weapon'")
UO.exec("arm 'weapon'")
```

*Infusion*

Infusion itself doesn't have any equivalent for setarm/arm. Instead UOErebor\equip.csx example script implements the same functionality:

```CSharp
var weapon = Equip.GetHand();
Equip.Set(weapon);
```

You must include UOErebor\equip.csx file at the beginning of your script:

```CSharp
#load "UOErebor\equip.csx"
```

## Waiting for something in journal

*Injection*

```Injection
repeat
    wait(100)
until UO.injournal('first awaited text') or UO.injournal('second awaited text')

if UO.injournal('first awaited text')
    # do some action
end if

if UO.injournal('second awaited text')
    # do other action
end if
```

*Infusion*

```CSharp
UO.Journal
    .When("first awaited text", () => { /* do some action */ })
    .When("second awaited text", () => { /* do other action */ })
    .WaitAny();
```

## Deleting specific text from journal

*Injection*

```Injection
    if UO.InJournal('specific text 1') then
        UO.DeleteJournal('specific text 1')
    end if

    if UO.InJournal('specific text 2') then
        UO.DeleteJournal('specific text 2')
    end if
```

*Infusion*

You don't need to delete a specific text from a journal. You can create two separate journals and delete them independently:

```CSharp
public static class MyScript
{
    private static SpeechJournal journal1 = UO.CreateSpeechJournal();
    private static SpeechJournal journal2 = UO.CreateSpeechJournal();

    public static void MyMethod()
    {
        if (journal1.Contains("specific text 1"))
            journal1.Delete();

        if (journal2.Contains("specific text 2"))
            journal2.Delete();
    }
}
```