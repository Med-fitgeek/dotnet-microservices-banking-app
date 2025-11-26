using System.Text;
using System.Text.Json;
using RabbitMQ.Client;


namespace TransactionService.Messaging
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(string routingKey, T message, CancellationToken ct = default);
    }

    public class RabbitPublisher : IMessagePublisher, IDisposable
    {
        private readonly IConnection _connection;
        private readonly string _exchange;

        public RabbitPublisher(IConnection connection, IConfiguration config)
        {
            _connection = connection;
            _exchange = config["RabbitMq:Exchange"] ?? "banking.events";
        }

        public Task PublishAsync<T>(string routingKey, T message, CancellationToken ct = default)
        {
            using var channel = _connection.CreateModel();

            // declare exchange (idempotent)
            channel.ExchangeDeclare(_exchange, ExchangeType.Topic, durable: true, autoDelete: false);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var props = channel.CreateBasicProperties();
            props.Persistent = true; // delivery_mode = 2
            props.ContentType = "application/json";
            props.MessageId = (message as dynamic)?.MessageId?.ToString() ?? Guid.NewGuid().ToString();
            props.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            channel.BasicPublish(exchange: _exchange,
                                 routingKey: routingKey,
                                 basicProperties: props,
                                 body: body);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            // pas de fermeture ici: la connection est gérée par le DI et fermée à l'application stop
        }
    }

}
