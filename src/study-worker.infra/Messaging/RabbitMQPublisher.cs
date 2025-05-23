using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace study_worker.infra.Messaging
{
    public class RabbitMQMessagePublisher : IMessagePublisher, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _exchangeName;
        private bool _disposed;

        public RabbitMQMessagePublisher(RabbitMQSettings settings, string exchangeName = "message_exchange")
        {
            var factory = new ConnectionFactory
            {
                HostName = settings.HostName,
                UserName = settings.UserName,
                Password = settings.Password,
                Port = settings.Port,
                VirtualHost = settings.VirtualHost
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _exchangeName = exchangeName;

            // Declare exchange if it doesn't exist
            _channel.ExchangeDeclare(
                exchange: _exchangeName,
                type: "topic",
                durable: true,
                autoDelete: false,
                arguments: null);
        }

        public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
        {
            // Use the message type name as default routing key
            string routingKey = typeof(T).Name;
            return PublishAsync(message, routingKey, cancellationToken);
        }

        public Task PublishAsync<T>(T message, string routingKey, CancellationToken cancellationToken = default) where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (string.IsNullOrEmpty(routingKey))
                throw new ArgumentException("Routing key is required", nameof(routingKey));

            // Serialize the message to JSON
            string messageJson = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(messageJson);

            // Create persistent message properties
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";
            properties.MessageId = Guid.NewGuid().ToString();
            properties.Type = typeof(T).FullName;
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            // Publish the message
            _channel.BasicPublish(
                exchange: _exchangeName,
                routingKey: routingKey,
                basicProperties: properties,
                body: body);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _channel?.Close();
                _connection?.Close();

                _channel?.Dispose();
                _connection?.Dispose();
            }

            _disposed = true;
        }
    }
}
