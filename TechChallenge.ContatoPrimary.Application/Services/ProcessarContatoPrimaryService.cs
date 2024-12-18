using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMq.Nuget;
using System.Net.Http.Json;
using System.Text.Json;
using TechChallenge.ContatoPrimary.Application.Resources;
using TechChallenge.ContatoPrimary.Domain.Entities;
using TechChallenge.ContatoPrimary.Domain.Interfaces.Services;

namespace TechChallenge.ContatoPrimary.Application.Services
{

    public class ProcessarContatoPrimaryService : IProcessarContatoPrimaryService
    {
        private readonly ILogger<ProcessarContatoPrimaryService> _logger;
        private readonly IRabbitMessageQueue _messageQueue;
        private readonly HttpClient _httpClient;
        private readonly string _resourceContato;

        public ProcessarContatoPrimaryService(ILogger<ProcessarContatoPrimaryService> logger, 
            IRabbitMessageQueue messageQueue, HttpClient httpClient, IOptions<ApiSettings> settings)
        {
            _logger = logger;
            _messageQueue = messageQueue;
            _httpClient = httpClient;
            _resourceContato = settings.Value.ResourceContato;
        }

        public void ProcessarMensagemRecebida()
        {
            _messageQueue.SubscribeWithConfirmation(ProcessarMensagem);
        }
        public bool ProcessarMensagem(string mensagem)
        {
            try
            {
                var contatoCommand = JsonSerializer.Deserialize<ContatoCommand>(mensagem);
                HttpResponseMessage response;

                switch (contatoCommand?.Evento)
                {
                    case "insert":
                        response = _httpClient.PostAsJsonAsync($"{_resourceContato}cadastrar", contatoCommand).Result;
                        response.EnsureSuccessStatusCode();
                        break;
                    case "update":
                        response = _httpClient.PutAsJsonAsync($"{_resourceContato}atualizar/", contatoCommand).Result;
                        response.EnsureSuccessStatusCode();
                        break;
                    case "delete":
                        response = _httpClient.DeleteAsync($"{_resourceContato}remover/{contatoCommand.Id}").Result;
                        break;
                    default:
                        _logger.LogError(string.Format(Message.EVENTO_NAO_MAPEADO, contatoCommand?.Evento, mensagem), Message.ERRO_PROCESSAR_SOLICITACAO_BAIXA_ERRO);
                        return true;
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                Task.Delay(5000);
                return false;
            }

        }
    }
}
