using Domain;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EveningRepository : IEveningRepository
{
    private readonly GameNightContext _context;

    public EveningRepository(GameNightContext context)
    {
        _context = context;
    }

    public IEnumerable<Evening> GetAll()
    {
        return _context.Evenings.Include(e => e.Games).ToList(); // Include games if needed
    }

    public Evening? GetById(int id)
    {
        return _context.Evenings.Include(e => e.Games)
            .FirstOrDefault(e => e.Id == id); 
    }
}
