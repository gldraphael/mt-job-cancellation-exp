using CommonLib.Extensions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using WorkerApp.Infrastructure.Redis;

namespace WorkerApp;

public class Program
{
    public static void Main(string[] args) =>
        CreateHostBuilder(args).Build().Run();

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureLogging((host, lb) => {
                lb.ClearProviders();
                var logger = new LoggerConfiguration()
                                .ReadFrom.Configuration(host.Configuration)
                                .CreateLogger(); ;
                lb.AddSerilog(logger);
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<Worker>();

                services.AddMassTransit(
                    config: hostContext.Configuration,
                    configureDependencies: x => {
                        x.AddConsumer<MyJobConsumer>();
                        x.AddConsumer<CancellationConsumer>();
                    },
                    configureEndpoints: (bc, rc, sp) => {

                        bc.ReceiveEndpoint("mt-job-cancellation-exp", ce =>
                        {
                            ce.ConfigureConsumer<MyJobConsumer>(sp);
                        });

                        bc.ReceiveEndpoint("mt-job-cancellation-requests", ce =>
                        {
                            ce.ConfigureConsumer<CancellationConsumer>(sp);
                        });
                    },
                    configureRabbitMq: (rbc, sp) => {
                        
                    },
                    configureAzureServiceBus: (sbc, sp) => { });

                services.Configure<RedisConfig>(hostContext.Configuration.GetSection("Redis"));
                services.AddSingleton<IRedisConnectionProvider, RedisConnectionProvider>();
                services.AddTransient<IRedisService, RedisService>();
                services.AddTransient<IJobCache, RedisService>();
            });
}
