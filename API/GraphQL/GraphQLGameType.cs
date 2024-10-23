using Domain;

namespace WebApplication1.GraphQL;

public class GraphQLGameType : ObjectType<Game>
{
    protected override void Configure(IObjectTypeDescriptor<Game> descriptor)
    {
        descriptor.Field(a => a.Id).Type<NonNullType<IntType>>();
        descriptor.Field(a => a.Name).Type<NonNullType<StringType>>();
        descriptor.Field(a => a.Description).Type<NonNullType<StringType>>();
        descriptor.Field(a => a.Genre).Type<NonNullType<StringType>>();
        descriptor.Field(a => a.IsAgeRestricted).Type<NonNullType<BooleanType>>();
        descriptor.Field(a => a.TypeOfGame).Type<NonNullType<StringType>>();
        descriptor.Field(a => a.ImageURL).Type<NonNullType<StringType>>();
        
    }
}