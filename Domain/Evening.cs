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

    private int Id { set; get; }
    private User Host { set; get; }
    private int MaxUsers { set; get; }
    private DateOnly HostDate { set; get; }
    private string? Allergy { set; get; }
    private Address Address { set; get; }
}