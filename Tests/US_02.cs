namespace Tests;

using Domain;
using Xunit;
using System;
using System.Collections.Generic;


public class EveningManagementTest
{
    [Fact]
    public void Evening_CanBeModified_WhenNoParticipants()
    {
        var address = new Address(1, "Main Street", "City");
        var evening = new Evening(1, "host123", 10, DateOnly.FromDateTime(DateTime.Today.AddDays(7)), null, address);

        Assert.Empty(evening.Participants);
        
    }

    [Fact]
    public void Evening_CannotBeModified_WhenHasParticipants()
    {
        var address = new Address(1, "Main Street", "City");
        var evening = new Evening(1, "host123", 10, DateOnly.FromDateTime(DateTime.Today.AddDays(7)), null, address);
        
        evening.Participants.Add(new EveningParticipant { ParticipantId = "player1", EveningId = evening.Id });
        
        Assert.NotEmpty(evening.Participants);
        Assert.Single(evening.Participants);
    }
}