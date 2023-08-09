using CommonLib;
using CommonLib.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit;
using Serilog;
using Microsoft.Extensions.Logging;

namespace WorkerApp
{
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
                    services.AddTransient<GetInfoService>();

                    services.AddMassTransit(
                        config: hostContext.Configuration,
                        configureDependencies: x => {
                            x.AddConsumer<GetInfoConsumer>();
                        },
                        configureEndpoints: (bc, rc, sp) => {
                            bc.ReceiveEndpoint("mt-req-resp-queries", ce =>
                            {
                                ce.ConfigureConsumer<GetInfoConsumer>(sp);
                            });
                        },
                        configureRabbitMq: (rbc, sp) => { },
                        configureAzureServiceBus: (sbc, sp) => { });
                });
    }
}
