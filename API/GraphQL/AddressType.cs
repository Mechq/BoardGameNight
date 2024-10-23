using Domain;

namespace WebApplication1.GraphQL;

public class AddressType : ObjectType<Address>
{
    protected override void Configure(IObjectTypeDescriptor<Address> descriptor)
    {
        descriptor.Field(a => a.Id).Type<NonNullType<IntType>>();
        descriptor.Field(a => a.Street).Type<NonNullType<StringType>>();
        descriptor.Field(a => a.City).Type<NonNullType<StringType>>();
    }
}
