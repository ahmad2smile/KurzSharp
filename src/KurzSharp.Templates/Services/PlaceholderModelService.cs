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

    public async Task<IEnumerable<PlaceholderModelDto>> GetPlaceholderModels(CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var allPlaceholderModels = await context.PlaceholderModels.ToListAsync(cancellationToken);

        var dtos = _model.OnBeforeAllRead(allPlaceholderModels);

        var data = dtos.Select(placeholderModel =>
        {
            var dto = _model.OnBeforeRead(placeholderModel);

            return dto;
        });

        return data;
    }

    public async Task<PlaceholderModelDto> AddPlaceholderModel(PlaceholderModelDto placeholderModelDto,
        CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var dto = _model.OnBeforeCreate(placeholderModelDto);

        var result = context.PlaceholderModels.Add(dto);
        await context.SaveChangesAsync(cancellationToken);

        return result.Entity;
    }

    public async Task<PlaceholderModelDto> DeletePlaceholderModel(PlaceholderModelDto placeholderModelDto,
        CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var dto = _model.OnBeforeDelete(placeholderModelDto);

        context.PlaceholderModels.Remove(dto);
        await context.SaveChangesAsync(cancellationToken);

        return placeholderModelDto;
    }

    public async Task<PlaceholderModelDto> UpdatePlaceholderModel(PlaceholderModelDto placeholderModelDto,
        CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var dto = _model.OnBeforeUpdate(placeholderModelDto);

        context.PlaceholderModels.Update(dto);
        await context.SaveChangesAsync(cancellationToken);

        return placeholderModelDto;
    }

#endif
}
