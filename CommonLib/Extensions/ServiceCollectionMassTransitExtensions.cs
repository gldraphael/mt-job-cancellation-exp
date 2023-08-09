using CommonLib.Internal;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CommonLib.Extensions;

public static class ServiceCollectionMassTransitExtensions
{
    /// <summary>
    /// Configure MassTransit for the application.
    /// </summary>
    /// <param name="services">The service collection instance</param>
    /// <param name="config">The IConfiguration instance</param>
    /// <param name="configureDependencies">Delegate to configure dependencies (Consumer, RequestClient, etc.)</param>
    /// <param name="configureEndpoints">Delegte to configure MassTransit endpoints</param>
    /// <param name="configureRabbitMq">Delegate to configure RabbitMq. Use this to configure prefetch count, ratelimits, message retries, etc.</param>
    /// <param name="configureAzureServiceBus">Delegate to configure the Azure Service Bus. Use this to configure prefetch count, ratelimits, message retries, etc.</param>
    public static IServiceCollection AddMassTransit(
        this IServiceCollection services,
        IConfiguration config,
        Action<IRegistrationConfigurator> configureDependencies,
        Action<IBusFactoryConfigurator, IReceiveConfigurator, IBusRegistrationContext> configureEndpoints,
        Action<IRabbitMqBusFactoryConfigurator, IServiceProvider> configureRabbitMq,
        Action<IServiceBusBusFactoryConfigurator, IServiceProvider> configureAzureServiceBus)
    {
        services.AddMassTransit(x =>
        {

            configureDependencies(x);

            // Setup transport
            var transport = config.GetMessagingTransport();
            switch (transport)
            {
                case MessagingTransport.RabbitMQ:

                    var rabbitmqOptions = config.GetRabbitMqOptions();
                    if (rabbitmqOptions.HasMissingValues is false) throw new InvalidOperationException("Some rabbitmq options are missing.");
                    x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(sbc =>
                    {
                        sbc.Host(rabbitmqOptions.Host, h =>
                        {
                            h.Username(rabbitmqOptions.Username);
                            h.Password(rabbitmqOptions.Password);
                        });
                        configureRabbitMq(sbc, provider);
                        configureEndpoints(sbc, sbc, provider);
                    }));
                    break;

                case MessagingTransport.AzureServiceBus:
                    x.AddBus(provider => Bus.Factory.CreateUsingAzureServiceBus(sbc =>
                    {
                        sbc.Host(config.GetAzureServiceBusConnectionString(), c => { });
                        configureAzureServiceBus(sbc, provider);
                        configureEndpoints(sbc, sbc, provider);
                    }));
                    break;

                default:
                    throw new NotSupportedException("The specified transport is not supported.");
            }
        });
        return services;
    }
}
