namespace study_worker.infra.Messaging
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;
        Task PublishAsync<T>(T message, string routingKey, CancellationToken cancellationToken = default) where T : class;
    }
}
