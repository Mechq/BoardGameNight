namespace Domain;

public class User
{
    private int Id { set; get; }
    private string Name { set; get; }
    private string EmailAddress { set; get; }
    private Gender Gender { set; get; }
    private DateOnly DateOfBirth { set; get; }
    private string Diet { set; get; }
    private Address Address { set; get; }

    public User(string name, string emailAddress, Gender gender, DateOnly dateOfBirth, string diet, Address address)
    {
        Name = name;
        EmailAddress = emailAddress;
        Gender = gender;
        DateOfBirth = dateOfBirth;
        Diet = diet;
        Address = address;
    }
}