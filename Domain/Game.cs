namespace Domain;

public class Game
{
    public int Id { set; get; }
    public string Name { set; get; }
    public string Description { set; get; }
    public Genre Genre { set; get; }
    public string ImageURL { set; get; }
    public GameType TypeOfGame { set; get; }
    public bool IsAgeRestricted { set; get; }
}