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

        return await _model.OnBeforeAllRead(context.PlaceholderModels.ToDtos()).ToListAsync(cancellationToken);
    }

    public async Task<PlaceholderModelDto> AddPlaceholderModel(PlaceholderModelDto placeholderModelDto,
        CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var dto = _model.OnBeforeCreate(placeholderModelDto);

        var result = context.PlaceholderModels.Add(dto.ToModel());
        await context.SaveChangesAsync(cancellationToken);

        return result.Entity.ToDto();
    }

    public async Task<PlaceholderModelDto> DeletePlaceholderModel(PlaceholderModelDto placeholderModelDto,
        CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var dto = _model.OnBeforeDelete(placeholderModelDto);

        context.PlaceholderModels.Remove(dto.ToModel());
        await context.SaveChangesAsync(cancellationToken);

        return placeholderModelDto;
    }

    public async Task<PlaceholderModelDto> UpdatePlaceholderModel(PlaceholderModelDto placeholderModelDto,
        CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var dto = _model.OnBeforeUpdate(placeholderModelDto);

        context.PlaceholderModels.Update(dto.ToModel());
        await context.SaveChangesAsync(cancellationToken);

        return placeholderModelDto;
    }

#endif
}
