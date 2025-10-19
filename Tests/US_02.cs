using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.Controllers;
using Presentation.ViewModels;

namespace Tests;

using Domain;
using Xunit;
using System;
using System.Collections.Generic;

public class Us02 : ControllerTestBase
{
    // 1. Happy Flow: Organisator voegt een nieuwe bordspellenavond toe
   [Fact]
    public async Task Form_Post_CreatesNewEveningAndRedirects()
    {
        var controller = SetupController(new HostController(_gameNightContext, _identityContext));
        var newHostDate = DateOnly.FromDateTime(DateTime.Now).AddDays(20);
        var newAddress = new Address { Street = "New Street", City = "New City", HouseNumber = 100 };
        var gameIds = _gameNightContext.Games.Select(g => g.Id).Take(1).ToList();

        var viewModel = new EveningFormViewModel
        {
            Evening = new Evening
            {
                HostDate = newHostDate,
                MaxUsers = 8,
                Allergy = "Lactose",
                Address = newAddress
            },
            SelectedGameIds = gameIds
        };

        var result = await controller.Form(viewModel);

        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);

        var addedEvening = await _gameNightContext.Evenings
            .Include(e => e.Address)
            .Include(e => e.Games)
            .ThenInclude(eg => eg.Game)
            .FirstOrDefaultAsync(e => e.HostDate == newHostDate && e.HostId == TestUserId);

        Assert.NotNull(addedEvening);
        Assert.Equal(8, addedEvening.MaxUsers);
        Assert.Equal("New Street", addedEvening.Address.Street);
        Assert.Contains(addedEvening.Games, eg => gameIds.Contains(eg.GameId));
    }

    // 2. Happy Flow: Organisator wijzigt een bestaande bordspellenavond (zonder deelnemers)
    [Fact]
    public async Task Form_Post_UpdatesExistingEveningAndRedirects()
    {
        var controller = SetupController(new HostController(_gameNightContext, _identityContext));
        var eveningToUpdateId = 5; // Evening 5 has no participants, hosted by "host-A".
        var originalEvening = await _gameNightContext.Evenings
            .Include(e => e.Address)
            .Include(e => e.Games)
            .FirstOrDefaultAsync(e => e.Id == eveningToUpdateId);

        Assert.NotNull(originalEvening);

        var updatedHostDate = DateOnly.FromDateTime(DateTime.Now).AddDays(25);
        var updatedGameIds = _gameNightContext.Games.Select(g => g.Id).Skip(1).Take(1).ToList(); // Selecteer een andere game

        var viewModel = new EveningFormViewModel
        {
            Evening = new Evening
            {
                Id = eveningToUpdateId,
                HostDate = updatedHostDate,
                MaxUsers = 12,
                Allergy = "Nut-Free",
                Address = new Address
                {
                    Id = originalEvening.AddressId,
                    Street = "Updated Street",
                    City = "Updated City",
                    HouseNumber = 99
                }
            },
            SelectedGameIds = updatedGameIds
        };

        var result = await controller.Form(viewModel);

        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);

        var updatedEvening = await _gameNightContext.Evenings
            .Include(e => e.Address)
            .Include(e => e.Games)
            .ThenInclude(eg => eg.Game)
            .FirstOrDefaultAsync(e => e.Id == eveningToUpdateId);

        Assert.NotNull(updatedEvening);
        Assert.Equal(updatedHostDate, updatedEvening.HostDate);
        Assert.Equal(12, updatedEvening.MaxUsers);
        Assert.Equal("Updated Street", updatedEvening.Address.Street);
        Assert.Contains(updatedEvening.Games, eg => updatedGameIds.Contains(eg.GameId));
    }

    // 3. Happy Flow: Organisator verwijdert een bordspellenavond zonder deelnemers
    [Fact]
    public async Task Delete_EveningWithoutParticipants_DeletesEveningAndRedirects()
    {
        var controller = SetupController(new HostController(_gameNightContext, _identityContext));
        var eveningToDeleteId = 5;

        var result = controller.Delete(eveningToDeleteId);

        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);

        var deletedEvening = await _gameNightContext.Evenings.FindAsync(eveningToDeleteId);
        Assert.Null(deletedEvening);
    }
}