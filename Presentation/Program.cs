using Domain;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();

var DefaultConnection = String.Empty;
var IdentityConnection = String.Empty;


    DefaultConnection = Environment.GetEnvironmentVariable("DEFAULTCONNECTION");
    IdentityConnection = Environment.GetEnvironmentVariable("IDENTITYCONNECTION");

    builder.Configuration["ConnectionStrings:DefaultConnection"] = DefaultConnection;
    builder.Configuration["ConnectionStrings:IdentityConnection"] = IdentityConnection;

builder.Services.AddDbContext<GameNightContext>(options =>
    options.UseSqlServer(DefaultConnection));

builder.Services.AddDbContext<IdentityContext>(options =>
    options.UseSqlServer(IdentityConnection));

builder.Services.AddIdentity<User, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 6; 
        options.Password.RequireDigit = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    })
    .AddEntityFrameworkStores<IdentityContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Profile/Login"; 
    options.AccessDeniedPath = "/Home/AccessDenied";
    options.Cookie.Name = ".AspNetCore.Identity.Application"; 
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true; 
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();



var app = builder.Build();  


if (!app.Environment.IsDevelopment())
{   
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication(); 
app.UseAuthorization();  

// Define the default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();