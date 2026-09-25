#!/usr/bin/env sh

set -eu

REPOSITORY="${DOTNET_MANAGER_REPOSITORY:-axies20/DotnetManager}"
INSTALL_DIR="${DOTNET_MANAGER_INSTALL_DIR:-$HOME/.dotnet-manager}"
BIN_DIR="${DOTNET_MANAGER_BIN_DIR:-$HOME/.local/bin}"
EXECUTABLE="$INSTALL_DIR/bin/dnm"
LINK="$BIN_DIR/dnm"
PATH_MARKER_START="# >>> DotnetManager CLI >>>"
PATH_MARKER_END="# <<< DotnetManager CLI <<<"

fail() {
    printf 'DotnetManager installation failed: %s\n' "$1" >&2
    exit 1
}

detect_rid() {
    os=$(uname -s)
    arch=$(uname -m)

    case "$os" in
        Linux) os=linux ;;
        Darwin) os=osx ;;
        *) fail "unsupported operating system: $os" ;;
    esac

    case "$arch" in
        x86_64|amd64) arch=x64 ;;
        arm64|aarch64) arch=arm64 ;;
        *) fail "unsupported architecture: $arch" ;;
    esac

    printf '%s-%s\n' "$os" "$arch"
}

download() {
    url=$1
    destination=$2

    if [ "${DOTNET_MANAGER_ALLOW_INSECURE:-0}" = "1" ]; then
        curl_protocol_options=""
    else
        curl_protocol_options="--proto =https --tlsv1.2"
    fi

    if command -v curl >/dev/null 2>&1; then
        # shellcheck disable=SC2086
        curl -fL --retry 3 $curl_protocol_options "$url" -o "$destination"
    elif command -v wget >/dev/null 2>&1; then
        wget --https-only --tries=3 -O "$destination" "$url"
    else
        fail "curl or wget is required"
    fi
}

verify_archive() {
    archive=$1
    checksum_file=$2

    expected=$(awk '{ print $1; exit }' "$checksum_file")
    [ -n "$expected" ] || fail "the release checksum is empty"

    if command -v sha256sum >/dev/null 2>&1; then
        actual=$(sha256sum "$archive" | awk '{ print $1 }')
    elif command -v shasum >/dev/null 2>&1; then
        actual=$(shasum -a 256 "$archive" | awk '{ print $1 }')
    else
        fail "sha256sum or shasum is required"
    fi

    [ "$actual" = "$expected" ] || fail "SHA-256 verification failed for the release archive"
}

configure_path() {
    case "${SHELL:-}" in
        */zsh) shell_config="$HOME/.zshrc" ;;
        */bash) shell_config="$HOME/.bashrc" ;;
        */fish) shell_config="$HOME/.config/fish/config.fish" ;;
        *) shell_config="" ;;
    esac

    [ -n "$shell_config" ] || return 0
    grep -F "$PATH_MARKER_START" "$shell_config" >/dev/null 2>&1 && return 0

    mkdir -p "$(dirname "$shell_config")"

    if [ "${SHELL##*/}" = "fish" ]; then
        path_line="fish_add_path --prepend \"$BIN_DIR\""
    else
        path_line="export PATH=\"$BIN_DIR:\$PATH\""
    fi

    {
        printf '\n%s\n' "$PATH_MARKER_START"
        printf '%s\n' "$path_line"
        printf '%s\n' "$PATH_MARKER_END"
    } >> "$shell_config"

    printf 'Added %s to PATH in %s.\n' "$BIN_DIR" "$shell_config"
}

rid=$(detect_rid)
archive_name="dotnet-manager-$rid.tar.gz"
download_url="${DOTNET_MANAGER_DOWNLOAD_URL:-https://github.com/$REPOSITORY/releases/latest/download/$archive_name}"
checksum_url="${DOTNET_MANAGER_CHECKSUM_URL:-$download_url.sha256}"
temporary_directory=$(mktemp -d "${TMPDIR:-/tmp}/dotnet-manager.XXXXXX")
trap 'rm -rf "$temporary_directory"' EXIT HUP INT TERM
archive="$temporary_directory/$archive_name"
checksum_file="$archive.sha256"
staging="$temporary_directory/staging"

printf 'Installing DotnetManager for %s...\n' "$rid"
download "$download_url" "$archive"
download "$checksum_url" "$checksum_file"
verify_archive "$archive" "$checksum_file"
mkdir -p "$staging"
tar -xzf "$archive" -C "$staging"

[ -f "$staging/dnm" ] || fail "the release archive does not contain dnm"

if [ -e "$LINK" ] && [ ! -L "$LINK" ]; then
    fail "$LINK already exists and is not a symbolic link"
fi

mkdir -p "$INSTALL_DIR/bin" "$BIN_DIR"
install -m 755 "$staging/dnm" "$EXECUTABLE"

if [ -f "$staging/appsettings.json" ]; then
    install -m 644 "$staging/appsettings.json" "$INSTALL_DIR/bin/appsettings.json"
fi

ln -sfn "$EXECUTABLE" "$LINK"
configure_path

printf '\nDotnetManager was installed in %s.\n' "$INSTALL_DIR"
printf 'Run: dnm --help\n'

case ":${PATH:-}:" in
    *:"$BIN_DIR":*) ;;
    *) printf 'Restart your shell or run: export PATH="%s:$PATH"\n' "$BIN_DIR" ;;
esac
