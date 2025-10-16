using Domain;

namespace Tests;

public class EveningTest
{
    [Fact]
    public void Evening_DefaultConstructor_InitializesCollections()
    {
        var evening = new Evening();
        Assert.NotNull(evening.Games);
        Assert.NotNull(evening.Participants);
    }

    [Fact]
    public void Evening_ParameterizedConstructor_SetsProperties()
    {
        var address = new Address();
        var evening = new Evening(1, "host123", 10, new DateOnly(2023, 10, 1), "Peanuts", address);

        Assert.Equal(1, evening.Id);
        Assert.Equal("host123", evening.HostId);
        Assert.Equal(10, evening.MaxUsers);
        Assert.Equal(new DateOnly(2023, 10, 1), evening.HostDate);
        Assert.Equal("Peanuts", evening.Allergy);
        Assert.Equal(address, evening.Address);
    }

    [Fact]
    public void Evening_ParameterizedConstructor_AllowsNullAllergy()
    {
        var address = new Address();
        var evening = new Evening(1, "host123", 10, new DateOnly(2023, 10, 1), null, address);

        Assert.Null(evening.Allergy);
    }

    [Fact]
    public void Evening_SetAddressId_UpdatesAddressId()
    {
        var evening = new Evening();
        evening.AddressId = 5;
        Assert.Equal(5, evening.AddressId);
    }
}