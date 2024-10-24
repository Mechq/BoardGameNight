using Domain;
using Infrastructure.Data;

namespace WebApplication1.GraphQL;


public class EveningService : IEveningService
{
    private readonly GameNightContext _context;

    public EveningService(GameNightContext context)
    {
        _context = context;
    }

    public List<Evening> GetEvenings()
    {
        return _context.Evenings.Select(e => new Evening
        {
            Id = e.Id,
            HostId = e.HostId,
            MaxUsers = e.MaxUsers,
            HostDate = e.HostDate,
            Allergy = e.Allergy,
        }).ToList();
    }

    public Evening? GetEvening(int eveningId)
    {
        var evening = _context.Evenings.FirstOrDefault(e => e.Id == eveningId);
        if (evening == null) return null;

        return new Evening
        {
            Id = evening.Id,
            HostId = evening.HostId,
            MaxUsers = evening.MaxUsers,
            HostDate = evening.HostDate,
            Allergy = evening.Allergy,
        };
    }
}