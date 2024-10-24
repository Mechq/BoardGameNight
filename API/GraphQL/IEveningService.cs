using Domain;

namespace WebApplication1.GraphQL;

public interface IEveningService
{
    List<Evening> GetEvenings();
    Evening? GetEvening(int eveningId);
}