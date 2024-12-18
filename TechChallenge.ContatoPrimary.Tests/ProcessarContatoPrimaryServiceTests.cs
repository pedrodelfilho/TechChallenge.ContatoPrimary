using Moq;
using System.Net.Http;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TechChallenge.ContatoPrimary.Application.Services;
using TechChallenge.ContatoPrimary.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;
using System;
using Moq.Protected;
using RabbitMq.Nuget;
using TechChallenge.ContatoPrimary.Domain.Entities;

namespace TechChallenge.ContatoPrimary.Tests
{
    public class ProcessarContatoPrimaryServiceTests
    {
        private readonly Mock<ILogger<ProcessarContatoPrimaryService>> _mockLogger;
        private readonly Mock<IRabbitMessageQueue> _mockMessageQueue;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly Mock<IOptions<ApiSettings>> _mockApiSettings;
        private readonly ProcessarContatoPrimaryService _service;

        public ProcessarContatoPrimaryServiceTests()
        {
            _mockLogger = new Mock<ILogger<ProcessarContatoPrimaryService>>();
            _mockMessageQueue = new Mock<IRabbitMessageQueue>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _mockApiSettings = new Mock<IOptions<ApiSettings>>();

            _mockApiSettings.Setup(x => x.Value).Returns(new ApiSettings { ResourceContato = "http://example.com/api/" });

            // Criando HttpClient com o Mock de HttpMessageHandler
            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);

            _service = new ProcessarContatoPrimaryService(_mockLogger.Object, _mockMessageQueue.Object, httpClient, _mockApiSettings.Object);
        }

        [Fact]
        public void ProcessarMensagemRecebida_InsereMensagemComSucesso()
        {
            // Arrange
            var contatoCommand = new ContatoCommand
            {
                Evento = "insert",
                Id = 1,
                Nome = "Teste"
            };

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Success")
            };

            // Setup do mock para interceptar o envio de requisição HTTP
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponseMessage);

            var mensagem = JsonSerializer.Serialize(contatoCommand);
            _mockMessageQueue.Setup(x => x.SubscribeWithConfirmation(It.IsAny<Func<string, bool>>()))
                .Callback<Func<string, bool>>(callback => callback(mensagem));

            // Act
            _service.ProcessarMensagemRecebida();

            // Assert
            _mockMessageQueue.Verify(x => x.SubscribeWithConfirmation(It.IsAny<Func<string, bool>>()), Times.Once);
        }

        [Fact]
        public void ProcessarMensagemRecebida_AtualizaMensagemComSucesso()
        {
            // Arrange
            var contatoCommand = new ContatoCommand
            {
                Evento = "update",
                Id = 1,
                Nome = "Teste Atualizado"
            };

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Success")
            };

            // Setup do mock para interceptar o envio de requisição HTTP
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponseMessage);

            var mensagem = JsonSerializer.Serialize(contatoCommand);
            _mockMessageQueue.Setup(x => x.SubscribeWithConfirmation(It.IsAny<Func<string, bool>>()))
                .Callback<Func<string, bool>>(callback => callback(mensagem));

            // Act
            _service.ProcessarMensagemRecebida();

            // Assert
            _mockMessageQueue.Verify(x => x.SubscribeWithConfirmation(It.IsAny<Func<string, bool>>()), Times.Once);
        }

        [Fact]
        public void ProcessarMensagemRecebida_DeletaMensagemComSucesso()
        {
            // Arrange
            var contatoCommand = new ContatoCommand
            {
                Evento = "delete",
                Id = 1,
                Nome = "Teste"
            };

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Success")
            };

            // Setup do mock para interceptar o envio de requisição HTTP
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponseMessage);

            var mensagem = JsonSerializer.Serialize(contatoCommand);
            _mockMessageQueue.Setup(x => x.SubscribeWithConfirmation(It.IsAny<Func<string, bool>>()))
                .Callback<Func<string, bool>>(callback => callback(mensagem));

            // Act
            _service.ProcessarMensagemRecebida();

            // Assert
            _mockMessageQueue.Verify(x => x.SubscribeWithConfirmation(It.IsAny<Func<string, bool>>()), Times.Once);
        }
    }
}
