using gRPC.Server.Protos;
using Grpc.Core;
using System.Text;

namespace gRPC.Server.Services
{
    public class TelemetryTrackingService : TrackingService.TrackingServiceBase
    {
        private readonly ILogger<TelemetryTrackingService> _logger;

        public TelemetryTrackingService(ILogger<TelemetryTrackingService> logger)
        {
            _logger = logger;
        }

        public override Task<TrackingResponse> SendMessage(TrackingMessage request, ServerCallContext context)
        {
            var logs = new StringBuilder();
            logs.Append("Recieve Input:-\n");
            logs.Append($"\t- DeviceId:({request.DeviceId})\n");
            logs.Append($"\t- Speed:({request.Speed})\n");
            logs.Append($"\t- Location:({request.Location.Lat},{request.Location.Long})\n");
            logs.Append("\t- Sensors:-\n");
            foreach (var sensor in request.Sensors)
            {
                logs.Append($"\t\t- Sensor:({sensor.Key},{sensor.Value})\n");
            }

            _logger.LogInformation(logs.ToString());

            return Task.FromResult(new TrackingResponse() { Success = true });
        }
    }
}