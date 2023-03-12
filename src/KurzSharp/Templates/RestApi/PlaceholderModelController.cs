#if NET7_0_OR_GREATER

using System.Net;
using KurzSharp.Templates.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

        return Ok(allPlaceholderModels);
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
            _context.Remove(placeholderModel);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Error while deleting {Entity}, {Message}", placeholderModel, e.Message);

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

        _context.PlaceholderModels.Update(placeholderModel);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(placeholderModel);
    }
}

#endif