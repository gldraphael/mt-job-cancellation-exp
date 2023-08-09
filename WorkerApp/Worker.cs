using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WorkerApp
{
    public class Worker : IHostedService
    {
        private readonly ILogger<Worker> logger;
        private readonly IBusControl busControl;

        public Worker(ILogger<Worker> logger, IBusControl busControl)
        {
            this.logger = logger;
            this.busControl = busControl;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting the bus...");
            await busControl.StartAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await busControl.StopAsync();
            logger.LogInformation("Stopped the bus.");
        }
    }
}
