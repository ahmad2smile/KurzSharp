# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.9] - 2023-11-27

### Added

- Added ability to hook into process before changes happen to Entity with following interfaces:
  - `IBeforeReadHook`
  - `IBeforeCreateHook`
  - `IBeforeDeleteHook`
  - `IBeforeUpdateHook`

## [1.0.8] - 2023-11-22

### Internal

- Upgrade to .NET 8
