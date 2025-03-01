using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.AddElasticsearchClient("elasticsearch", configureClientSettings: clientSettings => clientSettings.DefaultIndex("contents"));

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapContentApi();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // scalar/v1
}

app.UseHttpsRedirection();

app.Run();
