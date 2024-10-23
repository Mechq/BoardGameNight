using Domain;
using HotChocolate.Types;
using Infrastructure.Data;

namespace WebApplication1.GraphQL;



public class EveningType : ObjectType<Evening>
{
    protected override void Configure(IObjectTypeDescriptor<Evening> descriptor)
    {
        descriptor.Field(e => e.Id);
        descriptor.Field(e => e.HostId);
        descriptor.Field(e => e.MaxUsers);
        descriptor.Field(e => e.HostDate);
        descriptor.Field(e => e.Allergy);
        descriptor.Field(e => e.Address).Type<AddressType>();

        descriptor.Field("games")
            .Argument("id", a => a.Type<NonNullType<IntType>>()) 
            .ResolveWith<Query>(x => x.GetGamesByEveningId(default))
            .Type<ListType<NonNullType<GraphQLGameType>>>();


    }
}
