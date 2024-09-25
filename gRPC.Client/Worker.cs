using Google.Protobuf.WellKnownTypes;
using gRPC.Server.Protos;
using Grpc.Net.Client;
using System.Formats.Asn1;
using System.Security.Cryptography;

namespace gRPC.Client
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly TrackingMessage _request;
        private TrackingService.TrackingServiceClient? _client;

        private TrackingService.TrackingServiceClient Client
        {
            get
            {
                if (_client == null)
                {
                    // Create gRPC Server Channel
                    var address = "https://localhost:7241";
                    var channel = GrpcChannel.ForAddress(address);

                    // Create Client
                    _client = new TrackingService.TrackingServiceClient(channel);
                }

                return _client;
            }
        }

        public Worker(ILogger<Worker> logger, TrackingMessage request)
        {
            _logger = logger;
            _request = request;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var stream = Client.KeepAlive();

            var keepAliveAsync = Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await stream.RequestStream.WriteAsync(new PulseMessage()
                    {
                        Status = ClientStatus.Working,
                        Details = "Working...................",
                        Stamp = Timestamp.FromDateTime(DateTime.UtcNow)
                    });

                    await Task.Delay(1000, stoppingToken);
                }
            });

            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    await Task.Delay(2000, stoppingToken);
                }
                await SendMessageAsync(stoppingToken);
            }
        }

        private async Task SendMessageAsync(CancellationToken token)
        {
            // Send Message to gRPC Server
            _logger.LogInformation("Sending...");
            var response = await Client.SendMessageAsync(_request);

            await Task.Delay(1000, token);

            // Recieve Response
            _logger.LogInformation($"Recieve {response.Success}");
        }
    }
}