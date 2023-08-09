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
        try
        {
            await Task.WhenAny(
                DoJob(context, jobId),
                CheckCancelation(context, jobId)
            ).ConfigureAwait(false);
        }
        catch (JobCancelledException)
        {
            logger.LogInformation("Canceled job {@jobId}", jobId);
            await context.Publish(new JobCanceledEvent(jobId, context.Message.Name));
        }
    }

    private async Task DoJob(ConsumeContext<BeginJobCommand> context, Guid jobId)
    {
        
        await context.Publish(new JobStartedEvent(Name: context.Message.Name, JobId: jobId));

        for(int i = 0; i < 10000000; i++)
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
            logger.LogInformation("Running job {@jobId}", jobId);
            await context.Publish(new JobRunningEvent(Name: context.Message.Name, JobId: jobId));
        }

        IsRunning = Interlocked.Decrement(ref IsRunning);
    }

    private async Task CheckCancelation(ConsumeContext<BeginJobCommand> context, Guid jobId)
    {
        while (IsRunning > 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            if (FakeCache.CheckForCancellation(jobId))
            {
                throw new JobCancelledException();
            }
        }
    }
}


[Serializable]
public class JobCancelledException : Exception
{
    public JobCancelledException() { }
    public JobCancelledException(string message) : base(message) { }
    public JobCancelledException(string message, Exception inner) : base(message, inner) { }
    protected JobCancelledException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
