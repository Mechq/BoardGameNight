using GraphQL;
using GraphQL.Types;

namespace WebApplication1.GraphQL.Types;

public class EveningQuery : ObjectGraphType
{
    public EveningQuery(EveningService eveningService)
    {
        Field<EveningType>(
            name: "evening",
            arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "id" }),
            resolve: context =>
            {
                var id = context.GetArgument<int>("id");
                var evening = eveningService.GetEvening(id);
        
                
                if (evening == null)
                {
                    Console.WriteLine($"No evening found with id: {id}");
                }
                else
                {
                    Console.WriteLine($"Evening found: {evening.Id}");
                }

                return evening;
            }
        );

        
        Field<ListGraphType<EveningType>>(
            "evenings",
            resolve: context =>
            {
                var evenings = eveningService.GetEvenings();
                Console.WriteLine($"Found {evenings.Count} evenings");
                return evenings;
            } 
        );
    }
}