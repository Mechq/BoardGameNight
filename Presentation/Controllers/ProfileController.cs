using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using System.Threading.Tasks;
using Domain;
using Infrastructure.Data;
using Presentation.ViewModels;

namespace Presentation.Controllers
{
    public class ProfileController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        
        private readonly IdentityContext _identityContext;

        public ProfileController(SignInManager<User> signInManager, UserManager<User> userManager, IdentityContext identityContext)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _identityContext = identityContext;
        }
        
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _identityContext.Users.FindAsync(userId); 
            if (user == null)
            {
                return NotFound(); 
            }
            return View(user); 
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                var user = await _userManager.FindByEmailAsync(model.Email ?? throw new ArgumentNullException(nameof(model.Email)));
                if (user != null)
                {
                    
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password ?? throw new ArgumentNullException(nameof(model.Password)), model.RememberMe, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        Console.WriteLine("User logged in successfully.");

                        
                        await _signInManager.SignInAsync(user, isPersistent: model.RememberMe);

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            // Logging errors if model state is invalid
            foreach (var err in ModelState)
            {
                Console.WriteLine(err);
            }

            return View(model);
        }


        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User 
                { 
                    UserName = model.Email, 
                    Email = model.Email, 
                    Name = model.Name ?? throw new ArgumentNullException(nameof(model.Name)),
                    Gender = model.Gender,
                    DateOfBirth = model.DateOfBirth,
                    Diet = model.Diet,
                    AddressId = model.AddressId
                }; 

                var result = await _userManager.CreateAsync(user, model.Password?? throw new ArgumentNullException(nameof(model.Password)));

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home"); 
                }

                // Add validation errors to the model state
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            if (!ModelState.IsValid)
            {
                
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                foreach (var error in errors)
                {
                    Console.WriteLine(error);
                }
            }
            return View(model); 
        }

        
        
        

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home"); 
        }
    }
}