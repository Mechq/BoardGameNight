namespace Domain;

public class Game
{
    private int Id { set; get; }
    private string Name { set; get; }
    private string Description { set; get; }
    private Genre Genre { set; get; }
    private string ImageURL { set; get; }
    private GameType TypeOfGame { set; get; }
    private Boolean IsAgeRestricted { set; get; }
}