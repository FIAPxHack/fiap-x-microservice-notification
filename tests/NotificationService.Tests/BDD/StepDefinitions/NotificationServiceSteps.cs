using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NotificationService.Application.DTOs;
using NotificationService.Application.UseCases;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Interfaces.Repositories;
using NotificationService.Domain.Interfaces.Services;
using TechTalk.SpecFlow;

namespace NotificationService.Tests.BDD.StepDefinitions;

[Binding]
public class NotificationServiceSteps
{
    private readonly Mock<INotificationRepository> _mockRepository;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<ILogger<SendNotificationUseCase>> _mockSendLogger;
    private readonly Mock<ILogger<GetUserNotificationsUseCase>> _mockGetLogger;
    private SendNotificationUseCase _sendUseCase;
    private GetUserNotificationsUseCase _getUserNotificationsUseCase;

    private string? _userId;
    private string? _email;
    private string? _message;
    private NotificationType _notificationType;
    private NotificationResponseDto? _sendResult;
    private IEnumerable<NotificationHistoryDto>? _getResult;
    private bool _emailServiceAvailable = true;

    public NotificationServiceSteps()
    {
        _mockRepository = new Mock<INotificationRepository>();
        _mockEmailService = new Mock<IEmailService>();
        _mockSendLogger = new Mock<ILogger<SendNotificationUseCase>>();
        _mockGetLogger = new Mock<ILogger<GetUserNotificationsUseCase>>();

        SetupMocks();
        CreateUseCases();
    }

    private void SetupMocks()
    {
        _mockEmailService.Setup(x => x.SendEmailAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>()))
            .ReturnsAsync(() => _emailServiceAvailable);

        _mockRepository.Setup(x => x.AddAsync(It.IsAny<NotificationHistory>()))
            .Returns(Task.CompletedTask);

        _mockRepository.Setup(x => x.GetByUserIdAsync(It.IsAny<string>()))
            .ReturnsAsync((string userId) =>
            {
                return new List<NotificationHistory>();
            });
    }

    private void CreateUseCases()
    {
        _sendUseCase = new SendNotificationUseCase(
            _mockRepository.Object,
            _mockEmailService.Object,
            _mockSendLogger.Object);

        _getUserNotificationsUseCase = new GetUserNotificationsUseCase(
            _mockRepository.Object,
            _mockGetLogger.Object);
    }

    [Given(@"que tenho um usuário com ID ""(.*)"" e email ""(.*)""")]
    public void DadoQueTenhoUmUsuarioComIDEEmail(string userId, string email)
    {
        _userId = userId;
        _email = email;
        _notificationType = NotificationType.General;
    }

    [Given(@"que tenho um usuário sem notificações com ID ""(.*)""")]
    public void DadoQueTenhoUmUsuarioSemNotificacoesComID(string userId)
    {
        _userId = userId;
        _mockRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(new List<NotificationHistory>());
    }

    [Given(@"que foram enviadas (.*) notificações para este usuário")]
    public async Task DadoQueForamEnviadasNotificacoesParaEsteUsuario(int quantidade)
    {
        var notifications = new List<NotificationHistory>();
        for (int i = 0; i < quantidade; i++)
        {
            var notification = new NotificationHistory(
                _userId!,
                _email!,
                $"Message {i + 1}",
                NotificationType.General);
            notification.MarkAsSent();
            notifications.Add(notification);
        }

        _mockRepository.Setup(x => x.GetByUserIdAsync(_userId!))
            .ReturnsAsync(notifications);
    }

    [Given(@"que foram enviadas notificações com diferentes status")]
    public void DadoQueForamEnviadasNotificacoesComDiferentesStatus()
    {
        var notifications = new List<NotificationHistory>
        {
            new NotificationHistory(_userId!, _email!, "Sent message", NotificationType.General),
            new NotificationHistory(_userId!, _email!, "Failed message", NotificationType.General),
            new NotificationHistory(_userId!, _email!, "Pending message", NotificationType.General)
        };

        notifications[0].MarkAsSent();
        notifications[1].MarkAsFailed();
        // notifications[2] fica Pending

        _mockRepository.Setup(x => x.GetByUserIdAsync(_userId!))
            .ReturnsAsync(notifications);
    }

    [Given(@"que o serviço de email está indisponível")]
    public void DadoQueOServicoDeEmailEstaIndisponivel()
    {
        _emailServiceAvailable = false;
    }

    [When(@"eu envio uma notificação com mensagem ""(.*)""")]
    public async Task QuandoEuEnvioUmaNotificacaoComMensagem(string mensagem)
    {
        _message = mensagem;
        var request = new NotificationRequestDto
        {
            UserId = _userId!,
            Email = _email!,
            Message = mensagem,
            Type = _notificationType
        };

        _sendResult = await _sendUseCase.ExecuteAsync(request);
    }

    [When(@"eu envio uma notificação do tipo ""(.*)""")]
    public async Task QuandoEuEnvioUmaNotificacaoDoTipo(string tipo)
    {
        _notificationType = Enum.Parse<NotificationType>(tipo);
        var request = new NotificationRequestDto
        {
            UserId = _userId!,
            Email = _email!,
            Message = $"Notification of type {tipo}",
            Type = _notificationType
        };

        _sendResult = await _sendUseCase.ExecuteAsync(request);
    }

    [When(@"eu tento enviar uma notificação")]
    public async Task QuandoEuTentoEnviarUmaNotificacao()
    {
        var request = new NotificationRequestDto
        {
            UserId = _userId!,
            Email = _email!,
            Message = "Test message",
            Type = NotificationType.General
        };

        _sendResult = await _sendUseCase.ExecuteAsync(request);
    }

    [When(@"eu envio uma notificação com mensagem de (.*) caracteres")]
    public async Task QuandoEuEnvioUmaNotificacaoComMensagemDeCaracteres(int tamanho)
    {
        _message = new string('A', tamanho);
        var request = new NotificationRequestDto
        {
            UserId = _userId!,
            Email = _email!,
            Message = _message,
            Type = NotificationType.General
        };

        _sendResult = await _sendUseCase.ExecuteAsync(request);
    }

    [When(@"eu busco o histórico de notificações")]
    public async Task QuandoEuBuscoOHistoricoDeNotificacoes()
    {
        _getResult = await _getUserNotificationsUseCase.ExecuteAsync(_userId!);
    }

    [Then(@"a notificação deve ser enviada com sucesso")]
    public void EntaoANotificacaoDeveSerEnviadaComSucesso()
    {
        _sendResult.Should().NotBeNull();
        _sendResult!.Success.Should().BeTrue();
    }

    [Then(@"a notificação não deve ser enviada")]
    public void EntaoANotificacaoNaoDeveSerEnviada()
    {
        _sendResult.Should().NotBeNull();
        _sendResult!.Success.Should().BeFalse();
    }

    [Then(@"o status deve ser ""(.*)""")]
    public void EntaoOStatusDeveSer(string status)
    {
        var expectedStatus = Enum.Parse<NotificationStatus>(status);
        
        _mockRepository.Verify(x => x.AddAsync(
            It.Is<NotificationHistory>(n => n.Status == expectedStatus)), Times.Once);
    }

    [Then(@"o assunto do email deve conter ""(.*)""")]
    public void EntaoOAssuntoDoEmailDeveConter(string texto)
    {
        _mockEmailService.Verify(x => x.SendEmailAsync(
            It.IsAny<string>(),
            It.Is<string>(s => s.Contains(texto)),
            It.IsAny<string>()), Times.Once);
    }

    [Then(@"o tipo deve ser ""(.*)""")]
    public void EntaoOTipoDeveSer(string tipo)
    {
        var expectedType = Enum.Parse<NotificationType>(tipo);
        
        _mockRepository.Verify(x => x.AddAsync(
            It.Is<NotificationHistory>(n => n.Type == expectedType)), Times.Once);
    }

    [Then(@"deve retornar uma lista vazia")]
    public void EntaoDeveRetornarUmaListaVazia()
    {
        _getResult.Should().NotBeNull();
        _getResult.Should().BeEmpty();
    }

    [Then(@"deve retornar (.*) notificações")]
    public void EntaoDeveRetornarNotificacoes(int quantidade)
    {
        _getResult.Should().NotBeNull();
        _getResult.Should().HaveCount(quantidade);
    }

    [Then(@"todas devem pertencer ao usuário ""(.*)""")]
    public void EntaoTodasDevemPertencerAoUsuario(string userId)
    {
        _getResult.Should().NotBeNull();
        _getResult.Should().AllSatisfy(n => n.UserId.Should().Be(userId));
    }

    [Then(@"a mensagem deve estar completa")]
    public void EntaoAMensagemDeveEstarCompleta()
    {
        _mockRepository.Verify(x => x.AddAsync(
            It.Is<NotificationHistory>(n => n.Message == _message)), Times.Once);
    }

    [Then(@"deve retornar notificações com status ""(.*)"", ""(.*)"" e ""(.*)""")]
    public void EntaoDeveRetornarNotificacoesComStatus(string status1, string status2, string status3)
    {
        _getResult.Should().NotBeNull();
        _getResult.Should().Contain(n => n.Status == Enum.Parse<NotificationStatus>(status1));
        _getResult.Should().Contain(n => n.Status == Enum.Parse<NotificationStatus>(status2));
        _getResult.Should().Contain(n => n.Status == Enum.Parse<NotificationStatus>(status3));
    }
}
