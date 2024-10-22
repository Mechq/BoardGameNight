using Domain;

namespace Presentation.ViewModels;

public class GameNightViewModel
{
    public Evening? GameNight { get; set; }
    public User? Host { get; set; }
    public List<User>? Participants { get; set; }
}