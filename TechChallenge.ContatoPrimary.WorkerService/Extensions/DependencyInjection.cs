using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RabbitMq.Nuget;
using TechChallenge.ContatoPrimary.Application.Services;
using TechChallenge.ContatoPrimary.Domain.Entities;
using TechChallenge.ContatoPrimary.Domain.Interfaces.Services;

namespace TechChallenge.ContatoPrimary.WorkerService.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection ResolveAllDependencies(this IServiceCollection services, HostBuilderContext hostContext)
        {
            // Registrar o HttpClient
            services.AddHttpClient(); 

            // Registrar o ProcessarContatoPrimaryService
            services.Configure<ApiSettings>(hostContext.Configuration.GetSection("ApiBackEnd"));
            services.AddScoped<IProcessarContatoPrimaryService, ProcessarContatoPrimaryService>((sp) =>
            {
                var settings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;
                var httpClient = sp.GetRequiredService<HttpClient>();
                httpClient.BaseAddress = new Uri(ApiSettings.Url);
                httpClient.DefaultRequestHeaders.Add("Resource", settings.ResourceContato);

                return new ProcessarContatoPrimaryService(
                    sp.GetRequiredService<ILogger<ProcessarContatoPrimaryService>>(),
                    sp.GetRequiredService<IRabbitMessageQueue>(),
                    httpClient,
                    sp.GetRequiredService<IOptions<ApiSettings>>()
                );
            });

            // Configuração do RabbitMQ
            services.Configure<RabbitConfig>(hostContext.Configuration.GetSection("RabbitConfig"));
            services.AddTransient<IRabbitMessageQueue>(instance => {
                var rabbitConfig = hostContext.Configuration.GetSection("RabbitConfig").Get<RabbitConfig>();
                return new RabbitMessageQueue(RabbitConfig.Servidor, rabbitConfig?.VHost, RabbitConfig.Usuario, RabbitConfig.Senha, string.Empty, false, rabbitConfig?.FilaRabbit);
            });

            return services;
        }


        public static IConfigurationBuilder ResolveDependencies(this IConfigurationBuilder config, HostBuilderContext hostingContext)
        {
            IHostEnvironment env = hostingContext.HostingEnvironment;
            config.SetBasePath(env.ContentRootPath);
            config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);
            config.AddEnvironmentVariables();

            return config;
        }
    }
}