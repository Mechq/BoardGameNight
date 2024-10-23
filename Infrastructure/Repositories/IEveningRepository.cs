using Domain;

namespace Infrastructure.Repositories;

public interface IEveningRepository
{
    IEnumerable<Evening> GetAll();
    Evening GetById(int id);
}
