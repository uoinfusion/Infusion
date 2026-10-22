# Infusion for macOS

`Infusion.Mac` is a console host for running Infusion on macOS with [ClassicUO](https://github.com/ClassicUO/ClassicUO).
It does what `Infusion.Desktop` does on Windows - starts the proxy, launches the client, runs C# (`.csx`)
and Injection scripts, writes logs and keeps profiles - but without WPF:
the Infusion console is the terminal window and commands (`,reload`, `,help`, ...) are typed there.

It is built from the same sources as the Windows version (`Infusion`, `Infusion.LegacyApi`, `Infusion.Proxy`,
`Infusion.EngineScripts`), so scripts written for Windows run unchanged in most cases.

Tested on an Apple Silicon Mac (ClassicUO x86_64 through Rosetta 2) with the UO Erebor shard.

## Quick start

```bash
git clone --recursive https://github.com/uoinfusion/Infusion.git
cd Infusion
# copy your Windows profiles (*.profile, with the server, account and initial script) and scripts
mkdir -p ~/Infusion && cp -R /path/to/windows/infusion/profiles /path/to/windows/infusion/scripts ~/Infusion/
Infusion.Mac/scripts/infusion-mac.sh --uo /path/to/UO/data --profile MyProfile
```

`scripts/infusion-mac.sh` installs Rosetta 2 (Apple Silicon), a local .NET 8 SDK, builds `Infusion.Mac`,
downloads ClassicUO for macOS and starts everything. See the header of the script for settings
(`INFUSION_HOME`, `CUO_CLIENT_VER`, ...).

Manual build:

```bash
dotnet build Infusion.Mac/Infusion.Mac.csproj -c Release
dotnet Infusion.Mac/bin/Release/net8.0/Infusion.Mac.dll --root ~/Infusion --client ~/Infusion/ClassicUO/ClassicUO.bin.osx --profile MyProfile
```

`Infusion.Mac` is intentionally not part of `Infusion.sln`, so the Windows (net47) build is not affected.

## Options

| option | meaning |
|---|---|
| `--profile <name\|id>` | profile from `<root>/profiles` (default: the selected one) |
| `--client <path>` | client executable (`ClassicUO.bin.osx`) |
| `--uo <path>` | folder with Ultima Online data files (`*.mul`) |
| `--root <path>` | Infusion root with `scripts/`, `profiles/`, `logs/` |
| `--server <host,port>` | override the server from the profile |
| `--no-client` | start only the proxy, connect the client manually to `127.0.0.1:<port>` |
| `--check <script.csx>` | only compile a script and print errors (useful for editors / CI) |

## Profiles

Profiles are the same JSON `*.profile` files as on Windows (`LauncherOptions`: `ServerEndpoint`, `UserName`,
`Password`, `InitialScriptFileName`, `ProtocolVersion`, `ClassicUO.EncryptionSetup`, `ClassicUO.EncryptionVersion`,
plus `Options` saved by scripts). Unknown parts of the file are kept, so a profile can be used on both systems.
ClassicUO is started with `-ip 127.0.0.1 -port <proxy port> -username ... -password ...`; set
`"encryption": 0` in ClassicUO `settings.json`, Infusion handles the encryption according to the profile.

## Differences from Windows

- **Scripts**: `#load "UOErebor\Specs.csx"` (Windows separators) and different letter case in file names work.
  `#r "Infusion.Scripts.UOErebor.Extensions.dll"` is redirected to the cross-platform build
  (`ExampleScripts/UOErebor/Infusion.Scripts.UOErebor.Extensions.Mac`), `System.Media.SoundPlayer` to a small
  replacement playing sounds with `afplay` (`Compat/System.Windows.Extensions`).
- **Statuses window** (party, pets) - instead of WPF windows:
  - `,status-gump` - a gump inside the game (shown automatically after login). Responses to this gump never reach the server.
  - `,status-window` - a small window in Chrome app mode / the default browser (`http://127.0.0.1:47318/`).
  - Both show HP/mana/stamina, stats and party members; click on a member targets it.
  - Optional lines from scripts and script heartbeats, see `ScriptStatus.cs` - when a script registered a heartbeat
    and stops, the gump turns red ("... is not running!").
- **Title**: `Title.Text` from scripts is shown in the title of the terminal window (the ClassicUO window cannot be renamed).
- **Client window control** (focus, keys) uses AppleScript / System Events - the terminal needs the Accessibility permission.
  Mouse clicks into the client are not implemented.
- **UO Automap (UOAM)**: UOAM runs under Wine; `tools/uoam-bridge` creates the UOAssist window for it and talks
  to Infusion over TCP (`127.0.0.1:47317`), so `uoam.csx` (Track Player, chat) works.
- **Not available**: Injection window (Avalonia), DirectionPad, screenshots through `System.Drawing`.
- On UO Erebor, keeping the ClassicUO **World Map** open led to disconnects - use UOAM instead.

## Editing scripts

`,edit` opens the scripts folder in Visual Studio Code when it is installed (otherwise the default text editor).
`--check` can be used to compile a script without starting the game.
