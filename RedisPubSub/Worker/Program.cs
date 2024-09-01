var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.AddRedisDistributedCache("redis");
builder.Services.AddHostedService<Worker.Worker>();

var host = builder.Build();
await host.RunAsync();
