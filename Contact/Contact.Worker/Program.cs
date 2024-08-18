using Contact.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.AddAzureBlobClient("BlobConnection");
builder.AddAzureQueueClient("QueueConnection");

builder.AddNpgsqlDbContext<ContactContext>("contactdb");

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
