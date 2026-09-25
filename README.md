# DotnetManager

DotnetManager is a native command-line tool for discovering and installing .NET SDKs and runtimes from Microsoft's official release metadata. It selects the correct artifact for the current platform, verifies its SHA-512 hash, and configures the installed `dotnet` executable for the current user.

> The project is under active development. Browsing, listing, and installation are implemented; `update` and `remove` are currently reserved for future releases.

## Quick install

Linux and macOS on x64 and ARM64 are supported. The installer does not need `sudo` or an existing .NET installation:

```sh
curl -fsSL https://raw.githubusercontent.com/axies20/DotnetManager/master/install.sh | sh
```

The utility is stored in `~/.dotnet-manager`, with a command symlink at `~/.local/bin/dnm`. If `~/.local/bin` is not already in `PATH`, the installer adds a clearly marked block to the active Bash, Zsh, or Fish configuration file. Restart the shell after the first installation if the command is not found immediately.

Check the installation:

```sh
dnm --help
```

## What it can do

- Show supported .NET channels and the releases available in a channel.
- Discover SDKs, runtimes, and hosts already installed on the machine.
- Install an exact .NET release or select the latest eligible release.
- Install the SDK, .NET runtime, ASP.NET Core runtime, or both runtimes.
- Select an exact runtime identifier (RID), with no silent fallback to another platform.
- Verify the DotnetManager release archive with SHA-256 and every downloaded .NET artifact with the SHA-512 hash from Microsoft metadata.

## Commands

### Browse available releases

Show all available .NET channels:

```sh
dnm available
```

Show releases in one channel:

```sh
dnm available 10.0
```

### List installed .NET components

Show every discovered SDK, runtime, and host:

```sh
dnm list
```

Limit the output to one or more component types:

```sh
dnm list --sdk
dnm list --runtime
dnm list --host
```

### Install an exact release

With no component option, DotnetManager installs the SDK associated with the selected .NET release:

```sh
dnm install 10.0.12
```

Install runtimes instead:

```sh
dnm install 10.0.12 --runtime
dnm install 10.0.12 --aspnet
dnm install 10.0.12 --runtime --aspnet
```

An exact version refers to the .NET **release version**, not directly to an SDK version. For example, a release such as `10.0.12` may contain an SDK such as `10.0.401`.

### Install the latest release

By default, `latest` selects a security release and installs its SDK:

```sh
dnm install latest
```

Filter by release type or support phase:

```sh
dnm install latest --release-type Lts
dnm install latest --release-type Sts
dnm install latest --support-phase Active
```

Allow non-security releases when no security-only selection is required:

```sh
dnm install latest --include-non-security
```

All installation forms accept an explicit RID:

```sh
dnm install latest --rid linux-arm64
```

Use `dnm install latest --help` to see the values accepted by the current version.

## How installation works

```text
Microsoft release metadata
          │
          ▼
 channel and release selection
          │
          ▼
 exact component + RID artifact
          │
          ▼
 download → SHA-512 verification → extraction → PATH configuration
```

The downloaded .NET SDK/runtime is separate from the DotnetManager utility itself:

| Purpose | Non-root location | Root location |
| --- | --- | --- |
| DotnetManager utility | `~/.dotnet-manager` | same; the bootstrap installer is user-scoped |
| Managed .NET installation | `~/.dotnet` | `/usr/local/share/dotnet` |
| `dotnet` command entry point | shell configuration | `/usr/local/bin/dotnet` |

Running DotnetManager without root privileges is recommended. For a user installation it configures `DOTNET_ROOT` and `PATH` in the detected shell configuration. Root installation is intended for an explicitly system-wide .NET installation.

## Uninstall DotnetManager

```sh
curl -fsSL https://raw.githubusercontent.com/axies20/DotnetManager/master/uninstall.sh | sh
```

The uninstall script removes only the managed `~/.dotnet-manager` directory, its `~/.local/bin/dnm` symlink, and the marked PATH block created by the bootstrap installer. It does **not** remove SDKs or runtimes installed by DotnetManager.

## Install a specific DotnetManager release

Set an explicit release archive URL when reproducibility matters:

```sh
curl -fsSL https://raw.githubusercontent.com/axies20/DotnetManager/master/install.sh | \
  DOTNET_MANAGER_DOWNLOAD_URL=https://github.com/axies20/DotnetManager/releases/download/v0.1.0/dotnet-manager-linux-x64.tar.gz sh
```

## Build from source

The repository targets .NET 10 and publishes as a Native AOT executable:

```sh
git clone https://github.com/axies20/DotnetManager.git
cd DotnetManager
dotnet build
dotnet run --project DotnetManager -- --help
```

Create a native build for the current machine:

```sh
dotnet publish DotnetManager/DotnetManager.csproj \
  --configuration Release \
  --runtime linux-x64 \
  --self-contained true
```

## Tests

Run the isolated unit test suite:

```sh
dotnet test Tests/DotnetManager.UnitTests
```

The full installation scenario has its own opt-in xUnit project. It creates a fresh Podman container, copies the repository into it, installs an SDK entirely inside the container, verifies the resulting `dotnet` executable, and removes the container in `finally`:

```sh
DOTNET_MANAGER_RUN_CONTAINER_TESTS=1 \
  dotnet test Tests/DotnetManager.IntegrationTests
```

Without `DOTNET_MANAGER_RUN_CONTAINER_TESTS=1`, this test is reported as skipped. It never writes to the host's `~/.dotnet`, shell configuration, or system .NET directories.

## Release process

Pushing a tag whose name starts with `v` builds native archives for all supported platforms and attaches them to a GitHub release:

```sh
git tag v0.1.0
git push origin v0.1.0
```

After the first release is published, the quick-install command automatically downloads its matching platform archive.
