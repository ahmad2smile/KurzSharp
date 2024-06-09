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
    private readonly KurzSharpDbContext _context;
    private readonly PlaceholderModel _model;

    public PlaceholderModelService(ILogger<PlaceholderModelService> logger, KurzSharpDbContext context,
        PlaceholderModel model)
    {
        _logger = logger;
        _context = context;
        _model = model;
    }

    public async Task<IEnumerable<PlaceholderModelDto>> GetPlaceholderModels(CancellationToken cancellationToken)
    {
        var allPlaceholderModels = await _context.PlaceholderModels.ToListAsync(cancellationToken);

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
        var dto = _model.OnBeforeCreate(placeholderModelDto);

        var result = _context.PlaceholderModels.Add(dto);
        await _context.SaveChangesAsync(cancellationToken);

        return result.Entity;
    }

    public async Task<PlaceholderModelDto> DeletePlaceholderModel(PlaceholderModelDto placeholderModelDto,
        CancellationToken cancellationToken)
    {
        var dto = _model.OnBeforeDelete(placeholderModelDto);

        _context.PlaceholderModels.Remove(dto);
        await _context.SaveChangesAsync(cancellationToken);

        return placeholderModelDto;
    }

    public async Task<PlaceholderModelDto> UpdatePlaceholderModel(PlaceholderModelDto placeholderModelDto,
        CancellationToken cancellationToken)
    {
        var dto = _model.OnBeforeUpdate(placeholderModelDto);

        _context.PlaceholderModels.Update(dto);
        await _context.SaveChangesAsync(cancellationToken);

        return placeholderModelDto;
    }

#endif
}
