using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Contact.Api;
using Contact.Api.Endpoints;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddAzureBlobClient("BlobConnection");
builder.AddAzureQueueClient("QueueConnection");

builder.AddNpgsqlDbContext<ContactContext>("contactdb");

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapContactEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    var blobService = app.Services.GetRequiredService<BlobServiceClient>();
    var docsContainer = blobService.GetBlobContainerClient("fileuploads");

    await docsContainer.CreateIfNotExistsAsync();

    var queueService = app.Services.GetRequiredService<QueueServiceClient>();
    var queueClient = queueService.GetQueueClient("contacts");

    await queueClient.CreateIfNotExistsAsync();

    await using var scope = app.Services.CreateAsyncScope();
    var contactContext = scope.ServiceProvider.GetRequiredService<ContactContext>();
    await contactContext.Database.EnsureCreatedAsync();
}

app.UseHttpsRedirection();

app.Run();
