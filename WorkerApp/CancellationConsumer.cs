using CommonLib.MessageContracts;
using MassTransit;
using System.Threading.Tasks;

namespace WorkerApp;

public class CancellationConsumer : IConsumer<CancelJobCommand>
{
    private readonly IJobCache cache;

    public CancellationConsumer(IJobCache cache) =>
        this.cache = cache;

    public async Task Consume(ConsumeContext<CancelJobCommand> context)
    {
        await cache.AddCancellationRequest(context.Message.JobId);
    }
}
