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

   
    [HttpPost]
    [Authorize]
    [Route("/evening/{eveningId}/{userId}")]
    public void Enroll(int eveningId, string userId)
    {
        _eveningRepository.Enroll(eveningId, userId);
    }

    [HttpDelete]
    [Authorize]
    [Route("/evening/{eveningId}/{userId}")]
    public void Unroll(int eveningId, string userId)
    {
        _eveningRepository.Unroll(eveningId, userId);
    }


 
 
}