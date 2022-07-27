using QueryObjectFilter.DI.MicrosoftDependencyInjection;

namespace QueryObjectFilter.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddHostedService<Worker>();

                    services.AddQueryObjectFilterWithExpressionConversion();
                    services.AddQueryObjectFilterWithSqlConversion();
                })
                .Build();

            host.Run();
        }
    }
}