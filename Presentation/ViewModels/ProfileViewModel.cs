using Domain;

namespace Presentation.ViewModels;

public class ProfileViewModel
{
    public User? User { get; set; }
    public Address Address { get; set; } = new Address();
}