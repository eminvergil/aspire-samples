var builder = DistributedApplication.CreateBuilder(args);

var elasticsearch = builder.AddElasticsearch("elasticsearch")
    .WithDataVolume();

builder.AddProject<Projects.Content_Api>("content-api")
    .WithReference(elasticsearch);

builder.Build().Run();
