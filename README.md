<p align="center">
  <img src="style/banner.svg" alt="Extra Auth Settings banner" width="100%" />
</p>

<h1 align="center">Extra Auth Settings</h1>

<p align="center">
  Extra per-user self-service restrictions for Jellyfin.
</p>

<p align="center">
  <img alt="Jellyfin ABI" src="https://img.shields.io/badge/Jellyfin-10.11.0.0-00a4dc?style=for-the-badge" />
  <img alt=".NET" src="https://img.shields.io/badge/.NET-net9.0-512bd4?style=for-the-badge" />
  <img alt="Category" src="https://img.shields.io/badge/Category-Administration-101820?style=for-the-badge" />
</p>

## Overview

Extra Auth Settings adds a small set of admin-controlled restrictions for Jellyfin users. It is designed for servers where some accounts should not be able to change their own credentials or profile image without admin involvement.

## Installation

### From a plugin repository

Once this repository has a published release and `manifest` branch, add the repository manifest URL to Jellyfin:

```text
https://raw.githubusercontent.com/BananaBagel/jellyfin-ExtraAuthSettings/manifest/manifest.json
```

Then install **Extra Auth Settings** from the plugin catalog in Jellyfin.

### Manual installation

1. Download the latest release zip from this repository.
2. Extract the contents into a folder named `ExtraAuthSettings` under your Jellyfin plugins directory.
3. Restart Jellyfin.

## Features

- Disable self-service password changes per user
- Disable self-service profile picture changes per user
- Leave administrator accounts unaffected
- Manage settings from a simple plugin configuration page inside the Jellyfin dashboard

## Compatibility

- Jellyfin `10.11.0.0`
- .NET `net9.0`

## Configuration

After installation, open:

```text
Dashboard -> Plugins -> My Plugins -> ExtraAuthSettings
```

For each user, you can control:

- `Can change password`
- `Can change profile picture`

If either option is disabled, that user will receive a `403` when attempting the corresponding self-service action. Administrator accounts are still allowed.

## Development

Build locally with:

```bash
dotnet build ExtraAuthSettings.sln
```

Create a release package locally with:

```bash
dotnet publish ExtraAuthSettings/ExtraAuthSettings.csproj -c Release -o artifacts/publish
```

## License

This project is licensed under the terms of the [MIT License](LICENSE).
