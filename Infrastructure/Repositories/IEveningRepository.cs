using Domain;

namespace Infrastructure.Repositories;

public interface IEveningRepository
{
    IEnumerable<Evening> GetAll();
    Evening GetById(int id);
    
    Task<List<Evening>>  GetAllFuture();
    
    void Enroll(int eveningId, string participantId);
    void Unroll(int eveningId, string participantId);
}
