using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using PowerDF.Core.Models;
using RabbitMQ.Client;

namespace PowerMES.DeviceService.Controllers;

[ApiController]
[Route("[controller]")]
public class TelemetryController : ControllerBase
{
    private readonly ILogger<TelemetryController> _logger;
    private readonly IMongoCollection<Telemetry> _telemetryCollection;
    private readonly IConnection _connection;

    public TelemetryController(ILogger<TelemetryController> logger, IMongoClient mongoClient, IConnection connection)
    {
        _logger = logger;
        _connection = connection;

        var database = mongoClient.GetDatabase("powermes");
        _telemetryCollection = database.GetCollection<Telemetry>("telemetry");
    }

    [HttpGet(Name = "GetTelemetry")]
    public async Task<IEnumerable<Telemetry>> Get()
    {
        return await _telemetryCollection.Find(_ => true).ToListAsync();
    }

    [HttpPost(Name = "PostTelemetry")]
    public async void Post(Telemetry telemetry)
    {
        await _telemetryCollection.InsertOneAsync(telemetry);
    }
}
