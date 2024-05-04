using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Infrastructure.RabbitMq;

public class BasePublisher(IConnectionFactory factory, ILogger<BasePublisher> logger)
{
    public void Publish(string exchange, object message)
    {
        try
        {
            using var model = factory.CreateConnection().CreateModel();
            model.ExchangeDeclare(exchange, ExchangeType.Fanout);

            var properties = model.CreateBasicProperties();
            properties.Persistent = true;

            model.BasicPublish(
                exchange, string.Empty, properties, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)));
        }
        catch (Exception e)
        {
            logger.LogError("Error publishing message {message} to exchange {exchange}. Details: {e}",
                message, exchange, e);
            throw;
        }
    }
}