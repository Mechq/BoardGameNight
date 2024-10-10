using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using Domain;

namespace Presentation.Controllers;

public class HostController : Controller
{
    private readonly ILogger<HostController> _logger;

    public HostController(ILogger<HostController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var user = new User("Stef Rensma", 
            "stefrensa@gmail.com", 
            Gender.Man, 
            new DateOnly(2005, 08, 27),
            "",
            new Address(1, "Patrijs", "Barendrecht"));
        var gameNight = new Evening(1, user, 8, new DateOnly(2024, 11, 7), "",
            new Address(2, "Sandelhout", "Barendrecht"));
        var gameNight2 = new Evening(2, user, 8, new DateOnly(2024, 11, 7), "",
            new Address(2, "Sandelhout", "Barendrecht"));
        List<Evening> gameNightList = new List<Evening>() { gameNight, gameNight2 };
        return View(gameNightList);
    }
    public IActionResult Form()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}