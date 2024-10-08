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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlaceholderModel(Guid id, CancellationToken cancellationToken)
    {
        var result = await _placeholderModelService.GetPlaceholderModel(id, cancellationToken);

        return result is null ? NotFound(id) : Ok(result);
    }

    [HttpGet($"/{nameof(PlaceholderModel)}Rest/bulk")]
    public async Task<IActionResult> GetPlaceholderModels(CancellationToken cancellationToken) =>
        Ok(await _placeholderModelService.GetPlaceholderModels(cancellationToken));

    [HttpPost]
    public async Task<IActionResult> AddPlaceholderModel([FromBody] PlaceholderModelDto placeholderModelDto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogInformation("Invalid request to AddPlaceholderModel, {Issues}", ModelState);

            return BadRequest(ModelState);
        }

        try
        {
            var result = await _placeholderModelService.AddPlaceholderModel(placeholderModelDto, cancellationToken);

            return Created(nameof(GetPlaceholderModels), result);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error occured while adding PlaceholderModel: {e.Message}");
        }
    }

    [HttpPost($"/{nameof(PlaceholderModel)}Rest/bulk")]
    public async Task<IActionResult> AddPlaceholderModels(
        [FromBody] IEnumerable<PlaceholderModelDto> placeholderModelDtos,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogInformation("Invalid request to AddPlaceholderModels, {Issues}", ModelState);

            return BadRequest(ModelState);
        }

        try
        {
            var result = await _placeholderModelService.AddPlaceholderModels(placeholderModelDtos, cancellationToken);

            return Created(nameof(GetPlaceholderModels), result);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error occured while adding PlaceholderModels: {e.Message}");
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeletePlaceholderModel([FromBody] PlaceholderModelDto placeholderModelDto,
        CancellationToken cancellationToken)
    {
        try
        {
            var dto = await _placeholderModelService.DeletePlaceholderModel(placeholderModelDto, cancellationToken);

            return Ok(dto);
        }
        catch (Exception e)
        {
            _logger.LogError("Error while deleting {@Entity}, {Message}", placeholderModelDto, e.Message);

            return StatusCode(500, $"Error while trying to delete PlaceholderModel {e.Message}");
        }
    }

    [HttpDelete($"/{nameof(PlaceholderModel)}Rest/bulk")]
    public async Task<IActionResult> DeletePlaceholderModels(
        [FromBody] IEnumerable<PlaceholderModelDto> placeholderModelDtos,
        CancellationToken cancellationToken)
    {
        try
        {
            var dtos = await _placeholderModelService.DeletePlaceholderModels(placeholderModelDtos, cancellationToken);

            return Ok(dtos);
        }
        catch (Exception e)
        {
            _logger.LogError("Error while deleting {@Entity}, {Message}", placeholderModelDtos, e.Message);

            return StatusCode(500, $"Error while trying to delete bulk PlaceholderModels: {e.Message}");
        }
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

        try
        {
            await _placeholderModelService.UpdatePlaceholderModel(placeholderModelDto, cancellationToken);

            return Ok(placeholderModelDto);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error occured while updating PlaceholderModel: {e.Message}");
        }
    }

    [HttpPut($"/{nameof(PlaceholderModel)}Rest/bulk")]
    public async Task<IActionResult> UpdatePlaceholderModels(
        [FromBody] IEnumerable<PlaceholderModelDto> placeholderModelDtos,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogInformation("Invalid request to AddPlaceholderModels, {Issues}", ModelState);

            return BadRequest(ModelState);
        }

        try
        {
            await _placeholderModelService.UpdatePlaceholderModels(placeholderModelDtos, cancellationToken);

            return Ok(placeholderModelDtos);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error occured while updating PlaceholderModels: {e.Message}");
        }
    }
}
#endif

public partial class PlaceholderModelController
{
}
