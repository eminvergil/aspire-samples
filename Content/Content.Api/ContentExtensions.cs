using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Http.HttpResults;

public static class ContentExtensions
{
    public static void MapContentApi(this WebApplication app)
    {
        var group = app.MapGroup("/api/content");

        app.MapPost("/insert", async Task<Results<Ok<string>, ProblemHttpResult>> (ContentModel item, ElasticsearchClient client) =>
        {
            var response = await client.IndexAsync(item);
            return response.IsValidResponse
                ? TypedResults.Ok("Document inserted successfully.")
                : TypedResults.Problem(response.DebugInformation);
        });

        app.MapPost("/search", async Task<Results<Ok<IReadOnlyCollection<ContentModel>>, ProblemHttpResult>> (string text, ElasticsearchClient client) =>
        {
            var response = await client.SearchAsync<ContentModel>(s => s
                .Query(q => q
                    .Wildcard(w => w
                        .Field(i => i.Name)
                        .Value($"*{text}*")
                    )
                )
            );

            return response.IsValidResponse
                ? TypedResults.Ok(response.Documents)
                : TypedResults.Problem(response.DebugInformation);
        });
    }
}

public record ContentModel(string Id, string Name, string Description);