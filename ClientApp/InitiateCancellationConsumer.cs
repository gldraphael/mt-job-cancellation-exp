using CommonLib.MessageContracts;
using MassTransit;
using MassTransit.Contracts.JobService;
using System.Threading.Tasks;

namespace ClientApp;

public class InitiateCancellationConsumer : IConsumer<JobStartedEvent>
{
    public async Task Consume(ConsumeContext<JobStartedEvent> context)
    {
        await Task.Delay(17_000);
        await context.Publish(new CancelJobCommand(JobId: context.Message.JobId));
    }
}
