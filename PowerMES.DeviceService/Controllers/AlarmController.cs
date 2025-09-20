using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PowerDF.Core.Models;
using RabbitMQ.Client;
using System.Text;

namespace PowerMES.DeviceService.Controllers;

[ApiController]
[Route("[controller]")]
public class AlarmController : ControllerBase
{
    private readonly ILogger<AlarmController> _logger;
    private readonly IConnection _connection;

    public AlarmController(ILogger<AlarmController> logger, IConnection connection)
    {
        _logger = logger;
        _connection = connection;
    }

    [HttpPost(Name = "PostAlarm")]
    public async void Post(Alarm alarm)
    {
        var props = new BasicProperties();
        var body = JsonConvert.SerializeObject(alarm);
        var bodyBytes = Encoding.UTF8.GetBytes(body);

        var channel = await _connection.CreateChannelAsync();
        await channel.ExchangeDeclareAsync("alarms-broadcast", ExchangeType.Fanout, false, false);
        await channel.BasicPublishAsync("alarms-broadcast", "", false, props, bodyBytes);
    }
}