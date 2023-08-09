using ClientApp;
using CommonLib.Extensions;
using CommonLib.MessageContracts;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;


// setup configuration
var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

// configure serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .CreateLogger();

// setup the DI container
var serviceProvider = new ServiceCollection()
    .AddLogging(lb => lb.AddSerilog(dispose: true))
    .AddMassTransit(
            config: config,
            configureDependencies: x =>
            {
                x.AddConsumer<InitiateCancellationConsumer>();
            },
            configureEndpoints: (bc, rc, sp) => {
                //bc.ReceiveEndpoint("mt-job-started-handler", ce =>
                //{
                //    ce.Consumer<InitiateCancellationConsumer>();
                //});
            },
            configureRabbitMq: (rbc, sp) => { },
            configureAzureServiceBus: (sbc, sp) => { }
        )
    .BuildServiceProvider();
    


var bus = serviceProvider.GetRequiredService<IBusControl>();
await bus.StartAsync();

try
{
    // await bus.Publish(new BeginJobCommand("From Sergey and Galdin!"));

    // await Task.Delay(TimeSpan.FromHours(1));

    await bus.Publish(new CancelJobCommand(JobId: Guid.Parse("aa18b525-ebaa-4173-97c2-fc66f387b98b")));
}
finally
{
    await bus.StopAsync();
}
