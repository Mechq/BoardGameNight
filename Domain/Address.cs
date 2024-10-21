namespace Domain;

public class Address
{
    public int Id { set; get; }
    public int HouseNumber { set; get; }
    public string Street { set; get; }
    public string City { set; get; }
    public ICollection<User> Users { get; set; } = new List<User>();

    public Address()
    {
    }
    public Address(int houseNumber, string street, string city)
    {
        HouseNumber = houseNumber;
        Street = street;
        City = city;
    }
}