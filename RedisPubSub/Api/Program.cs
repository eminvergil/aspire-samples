using Api.Models;
using Microsoft.AspNetCore.Mvc;
using RedisPubSub.ServiceDefaults;
using Scalar.AspNetCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.AddRedisDistributedCache("redis");

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapPost("/send-message", async ([FromBody] SendMessageRequest request, IConnectionMultiplexer connectionMultiplexer) =>
{
    await connectionMultiplexer.GetSubscriber().PublishAsync(Constants.DefaultChannel, request.Message);
    return Results.Ok();
});

app.Run();
