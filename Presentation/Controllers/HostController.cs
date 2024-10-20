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
        /*ViewBag.AllGames = await _context.Games
            .Select(g => new 
            {
                Id = (int)g.Id, // Cast to int explicitly
                g.Name,
                g.Description,
                g.Genre,
                g.ImageURL,
                g.IsAgeRestricted,
                g.TypeOfGame
            })
            .ToListAsync();*/
        /*ViewBag.AllGames = await _context.Games.ToListAsync();*/
    
        return View(evening);
    }
    
    [HttpPost]
    public async Task<IActionResult> Form(Evening evening)
    {
        
        Console.WriteLine($"HostId: {evening.HostId}");

        
        var host = await _context.Users.FindAsync(evening.HostId);
        if (host == null)
        {
            ModelState.AddModelError("HostId", "The selected host does not exist.");
            return View(evening); 
        }

        
        evening.Host = host;

        
        if (ModelState.IsValid)
        {
            
            if (evening.Id == 0)
            {
                _context.Evenings.Add(evening);
            }
            else
            {
                var existingEvening = await _context.Evenings.FindAsync(evening.Id);
                if (existingEvening == null)
                {
                    return NotFound();
                }

                existingEvening.HostId = evening.HostId;
                existingEvening.Host = evening.Host; 
                existingEvening.MaxUsers = evening.MaxUsers;
                existingEvening.HostDate = evening.HostDate;
                existingEvening.Allergy = evening.Allergy;
                existingEvening.Address = evening.Address;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        
        foreach (var state in ModelState)
        {
            foreach (var error in state.Value.Errors)
            {
                Console.WriteLine($"Field: {state.Key}, Error: {error.ErrorMessage}");
            }
        }

        
        return View(evening);
    }





    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}