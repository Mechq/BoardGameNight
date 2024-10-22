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
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        IEnumerable<Evening> gameNights;
        if (userId == null)
        {
            gameNights = await _gameNightContext.Evenings
                .Include(e => e.Address)
                .Include(e => e.Participants)
                .Where(e => e.Participants.Count < e.MaxUsers && e.HostDate > DateOnly.FromDateTime(DateTime.Now)) 
                .OrderBy(e => e.HostDate)
                .ToListAsync();
        }
        else
        {
            var joinedEveningDates = await _gameNightContext.EveningParticipants
                .Where(p => p.ParticipantId == userId)
                .Select(p => p.Evening.HostDate)
                .ToListAsync();
            
            gameNights = await _gameNightContext.Evenings
                .Include(e => e.Address)
                .Include(e => e.Participants)
                .Where(e => !e.Participants.Any(p => p.ParticipantId == userId) && 
                            e.Participants.Count < e.MaxUsers && 
                            e.HostDate > DateOnly.FromDateTime(DateTime.Now) &&
                            !joinedEveningDates.Contains(e.HostDate))
                .OrderBy(e => e.HostDate)
                .ToListAsync();
        }
       

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
    
    
    public async Task<IActionResult> Detailpage(int id)
    {
        var gameNight = await _gameNightContext.Evenings
            .Include(e => e.Address)
            .Include(e => e.Participants)
            .Include(e => e.Games)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (gameNight == null)
        {
            return NotFound();
        }

        var host = await _identityContext.Users
            .FirstOrDefaultAsync(u => u.Id == gameNight.HostId);

        var games = await _gameNightContext.Games
            .Where(g => gameNight.Games.Select(gn => gn.GameId).Contains(g.Id))
            .ToListAsync();

        var participantIds = gameNight.Participants.Select(p => p.ParticipantId).Distinct();
        var participants = await _identityContext.Users
            .Where(u => participantIds.Contains(u.Id))
            .ToListAsync();

        // Fetch attendance rates
        var attendanceRates = new Dictionary<string, double>();
        var totalCount = await _gameNightContext.EveningParticipants.CountAsync(ep => ep.EveningId == id);
    
        if (totalCount > 0)
        {
            var attendanceData = await _gameNightContext.EveningParticipants
                .Where(ep => ep.EveningId == id)
                .GroupBy(ep => ep.ParticipantId)
                .Select(g => new
                {
                    ParticipantId = g.Key,
                    AttendedCount = g.Count(ep => ep.ShowUp == true), 
                }).ToListAsync();

            foreach (var data in attendanceData)
            {
                double rate = (double)data.AttendedCount / totalCount * 100;
                attendanceRates[data.ParticipantId] = rate;
            }
        }

        var gameNightViewModel = new GameNightViewModel
        {
            GameNight = gameNight,
            Host = host ?? new User { Name = "Unknown" },
            Participants = participants,
            Games = games,
            Attendance = attendanceRates 
        };

        return View(gameNightViewModel);
    }


    
    [Authorize]
    public IActionResult Join(int eveningId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("User not found.");
        var ageRestrictedGames = _gameNightContext.Games
            .Where(g => g.IsAgeRestricted)
            .Select(g => g.Id)
            .ToList();

        
        
        var user = _identityContext.Users.Find(userId);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        var userAge = DateTime.Now.Year - user.DateOfBirth.Year;
        var evening = _gameNightContext.Evenings.Include(e => e.Games).FirstOrDefault(e => e.Id == eveningId);
        
        
        
        if (evening == null)
        {
            return NotFound();
        }

        
        if (evening.Allergy != null && user.Diet != null )
        {
            var userAllergies = user.Diet.Split(",");
            var eveningAllergies = evening.Allergy.Split(",");
            
            if (userAllergies.Intersect(eveningAllergies).Any())
            {
                ViewBag.EveningId = eveningId;
                ViewBag.Message = "Je hebt een allergie voor een van de gerechten op deze avond. weet je zeker dat je wilt aanmelden?";
                return View("Allergy");
            }
        }
        
        bool hasAgeRestrictedGames = evening.Games != null && evening.Games.Select(g => g.GameId).Intersect(ageRestrictedGames).Any();
    
        if (userAge < 18 && hasAgeRestrictedGames)
        {
            ViewBag.Message =
                "Je bent jonger dan 18 jaar en er zijn spellen op deze avond die alleen voor 18+ zijn. Je kunt je niet aanmelden.";
            return View("AgeRestricted");
        }

        _gameNightContext.EveningParticipants.Add(new EveningParticipant { EveningId = eveningId, ParticipantId = userId });
        _gameNightContext.SaveChanges();
        return RedirectToAction("Index");
    }
    
    [Authorize]
    public IActionResult JoinWithAllergy(int eveningId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("User not found.");
        var ageRestrictedGames = _gameNightContext.Games
            .Where(g => g.IsAgeRestricted)
            .Select(g => g.Id)
            .ToList();
        
        var user = _identityContext.Users.Find(userId);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        var userAge = DateTime.Now.Year - user.DateOfBirth.Year;
        var evening = _gameNightContext.Evenings.Include(e => e.Games).FirstOrDefault(e => e.Id == eveningId);
        
        
        
        if (evening == null)
        {
            return NotFound();
        }
        
        
        bool hasAgeRestrictedGames = evening.Games != null && evening.Games.Select(g => g.GameId).Intersect(ageRestrictedGames).Any();
    
        if (userAge < 18 && hasAgeRestrictedGames)
        {
            ViewBag.Message =
                "Je bent jonger dan 18 jaar en er zijn spellen op deze avond die alleen voor 18+ zijn. Je kunt je niet aanmelden.";
            return View("AgeRestricted");
        }

        _gameNightContext.EveningParticipants.Add(new EveningParticipant { EveningId = eveningId, ParticipantId = userId });
        _gameNightContext.SaveChanges();
        return RedirectToAction("Index");
    }
    

    
    
   
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}