using Domain;
using GraphQL.Types;

namespace WebApplication1.GraphQL.Types;

public class EveningType : ObjectGraphType<Evening>
{
    public EveningType()
    {
        Field(x => x.Id);
        Field(x => x.HostId, nullable:true);
        Field(x => x.MaxUsers);
        Field(x => x.HostDate);
        Field(x => x.Allergy, nullable: true);
    }
}