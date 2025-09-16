using MongoDB.Bson;
using MongoDB.Driver;
using System.ComponentModel.DataAnnotations.Schema;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.AddMongoDBClient(connectionName: "mongodb");

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

app.MapGet("/postForecast", async (HttpRequest request) =>
{

    using var reader = new StreamReader(request.Body);
    var body = await reader.ReadToEndAsync();

    using (var scope = app.Services.CreateScope())
    {
        var mongoClient = scope.ServiceProvider.GetRequiredService<IMongoClient>();

        var database = mongoClient.GetDatabase("mongodb");
        var collection = database.GetCollection<WeatherForecast>("WeatherForecasts");

        // Check if data exists
        var count = collection.CountDocuments(new BsonDocument());

        if (count == 0)
        {
            var weatherForecasts = new List<WeatherForecast>();

            foreach (var index in Enumerable.Range(1, 5))
            {
                var date = DateOnly.FromDateTime(DateTime.Now.AddDays(index));
                var temperatureC = Random.Shared.Next(-20, 55);
                var summary = summaries[Random.Shared.Next(summaries.Length)];

                weatherForecasts.Add(new WeatherForecast
                {
                    Id = Guid.NewGuid().ToString(),
                    Date = date,
                    TemperatureC = temperatureC,
                    Summary = summary
                });
            }

            collection.InsertMany(weatherForecasts);
        }
    }
})
.WithName("GetWeatherForecast");

app.MapDefaultEndpoints();

app.Run();

public class WeatherForecast
{
    public string Id { get; set; }

    public DateOnly Date { get; set; }

    public int TemperatureC { get; set; }

    public string? Summary { get; set; }

    [NotMapped]
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}