using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Http.HttpResults;

public static class ContentExtensions
{
    public static void MapContentApi(this WebApplication app)
    {
        var group = app.MapGroup("/api/content");

        app.MapPost("/insert", async Task<Results<Ok<string>, InternalServerError<string>>> (ContentModel item, ElasticsearchClient client) =>
        {
            var response = await client.IndexAsync(item);
            return response.IsValidResponse
                ? TypedResults.Ok("Document inserted successfully.")
                : TypedResults.InternalServerError(response.DebugInformation);
        });

        app.MapPost("/search", async Task<Results<Ok<IReadOnlyCollection<ContentModel>>, InternalServerError<string>>> (string text, ElasticsearchClient client) =>
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
                : TypedResults.InternalServerError(response.DebugInformation);
        });
    }
}

public record ContentModel(string Id, string Name, string Description);