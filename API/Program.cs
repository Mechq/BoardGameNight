using API.Queries;
using Domain;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IEveningRepository, EveningRepository>();


var identityConnectionString = builder.Configuration.GetConnectionString("IdentityConnection");
var gameNightConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddDbContext<IdentityContext>(options =>
    options.UseSqlServer(identityConnectionString));


builder.Services.AddDbContext<GameNightContext>(options =>
    options.UseSqlServer(gameNightConnectionString));


builder.Services.AddAuthorization();


builder.Services.AddIdentityApiEndpoints<User>()
    .AddEntityFrameworkStores<IdentityContext>();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<EveningQueries>();


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

   
}

app.UseHttpsRedirection();
app.MapGraphQL();
app.UseAuthorization();
app.MapControllers();
app.MapIdentityApi<User>();

app.Run();