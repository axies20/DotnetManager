# DotnetManager

DotnetManager (`dnm`) is a native Unix-focused command-line tool for browsing official Microsoft
.NET release metadata, discovering local installations, and installing or removing .NET SDKs and
runtimes for the current user.

The project is under active development. Its current command status is:

| Command | Status | Notes |
| --- | --- | --- |
| `available` | Implemented | Shows channels or the releases in one channel. |
| `list` | Implemented | Discovers SDKs, runtimes, and hosts from known .NET roots. |
| `install` | Implemented | Installs an exact release or the newest release matching channel filters. |
| `remove` | Implemented, experimental | Deletes matching directories from discovered .NET roots; read the safety note below. |
| `update` | Not implemented | Prints a warning and exits with code `1`. |

## Quick install

The bootstrap installer supports Linux and macOS on x64 and ARM64. It does not require `sudo` or
an existing .NET installation:

```sh
curl -fsSL https://raw.githubusercontent.com/axies20/DotnetManager/master/install.sh | sh
```

The script downloads the latest matching Native AOT archive, verifies its SHA-256 checksum, and
installs the executable at `~/.dotnet-manager/bin/dnm`. It creates a symlink at
`~/.local/bin/dnm` and, when necessary, adds that directory to the active Bash, Zsh, or Fish
configuration.

Restart the shell after the first installation if `dnm` is not found immediately, then verify it:

```sh
dnm --help
```

## Browse available releases

Show all channels from Microsoft's release index:

```sh
dnm available
```

Show the releases and SDK feature bands in one channel:

```sh
dnm available 10.0
```

The `Security` column is informational. Installation does not prefer or filter releases by that
flag.

## List installed components

With no options, `list` shows every discovered SDK, runtime, and host:

```sh
dnm list
```

Limit the output to selected component categories:

```sh
dnm list --sdk
dnm list --runtime
dnm list --host
dnm list --sdk --runtime
```

Discovery combines `DOTNET_ROOT*` variables, `dotnet` executables found through `PATH`, and Unix
`/etc/dotnet/install_location*` files. Duplicate and missing roots are filtered out.

## Install .NET

DotnetManager installs into `~/.dotnet` and must be run as a regular user.

### Install an exact release

With no component option, the SDK associated with the selected .NET release is installed:

```sh
dnm install 10.0.12
```

Install one or both runtime packages instead:

```sh
dnm install 10.0.12 --runtime
dnm install 10.0.12 --aspnet
dnm install 10.0.12 --runtime --aspnet
```

The positional version is a .NET **release version**, not an SDK feature-band version. For
example, release `10.0.12` may contain SDK `10.0.401`.

### Install the latest matching release

`latest` selects the newest release from the newest channel matching the optional release-type and
support-phase filters:

```sh
dnm install latest
dnm install latest --release-type Lts
dnm install latest --release-type Sts
dnm install latest --support-phase Active
```

All installation forms accept an explicit runtime identifier. Artifact selection requires an exact
RID match and does not silently fall back to another platform:

```sh
dnm install latest --rid linux-arm64
```

Run `dnm install latest --help` to see the enum values accepted by the current build.

### Environment configuration

After extraction, DotnetManager verifies that `~/.dotnet/dotnet` exists and configures every
supported environment it detects:

| Environment | Generated file |
| --- | --- |
| Fish found through `PATH` | `$XDG_CONFIG_HOME/fish/conf.d/dnm.fish`, or `~/.config/fish/conf.d/dnm.fish` when unset |
| Oh My Zsh | `$ZSH_CUSTOM/60-dnm.zsh`, `$ZSH/custom/60-dnm.zsh`, or `~/.oh-my-zsh/custom/60-dnm.zsh` |
| Linux with systemd | `$XDG_CONFIG_HOME/environment.d/60-dnm.conf`, or `~/.config/environment.d/60-dnm.conf` when unset |

These files configure `DOTNET_ROOT`, `~/.dotnet`, and `~/.dotnet/tools`. If no supported target is
detected, installation completes with a warning instead of modifying an unrelated shell file.

### Installation pipeline and current limitations

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
download → SHA-512 verification → extraction into ~/.dotnet → environment configuration
```

The current installer extracts directly into the live `~/.dotnet` tree. It does not yet maintain an
ownership database, skip already installed versions, lock concurrent operations, stage publication,
or roll back a partially completed multi-component installation. Repeating an install downloads and
extracts the selected artifacts again.

## Remove installed components

The version selector accepts a major version, major/minor version, or an exact version:

```sh
dnm remove 10
dnm remove 10.0
dnm remove 10.0.12
```

Removal matches the versions of installed components, not a Microsoft release bundle. For example,
an SDK may be version `10.0.401` while its bundled runtime is version `10.0.12`.

With no component option, all matching SDK, .NET runtime, ASP.NET Core runtime, and host directories
are removed. Options restrict removal to selected categories:

```sh
dnm remove 10 --sdk
dnm remove 10 --runtime
dnm remove 10 --aspnet
dnm remove 10 --host
dnm remove 10 --sdk --runtime
```

> **Safety:** removal currently operates on installations discovered from `PATH`, `DOTNET_ROOT*`,
> and `/etc/dotnet/install_location*`. It does not track ownership and therefore cannot distinguish
> installations created by DotnetManager from other user-accessible installations. Review the
> selected version and component options carefully before running it. Deletion starts immediately;
> there is currently no confirmation or dry-run mode.

Major and major/minor selectors include prerelease versions in their range. An exact selector only
matches that exact NuGet version.

## Update

The command is reserved but not implemented yet:

```sh
dnm update
```

It prints a warning and returns exit code `1`; it does not modify installed components.

## Installed locations

The DotnetManager executable and its .NET installation target are separate:

| Purpose | User location |
| --- | --- |
| DotnetManager executable | `~/.dotnet-manager/bin/dnm` |
| Command symlink | `~/.local/bin/dnm` |
| .NET SDKs and runtimes installed by `dnm install` | `~/.dotnet` |
| .NET global tools | `~/.dotnet/tools` |

## Uninstall DotnetManager

```sh
curl -fsSL https://raw.githubusercontent.com/axies20/DotnetManager/master/uninstall.sh | sh
```

The uninstall script removes `~/.dotnet-manager`, its managed `~/.local/bin/dnm` symlink, and the
marked bootstrap PATH block. It deliberately leaves `~/.dotnet`, installed SDKs/runtimes, and the
.NET environment files listed above intact.

## Install a specific DotnetManager release

Override both URLs when pinning an archive outside the default latest-release flow:

```sh
curl -fsSL https://raw.githubusercontent.com/axies20/DotnetManager/master/install.sh | \
  DOTNET_MANAGER_DOWNLOAD_URL=https://github.com/axies20/DotnetManager/releases/download/v0.1.0/dotnet-manager-linux-x64.tar.gz \
  DOTNET_MANAGER_CHECKSUM_URL=https://github.com/axies20/DotnetManager/releases/download/v0.1.0/dotnet-manager-linux-x64.tar.gz.sha256 \
  sh
```

## Repository structure

| Project | Responsibility |
| --- | --- |
| `DotnetManager` | CLI commands, composition root, console output, and top-level error handling. |
| `DotnetManager.Core` | Shared paths, endpoints, application information, and component types. |
| `DotnetManager.ReleaseMetadata` | Microsoft JSON contracts, mapping, models, and HTTP retrieval. |
| `DotnetManager.Installation` | Release selection, downloading, checksum verification, extraction, and finalization. |
| `DotnetManager.InstalledDotnet` | Root discovery and installed SDK/runtime/host enumeration. |
| `DotnetManager.Removal` | Version matching and directory removal. |
| `DotnetManager.UserEnvironment` | Fish, Oh My Zsh, and `environment.d` configuration. |

## Build from source

The repository targets .NET 10 and pins its SDK policy in `global.json`:

```sh
git clone https://github.com/axies20/DotnetManager.git
cd DotnetManager
dotnet restore DotnetManager.slnx
dotnet build DotnetManager.slnx --no-restore
dotnet run --project DotnetManager --no-build -- --help
```

Publish a self-contained Native AOT executable for Linux x64:

```sh
dotnet publish DotnetManager/DotnetManager.csproj \
  --configuration Release \
  --runtime linux-x64 \
  --self-contained true
```

Release builds also target `linux-arm64`, `osx-x64`, and `osx-arm64`.

## Tests

Run the isolated unit suite:

```sh
dotnet test Tests/DotnetManager.UnitTests/DotnetManager.UnitTests.csproj
```

The opt-in integration test creates a fresh Podman container, creates a non-root `dnm` user, copies
the repository into the container, installs the latest Linux x64 SDK into `/home/dnm/.dotnet`, runs
the installed `dotnet --info`, and removes the container in `finally`:

```sh
DOTNET_MANAGER_RUN_CONTAINER_TESTS=1 \
  dotnet test Tests/DotnetManager.IntegrationTests/DotnetManager.IntegrationTests.csproj
```

Without `DOTNET_MANAGER_RUN_CONTAINER_TESTS=1`, the integration test is reported as skipped. It does
not write to the host's home directory or system .NET locations.

GitHub Actions separately runs formatting, build, unit coverage, CodeQL, the scheduled container
integration test, and a Native AOT smoke test.

## Release process

Pushing a tag whose name starts with `v` verifies the solution, publishes archives for all supported
RIDs, generates SHA-256 files, and creates a GitHub release:

```sh
git tag v0.1.0
git push origin v0.1.0
```
