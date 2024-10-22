using Domain;

namespace Presentation.ViewModels;

public class GameNightViewModel
{
    public Evening? GameNight { get; set; }
    public User? Host { get; set; }
    public List<User>? Participants { get; set; }
    public List<Game>? Games { get; set; }
    public Dictionary<string, double>? Attendance { get; set; }
}