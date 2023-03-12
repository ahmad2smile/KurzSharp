using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KurzSharp.Templates;

[ApiController]
[Route("[controller]")]
public class NameController : ControllerBase
{
    private readonly ILogger<NameController> _logger;

    public NameController(ILogger<NameController> logger)
    {
        _logger = logger;
    }
    
    [HttpGet]
    public IActionResult GetNames()
    {
        _logger.LogInformation("Example log here from get request");

        return Ok(Enumerable.Range(1, 5).Select(index => new PlaceholderModel
        {
            Id = Guid.NewGuid()
        }));
    }
}