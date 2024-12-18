using NLog.Extensions.Logging;
using TechChallenge.ContatoPrimary.WorkerService.Extensions;

namespace TechChallenge.ContatoPrimary.WorkerService
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
                .UseWindowsService(options =>
                {
                    options.ServiceName = "Tech Challenge - Contato Primary";
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.ResolveDependencies(hostingContext);
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddNLog(hostingContext.Configuration.GetSection("Logging"));
                })
               .ConfigureServices((hostContext, services) =>
               {
                   services.ResolveAllDependencies(hostContext);

                   services.AddHostedService<Worker>();
               });
    }
}