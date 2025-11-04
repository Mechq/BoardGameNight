using Domain;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/evening")]
public class EveningController : ControllerBase
{


    private readonly ILogger<EveningController> _logger;
    private readonly IEveningRepository _eveningRepository;

    
    public EveningController(ILogger<EveningController> logger, IEveningRepository eveningRepository)
    {
        _logger = logger;
        _eveningRepository = eveningRepository;
    }

   
    [HttpPost("{eveningId}/{userId}")]
    /*
    [Authorize]
    */
    public IActionResult Enroll(int eveningId, string userId)
    {
        _eveningRepository.Enroll(eveningId, userId);
        return Ok(new { message = "User enrolled successfully" });
    }

    [HttpDelete("{eveningId}/{userId}")]
    /*
    [Authorize]
    */
    public IActionResult Unroll(int eveningId, string userId)
    {
        _eveningRepository.Unroll(eveningId, userId);
        return Ok(new { message = "User unrolled successfully" });
    }


 
 
}