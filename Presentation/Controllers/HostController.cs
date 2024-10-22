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
        ViewBag.Message =
            "Minimaal één deelnemer heeft zich al aangemeld voor deze avond. Je kunt de avond niet meer aanpassen.";
        return View();
    }


    public IActionResult Delete(int id)
    {
        Console.WriteLine("Delete evening with id: " + id);
        var evening = _gameNightContext.Evenings.FirstOrDefault(e => e.Id == id);
        Console.WriteLine("Evening found: " + evening);
        _gameNightContext.Evenings.Remove(evening ?? throw new InvalidOperationException("evening must exist."));
        _gameNightContext.SaveChanges();
        return RedirectToAction("Index");
    }


    public async Task<IActionResult> Form(int? id)
    {
        Evening? evening;
        List<Game> games;
        List<int> selectedGameIds = new List<int>();

        if (!id.HasValue)
        {
            evening = new Evening();
            games = await _gameNightContext.Games.ToListAsync();
        }
        else
        {

            evening = await _gameNightContext.Evenings
                .Include(e => e.Address)
                .FirstOrDefaultAsync(e => e.Id == id);
            
            games = await _gameNightContext.Games.ToListAsync();
            
            selectedGameIds = await _gameNightContext.EveningGame
                .Where(eg => eg.EveningId == id)
                .Select(eg => eg.GameId)
                .ToListAsync();
        }

        

        var viewModel = new EveningFormViewModel
        {
            Evening = evening ?? throw new InvalidOperationException("evening must exist."),
            AllGames = games,
            SelectedGameIds = selectedGameIds
        };
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Form(EveningFormViewModel eveningFormViewModel)
    {
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
     
    try
    {
        Evening? newEvening;
        
        if (eveningFormViewModel.Evening?.Id > 0)
        {
            newEvening = await _gameNightContext.Evenings.FindAsync(eveningFormViewModel.Evening.Id);
            
            if (newEvening == null)
            {
                return NotFound();
            }
            
            newEvening.HostId = userId ?? throw new InvalidOperationException("User must be logged in.");
            newEvening.HostDate = eveningFormViewModel.Evening.HostDate;
            newEvening.MaxUsers = eveningFormViewModel.Evening.MaxUsers;
            newEvening.Allergy = eveningFormViewModel.Evening.Allergy;
        if (eveningFormViewModel.Evening.AddressId > 0)
            {
                var existingAddress = await _gameNightContext.Addresses.FindAsync(eveningFormViewModel.Evening.AddressId);
                if (existingAddress != null)
                {
                    existingAddress.Street = eveningFormViewModel.Evening.Address.Street;
                    existingAddress.City = eveningFormViewModel.Evening.Address.City;
                    existingAddress.HouseNumber = eveningFormViewModel.Evening.Address.HouseNumber;
                }
            }
            else
            {
                
                var newAddress = new Address
                {
                    Street = eveningFormViewModel.Evening.Address.Street,
                    City = eveningFormViewModel.Evening.Address.City,
                    HouseNumber = eveningFormViewModel.Evening.Address.HouseNumber
                };
                newEvening.Address = newAddress; 
            }
        } 
        else
        {
            newEvening = new Evening
            {
                HostId = userId ?? throw new InvalidOperationException("User must be logged in."),
                HostDate = eveningFormViewModel.Evening.HostDate ,
                MaxUsers = eveningFormViewModel.Evening.MaxUsers,
                Allergy = eveningFormViewModel.Evening.Allergy,
                Address = new Address
                {
                    Street = eveningFormViewModel.Evening.Address.Street,
                    City = eveningFormViewModel.Evening.Address.City,
                    HouseNumber = eveningFormViewModel.Evening.Address.HouseNumber
                }
            };
            await _gameNightContext.Evenings.AddAsync(newEvening);
        }
        await _gameNightContext.SaveChangesAsync();
        if (eveningFormViewModel.Evening.Id > 0)
            {
                var existingGames = await _gameNightContext.EveningGame
                    .Where(eg => eg.EveningId == newEvening.Id)
                    .ToListAsync();

                _gameNightContext.EveningGame.RemoveRange(existingGames);
            }

            
            foreach (var gameId in eveningFormViewModel.SelectedGameIds)
            {
                var eveningGame = new EveningGame
                {
                    EveningId = newEvening.Id,
                    GameId = gameId
                };
                _gameNightContext.EveningGame.Add(eveningGame);
            }
            await _gameNightContext.SaveChangesAsync();
        return RedirectToAction("Index");
    }
    catch (Exception ex)
    {
        ModelState.AddModelError(string.Empty, "An error occurred while saving data. Please try again." + ex.Message);
        return View(eveningFormViewModel);
    }
}


[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}