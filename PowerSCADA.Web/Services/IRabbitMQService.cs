using PowerDF.Core.Models;
using RabbitMQ.Client;

namespace PowerSCADA.Services
{
    public interface IRabbitMQService
    {
        Task<IChannel> CreateChannelAsync();
        Task SubscribeToAlarmsAsync(Func<Alarm, Task> onAlarmReceived);
    }
}
