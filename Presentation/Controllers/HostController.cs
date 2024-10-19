using System.Diagnostics;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.Data;
using Presentation.Models;

namespace Presentation.Controllers;

public class HostController : Controller
{
    private readonly GameNightContext _context;

    public HostController(GameNightContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var gameNights = await _context.Evenings
            .Include(e => e.Host)  
            .Include(e => e.Address)  
            .Include(e => e.Participants)
            .ThenInclude(p => p.Participant)  
            .ToListAsync();


        return View(gameNights);
    }

    public async Task<IActionResult> Form(int? id)
    {
        Evening? evening = null;
    
        if (!id.HasValue)
        {
            Console.WriteLine("aa");
            evening = new Evening();
        }
        else
        {
            
            evening = await _context.Evenings
                .Include(e => e.Host)
                .Include(e => e.Address)
                .Include(e => e.Participants)
                .ThenInclude(ep => ep.Participant)
                .Include(e => e.Games) 
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evening == null)
            {
                return NotFound();
            }
        }

        // Load the list of available games from the database.
        ViewBag.AllGames = await _context.Games.ToListAsync();
    
        return View(evening);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}