using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.GraphQL;

[Route("api/[controller]")]
[ApiController]
public class EveningController : ControllerBase
{
    private readonly EveningService eveningService;
    public EveningController(EveningService eveningService)
    {
        this.eveningService = eveningService;
    }
    [HttpGet]
    public IActionResult GetAllEvenings()
    {
        return new ObjectResult(eveningService.GetEvenings());
    }
    [HttpGet("{id}")]
    public IActionResult GetEveningById(int id)
    {
        return new ObjectResult(eveningService.GetEvening(id));
    }
}