using System.Diagnostics;
using Domain;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.Models;
using Presentation.ViewModels;

namespace Presentation.Controllers;

public class HostController : Controller
{
    private readonly GameNightContext _gameNightContext;
    private readonly IdentityContext _identityContext;

    public HostController(GameNightContext gameNightContext, IdentityContext identityContext)
    {
        _gameNightContext = gameNightContext;
        _identityContext = identityContext;
    }

    public async Task<IActionResult> Index()
    {
        var gameNights = await _gameNightContext.Evenings
            .Include(e => e.Address)
            .Include(e => e.Participants)
            .ToListAsync();

        var hostIds = gameNights.Select(e => e.HostId).Distinct();
        var hosts = await _identityContext.Users
            .Where(u => hostIds.Contains(u.Id))
            .ToListAsync();

        var participantIds = gameNights.SelectMany(e => e.Participants.Select(p => p.ParticipantId)).Distinct();
        var participants = await _identityContext.Users
            .Where(u => participantIds.Contains(u.Id))
            .ToListAsync();

        
        var gameNightsWithDetails = gameNights.Select(gameNight => new GameNightViewModel
        {
            GameNight = gameNight,
            Host = hosts.FirstOrDefault(u => u.Id == gameNight.HostId) ?? new User { Name = "Unknown" }, 
            Participants = gameNight.Participants
                .Select(p => participants.FirstOrDefault(u => u.Id == p.ParticipantId) ?? new User { Name = "Unknown" }) 
                .ToList()
        }).ToList();

        return View(gameNightsWithDetails);  
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
            
            /*evening = await _context.Evenings
                .Include(e => e.Host)
                .Include(e => e.Address)
                .Include(e => e.Participants)
                .ThenInclude(ep => ep.Participant)
                .Include(e => e.Games)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evening == null)
            {
                return NotFound();
            }*/
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
    
    /*[HttpPost]
    public async Task<IActionResult> Form(Evening evening)
    {
        
        /*Console.WriteLine($"HostId: {evening.HostId}");

        
        var host = await _context.Users.FindAsync(evening.HostId);
        if (host == null)
        {
            ModelState.AddModelError("HostId", "The selected host does not exist.");
            return View(evening); 
        }

        
        evening.Host = host;
        #1#

        
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

                /*existingEvening.HostId = evening.HostId;
                existingEvening.Host = evening.Host; 
                existingEvening.MaxUsers = evening.MaxUsers;
                existingEvening.HostDate = evening.HostDate;
                existingEvening.Allergy = evening.Allergy;
                existingEvening.Address = evening.Address;#1#
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
    }*/





    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}