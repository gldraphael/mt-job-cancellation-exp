using CommonLib.MessageContracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WorkerApp;

public abstract class ImageCapJobConsumer<T> : IConsumer<T> where T : class
{
    public async Task Consume(ConsumeContext<T> context)
    {
        var jobId = Guid.NewGuid();

        using var cts = new CancellationTokenSource();
        var completedTask = await Task.WhenAny(
                DoJob(context, jobId, cts.Token),
                CheckCancelation(jobId, cts.Token)
            );
        cts.Cancel();
    }

    public abstract Task DoJob(ConsumeContext<T> context, Guid jobId, CancellationToken token);

    private async Task CheckCancelation(Guid jobId, CancellationToken token)
    {
        while (true)
        {
            if (token.IsCancellationRequested) return;

            await Task.Delay(TimeSpan.FromSeconds(1));
            if (FakeCache.CheckForCancellation(jobId))
            {
                break;
            }
        }
    }
}

public class MyJobConsumer : ImageCapJobConsumer<BeginJobCommand>
{
    private readonly ILogger<MyJobConsumer> logger;

    public MyJobConsumer(ILogger<MyJobConsumer> logger)
    {
        this.logger = logger;
    }

    public override async Task DoJob(ConsumeContext<BeginJobCommand> context, Guid jobId, CancellationToken token)
    {
        await context.Publish(new JobStartedEvent(Name: context.Message.Name, JobId: jobId));

        for (int i = 0; i < 10000000; i++) //1
        {
            if (token.IsCancellationRequested) return;

            await Task.Delay(TimeSpan.FromSeconds(5));
            logger.LogInformation("Running job {@jobId}", jobId);
            await context.Publish(new JobRunningEvent(Name: context.Message.Name, JobId: jobId));
        }
    }
}