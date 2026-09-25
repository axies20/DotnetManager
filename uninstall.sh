#!/usr/bin/env sh

set -eu

INSTALL_DIR="${DOTNET_MANAGER_INSTALL_DIR:-$HOME/.dotnet-manager}"
BIN_DIR="${DOTNET_MANAGER_BIN_DIR:-$HOME/.local/bin}"
LINK="$BIN_DIR/dnm"
EXECUTABLE="$INSTALL_DIR/bin/dnm"
PATH_MARKER_START="# >>> DotnetManager CLI >>>"
PATH_MARKER_END="# <<< DotnetManager CLI <<<"

case "$INSTALL_DIR" in
    ""|/|.|"$HOME")
        printf 'Refusing to remove unsafe installation directory: %s\n' "$INSTALL_DIR" >&2
        exit 1
        ;;
esac

remove_path_block() {
    config=$1
    [ -f "$config" ] || return 0
    grep -F "$PATH_MARKER_START" "$config" >/dev/null 2>&1 || return 0

    temporary_file=$(mktemp "${config}.XXXXXX")
    cp -p "$config" "$temporary_file"
    awk -v start="$PATH_MARKER_START" -v end="$PATH_MARKER_END" '
        $0 == start { removing = 1; next }
        $0 == end && removing { removing = 0; next }
        !removing { print }
    ' "$config" > "$temporary_file"
    mv "$temporary_file" "$config"
}

if [ -L "$LINK" ]; then
    link_target=$(readlink "$LINK")
    if [ "$link_target" = "$EXECUTABLE" ]; then
        rm "$LINK"
    else
        printf 'Keeping %s because it points outside the managed installation.\n' "$LINK"
    fi
fi

if [ -d "$INSTALL_DIR" ]; then
    rm -rf "$INSTALL_DIR"
fi

remove_path_block "$HOME/.bashrc"
remove_path_block "$HOME/.zshrc"
remove_path_block "$HOME/.config/fish/config.fish"

printf 'DotnetManager was removed. Restart the shell to refresh PATH.\n'
