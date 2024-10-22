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
[Authorize]

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
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        
        var gameNights = await _gameNightContext.Evenings
            .Include(e => e.Address)
            .Include(e => e.Participants)
            .Where(e => e.HostId == userId)
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

    public IActionResult CantEdit()
    {
        //todo add view
        ViewBag.Message = "Minimaal één deelnemer heeft zich al aangemeld voor deze avond. Je kunt de avond niet meer aanpassen.";
        return View();
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
            
            evening = await _gameNightContext.Evenings
                .Include(e => e.Address)
                .FirstOrDefaultAsync(e => e.Id == id);
        }
    
        return View(evening);
    }
    
    [HttpPost]
    public async Task<IActionResult> Form(Evening evening)
    {
        
        if (ModelState.IsValid)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var address = new Address
            {
                Street = evening.Address.Street,
                City = evening.Address.City,
                HouseNumber = evening.Address.HouseNumber
            };
            
            if (evening.AddressId > 0) 
            {
                var existingAddress = await _gameNightContext.Addresses.FindAsync(evening.AddressId);
                if (existingAddress != null)
                {
                    existingAddress.Street = address.Street;
                    existingAddress.City = address.City;
                    existingAddress.HouseNumber = address.HouseNumber;
                    await _gameNightContext.SaveChangesAsync();
                }
            }
            else
            {
                _gameNightContext.Addresses.Add(address);
                await _gameNightContext.SaveChangesAsync();
            }

            Evening? newEvening;
            
            if (evening.Id > 0) 
            {
                newEvening = await _gameNightContext.Evenings.FindAsync(evening.Id);
                if (newEvening == null)
                {
                    return NotFound(); 
                }
               
                newEvening.HostId = userId ?? throw new InvalidOperationException("User must be logged in.");
                newEvening.HostDate = evening.HostDate;
                newEvening.MaxUsers = evening.MaxUsers;
                newEvening.Allergy = evening.Allergy;
                newEvening.AddressId = address.Id;
            }
            else
            {
                newEvening = new Evening
                {
                    HostId = userId ?? throw new InvalidOperationException("User must be logged in."),
                    HostDate = evening.HostDate,
                    MaxUsers = evening.MaxUsers,
                    Allergy = evening.Allergy,
                    AddressId = address.Id
                };
                await _gameNightContext.Evenings.AddAsync(newEvening);
            }

            
            
            await _gameNightContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        return View(evening);
    }





    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}