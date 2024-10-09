namespace Domain;

public class Address
{
    private int Id { set; get; }
    public int HouseNumber { set; get; }
    public string Street { set; get; }
    public string City { set; get; }

    public Address( int houseNumber, string street, string city)
    {
        HouseNumber = houseNumber;
        Street = street;
        City = city;
    }
}