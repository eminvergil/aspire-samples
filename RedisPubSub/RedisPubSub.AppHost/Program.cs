var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis");

builder.AddProject<Projects.Api>("api")
    .WithReference(redis)
    .WithReplicas(1);

builder.AddProject<Projects.Worker>("worker")
    .WithReference(redis);

await builder.Build().RunAsync();
