using System.Net;
using KurzSharp.Templates.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// NOTE: Do not change namespace as it's referenced by string in `RestApiSourceGenerator`
// but isn't linked as 'Compiled' due to not being support for netstandard2.0
namespace KurzSharp.Templates.RestApi;

[ApiController]
[Route("[controller]")]
public class PlaceholderModelController : ControllerBase
{
    private readonly ILogger<PlaceholderModelController> _logger;
    private readonly KurzSharpDbContext _context;

    public PlaceholderModelController(ILogger<PlaceholderModelController> logger, KurzSharpDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetPlaceholderModels(CancellationToken cancellationToken)
    {
        var allPlaceholderModels = await _context.PlaceholderModels.ToListAsync(cancellationToken: cancellationToken);

        return Ok(allPlaceholderModels.Select((placeholderModel) =>
        {
            // placeholderModel.OnBeforeRead();
            return placeholderModel;
        }));
    }

    [HttpPost]
    public async Task<IActionResult> AddPlaceholderModel(PlaceholderModel placeholderModel,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogInformation("Invalid request to AddPlaceholderModel, {Issues}", ModelState);

            return BadRequest(ModelState);
        }

        // placeholderModel.OnBeforeCreate();

        var result = await _context.PlaceholderModels.AddAsync(placeholderModel, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return StatusCode((int)HttpStatusCode.Created, result.Entity);
    }

    [HttpDelete]
    public async Task<IActionResult> DeletePlaceholderModel(PlaceholderModel placeholderModel,
        CancellationToken cancellationToken)
    {
        try
        {
            // placeholderModel.OnBeforeDelete();

            _context.Remove(placeholderModel);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Error while deleting {@Entity}, {Message}", placeholderModel, e.Message);

            return BadRequest($"Error while trying to delete {placeholderModel}");
        }

        return Ok(placeholderModel);
    }

    [HttpPut]
    public async Task<IActionResult> UpdatePlaceholderModel(PlaceholderModel placeholderModel,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogInformation("Invalid request to AddPlaceholderModel, {Issues}", ModelState);

            return BadRequest(ModelState);
        }

        // placeholderModel.OnBeforeUpdate();

        _context.PlaceholderModels.Update(placeholderModel);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(placeholderModel);
    }
}