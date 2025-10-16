using Domain;
using Infrastructure.Repositories;

namespace API.Queries;

public class EveningQueries
{
    public IEnumerable <Evening> GetEvenings([Service] IEveningRepository eveningRepository) => eveningRepository.GetAll();
    public Evening GetEvening([Service] IEveningRepository eveningRepository, int id) => eveningRepository.GetById(id);

}