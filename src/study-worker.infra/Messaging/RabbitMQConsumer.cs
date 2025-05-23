using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace study_worker.infra.Messaging
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly RabbitMQ.Client.IModel _channel;
        private readonly string _queueName;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RabbitMQConsumer> _logger;

        public RabbitMQConsumer(RabbitMQSettings settings, IServiceProvider serviceProvider,
                               ILogger<RabbitMQConsumer> logger)
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
            _queueName = "course_events";
            _serviceProvider = serviceProvider;
            _logger = logger;

            _channel.QueueDeclare(queue: _queueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation($"Received message: {message}");

                try
                {
                    ProcessMessage(message);
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message");
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            _channel.BasicConsume(queue: _queueName,
                                 autoAck: false,
                                 consumer: consumer);

            return Task.CompletedTask;
        }

        private void ProcessMessage(string message)
        {
            // Process message using services
            using (var scope = _serviceProvider.CreateScope())
            {
                // You can get your service here
                // var courseService = scope.ServiceProvider.GetRequiredService<ICourseService>();
                // courseService.ProcessMessage(message);
            }
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();

            _channel?.Dispose();
            _connection?.Dispose();

            base.Dispose();
        }
    }
}
