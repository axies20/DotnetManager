# DotnetManager

A convenient system-wide Microsoft .NET SDK manager for Linux.

DotnetManager lets you install an SDK, subscribe to update channels, and keep
multiple major .NET versions side by side with a single command. It uses only
Microsoft's official release metadata and the official `dotnet-install.sh`
installer.

```text
sources.conf                 Microsoft release metadata
     │                                  │
     └──────────┐             ┌─────────┘
                ▼             ▼
              dotnet-manager update
                        │
                 staging + validation
                        │
                        ▼
              /usr/local/share/dotnet
                  ├── sdk/8.0.x
                  ├── sdk/10.0.x
                  └── sdk/11.0.x
```

## Features

- Install the latest SDK from any published version channel, including `8.0`,
  `9.0`, `10.0`, and `11.0`.
- Follow the moving `LTS` and `STS` aliases.
- Choose between `ga`, `go-live`, and `preview` release policies.
- Pin any specific SDK version published in Microsoft release metadata.
- Keep multiple SDK versions in one system-wide installation.
- Update automatically every week with a systemd timer.
- Rebuild the managed SDK set cleanly without accumulating obsolete patches.
- Stage, validate, and atomically activate every update with automatic rollback.
- Use one `dotnet` executable from the terminal, Rider, and other IDEs.
- Complete commands, options, channels, policies, and configured sources in Zsh
  and Oh My Zsh by pressing `Tab`.

## Quick start

DotnetManager requires `bash`, `curl`, `jq`, `util-linux`, `tar`, and `gzip`.

On Fedora:

```bash
sudo dnf install curl jq util-linux tar gzip
git clone <repository-url>
cd DotnetManager
./install.sh
```

The installer will:

1. Install the CLI as `/usr/local/bin/dotnet-manager`.
2. Create `/etc/dotnet-manager/sources.conf`.
3. Install the SDK set defined by the initial configuration.
4. Install and enable the weekly systemd update timer.
5. Create `/usr/local/bin/dotnet`.

If the legacy `dotnet-sdk-update.timer` is present, the installer disables it
to prevent two independent updaters from replacing the same .NET directory.

The default configuration follows the latest stable .NET 10 SDK:

```text
channel 10.0 ga
```

## Zsh and Oh My Zsh completion

`./install.sh` installs the completion definition into the standard Zsh path:

```text
/usr/local/share/zsh/site-functions/_dotnet-manager
```

This path is part of the default `fpath` on supported Linux distributions. Oh
My Zsh initializes the same native Zsh completion system, so no additional Oh
My Zsh plugin or `plugins=(...)` entry is required.

Restart the shell after installation:

```bash
exec zsh
```

You can then type commands such as the following and press `Tab`:

```bash
dotnet-manager <Tab>
dotnet-manager install --<Tab>
dotnet-manager install --policy <Tab>
dotnet-manager remove --channel <Tab>
```

Channel removal suggestions are read from
`/etc/dotnet-manager/sources.conf`, so completion only offers channels that are
actually configured.

## Commands

Show configured sources and installed SDKs:

```bash
dotnet-manager list
```

List every SDK published in a channel:

```bash
dotnet-manager available 11.0
```

Install the latest stable SDK from a channel:

```bash
sudo dotnet-manager install --channel 10.0
```

Install .NET 11 starting with a supported Go Live release candidate:

```bash
sudo dotnet-manager install --channel 11.0 --policy go-live
```

Install the latest preview:

```bash
sudo dotnet-manager install --channel 11.0 --policy preview
```

Install a specific published SDK version:

```bash
sudo dotnet-manager install --version 11.0.100-rc.1
```

You can pass either a display version such as `11.0.100-rc.1` or the complete
version with its build suffix. DotnetManager resolves the exact artifact for
you.

Update all tracked channels immediately:

```bash
sudo dotnet-manager update
```

Rebuild the installation even when the resolved versions have not changed:

```bash
sudo dotnet-manager update --force
```

Remove a tracked channel or pinned version:

```bash
sudo dotnet-manager remove --channel 10.0
sudo dotnet-manager remove --version 11.0.100-rc.1
```

DotnetManager refuses to remove the final SDK source accidentally. Add another
channel or pinned version first.

## Release policies

| Policy | Accepted releases | Typical use |
|---|---|---|
| `ga` | Stable GA releases only | Production and everyday development |
| `go-live` | Go Live RC releases and subsequent GA releases | Early adoption of a supported RC |
| `preview` | Preview, RC, and GA releases | Testing an upcoming .NET version |

The default policy is the conservative `ga` option.

The following command waits until .NET 11 reaches GA and does not install a
preview or RC build:

```bash
sudo dotnet-manager install --channel 11.0 --policy ga
```

To use an RC that Microsoft has marked as Go Live, run:

```bash
sudo dotnet-manager install --channel 11.0 --policy go-live
```

When the channel reaches GA, the same source automatically advances from the
release candidate to the stable SDK.

## LTS and STS channels

You can follow a support track instead of a numbered channel:

```bash
sudo dotnet-manager install --channel LTS --policy ga
sudo dotnet-manager install --channel STS --policy ga
```

`LTS` and `STS` are moving aliases. When Microsoft publishes a new matching
channel, a future update may move to a new major version. Use a numbered channel
such as `10.0` or `11.0` when major-version upgrades must remain explicit.

## Configuration

SDK sources are stored in `/etc/dotnet-manager/sources.conf`:

```text
# Latest stable .NET 10 SDK
channel 10.0 ga

# Latest Go Live or GA .NET 11 SDK
channel 11.0 go-live

# An additional SDK pinned indefinitely
version 8.0.408
```

The CLI updates this file automatically. You may also edit it manually and then
apply the new configuration with:

```bash
sudo dotnet-manager update
```

## How updates work

DotnetManager never extracts a new SDK directly over the active installation.

1. It downloads Microsoft release metadata for every configured source.
2. Each source and policy is resolved to an exact full SDK version.
3. All required SDKs are installed into a fresh staging directory.
4. Every SDK is verified with `dotnet --list-sdks`.
5. The staged directory atomically becomes `/usr/local/share/dotnet`.
6. If final validation fails, the previous installation is restored.

This approach keeps only the current SDK from each tracked channel while
preserving every explicitly pinned version.

## Automatic updates

The installer enables `dotnet-manager-update.timer`. Inspect it with:

```bash
systemctl status dotnet-manager-update.timer
systemctl list-timers dotnet-manager-update.timer
```

Start an update through systemd immediately:

```bash
sudo systemctl start dotnet-manager-update.service
```

Read the update log:

```bash
journalctl -u dotnet-manager-update.service
```

The timer uses `Persistent=true`, so a missed update runs after the machine is
started again.

## Project SDK selection

Inspect the installed SDKs:

```bash
dotnet --list-sdks
dotnet --info
```

A project can select an SDK with `global.json`:

```json
{
  "sdk": {
    "version": "11.0.100-rc.1.26425.128",
    "rollForward": "latestPatch",
    "allowPrerelease": true
  }
}
```

If a project requires an older exact SDK, pin it in DotnetManager:

```bash
sudo dotnet-manager install --version 8.0.408
```

## Rider

Configure Rider to use:

```text
.NET CLI executable path: /usr/local/bin/dotnet
```

You can disable Rider's automatic SDK download to prevent it from creating a
second installation under `~/.dotnet`.

## Files and directories

| Purpose | Path |
|---|---|
| Manager CLI | `/usr/local/bin/dotnet-manager` |
| Zsh completion | `/usr/local/share/zsh/site-functions/_dotnet-manager` |
| .NET CLI | `/usr/local/bin/dotnet` |
| SDKs and runtimes | `/usr/local/share/dotnet` |
| Source configuration | `/etc/dotnet-manager/sources.conf` |
| Systemd service | `/etc/systemd/system/dotnet-manager-update.service` |
| Systemd timer | `/etc/systemd/system/dotnet-manager-update.timer` |

## Important limitations

- Do not mix this binary installation with `dotnet-sdk-*` RPM packages in the
  same system environment.
- A clean rebuild does not preserve separately installed .NET workloads. Install
  those workloads again after an SDK update.
- Global tools under `~/.dotnet/tools` are preserved because they live outside
  the system SDK directory.
- Resolving metadata and downloading SDKs requires access to Microsoft's
  servers.
- Preview releases are not supported for production. For early production use,
  choose only an RC that Microsoft explicitly marks as Go Live.

## Development and testing

Run syntax checks, optional ShellCheck analysis, and the isolated integration
test suite with:

```bash
make test
```

The integration test uses local metadata and a fake SDK installer. It never
changes `/etc`, `/usr/local`, or the machine's installed .NET environment.

GitHub Actions runs the same test suite and ShellCheck for every push and pull
request.

## Uninstalling DotnetManager

```bash
./uninstall.sh
```

The uninstall script removes the CLI and systemd units but deliberately keeps
the installed SDKs and `/etc/dotnet-manager/sources.conf`. This prevents an
accidental loss of a working development environment. Remove those files
manually only when you no longer need them.
