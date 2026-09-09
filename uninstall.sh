#!/usr/bin/env bash
set -Eeuo pipefail

if [ "$EUID" -eq 0 ]; then
  echo "ERROR: Run ./uninstall.sh as your normal user; it will invoke sudo." >&2
  exit 1
fi

sudo systemctl disable --now dotnet-manager-update.timer 2>/dev/null || true
sudo rm -f \
  /etc/systemd/system/dotnet-manager-update.timer \
  /etc/systemd/system/dotnet-manager-update.service \
  /usr/local/share/zsh/site-functions/_dotnet-manager \
  /usr/local/bin/dotnet-manager
sudo systemctl daemon-reload

echo "DotnetManager was removed."
echo "The SDK installation and /etc/dotnet-manager/sources.conf were preserved."
echo "Remove them manually only if you no longer need the installed SDKs."
