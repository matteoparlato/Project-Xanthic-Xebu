namespace PowerDF.Core.Models;

public class Telemetry
{
    public string Id { get; set; }

    public string DeviceId { get; set; }

    public string Payload { get; set; }

    public DateOnly Date { get; set; }
}
