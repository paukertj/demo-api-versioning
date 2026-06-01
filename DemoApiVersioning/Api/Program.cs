using Api;
using Core;
using Infrastructure.Database;
using Infrastructure.Database.Options;
using Majipro.Converter;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services
    .AddOpenApi()
    .AddApi()
    .AddCore()
    .AddDatabase()
    .AddConverting(typeof(Program).Assembly);

builder.Services.AddControllers();

var app = builder.Build();

var databaseOptions = app.Services.GetRequiredService<IOptions<DatabaseOptions>>().Value;
if (databaseOptions.ApplyMigrationsOnStartup)
{
    await DatabaseInitializer.MigrateAsync(app.Services);
}

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Settings API v1");
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
