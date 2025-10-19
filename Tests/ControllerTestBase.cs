using System.Security.Claims;
using Domain;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Tests;



    public abstract class ControllerTestBase : IDisposable
    {
        protected const string TestUserId = "user-123";
        protected readonly GameNightContext _gameNightContext;
        protected readonly IdentityContext _identityContext;
        private readonly string _gameNightDbName;
        private readonly string _identityDbName;

        public ControllerTestBase()
        {
            _gameNightDbName = Guid.NewGuid().ToString();
            _identityDbName = Guid.NewGuid().ToString();

            
            var optionsGameNights = new DbContextOptionsBuilder<GameNightContext>()
                .UseInMemoryDatabase(_gameNightDbName)
                .Options;
            var optionsIdentity = new DbContextOptionsBuilder<IdentityContext>()
                .UseInMemoryDatabase(_identityDbName)
                .Options;

            _gameNightContext = new GameNightContext(optionsGameNights);
            _identityContext = new IdentityContext(optionsIdentity);

            
            _gameNightContext.Database.EnsureDeleted();
            _identityContext.Database.EnsureDeleted();
            _gameNightContext.Database.EnsureCreated();
            _identityContext.Database.EnsureCreated();

            SeedTestData();
        }

        public void Dispose()
        {
            
            _gameNightContext.Database.EnsureDeleted();
            _identityContext.Database.EnsureDeleted();
            _gameNightContext.Dispose();
            _identityContext.Dispose();
        }

        private void SeedTestData()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var yesterday = today.AddDays(-1);
            var futureDate1 = today.AddDays(5);
            var futureDate2 = today.AddDays(10);
            var futureDate3 = today.AddDays(15);

            
            var address1 = new Address { Id = 1, Street = "Street A", City = "City X", HouseNumber = 12 };
            var address2 = new Address { Id = 2, Street = "Street B", City = "City Y", HouseNumber = 11 };
            var address3 = new Address { Id = 3, Street = "Street C", City = "City Z", HouseNumber = 10 };

            _gameNightContext.Addresses.AddRange(address1, address2, address3);
            
            var game1 = new Game { Id = 1, Name = "Catan", Description = "Settling new lands", Genre = Genre.Medieval, TypeOfGame = GameType.BoardGame, IsAgeRestricted = false, ImageURL = "" };
            var game2 = new Game { Id = 2, Name = "Ticket to Ride", Description = "Connecting cities", Genre = Genre.Other, TypeOfGame = GameType.BoardGame, IsAgeRestricted = false, ImageURL = "" };
            var game3 = new Game { Id = 3, Name = "Carcassonne", Description = "Building a medieval landscape", Genre = Genre.Medieval, TypeOfGame = GameType.BoardGame, IsAgeRestricted = false, ImageURL = "" };
            _gameNightContext.Games.AddRange(game1, game2, game3);
            
            _identityContext.Users.AddRange(
                new User { Id = TestUserId, Name = "Charlie (Current User)", DateOfBirth = new DateOnly(1995, 1, 1) },
                new User { Id = "host-A", Name = "Host A", DateOfBirth = new DateOnly(1980, 1, 1) },
                new User { Id = "participant-B", Name = "Participant B", DateOfBirth = new DateOnly(2000, 1, 1) }
            );

            
            var evening1 = new Evening(1, "host-A", 5, futureDate1, null, address1);
            var evening2 = new Evening(2, "host-A", 2, futureDate2, null, address2);
            var evening3 = new Evening(3, TestUserId, 10, futureDate1, "Gluten", address3);
            var evening4 = new Evening(4, "host-A", 5, yesterday, null, address1);
            var evening5 = new Evening(5, "host-A", 5, futureDate3, null, address2);

            _gameNightContext.Evenings.AddRange(evening1, evening2, evening3, evening4, evening5);

            
            _gameNightContext.EveningParticipants.AddRange(
                // Avond 1: Huidige gebruiker neemt deel
                new EveningParticipant { EveningId = 1, ParticipantId = TestUserId },
                // Avond 2: Vol
                new EveningParticipant { EveningId = 2, ParticipantId = "participant-B" },
                new EveningParticipant { EveningId = 2, ParticipantId = "host-A" },
                // Avond 3: Huidige gebruiker is host, maar neemt ook deel aan zijn eigen avond (moet gefilterd worden in GameNightsController)
                new EveningParticipant { EveningId = 3, ParticipantId = TestUserId }
            );
            
            _gameNightContext.EveningGame.AddRange(
                new EveningGame { EveningId = 1, GameId = 1 }, // Evening 1 has Catan
                new EveningGame { EveningId = 2, GameId = 2 }, // Evening 2 has Ticket to Ride
                new EveningGame { EveningId = 3, GameId = 1 }, // Evening 3 has Catan
                new EveningGame { EveningId = 3, GameId = 3 }  // Evening 3 has Carcassonne
            );

            _gameNightContext.SaveChanges();
            _identityContext.SaveChanges();
        }

        protected T SetupController<T>(T controller) where T : Controller
        {
            // Simulate the logged in user
            var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, TestUserId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            return controller;
        }

        protected T SetupUnauthenticatedController<T>(T controller) where T : Controller
        {
            // Simulate a non logged-in user
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            return controller;
        }
    }