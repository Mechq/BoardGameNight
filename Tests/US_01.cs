using System.Security.Claims;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers;
using Presentation.ViewModels;

namespace Tests;

public class Us01 : ControllerTestBase
{
    // Happy Flow: Alle toekomstige, niet-volle avonden voor niet-ingelogde gebruiker
    [Fact]
    public async Task Index_UnauthenticatedUser_ReturnsOnlyFutureAndAvailableNights()
    {
        var controller = SetupUnauthenticatedController(new GameNightsController(_gameNightContext, _identityContext));

        var result = await controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<GameNightViewModel>>(viewResult.Model);

        // Verwacht: Avond 1 (Beschikbaar) en Avond 5 (Beschikbaar) en Avond 3 (Host door TestUser)
        Assert.Equal(3, model.Count());
        Assert.Contains(model, vm => vm.GameNight.Id == 1);
        Assert.Contains(model, vm => vm.GameNight.Id == 5);
        Assert.Contains(model, vm => vm.GameNight.Id == 3);

        Assert.DoesNotContain(model, vm => vm.GameNight.Id == 4);
        Assert.DoesNotContain(model, vm => vm.GameNight.Id == 2);
    }

    // Fout Scenario: Avonden met toekomstige datum en deelnemende avonden worden uitgesloten.
    [Fact]
    public async Task Index_AuthenticatedUser_ExcludesJoinedAndConflictingDateNights()
    {
        var controller = SetupController(new GameNightsController(_gameNightContext, _identityContext));
        
        var result = await controller.Index();
        
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<GameNightViewModel>>(viewResult.Model);

        // Verwacht: Alleen Avond 5 is over.
        Assert.Single(model);
        Assert.Equal(5, model.First().GameNight.Id);
        Assert.DoesNotContain(model, vm => vm.GameNight.Id == 1);  
        Assert.DoesNotContain(model, vm => vm.GameNight.Id == 3);
    }
}

// USE CASE 1 For all hosting users
public class HostControllerTests : ControllerTestBase
{
    // Happy Flow: Alle avonden georganiseerd door de ingelogde gebruiker worden getoond.
    [Fact]
    public async Task Index_AuthenticatedUser_ReturnsOnlyHostedNights()
    {
        var controller = SetupController(new HostController(_gameNightContext, _identityContext));
        
        var result = await controller.Index();
        
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<GameNightViewModel>>(viewResult.Model);

        // Verwacht: Avond 3 is de enige avond gehost door TestUserId.
        Assert.Single(model);
        Assert.Equal(3, model.First().GameNight.Id);
        Assert.Equal(TestUserId, model.First().GameNight.HostId);
    }

    
}

// USE CASE 1 For all attending evenings
public class AttendedNightsControllerTests : ControllerTestBase
{
    // Happy Flow: Alle avonden waaraan de ingelogde gebruiker deelneemt worden getoond (toekomst en verleden).
    [Fact]
    public async Task Index_AuthenticatedUser_ReturnsOnlyAttendedNights()
    {
        var controller = SetupController(new AttendedNightsController(_gameNightContext, _identityContext));
        // TestData bevat Avond 1 (deelgenomen) en Avond 3 (gehost/deelgenomen) voor TestUserId.
        
        var result = await controller.Index();
        
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<GameNightViewModel>>(viewResult.Model);

        // Verwacht: Avond 1 en Avond 3.
        Assert.Equal(2, model.Count());
        Assert.Contains(model, vm => vm.GameNight.Id == 1);
        Assert.Contains(model, vm => vm.GameNight.Id == 3);
        Assert.DoesNotContain(model, vm => vm.GameNight.Id == 4);
    }

    // Fout Scenario: Gebruiker verlaat een avond
    [Fact]
    public void Leave_UserSuccessfullyLeavesEvening_RedirectsToIndex()
    {
        var eveningIdToLeave = 1;
        var controller = SetupController(new AttendedNightsController(_gameNightContext, _identityContext));

        var result = controller.Leave(eveningIdToLeave);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Null(_gameNightContext.EveningParticipants
            .FirstOrDefault(ep => ep.EveningId == eveningIdToLeave && ep.ParticipantId == TestUserId));
    }
}