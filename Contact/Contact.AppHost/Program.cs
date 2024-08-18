using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var contactDb = postgres.AddDatabase("contactdb");

var storage = builder.AddAzureStorage("Storage");

if (builder.Environment.IsDevelopment())
{
    storage.RunAsEmulator(c => c.WithImageTag("3.31.0"));
}

var blobs = storage.AddBlobs("BlobConnection");
var queues = storage.AddQueues("QueueConnection");

builder.AddProject<Projects.Contact_Api>("contact-api")
    .WithReference(blobs)
    .WithReference(queues)
    .WithReference(contactDb);

builder.AddProject<Projects.Contact_Worker>("contact-worker")
    .WithReference(blobs)
    .WithReference(queues)
    .WithReference(contactDb);

builder.Build().Run();
