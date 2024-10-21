using System.Diagnostics;
using System.Security.Claims;
using Domain;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.Models;
using Presentation.ViewModels;

namespace Presentation.Controllers;

public class GameNightsController : Controller
{

    private readonly GameNightContext _gameNightContext;
    private readonly IdentityContext _identityContext;

    public GameNightsController(GameNightContext gameNightContext, IdentityContext identityContext)
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
    
    [Authorize]
    public  IActionResult Join(int eveningId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception();
        _gameNightContext.EveningParticipants.Add(new EveningParticipant { EveningId = eveningId, ParticipantId = userId });
        _gameNightContext.SaveChanges();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Detailpage(int id)
    {
        if (id == 0)
        {
            return NotFound();
        }

        
        /*var evening = await _context.Evenings
            .Include(e => e.Host) 
            .Include(e => e.Address) 
            .Include(e => e.Participants) 
            .ThenInclude(ep => ep.Participant) 
            .FirstOrDefaultAsync(e => e.Id == id); 

        if (evening == null)
        {
            return NotFound();
        }

        return View(evening);*/
        return View();
    }



    
    //todo implement form
    /*public IActionResult Create()
    {
        // Populate necessary dropdowns (e.g., Hosts, Games, Participants)
        ViewBag.Hosts = new SelectList(_userRepository.GetAll(), "Id", "Name");
        ViewBag.BoardGames = new SelectList(_gameRepository.GetAll(), "Id", "Title");
        ViewBag.Participants = new SelectList(_userRepository.GetAll(), "Id", "Name");

        return View();
    }

    [HttpPost]
    public IActionResult Create(Evening newEvening)
    {
        if (ModelState.IsValid)
        {
            _eveningRepository.Add(newEvening);
            return RedirectToAction("Index");
        }

        // Repopulate dropdowns if the form submission fails
        ViewBag.Hosts = new SelectList(_userRepository.GetAll(), "Id", "Name");
        ViewBag.BoardGames = new SelectList(_gameRepository.GetAll(), "Id", "Title");
        ViewBag.Participants = new SelectList(_userRepository.GetAll(), "Id", "Name");

        return View(newEvening);
    }*/

   

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}