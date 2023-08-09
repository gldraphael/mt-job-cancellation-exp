using CommonLib.MessageContracts;
using MassTransit;
using System.Threading.Tasks;

namespace WorkerApp;

public class CancellationConsumer : IConsumer<CancelJobCommand>
{
    public Task Consume(ConsumeContext<CancelJobCommand> context)
    {
        FakeCache.AddCancellationRequest(context.Message.JobId);
        return Task.CompletedTask;
    }
}
