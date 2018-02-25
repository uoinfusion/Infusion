## Installing Infusion

> Warning: Infusion currently supports Third Dawn (3.x) clients only. Other client versions may work as well but I don't test them and you will be most likely disappointed with serious bugs.

1. Make sure that you have [.NET Framework 4.6](https://www.microsoft.com/cs-cz/download/details.aspx?id=48130) or newer installed. If you are using Windows 10, you have already .NET Framework 4.6 installed.
2. Download Infusion [binary distribution](https://github.com/JakubLinhart/Infusion/releases/latest). For example currently there are 3 files on the download page: Infusion-1.0.2-alpha.zip, Source code (zip), Source code (tar.gz). The binary distribution is in Infusion-1.0.2-alpha.zip. The other files contain Infusion source code and you don't need to worry about them.
3. Unpack the downloaded zip file to an Infusion installation folder. It can be any folder, for example c:\Program Files\Infusion.

> Note: If you put Infusion to c:\Program Files\Infusion folder, make sure that you have read/write access to Logs and Scripts sub-folders. Infusion modifies files in these folders. It is the same problem as with Desktop folder of Ultima Online client.

4. Infusion requires that you use a client without encryption. If your Ultima Online server allows using Third Dawn (3.x) clients, you can download a client without encryption [here](https://ulozto.cz/!9w2rZmJfmcvA/client306m-patches-zip). The zip file contains `NoCryptClient.exe`. Copy it to your Ultima Online installation folder (typically c:\Program Files\Ultima Online 2D\).
> Note: SHA-256 checksum of client306m-patches.zip archive is `b8aad641350334b714c567883ef065f44e541b166efd5031efeca4015471af85`. You can verify that nobody
> has hacked the file and inserted a virus by [comparing SHA-256 checksum](https://superuser.com/a/898377). 

Now, Infusion is ready to start.

## Starting Ultima Online client with Infusion

1. Start `Run.cmd` from the Infusion installation folder (for example c:\Program Files\Infusion). Infusion launcher window appears:

[[getting-started-launcher.png|Infusion launcher]]

1. Put your profile name to `Profile` edit box. The profile name doesn't have any meaning for Infusion and contain any text that is understandable for you. I use this pattern for my profile names: `<server> - <character name>`.
1. Put an address of you server with port to `Server address`.
1. You can use `Initial Script` to load different script at startup for each profile. For example I load `craft.csx` script for my craft character and `mage.csx` for my mage. Infusion by default tries to load and execute `startup.csx` file from scripts folder that is part of binary distribution. If Infusion cannot find `startup.csx` file, then this field is empty.
1. `User name` and `Password` fields are optional. Infusion uses them to pre-fill login dialog in Ultima Online client. 
1. Click `Launch`.

Infusion starts Ultima Online client by executing `NoCryptClient.exe` from Ultima Online installation folder (typically c:\Program Files\Ultima Online 2D\) and you can login to the game as usually.

## First steps

The Infusion main window is a very simple text console. In this window you can see 'journal' from the game and script output and you can enter your character speech and commands on the input line prefixed with `>>`. It is the main communication channel with Infusion.

Switch to Infusion console and start typing a message you want your character to say and then press enter.

[[getting-started-console.png|Infusion console]]
[[getting-started-client.png|Ultime Online client window]]

You can start commands either from Infusion console or Ultima Online client by prefixing the command name with comma, for example:

```InfusionCommands
,help
```

prints all known command to Infusion console.

```InfusionCommands
,help info
```

prints `info` command documentation to the Infusion console.

Injection and other tools use very similar way to invoke commands.