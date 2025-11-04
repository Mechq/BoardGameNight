using System.Security.Claims;
using Domain;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Presentation.Controllers;
using Presentation.ViewModels;
using Moq.EntityFrameworkCore; 

namespace Tests;

public class MockTestExample
{
    [Fact]
    public async Task Index_UnauthenticatedUser_CallsRepositoryAndReturnsView()
    {
        // Arrange evenings and users
        var fakeEvenings = new List<Evening>
        {
            new Evening(10, "host-A", 5, DateOnly.FromDateTime(DateTime.Now.AddDays(5)), null, 
                new Address { Street = "Street A", City = "City X" })
            {
                Participants = new List<EveningParticipant>()
            },
            new Evening(20, "host-B", 10, DateOnly.FromDateTime(DateTime.Now.AddDays(10)), null, 
                new Address { Street = "Street B", City = "City Y" })
            {
                Participants = new List<EveningParticipant>()
            }
        };
        
        var fakeUsers = new List<User>
        {
            new User { Id = "host-A", Name = "Alice" },
            new User { Id = "host-B", Name = "Bob" }
        };

        //mock the contexts
        var mockIdentityContext = new Mock<IdentityContext>(new DbContextOptions<IdentityContext>());
        mockIdentityContext.Setup(c => c.Users).ReturnsDbSet(fakeUsers);

        var mockGameNightContext = new Mock<GameNightContext>(new DbContextOptions<GameNightContext>());
        
        //mock the repository
        var mockRepo = new Mock<IEveningRepository>();
        mockRepo.Setup(r => r.GetAllFuture()).ReturnsAsync(fakeEvenings);
        
        var controller = new GameNightsController(mockGameNightContext.Object, mockIdentityContext.Object, mockRepo.Object);

        //not logged in user to test
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        //test
        var result = await controller.Index();

        //check result
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<GameNightViewModel>>(viewResult.Model);
        
        // Verify model contains the fake data
        Assert.Equal(2, model.Count());
        Assert.Contains(model, m => m.GameNight.Id == 10);
        Assert.Contains(model, m => m.GameNight.Id == 20);
    }
}