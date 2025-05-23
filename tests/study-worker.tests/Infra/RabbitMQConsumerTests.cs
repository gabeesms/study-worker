using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using study_worker.infra.Messaging;
using System.Text;

namespace study_worker.tests.Infra
{
    public class RabbitMQConsumerTests
    {
        [Fact]
        public async Task ExecuteAsync_DeveProcessarMensagem_QuandoMensagemValidaRecebida()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<RabbitMQConsumer>>();
            var connectionMock = new Mock<IConnection>();
            var channelMock = new Mock<IModel>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            var scopeMock = new Mock<IServiceScope>();
            var scopeServiceProviderMock = new Mock<IServiceProvider>();
            var scopeFactoryMock = new Mock<IServiceScopeFactory>();

            connectionMock.Setup(x => x.CreateModel()).Returns(channelMock.Object);
            serviceProviderMock.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(scopeFactoryMock.Object);
            scopeFactoryMock.Setup(x => x.CreateScope()).Returns(scopeMock.Object);
            scopeMock.Setup(x => x.ServiceProvider).Returns(scopeServiceProviderMock.Object);

            // Mock da fábrica de conexão para retornar nossos mocks
            var settings = new RabbitMQSettings
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                Port = 5672,
                VirtualHost = "/"
            };

            // Criar uma instância de teste com injeção de dependências dos nossos mocks
            var consumer = new TestableRabbitMQConsumer(
                settings,
                serviceProviderMock.Object,
                loggerMock.Object,
                connectionMock.Object,
                channelMock.Object);

            // Criar payload de teste
            var payload = new
            {
                EventType = "StudentEnrolled",
                StudentId = 1,
                CourseId = 2,
                EnrolledAt = "2025-05-23T19:42:50.7203567Z"
            };
            var messageBody = JsonConvert.SerializeObject(payload);
            var messageBytes = Encoding.UTF8.GetBytes(messageBody);

            // Configurar o canal para capturar o consumidor que é registrado
            BasicDeliverEventArgs eventArgs = null;
            EventingBasicConsumer capturedConsumer = null;

            channelMock
                .Setup(x => x.BasicConsume(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IDictionary<string, object>>(), It.IsAny<IBasicConsumer>()))
                .Callback<string, bool, string, bool, bool, IDictionary<string, object>, IBasicConsumer>((queue, autoAck, consumerTag, noLocal, exclusive, arguments, consumer) =>
                {
                    capturedConsumer = consumer as EventingBasicConsumer;

                    // Criar argumentos de entrega com nossa mensagem de teste
                    eventArgs = new BasicDeliverEventArgs
                    {
                        DeliveryTag = 1,
                        Body = new ReadOnlyMemory<byte>(messageBytes)
                    };
                });

            // Act
            await consumer.StartAsync(CancellationToken.None);

            // Simular recebimento de mensagem se o consumidor foi capturado
            if (capturedConsumer != null)
            {
                capturedConsumer.HandleBasicDeliver(
                    "consumerTag",
                    eventArgs.DeliveryTag,
                    false,
                    "exchange",
                    "routingKey",
                    null,
                    eventArgs.Body);
            }

            // Assert
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains(messageBody)),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            channelMock.Verify(x => x.BasicAck(eventArgs.DeliveryTag, false), Times.Once);

            await consumer.StopAsync(CancellationToken.None);
        }

        [Fact]
        public async Task ExecuteAsync_DeveTratarExcecao_QuandoProcessamentoFalha()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<RabbitMQConsumer>>();
            var connectionMock = new Mock<IConnection>();
            var channelMock = new Mock<IModel>();
            var serviceProviderMock = new Mock<IServiceProvider>();

            connectionMock.Setup(x => x.CreateModel()).Returns(channelMock.Object);

            // Criar um service provider que lança exceção ao criar scope
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Throws(new Exception("Exceção de teste"));

            var settings = new RabbitMQSettings
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                Port = 5672,
                VirtualHost = "/"
            };

            // Criar uma instância de teste com injeção de dependências dos nossos mocks
            var consumer = new TestableRabbitMQConsumer(
                settings,
                serviceProviderMock.Object,
                loggerMock.Object,
                connectionMock.Object,
                channelMock.Object);

            // Criar payload de teste
            var payload = new
            {
                EventType = "StudentEnrolled",
                StudentId = 1,
                CourseId = 2,
                EnrolledAt = "2025-05-23T19:42:50.7203567Z"
            };
            var messageBody = JsonConvert.SerializeObject(payload);
            var messageBytes = Encoding.UTF8.GetBytes(messageBody);

            // Configurar o canal para capturar o consumidor que é registrado
            BasicDeliverEventArgs eventArgs = null;
            EventingBasicConsumer capturedConsumer = null;

            channelMock
                .Setup(x => x.BasicConsume(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IDictionary<string, object>>(), It.IsAny<IBasicConsumer>()))
                .Callback<string, bool, string, bool, bool, IDictionary<string, object>, IBasicConsumer>((queue, autoAck, consumerTag, noLocal, exclusive, arguments, consumer) =>
                {
                    capturedConsumer = consumer as EventingBasicConsumer;

                    // Criar argumentos de entrega com nossa mensagem de teste
                    eventArgs = new BasicDeliverEventArgs
                    {
                        DeliveryTag = 1,
                        Body = new ReadOnlyMemory<byte>(messageBytes)
                    };
                });

            // Act
            await consumer.StartAsync(CancellationToken.None);

            // Simular recebimento de mensagem se o consumidor foi capturado
            if (capturedConsumer != null)
            {
                capturedConsumer.HandleBasicDeliver(
                    "consumerTag",
                    eventArgs.DeliveryTag,
                    false,
                    "exchange",
                    "routingKey",
                    null,
                    eventArgs.Body);
            }

            // Assert
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            channelMock.Verify(x => x.BasicNack(eventArgs.DeliveryTag, false, true), Times.Once);

            await consumer.StopAsync(CancellationToken.None);
        }
    }

    // Versão testável do RabbitMQConsumer que permite injetar mocks
    public class TestableRabbitMQConsumer : RabbitMQConsumer
    {
        public TestableRabbitMQConsumer(
            RabbitMQSettings settings,
            IServiceProvider serviceProvider,
            ILogger<RabbitMQConsumer> logger,
            IConnection connection,
            IModel channel)
            : base(settings, serviceProvider, logger)
        {
            // Usar reflexão para definir os campos privados com nossos mocks
            var connectionField = typeof(RabbitMQConsumer).GetField("_connection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var channelField = typeof(RabbitMQConsumer).GetField("_channel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            connectionField?.SetValue(this, connection);
            channelField?.SetValue(this, channel);
        }
    }
}

