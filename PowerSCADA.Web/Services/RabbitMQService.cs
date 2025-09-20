using Newtonsoft.Json;
using PowerDF.Core.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace PowerSCADA.Services
{
    // Services/RabbitMQService.cs
    public class RabbitMQService : IRabbitMQService, IAsyncDisposable
    {
        private readonly IConnection _connection;
        private readonly ILogger<RabbitMQService> _logger;
        private IChannel? _channel;
        private string? _queueName;
        private string? _consumerTag;

        public RabbitMQService(IConfiguration configuration, ILogger<RabbitMQService> logger)
        {
            _logger = logger;

            var connectionString = configuration.GetConnectionString("powermes-messagebroker");
            var factory = new ConnectionFactory();

            if (!string.IsNullOrEmpty(connectionString))
            {
                factory.Uri = new Uri(connectionString);
            }

            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        }

        public async Task<IChannel> CreateChannelAsync()
        {
            return await _connection.CreateChannelAsync();
        }

        public async Task SubscribeToAlarmsAsync(Func<Alarm, Task> onAlarmReceived)
        {
            _channel = await CreateChannelAsync();
            var queue = await _channel.QueueDeclareAsync();
            _queueName = queue.QueueName;

            await _channel.QueueBindAsync(_queueName, "alarms-broadcast", "");

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (sender, args) =>
            {
                try
                {
                    var body = args.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var alarm = JsonConvert.DeserializeObject<Alarm>(message);
                    if (alarm != null)
                    {
                        await onAlarmReceived(alarm);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing alarm message");
                }
            };

            _consumerTag = await _channel.BasicConsumeAsync(_queueName, true, consumer);
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                if (_channel != null)
                {
                    // Cancel consumer if it exists
                    if (!string.IsNullOrEmpty(_consumerTag))
                    {
                        await _channel.BasicCancelAsync(_consumerTag);
                    }

                    // Unbind from exchange
                    if (!string.IsNullOrEmpty(_queueName))
                    {
                        await _channel.QueueUnbindAsync(_queueName, "alarms-broadcast", "");

                        // Delete the temporary queue (optional - queues declared without a name are usually temporary)
                        await _channel.QueueDeleteAsync(_queueName);
                    }

                    await _channel.CloseAsync();
                    _channel.Dispose();
                }

                if (_connection != null)
                {
                    await _connection.CloseAsync();
                    _connection.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during RabbitMQ cleanup");
            }
        }
    }
}
