using System.Diagnostics;
using System.Security.Claims;

using Domain;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.Models;
using Presentation.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Controllers;
[Authorize]
public class AttendedNightsController : Controller
{
   
    private readonly GameNightContext _gameNightContext;
    private readonly IdentityContext _identityContext;

    public AttendedNightsController(GameNightContext gameNightContext, IdentityContext identityContext)
    {
        _gameNightContext = gameNightContext;
        _identityContext = identityContext;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var gameNights = await _gameNightContext.Evenings
            .Include(e => e.Address)
            .Include(e => e.Participants)
            .Where(e =>  e.Participants.Any(p => p.ParticipantId == userId))
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
    
    public IActionResult Leave(int eveningId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("User must be logged in.");
        
        var eveningParticipant = _gameNightContext.EveningParticipants
            .FirstOrDefault(ep => ep.EveningId == eveningId && ep.ParticipantId == userId);
        
        _gameNightContext.EveningParticipants.Remove(eveningParticipant?? throw new InvalidOperationException("User must be participant of the evening."));
        _gameNightContext.SaveChanges();

        return RedirectToAction("Index");
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}