namespace Domain;

public class Evening
{
    public Evening(int id, User host, int maxUsers, DateOnly hostDate, string? allergy, Address address)
    {
        Id = id;
        Host = host;
        MaxUsers = maxUsers;
        HostDate = hostDate;
        Allergy = allergy;
        Address = address;
    }

    public int Id { set; get; }
    public User Host { set; get; }
    public int MaxUsers { set; get; }
    public DateOnly HostDate { set; get; }
    public string? Allergy { set; get; }
    public Address Address { set; get; }
    
    public List<Game> BoardGames { set; get; }
    public List<User> Participants { set; get; }
}