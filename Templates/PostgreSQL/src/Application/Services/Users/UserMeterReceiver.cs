using Application.Observability;
using Infrastructure.RabbitMq;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Application.Services.Users;

public class UserMeterReceiver(IConnectionFactory factory, ILogger<UserMeterReceiver> logger)
    : BaseReceiver<int>(Exchange, Queue, factory, logger)
{
    private const string Exchange = nameof(ExchangeEnum.UserEntityRequestsExchange);
    private const string Queue = nameof(QueueEnum.UserEntityRequestsQueue);

    protected override Task ProcessAsync(int message)
    {
        using (Tracer.Instance.StartActivity())
            Meter.UserEntityRequests.Add(message);

        return Task.CompletedTask;
    }
}