using System.Diagnostics;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.Models;

namespace Presentation.Controllers;

public class AttendedNightsController : Controller
{
    private readonly GameNightContext _context;

    public AttendedNightsController(GameNightContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        /*var gameNights = await _context.Evenings
            .Include(e => e.Host)  
            .Include(e => e.Address)  
            .Include(e => e.Participants)
            .ThenInclude(p => p.Participant)  
            .ToListAsync();*/


        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}