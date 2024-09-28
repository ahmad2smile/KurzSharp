# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [4.1.0]

### Fixed

- Fixed `GraphQlApi` bulk mutation input parameter type and naming issues.

## [4.0.0]

### Internal

- Fixed build by disabling `OpenApiGenerateDocumentsOnBuild` for SourceGen projects.

### Added

- Added "GetById" method which would get item matched on `id`, where `id` could be any property name which contains `Id`
  or `Name`, or has `Key` Attribute.
    - GET `/Product{Rest|Grpc}/{id|name|anyDbKey}`
- Added new `API` methods for Bulk changes ex:
    - POST `/Product{Rest|Grpc}/bulk` also relevant `GraphQl` method
    - DELETE `/Product{Rest|Grpc}/bulk` also relevant `GraphQl` method
    - PUT `/Product{Rest|Grpc}/bulk` also relevant `GraphQl` method
- Added overload for `OnBeforeCreate` with `IEnumerable<TDto>` which would run on `bulk` method.
- Added error handling in all `API`s.

### Changed

- Changed `HttpGet` at `/{ModelName}{Architechture}` ex: `/ProductRest` method to `/{ModelName}{Architechture}/bulk`.
    - GET `/ProductGrpc/bulk`
- Changed `OnBeforeAllRead` hook to `OnBeforeRead`.

## [3.1.1]

### Changed

- Changed Swagger UI to be included by default if `RestApi` or `GrpcApi` is mapped.

## [3.1.0]

### Changed

- Changed storing `Model` in Db rather `Dto` and setup `Dto` to `Model` mapping and vice versa with an extension on
  `Model`.

### Fixed

- Fixed `GraphQlApi` not working alone without any other Api.

## [3.0.0]

### Added

- Added support for `GraphQlApi` with [Hotchocolate](https://github.com/ChilliCream/graphql-platform).
- Added support for Property based Attributes, which will be applied to created `Dto` for the model. ex:
  `JsonIgnoreAttribute`

## [2.1.0] - 2024-06-12

### Fixed

- Fixed issue when Model has no constructor and default compiler generated constructor is overwritten by
  JsonConstructor, essentially enabling support for Models without any explicit constructors.

### Added

- Added support for `GrpcApi` with Code First [protobuf-net.Grpc](https://github.com/protobuf-net/protobuf-net.Grpc).

## [2.0.0] - 2024-04-21

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
