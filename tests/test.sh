#!/usr/bin/env bash
set -Eeuo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
test_root="$(mktemp -d)"
trap 'rm -rf -- "$test_root"' EXIT INT TERM

mkdir -p \
  "$test_root/metadata/10.0" \
  "$test_root/metadata/11.0" \
  "$test_root/config" \
  "$test_root/bin" \
  "$test_root/install-parent"

cat >"$test_root/metadata/releases-index.json" <<'JSON'
{
  "releases-index": [
    {"channel-version":"11.0","release-type":"sts","support-phase":"go-live"},
    {"channel-version":"10.0","release-type":"lts","support-phase":"active"}
  ]
}
JSON

cat >"$test_root/metadata/10.0/releases.json" <<'JSON'
{
  "releases": [
    {
      "release-date":"2026-09-08",
      "release-version":"10.0.12",
      "security":true,
      "sdk":{"version":"10.0.401","version-display":"10.0.401"}
    }
  ]
}
JSON

cat >"$test_root/metadata/11.0/releases.json" <<'JSON'
{
  "releases": [
    {
      "release-date":"2026-09-08",
      "release-version":"11.0.0-rc.1",
      "security":true,
      "sdk":{"version":"11.0.100-rc.1.26425.128","version-display":"11.0.100-rc.1"}
    },
    {
      "release-date":"2026-08-11",
      "release-version":"11.0.0-preview.7",
      "security":false,
      "sdk":{"version":"11.0.100-preview.7.26375.9","version-display":"11.0.100-preview.7"}
    }
  ]
}
JSON

cat >"$test_root/fake-dotnet-install.sh" <<'SCRIPT'
#!/usr/bin/env bash
set -Eeuo pipefail
version=""
install_dir=""
while [ "$#" -gt 0 ]; do
  case "$1" in
    --version) version="$2"; shift 2 ;;
    --install-dir) install_dir="$2"; shift 2 ;;
    *) shift ;;
  esac
done
mkdir -p "$install_dir/sdk/$version"
cat >"$install_dir/dotnet" <<'DOTNET'
#!/usr/bin/env bash
set -Eeuo pipefail
root="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
case "${1:-}" in
  --list-sdks)
    for sdk_dir in "$root"/sdk/*; do
      [ -d "$sdk_dir" ] || continue
      printf '%s [%s/sdk]\n' "$(basename "$sdk_dir")" "$root"
    done
    ;;
  --version)
    find "$root/sdk" -mindepth 1 -maxdepth 1 -type d -printf '%f\n' | sort -V | tail -1
    ;;
  *) exit 0 ;;
esac
DOTNET
chmod 0755 "$install_dir/dotnet"
SCRIPT
chmod 0755 "$test_root/fake-dotnet-install.sh"

export DOTNET_MANAGER_ALLOW_NON_ROOT=1
export DOTNET_MANAGER_INSTALL_ROOT="$test_root/install-parent/dotnet"
export DOTNET_MANAGER_CONFIG_DIR="$test_root/config"
export DOTNET_MANAGER_CONFIG_FILE="$test_root/config/sources.conf"
export DOTNET_MANAGER_LOCK_FILE="$test_root/dotnet-manager.lock"
export DOTNET_MANAGER_METADATA_BASE="file://$test_root/metadata"
export DOTNET_MANAGER_INSTALLER_URL="file://$test_root/fake-dotnet-install.sh"
export DOTNET_MANAGER_DOTNET_LINK="$test_root/bin/dotnet"

manager="$repo_root/bin/dotnet-manager"

"$manager" update
grep -Fxq $'channel\t10.0\tga\t10.0.401' \
  "$DOTNET_MANAGER_INSTALL_ROOT/.dotnet-manager-manifest"

"$manager" install --channel 11.0 --policy go-live
installed_sdks="$("$DOTNET_MANAGER_INSTALL_ROOT/dotnet" --list-sdks)"
grep -Fq '10.0.401' <<<"$installed_sdks"
grep -Fq '11.0.100-rc.1.26425.128' <<<"$installed_sdks"

"$manager" install --version 11.0.100-preview.7
installed_sdks="$("$DOTNET_MANAGER_INSTALL_ROOT/dotnet" --list-sdks)"
grep -Fq '11.0.100-preview.7.26375.9' <<<"$installed_sdks"

"$manager" remove --version 11.0.100-preview.7
installed_sdks="$("$DOTNET_MANAGER_INSTALL_ROOT/dotnet" --list-sdks)"
if grep -Fq '11.0.100-preview.7.26375.9' <<<"$installed_sdks"; then
  echo "ERROR: pinned preview SDK was not removed" >&2
  exit 1
fi

output="$("$manager" update)"
grep -Fq 'All configured SDK channels are current.' <<<"$output"
output="$("$manager" available STS)"
grep -Fq '11.0.100-rc.1.26425.128' <<<"$output"
output="$("$manager" list)"
grep -Fq 'channel 11.0 go-live' <<<"$output"

if "$manager" install --channel 11.0 --version 10.0.401 >/dev/null 2>&1; then
  echo "ERROR: conflicting install selectors were accepted" >&2
  exit 1
fi
if "$manager" remove --version 9.0.100 >/dev/null 2>&1; then
  echo "ERROR: removal of an unconfigured version was accepted" >&2
  exit 1
fi

echo "All DotnetManager tests passed."
