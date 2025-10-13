using Domain;

namespace Infrastructure.Repositories;

public interface IEveningRepository
{
    IEnumerable<Evening> GetAll();
    Evening GetById(int id);
    
    void Enroll(int eveningId, string participantId);
    void Unroll(int eveningId, string participantId);
}
