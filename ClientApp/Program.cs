using CommonLib.Extensions;
using CommonLib.MessageContracts;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.IO;


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
                x.AddRequestClient<GetInfo>();
            },
            configureEndpoints: (bc, rc, sp) => {  },
            configureRabbitMq: (rbc, sp) => { },
            configureAzureServiceBus: (sbc, sp) => { }
        )
    .BuildServiceProvider();
    


var bus = serviceProvider.GetRequiredService<IBusControl>();
await bus.StartAsync();

try
{
    
}
finally
{
    await bus.StopAsync();
}
