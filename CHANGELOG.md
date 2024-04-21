# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Unreleased

### Added

- Enabled Dependency Injection for Model.

```csharp
  private readonly ILogger<Product> _logger;
  private readonly ILoggerFactory _loggerFactory;

  public Product(ILogger<Product> logger, ILoggerFactory loggerFactory)
  {
      _logger = logger;
      _loggerFactory = loggerFactory;
  }
```

### Changed

- The Model class is now required to be `partial`.
- Changed to store DTO created based on `public` Properties from the Model rather than the Model it's self.
- **[Breaking Change]** Replaced interface based hooks with `virtual` methods based hooks.

```csharp
    public virtual TDto OnBeforeCreate(TDto dto) => dto;
    public virtual IEnumerable<TDto> OnBeforeAllRead(IEnumerable<TDto> dtos) => dtos;
    public virtual TDto OnBeforeRead(TDto dto) => dto;
    public virtual TDto OnBeforeUpdate(TDto dto) => dto;
    public virtual TDto OnBeforeDelete(TDto dto) => dto;
```

- Changed API to return DTOs rather than the Model it's self which can be modified using Hooks.

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
