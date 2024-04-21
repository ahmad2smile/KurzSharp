using System.Net;
using KurzSharp.Templates.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


#if NET5_0_OR_GREATER
using Microsoft.EntityFrameworkCore;
#endif

// NOTE: Do not change namespace as it's referenced by string in `RestApiSourceGenerator`
// but isn't linked as 'Compiled' due to not being support for netstandard2.0
namespace KurzSharp.Templates.RestApi;

[ApiController]
[Route("[controller]")]
public class PlaceholderModelController : ControllerBase
{
    private readonly ILogger<PlaceholderModelController> _logger;
    private readonly KurzSharpDbContext _context;
    private readonly PlaceholderModel _model;

    public PlaceholderModelController(ILogger<PlaceholderModelController> logger, KurzSharpDbContext context,
        PlaceholderModel model)
    {
        _logger = logger;
        _context = context;
        _model = model;
    }

#if NET5_0_OR_GREATER
    [HttpGet]
    public async Task<IActionResult> GetPlaceholderModels(CancellationToken cancellationToken)
    {
        var allPlaceholderModels = await _context.PlaceholderModels.ToListAsync(cancellationToken);

        var dtos = _model.OnBeforeAllRead(allPlaceholderModels);

        return Ok(dtos.Select(placeholderModel =>
        {
            var dto = _model.OnBeforeRead(placeholderModel);

            return dto;
        }).ToList());
    }

    [HttpPost]
    public async Task<IActionResult> AddPlaceholderModel([FromBody] PlaceholderModelDto placeholderModelDto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogInformation("Invalid request to AddPlaceholderModel, {Issues}", ModelState);

            return BadRequest(ModelState);
        }

        var dto = _model.OnBeforeCreate(placeholderModelDto);

        var result = await _context.PlaceholderModels.AddAsync(dto, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return StatusCode((int)HttpStatusCode.Created, result.Entity);
    }

    [HttpDelete]
    public async Task<IActionResult> DeletePlaceholderModel([FromBody] PlaceholderModelDto placeholderModelDto,
        CancellationToken cancellationToken)
    {
        try
        {
            var dto = _model.OnBeforeDelete(placeholderModelDto);

            _context.Remove(dto);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Error while deleting {@Entity}, {Message}", placeholderModelDto, e.Message);

            return BadRequest($"Error while trying to delete {placeholderModelDto}");
        }

        return Ok(placeholderModelDto);
    }

    [HttpPut]
    public async Task<IActionResult> UpdatePlaceholderModel([FromBody] PlaceholderModelDto placeholderModelDto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogInformation("Invalid request to AddPlaceholderModel, {Issues}", ModelState);

            return BadRequest(ModelState);
        }

        var dto = _model.OnBeforeUpdate(placeholderModelDto);

        _context.PlaceholderModels.Update(dto);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(placeholderModelDto);
    }

#endif
}
