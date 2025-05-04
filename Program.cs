using Scalar.AspNetCore;
using ToDo.Api.Endpoints;
using ToDo.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
// Dodanie usług do kontenera DI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddOpenApi();


var app = builder.Build();

// Konfiguracja middleware
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/api");
}

// Dodanie Scalar do aplikacji
app.MapGet("/", () => Results.Redirect("/api"))
            .ExcludeFromDescription();

// Mapowanie endpointów z wydzielonej klasy
app.MapToDoEndpoints();

// Stosowanie migracji do bazy danych
app.ApplyMigrations();

app.Run();
