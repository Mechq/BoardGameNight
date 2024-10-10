using System.Diagnostics;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;

namespace Presentation.Controllers;

public class GameNightsController : Controller
{
    private readonly ILogger<GameNightsController> _logger;
    
    

    public GameNightsController(ILogger<GameNightsController> logger)
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

    public IActionResult Detailpage(int id)
    {
        if (id == 0)
        {
            return NotFound();
        }
        //temp code
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
        Evening detailGameNight = gameNightList.Find(e => e.Id == id);
        return View(detailGameNight);
    }

    
    
    //todo implement form
    /*public IActionResult Create()
    {
        // Populate necessary dropdowns (e.g., Hosts, Games, Participants)
        ViewBag.Hosts = new SelectList(_userRepository.GetAll(), "Id", "Name");
        ViewBag.BoardGames = new SelectList(_gameRepository.GetAll(), "Id", "Title");
        ViewBag.Participants = new SelectList(_userRepository.GetAll(), "Id", "Name");

        return View();
    }

    [HttpPost]
    public IActionResult Create(Evening newEvening)
    {
        if (ModelState.IsValid)
        {
            _eveningRepository.Add(newEvening);
            return RedirectToAction("Index");
        }

        // Repopulate dropdowns if the form submission fails
        ViewBag.Hosts = new SelectList(_userRepository.GetAll(), "Id", "Name");
        ViewBag.BoardGames = new SelectList(_gameRepository.GetAll(), "Id", "Title");
        ViewBag.Participants = new SelectList(_userRepository.GetAll(), "Id", "Name");

        return View(newEvening);
    }*/

   

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}