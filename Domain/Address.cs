namespace Domain;

public class Address
{
    private int id { set; get; }
    private int HouseNumber { set; get; }
    private string Street { set; get; }
    private string City { set; get; }

    public Address( int houseNumber, string street, string city)
    {
        HouseNumber = houseNumber;
        Street = street;
        City = city;
    }
}