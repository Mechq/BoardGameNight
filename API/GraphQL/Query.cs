using Domain;
using Infrastructure.Data; 
using Microsoft.EntityFrameworkCore; 
using HotChocolate; 

namespace WebApplication1.GraphQL
{
    public class Query
    {
        private readonly GameNightContext _context;

        public Query(GameNightContext context)
        {
            _context = context;
        }

        [GraphQLDescription("Get all evenings.")]
        public IQueryable<Evening> GetAllEvenings() 
        {
            return _context.Evenings.Include(e => e.Address);
        }

        [GraphQLDescription("Get an evening by its ID.")]
        public Evening? GetEvening(int id) 
        {
            return _context.Evenings
                .Include(e => e.Address)
                .Include(e => e.Games)
                .ThenInclude(eg => eg.Game)
                .FirstOrDefault(e => e.Id == id) ?? null;
        }

        [GraphQLDescription("Get all games for a specific evening.")]
        public IQueryable<Game?> GetGamesByEveningId(int id) 
        {
            return _context.EveningGame
                .Where(eg => eg.EveningId == id)
                .Select(eg => eg.Game);
        }

    }

}