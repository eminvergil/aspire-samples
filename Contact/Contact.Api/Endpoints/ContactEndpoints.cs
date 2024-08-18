using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.EntityFrameworkCore;

namespace Contact.Api.Endpoints;

public static class ContactEndpoints
{
    public static void MapContactEndpoints(this WebApplication app)
    {
        app.MapPost("/upload", Upload)
            .WithName("Upload")
            .Produces<IResult>(StatusCodes.Status200OK)
            .WithTags("Contacts")
            .DisableAntiforgery();

        app.MapGet("/", GetItems)
            .WithName("GetItems")
            .Produces<IResult>(StatusCodes.Status200OK)
            .WithTags("Contacts")
            .DisableAntiforgery();
    }

    private static async Task<IResult> Upload(IFormFile file, QueueServiceClient queueServiceClient, BlobServiceClient blobServiceClient, CancellationToken cancellationToken)
    {
        var docsContainer = blobServiceClient.GetBlobContainerClient("fileuploads");
        var queueClient = queueServiceClient.GetQueueClient("contacts");

        var fileKey = Guid.NewGuid().ToString();
        using var fileStream = file.OpenReadStream();
        await docsContainer.UploadBlobAsync(fileKey, fileStream, cancellationToken);
        await queueClient.SendMessageAsync(fileKey, cancellationToken);

        return Results.Ok();
    }

    private static async Task<IResult> GetItems(ContactContext contactContext, CancellationToken cancellationToken)
    {
        var contactModels = await contactContext.Contacts.ToListAsync(cancellationToken);
        return Results.Ok(contactModels);
    }
}
