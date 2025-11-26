using Microsoft.EntityFrameworkCore;
using AccountService.Data;
using AccountService.Models;
using AccountService.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using DineroBank.Shared.DTOs.Transaction;

namespace AccountService.Messaging
{
    public class TransactionConsumer : BackgroundService
    {
        private readonly IConnection _connection;
        private IModel _channel;
        private readonly IServiceScopeFactory _scopeFactory; // for scoped services like DbContext
        private readonly ILogger<TransactionConsumer> _logger;
        private const string QueueName = "account.transaction.created.queue";

        public TransactionConsumer(IConnection connection,
                                   IServiceScopeFactory scopeFactory,
                                   ILogger<TransactionConsumer> logger)
        {
            _connection = connection;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _connection.CreateModel();

            const string exchange = "banking.events";

            // Exchange
            _channel.ExchangeDeclare(exchange, ExchangeType.Topic, durable: true);

            // DLX / retry / dead-letter
            _channel.ExchangeDeclare("banking.dlx", ExchangeType.Direct, durable: true);
            _channel.QueueDeclare("dlq.transaction", durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind("dlq.transaction", "banking.dlx", "dlq.transaction");

            var retryArgs = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", exchange },
                { "x-dead-letter-routing-key", "transaction.created" },
                { "x-message-ttl", 5000 }
            };
                    _channel.QueueDeclare("account.retry.transaction.queue", durable: true, exclusive: false, autoDelete: false, arguments: retryArgs);

                    var mainArgs = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", "banking.dlx" },
                { "x-dead-letter-routing-key", "dlq.transaction" }
            };
            _channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false, arguments: mainArgs);
            _channel.QueueBind(QueueName, exchange, "transaction.created");

            _channel.BasicQos(0, 10, false);

            return base.StartAsync(cancellationToken); // StartAsync maintenant sûr que tout est créé
        }



        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var deliveryTag = ea.DeliveryTag;
                var props = ea.BasicProperties;
                var messageId = props.MessageId;
                var body = ea.Body.ToArray();
                string text = Encoding.UTF8.GetString(body);

                try
                {
                    // Idempotence check (DB) - use scoped service
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AccountDbContext>();

                    if (await db.ProcessedMessages.AnyAsync(m => m.MessageId == Guid.Parse(messageId)))
                    {
                        _channel.BasicAck(deliveryTag, false);
                        return;
                    }

                    var dto = JsonSerializer.Deserialize<TransactionDto>(text);

                    // Appel logique métier
                    var accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();
                    var result = await accountService.ProcessTransactionAsync(dto);

                    if (result.Success)
                    {
                        // marquer traité
                        db.ProcessedMessages.Add(new ProcessedMessage { MessageId = Guid.Parse(messageId), Handler = "AccountService.TransactionCreated", ProcessedAt = DateTime.UtcNow });
                        await db.SaveChangesAsync();

                        _channel.BasicAck(deliveryTag, false);
                    }
                    else
                    {
                        // si result indique temporaire => envoyer vers retry queue (publish to delay)
                        // ici on nack pour aller en DLX/retry depending on config
                        _channel.BasicNack(deliveryTag, false, requeue: false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message {MessageId}", messageId);

                    // BasicNack with requeue=false moves to DLX configured on the queue (or you can republish to retry)
                    _channel.BasicNack(deliveryTag, false, requeue: false);
                }
            };

            _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.Close();
            _channel?.Dispose();
            return base.StopAsync(cancellationToken);
        }
    }


}
