using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.GraphQL.Types;

namespace WebApplication1.GraphQL;

[Route("/graphql")]
public class GraphQLController : Controller
{
    private readonly EveningService _eveningService;

    public GraphQLController(EveningService eveningService)
    {
        _eveningService = eveningService;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] GraphQLQuery graphQlQuery)
    {
        var schema = new Schema { Query = new EveningQuery(_eveningService) };
        var result = await new DocumentExecuter().ExecuteAsync(x =>
        {
            x.Schema = schema;
            x.Query = graphQlQuery.Query;
            /*x.Inputs = graphQlQuery.Variables;*/
        });

        if (result.Errors?.Count > 0)
        {
            // Log the errors for better debugging
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"Error: {error.Message}");
            }
            return BadRequest(result.Errors);
        }

        return Ok(result);
    }
}
