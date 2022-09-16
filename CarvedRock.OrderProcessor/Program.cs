using Serilog;
using Serilog.Events;

namespace CarvedRock.OrderProcessor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddHostedService<Worker>();
                })
                .UseSerilog()
                .Build();


            var name = typeof(Program).Assembly.GetName().Name;

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithProperty("Assembly", name)
            .WriteTo.Seq("http://host.docker.internal:5341")
            .WriteTo.Console()
            .CreateLogger();

            try
            {
                Log.Information("Starting host");
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}