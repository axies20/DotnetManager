#!/usr/bin/env bash
set -Eeuo pipefail

root="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

if [ "$EUID" -eq 0 ]; then
  echo "ERROR: Run ./install.sh as your normal user; it will invoke sudo." >&2
  exit 1
fi

for command_name in curl jq flock column tar gzip sudo; do
  if ! command -v "$command_name" >/dev/null 2>&1; then
    echo "ERROR: Required command is missing: $command_name" >&2
    echo "On Fedora: sudo dnf install curl jq util-linux" >&2
    exit 1
  fi
done

echo "Installing DotnetManager system-wide..."
if systemctl cat dotnet-sdk-update.timer >/dev/null 2>&1; then
  echo "Disabling the legacy dotnet-sdk-update.timer to prevent conflicting updates..."
  sudo systemctl disable --now dotnet-sdk-update.timer
fi

sudo install -Dm755 "$root/bin/dotnet-manager" /usr/local/bin/dotnet-manager
sudo install -d -m755 /etc/dotnet-manager
if [ ! -f /etc/dotnet-manager/sources.conf ]; then
  sudo install -m644 "$root/config/sources.conf" /etc/dotnet-manager/sources.conf
fi
sudo install -Dm644 \
  "$root/systemd/dotnet-manager-update.service" \
  /etc/systemd/system/dotnet-manager-update.service
sudo install -Dm644 \
  "$root/systemd/dotnet-manager-update.timer" \
  /etc/systemd/system/dotnet-manager-update.timer

sudo systemctl daemon-reload
sudo dotnet-manager update
sudo systemctl enable --now dotnet-manager-update.timer

echo
echo "DotnetManager installed successfully."
echo "Run: dotnet-manager list"
