using TechChallenge.ContatoPrimary.Application.Resources;
using TechChallenge.ContatoPrimary.Domain.Interfaces.Services;

namespace TechChallenge.ContatoPrimary.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IProcessarContatoPrimaryService _processarContatoPrimaryService;

        public Worker(ILogger<Worker> logger, IProcessarContatoPrimaryService processarContatoPrimaryService)
        {
            _logger = logger;
            _processarContatoPrimaryService = processarContatoPrimaryService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            List<Task> tasks = [Task.Run(() => _processarContatoPrimaryService.ProcessarMensagemRecebida(), stoppingToken)];

            return Task.WhenAny(tasks.ToArray());
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(Message.INICIALIZANDO_SERVICO, nameof(Worker));
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(Message.PARANDO_SERVICO, nameof(Worker));
            return Task.CompletedTask;
        }
    }
}


