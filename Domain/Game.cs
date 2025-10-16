using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public class Game
{
    public Game()
    {
        EveningGames = new List<EveningGame>();
    }
    public int Id { set; get; }
    public string Name { set; get; }
    public string Description { set; get; }
    
    [Column(TypeName = "nvarchar(50)")]    
    public Genre Genre { set; get; }
    
    public string ImageURL { set; get; }
    
    [Column(TypeName = "nvarchar(50)")]
    public GameType TypeOfGame { set; get; }
    
    public bool IsAgeRestricted { set; get; }
    public List<EveningGame> EveningGames { get; set; }
}