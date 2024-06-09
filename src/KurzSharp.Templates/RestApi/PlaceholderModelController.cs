using KurzSharp.Templates.Models;
using KurzSharp.Templates.Services;
#if NET8_0_OR_GREATER
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
#endif

// NOTE: Do not change namespace as it's referenced by string in `RestApiSourceGenerator`
// but isn't linked as 'Compiled' due to not being support for netstandard2.0
namespace KurzSharp.Templates.RestApi;

#if NET8_0_OR_GREATER
[Tags("PlaceholderModelRest")]
[ApiController]
[Route($"{nameof(PlaceholderModel)}Rest")]
// NOTE: Leave out class/interface declarations out of if NET8_0_OR_GREATER check to make it easier use in SourceGen with netstandard2.0
public partial class PlaceholderModelController : ControllerBase
{
    private readonly ILogger<PlaceholderModelController> _logger;
    private readonly IPlaceholderModelService _placeholderModelService;

    public PlaceholderModelController(IPlaceholderModelService placeholderModelService,
        ILogger<PlaceholderModelController> logger)
    {
        _logger = logger;
        _placeholderModelService = placeholderModelService;
    }


    [HttpGet]
    public async Task<IActionResult> GetPlaceholderModels(CancellationToken cancellationToken)
    {
        return Ok(await _placeholderModelService.GetPlaceholderModels(cancellationToken));
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

        var result = await _placeholderModelService.AddPlaceholderModel(placeholderModelDto, cancellationToken);

        return Created(nameof(GetPlaceholderModels), result);
    }

    [HttpDelete]
    public async Task<IActionResult> DeletePlaceholderModel([FromBody] PlaceholderModelDto placeholderModelDto,
        CancellationToken cancellationToken)
    {
        try
        {
            await _placeholderModelService.DeletePlaceholderModel(placeholderModelDto, cancellationToken);
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

        await _placeholderModelService.UpdatePlaceholderModel(placeholderModelDto, cancellationToken);

        return Ok(placeholderModelDto);
    }

}
#endif

public partial class PlaceholderModelController {}
