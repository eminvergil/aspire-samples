using RedisPubSub.ServiceDefaults;
using StackExchange.Redis;

namespace Worker;

public class Worker(ILogger<Worker> logger, IConnectionMultiplexer connectionMultiplexer) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await connectionMultiplexer.GetSubscriber().SubscribeAsync(Constants.DefaultChannel, (channel, message) =>
            {
                logger.LogWarning("Here is the message: {Message} from channel: {ChannelName}", message, Constants.DefaultChannel);
            });

            await Task.Delay(5000, stoppingToken);
        }
    }
}
