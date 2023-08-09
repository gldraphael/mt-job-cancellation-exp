using CommonLib.MessageContracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WorkerApp;

public class MyJobConsumer : IConsumer<BeginJobCommand>
{
    private int IsRunning = 1;
    private readonly ILogger<MyJobConsumer> logger;

    public MyJobConsumer(ILogger<MyJobConsumer> logger)
    {
        this.logger = logger;
    }

    public async Task Consume(ConsumeContext<BeginJobCommand> context)
    {
        var jobId = Guid.NewGuid();

        using var cts = new CancellationTokenSource();
        var completedTask = await Task.WhenAny(
                DoJob(context, jobId, cts.Token),
                CheckCancelation(jobId, cts.Token)
            );
        cts.Cancel();
    }

    private async Task DoJob(ConsumeContext<BeginJobCommand> context, Guid jobId, CancellationToken token)
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