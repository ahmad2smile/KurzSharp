#nullable enable
using KurzSharp.Templates.Database;
using KurzSharp.Templates.Models;
#if NET8_0_OR_GREATER
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
#endif

namespace KurzSharp.Templates.Services;

public class PlaceholderModelService : IPlaceholderModelService
{
#if NET8_0_OR_GREATER
    private readonly ILogger<PlaceholderModelService> _logger;
    private readonly IDbContextFactory<KurzSharpDbContext> _contextFactory;
    private readonly PlaceholderModel _model;

    public PlaceholderModelService(ILogger<PlaceholderModelService> logger,
        IDbContextFactory<KurzSharpDbContext> contextFactory,
        PlaceholderModel model)
    {
        _logger = logger;
        _contextFactory = contextFactory;
        _model = model;
    }

    public async Task<PlaceholderModelDto?> GetPlaceholderModel(Guid id, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var result =
            await context.PlaceholderModels.FirstOrDefaultAsync(m => m.Id == id, cancellationToken: cancellationToken);

        return result is null ? null : _model.OnBeforeRead(result.ToDto());
    }

    public async Task<IEnumerable<PlaceholderModelDto>> GetPlaceholderModels(CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await _model.OnBeforeRead(context.PlaceholderModels.ToDtos()).ToListAsync(cancellationToken);
    }

    public async Task<PlaceholderModelDto> AddPlaceholderModel(PlaceholderModelDto placeholderModelDto,
        CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var dto = _model.OnBeforeCreate(placeholderModelDto);

        var result = await context.PlaceholderModels.AddAsync(dto.ToModel(), cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Added 1 PlaceholderModel");

        return result.Entity.ToDto();
    }

    public async Task<IEnumerable<PlaceholderModelDto>> AddPlaceholderModels(
        IEnumerable<PlaceholderModelDto> placeholderModelDtos, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var dtos = _model.OnBeforeCreate(placeholderModelDtos).ToList();

        await context.PlaceholderModels.AddRangeAsync(dtos.Select(d => d.ToModel()), cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Added {Count} PlaceholderModel", dtos.Count);

        return dtos;
    }

    public async Task<PlaceholderModelDto> DeletePlaceholderModel(PlaceholderModelDto placeholderModelDto,
        CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var dto = _model.OnBeforeDelete(placeholderModelDto);

        context.PlaceholderModels.Remove(dto.ToModel());
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted 1 PlaceholderModel");

        return dto;
    }

    public async Task<IEnumerable<PlaceholderModelDto>> DeletePlaceholderModels(
        IEnumerable<PlaceholderModelDto> placeholderModelDtos, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var dto = _model.OnBeforeDelete(placeholderModelDtos).ToList();

        context.PlaceholderModels.RemoveRange(dto.Select(d => d.ToModel()));
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted {Count} PlaceholderModel", dto.Count);

        return dto;
    }

    public async Task<PlaceholderModelDto> UpdatePlaceholderModel(PlaceholderModelDto placeholderModelDto,
        CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var dto = _model.OnBeforeUpdate(placeholderModelDto);

        context.PlaceholderModels.Update(dto.ToModel());
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated 1 PlaceholderModel");

        return dto;
    }

    public async Task<IEnumerable<PlaceholderModelDto>> UpdatePlaceholderModels(
        IEnumerable<PlaceholderModelDto> placeholderModelDtos, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var dtos = _model.OnBeforeUpdate(placeholderModelDtos).ToList();

        context.PlaceholderModels.UpdateRange(dtos.Select(d => d.ToModel()));
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated {Count} PlaceholderModel", dtos.Count);

        return dtos;
    }
#endif
}
