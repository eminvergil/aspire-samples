var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis");

var libraryDb = builder.AddPostgres("postgres")
    .WithDataVolume()
    .AddDatabase("library");

builder.AddProject<Projects.Library_Api>("library-api")
    .WithReference(redis)
    .WithReference(libraryDb);

builder.Build().Run();
