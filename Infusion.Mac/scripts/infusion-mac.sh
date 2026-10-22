#!/bin/bash
# Infusion for macOS - one-click setup and start.
#
# First run (everything goes to folders next to this repository, no admin rights needed
# except for Rosetta 2 on Apple Silicon):
#   1. installs Rosetta 2 on Apple Silicon (ClassicUO for macOS is x86_64 only),
#   2. installs .NET 8 SDK locally into <repo>/.dotnet,
#   3. builds Infusion.Mac,
#   4. downloads ClassicUO for macOS into $INFUSION_HOME/ClassicUO and writes its settings.json.
# Every run: starts Infusion, which starts ClassicUO connected through the proxy.
#
# Usage:
#   Infusion.Mac/scripts/infusion-mac.sh --uo <folder with *.mul files> [--profile <name>] [other Infusion.Mac options]
#
# Environment:
#   INFUSION_HOME     Infusion root with scripts/, profiles/, logs/ (default: ~/Infusion)
#   CUO_URL           ClassicUO macOS build to download
#   CUO_CLIENT_VER    client version written to ClassicUO settings.json (default: 7.0.10.3)
#   CUO_ENCRYPTION    ClassicUO encryption setting (default: 0 = Infusion handles encryption)

set -u
REPO="$(cd "$(dirname "$0")/../.." && pwd)"
INFUSION_HOME="${INFUSION_HOME:-$HOME/Infusion}"
CUO_URL="${CUO_URL:-https://github.com/ClassicUO/ClassicUO/releases/download/ClassicUO-dev-release/ClassicUO-osx-x64-release.zip}"
CUO_CLIENT_VER="${CUO_CLIENT_VER:-7.0.10.3}"
CUO_ENCRYPTION="${CUO_ENCRYPTION:-0}"
DOTNET_DIR="$REPO/.dotnet"
DOTNET="$DOTNET_DIR/dotnet"
OUT="$REPO/Infusion.Mac/bin/Release/net8.0"

say()  { printf '\033[1;36m==> %s\033[0m\n' "$*"; }
fail() { printf '\033[1;31mERROR: %s\033[0m\n' "$*"; exit 1; }

# the --uo value is also needed for ClassicUO settings.json
UO_DIR=""
args=("$@")
for ((i = 0; i < ${#args[@]}; i++)); do
  [ "${args[$i]}" = "--uo" ] && UO_DIR="${args[$((i + 1))]:-}"
done

# 1) Rosetta 2
if [ "$(uname -m)" = "arm64" ] && ! /usr/bin/arch -x86_64 /usr/bin/true 2>/dev/null; then
  say "Installing Rosetta 2 (ClassicUO for macOS is x86_64)..."
  /usr/sbin/softwareupdate --install-rosetta --agree-to-license || fail "Rosetta 2 was not installed."
fi

# 2) .NET 8 SDK (local)
if [ ! -x "$DOTNET" ] || ! "$DOTNET" --list-sdks 2>/dev/null | grep -q "^8\."; then
  say "Installing .NET 8 SDK into $DOTNET_DIR..."
  curl -fsSL https://dot.net/v1/dotnet-install.sh -o "$REPO/.dotnet-install.sh" || fail "Cannot download dotnet-install.sh"
  bash "$REPO/.dotnet-install.sh" --channel 8.0 --install-dir "$DOTNET_DIR" --no-path || fail ".NET SDK was not installed."
  rm -f "$REPO/.dotnet-install.sh"
fi
export DOTNET_ROOT="$DOTNET_DIR"
export DOTNET_CLI_TELEMETRY_OPTOUT=1

# 3) build (submodules are needed by Infusion.LegacyApi)
if [ ! -f "$OUT/Infusion.Mac.dll" ] || [ -n "${INFUSION_REBUILD:-}" ]; then
  say "Building Infusion.Mac..."
  git -C "$REPO" submodule update --init UltimaSdk InjectionScript || fail "git submodule update failed."
  "$DOTNET" build "$REPO/Infusion.Mac/Infusion.Mac.csproj" -c Release || fail "Build failed."
fi

# 4) ClassicUO for macOS
CUO="$INFUSION_HOME/ClassicUO"
if [ ! -f "$CUO/ClassicUO.bin.osx" ]; then
  say "Downloading ClassicUO for macOS..."
  mkdir -p "$CUO"
  curl -fL "$CUO_URL" -o "$CUO/.cuo.zip" || fail "Cannot download ClassicUO."
  unzip -q -o "$CUO/.cuo.zip" -d "$CUO" || fail "Cannot unzip ClassicUO."
  rm -f "$CUO/.cuo.zip"
  chmod +x "$CUO/ClassicUO.bin.osx"
  xattr -dr com.apple.quarantine "$CUO" 2>/dev/null
fi

if [ ! -f "$CUO/settings.json" ]; then
  [ -n "$UO_DIR" ] || fail "First run needs --uo <folder with Ultima Online *.mul files>."
  say "Writing $CUO/settings.json"
  cat > "$CUO/settings.json" <<EOF
{
  "ip": "127.0.0.1",
  "port": 2593,
  "ultimaonlinedirectory": "$UO_DIR",
  "clientversion": "$CUO_CLIENT_VER",
  "lang": "ENG",
  "fps": 60,
  "saveaccount": false,
  "autologin": false,
  "reconnect": false,
  "encryption": $CUO_ENCRYPTION,
  "plugins": []
}
EOF
fi

mkdir -p "$INFUSION_HOME/scripts" "$INFUSION_HOME/profiles" "$INFUSION_HOME/logs"
if ! ls "$INFUSION_HOME/profiles/"*.profile >/dev/null 2>&1; then
  fail "No profile in $INFUSION_HOME/profiles. Copy your *.profile files from Infusion on Windows (see Infusion.Mac/README.md)."
fi

# 5) start
say "Starting Infusion..."
exec "$DOTNET" "$OUT/Infusion.Mac.dll" --root "$INFUSION_HOME" --client "$CUO/ClassicUO.bin.osx" "$@"
