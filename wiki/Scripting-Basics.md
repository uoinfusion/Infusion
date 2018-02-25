## Editing scripts

You can write Infusion scripts in [C# language](https://en.wikipedia.org/wiki/C_Sharp_(programming_language)) or more strictly speaking in [C# script](https://msdn.microsoft.com/en-us/magazine/mt614271.aspx) variant of the language. You can get a quick introduction to the language from this nice [C# tutorial](http://csharp.net-tutorials.com/), or if you are willing to invest much more time, you can become a master of C# after reading a Jon Skeet's book [C# in Depth](https://www.manning.com/books/c-sharp-in-depth-fourth-edition) to master the language.

C# script files have `.csx` extension. You can edit them in any editor of your choice. [Visual Studio](https://www.visualstudio.com/vs/community/) and [Visual Studio Code](https://code.visualstudio.com/) are most favored editors for C#. But you can use any other editor, even _Notepad_ if you like.

The best part is that Infusion contains a decent text editor with syntax highlighter and code completion and is very well integrated with Infusion and allows you to write, test and fine tune your scripts at a rapid pace. To open the integrated editor:

1. Switch to Infusion console.
1. Type: `,edit`.
1. Press enter.

This runs `,edit` command which opens the integrated editor:

[[scripting-basics-empty-editor.png|Empty editor]]

> Note: if you haven't set the initial script path, then Infusion writes an error to the console. You can set initial script on [[Infusion launcher window|getting-started#starting-ultima-online-client-with-infusion]] or by running command `,load <absolute path to initial script file>`.

You can either create a new file by using `Ctrl+N` or you can open already existing script files:

1. Click on vertically oriented `Scripts` tab at the top left border of the editor window.
1. Double-click a script file from the list, for example `startup.csx`.

`startup.csx` contains source code similar to this:

```csharp
using System;
using System.Collections.Generic;
using System.Threading;
using Infusion.Proxy;
using Infusion.Packets;
using Infusion.Proxy.LegacyApi;
using Infusion.Packets.Parsers;
using Infusion.Gumps;
using static Infusion.Proxy.LegacyApi.Legacy;
```

Let's skip the meaning of this code for now. Put cursor at the end of the file and type:

```csharp
Say("Hello World!");
Say("Good morning!");
```

You use `F5` and Infusion executes whole script file and the script forces your character to say "Hello World!" and "Good morning!".

Now, select the first line with "Hello World!" greeting:

```csharp
Say("Hello World!");
```

If you press `F5` key. Infusion executes just the selected line and ignores the rest of the script file. Your character says just "Hello World!" this time.

## Creating commands

Starting scripts from the text editor is clumsy and barely usable from the game. Commands are there to make it starting script more convenient. You start commands by typing their name prefixed with comma on the Infusion console or from the game client which makes it easy to integrate Infusion commands with "in game" macros.

You already know a command: `,edit` and you can create your own commands.

Let's continue with the previous example and try create a command that drives your character to say two greetings. First we have to move the code to a method:

```csharp
void SayHello()
{
    Say("Hello World!");
    Say("Good morning!");
}
```

If you press F5 now, your character doesn't say anything. Infusion just remembers that there is a method named `SayHello` and what it should do. You can  execute the method from the editor by selecting `SayHello();` and using `F5`:

```csharp
void SayHello()
{
    Say("Hello World!");
    Say("Good morning!");
}

SayHello();
```

Your character says something but  you cannot still start `SayHello` method from the game. Let's replace `SayHello();` and register a new command instead:

```csharp
void SayHello()
{
    Say("Hello World!");
    Say("Good morning!");
}

RegisterCommand("say-hello", SayHello);
```

Now, if you write `,say-hello` from the game, then Infusion executes `SayHello` method and you character says both greetings. Congratulation! You have created your first command.

## Modifying scripts and commands

Let's continue with the previous example.

The message of `say-hello` command is really trivial, maybe even stupid. Let's change it to express something much more clever. Change `SayHello` method, select just this code and use `F5`:

```csharp
void SayHello()
{
    Say("Infusion is the best!");
}
```

Infusion executes the method and notices your changes.

Try to run the method from editor (select the code, use `F5`):

```csharp
SayHello();
```

Your character says the right words: "Infusion is the best!". So far so good.

Try to run `,say-hello` command. Your character says: "Hello World!" and "Good morning!". What is going on? Why Infusion ignores your changes?

Infusion still remembers `say-hello` as a call to the `SayHello` method with 2 greetings messages. Running  the changed method doesn't affect any dependent commands or other methods. To accept your changes Infusion needs to refresh its memory by re-running command registration:

```csharp
RegisterCommand("say-hello", SayHello);
```

If you suspect Infusion that it ignores your changes, try to run the whole file (`F5` without selected text in the editor) or running `,reload` command either from Infusion console or from the game. `,reload` command reloads the initial script.