using Domain;

namespace Presentation.ViewModels;

public class EveningFormViewModel
{
    public Evening? Evening { get; set; } 
    public List<Game>? AllGames { get; set; } 
    public List<int> SelectedGameIds { get; set; } = new List<int>(); 

}
