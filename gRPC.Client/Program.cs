using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using gRPC.Server.Protos;

namespace gRPC.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Generate Message...");
            Task.Delay(2000);

            var request = new TrackingMessage()
            {
                DeviceId = Random.Shared.Next(1, int.MaxValue),
                Speed = Random.Shared.Next(120, int.MaxValue),
                Location = new Location()
                {
                    Lat = Random.Shared.NextDouble(),
                    Long = Random.Shared.NextDouble()
                },
                Stamp = Timestamp.FromDateTime(DateTime.UtcNow)
            };

            request.Sensors.AddRange(new List<Sensor>() {
                new Sensor(){
                    Key = "Sensor-01",
                    Value = Random.Shared.Next(1, int.MaxValue)
                },
                new Sensor(){
                    Key = "Sensor-02",
                    Value = Random.Shared.Next(1, int.MaxValue)
                },
                new Sensor(){
                    Key = "Sensor-03",
                    Value = Random.Shared.Next(1, int.MaxValue)
                }
             });

            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService(provider =>
            {
                var logger = provider.GetService<ILogger<Worker>>();
                if (logger == null)
                {
                    throw new NullReferenceException("No ILogger Services!");
                }

                return new Worker(logger, request);
            });

            var host = builder.Build();
            host.Run();
        }
    }
}