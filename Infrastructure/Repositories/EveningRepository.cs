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
        return _context.Evenings
            .Include(e => e.Games)            
            .ThenInclude(eg => eg.Game)
            .ToList();
    }

    public Evening? GetById(int id)
    {
        return _context.Evenings
            .Include(e => e.Games)
            .FirstOrDefault(e => e.Id == id); 
    }

    public async Task<List<Evening>> GetAllFuture()
    {
        return await _context.Evenings
            .Include(e => e.Address)
            .Include(e => e.Participants)
            .Where(e => e.Participants.Count < e.MaxUsers && e.HostDate > DateOnly.FromDateTime(DateTime.Now)) 
            .OrderBy(e => e.HostDate)
            .ToListAsync();
    }

    public void Enroll(int eveningId, string participantId)
    {
        var evening = _context.Evenings
            .Include(e => e.Participants)
            .FirstOrDefault(e => e.Id == eveningId);

        if (evening == null)
        {
            throw new Exception("Evening not found");
        }

        if (evening.Participants.Any(p => p.ParticipantId == participantId))
        {
            throw new Exception("Participant already enrolled");
        }

        evening.Participants.Add(new EveningParticipant
        {
            EveningId = eveningId,
            ParticipantId = participantId
        });

        _context.SaveChanges();
    }
    
    public void Unroll(int eveningId, string participantId)
    {
        var evening = _context.Evenings
            .Include(e => e.Participants)
            .FirstOrDefault(e => e.Id == eveningId);

        if (evening == null)
        {
            throw new Exception("Evening not found");
        }

        var participant = evening.Participants.FirstOrDefault(p => p.ParticipantId == participantId);
        if (participant == null)
        {
            throw new Exception("Participant not enrolled");
        }

        evening.Participants.Remove(participant);
        _context.SaveChanges();
    }
  
    
}
