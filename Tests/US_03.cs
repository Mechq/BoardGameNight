using Domain;
using Xunit;
using System;
using System.Collections.Generic;

namespace Tests;

public class US_03
{
    [Fact]
    public void Game_IsAgeRestricted_PropertyWorks()
    {
        var game18Plus = new Game { Id = 1, Name = "Cards Against Humanity", IsAgeRestricted = true };
        var gameAllAges = new Game { Id = 2, Name = "Catan", IsAgeRestricted = false };

        Assert.True(game18Plus.IsAgeRestricted);
        Assert.False(gameAllAges.IsAgeRestricted);
    }

    [Fact]
    public void Evening_BecomesAgeRestricted_WhenAdding18PlusGame()
    {
        // Arrange
        var address = new Address(1, "Main Street", "City");
        var evening = new Evening(1, "host123", 10, DateOnly.FromDateTime(DateTime.Today.AddDays(7)), null, address);
        var game18Plus = new Game { Id = 1, Name = "Cards Against Humanity", IsAgeRestricted = true };
        var gameAllAges = new Game { Id = 2, Name = "Catan", IsAgeRestricted = false };

        evening.Games.Add(new EveningGame { Evening = evening, Game = game18Plus, EveningId = evening.Id, GameId = game18Plus.Id });
        
        Assert.True(evening.IsAgeRestricted());

        evening.Games.Add(new EveningGame { Evening = evening, Game = gameAllAges, EveningId = evening.Id, GameId = gameAllAges.Id });
        Assert.True(evening.IsAgeRestricted());
    }

    [Fact]
    public void Evening_IsNotAgeRestricted_WhenOnlyAddingAllAgesGames()
    {
        var address = new Address(1, "Main Street", "City");
        var evening = new Evening(1, "host123", 10, DateOnly.FromDateTime(DateTime.Today.AddDays(7)), null, address);
        var gameAllAges = new Game { Id = 2, Name = "Catan", IsAgeRestricted = false };

        evening.Games.Add(new EveningGame { Evening = evening, Game = gameAllAges, EveningId = evening.Id, GameId = gameAllAges.Id });
        
        Assert.False(evening.IsAgeRestricted());
    }

    [Fact]
    public void User_CannotJoin18PlusEvening_IfUnder18()
    {
        var address = new Address(1, "Main Street", "City");
        var evening = new Evening(1, "host123", 10, DateOnly.FromDateTime(DateTime.Today.AddDays(7)), null, address);
        evening.Games.Add(new EveningGame { Evening = evening, Game = new Game { IsAgeRestricted = true }, EveningId = evening.Id, GameId = 99 }); // Maak avond 18+

        var adultUser = new User("Adult", "adult@example.com", Gender.Other, DateOnly.FromDateTime(DateTime.Today.AddYears(-20)), null, null); // 20 jaar oud
        var minorUser = new User("Minor", "minor@example.com", Gender.Other, DateOnly.FromDateTime(DateTime.Today.AddYears(-15)), null, null); // 15 jaar oud

        Assert.True(evening.CanUserJoin(adultUser));
        
        Assert.False(evening.CanUserJoin(minorUser));
    }

    [Fact]
    public void User_CanJoinNon18PlusEvening_RegardlessOfAge()
    {
        var address = new Address(1, "Main Street", "City");
        var evening = new Evening(1, "host123", 10, DateOnly.FromDateTime(DateTime.Today.AddDays(7)), null, address);
        evening.Games.Add(new EveningGame { Evening = evening, Game = new Game { IsAgeRestricted = false }, EveningId = evening.Id, GameId = 98 }); // Maak avond niet 18+

        var adultUser = new User("Adult", "adult@example.com", Gender.Other, DateOnly.FromDateTime(DateTime.Today.AddYears(-20)), null, null);
        var minorUser = new User("Minor", "minor@example.com", Gender.Other, DateOnly.FromDateTime(DateTime.Today.AddYears(-15)), null, null);

        Assert.True(evening.CanUserJoin(adultUser));
        Assert.True(evening.CanUserJoin(minorUser));
    }
}


public static class EveningExtensions
{
    public static bool IsAgeRestricted(this Evening evening)
    {
        foreach (var eveningGame in evening.Games)
        {
            if (eveningGame.Game != null && eveningGame.Game.IsAgeRestricted)
            {
                return true;
            }
        }
        return false;
    }

    public static bool CanUserJoin(this Evening evening, User user)
    {
        if (evening.IsAgeRestricted())
        {
            
            int age = DateTime.Today.Year - user.DateOfBirth.Year;
            if (user.DateOfBirth > DateOnly.FromDateTime(DateTime.Today.AddYears(-age))) age--;
            return age >= 18;           ;
        }
        return true;
    }
}