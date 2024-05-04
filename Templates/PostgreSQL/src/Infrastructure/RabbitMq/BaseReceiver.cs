using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Infrastructure.RabbitMq;

public abstract class BaseReceiver<T> : IHostedService where T : new()
{
    private readonly IConnection _connection;
    private readonly IModel _model;
    private readonly string _exchange;
    private readonly string _queue;
    private readonly ILogger<BaseReceiver<T>> _logger;

    protected BaseReceiver(string exchange, string queue, IConnectionFactory factory, ILogger<BaseReceiver<T>> logger)
    {
        _exchange = exchange;
        _queue = queue;
        _logger = logger;

        try
        {
            _connection = factory.CreateConnection();

            _model = _connection.CreateModel();
            _model.ExchangeDeclare(_exchange, ExchangeType.Fanout);
            _model.QueueDeclare(_queue, true, false, false, null);
            _model.QueueBind(_queue, _exchange, string.Empty);
        }
        catch (Exception e)
        {
            logger.LogError("Error constructing receiver for queue {queue} and exchange {exchange}. Details: {e}",
                _queue, _exchange, e);
            throw;
        }
    }

    protected abstract Task ProcessAsync(T message);

    public Task StartAsync(CancellationToken token)
    {
        try
        {
            var consumer = new AsyncEventingBasicConsumer(_model);
            consumer.Received += async (_, args) =>
            {
                await ProcessAsync(JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(args.Body.ToArray())));

                _model.BasicAck(args.DeliveryTag, false);
            };

            _model.BasicConsume(_queue, false, consumer);
        }
        catch (Exception e)
        {
            _logger.LogError("Error receiving message from queue {queue} and exchange {exchange}. Details: {e}",
                _queue, _exchange, e);
            throw;
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken token)
    {
        _model.Close();
        _model.Dispose();

        _connection.Close();
        _connection.Dispose();

        return Task.CompletedTask;
    }
}