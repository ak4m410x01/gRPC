using gRPC.Server.Protos;
using Grpc.Net.Client;

namespace gRPC.Client
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly TrackingMessage _request;

        public Worker(ILogger<Worker> logger, TrackingMessage request)
        {
            _logger = logger;
            _request = request;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    await Task.Delay(2000, stoppingToken);
                }

                // Create gRPC Server Channel
                var address = "https://localhost:7241";
                var channel = GrpcChannel.ForAddress(address);

                // Create Client
                var client = new TrackingService.TrackingServiceClient(channel);

                // Send Message to gRPC Server
                _logger.LogInformation("Sending...");
                var response = await client.SendMessageAsync(_request);
                await Task.Delay(1000, stoppingToken);

                // Recieve Response
                _logger.LogInformation($"Recieve {response.Success}");
            }
        }
    }
}