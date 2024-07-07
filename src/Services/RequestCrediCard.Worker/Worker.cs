using Marraia.Queue;
using Marraia.Queue.Interfaces;
using ProposalCreditCard.Application.UseCase.RegisterCustomer.Event;
using ProposalCreditCard.Application.UseCase.RequestCreditCard.Interfaces;

namespace RequestCrediCard.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IEventBus _eventBus;
    private readonly Consumer _consumer;
    private readonly IRequestCreditCardUseCase _requestCreditCardUseCase;
    private readonly IConfiguration _configuration;
    private IDisposable _disposable;

    public Worker(ILogger<Worker> logger,
                  IEventBus eventBus,
                  Consumer consumer,
                  IRequestCreditCardUseCase requestCreditCardUseCase,
                  IConfiguration configuration)
    {
        _logger = logger;
        _eventBus = eventBus;
        _consumer = consumer;
        _configuration = configuration;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _disposable = _consumer.Start<ProposalCreditCardEvent>(_configuration.GetSection("RabbitMq:QueueName").Value, async (message) =>
        {
            await ProcessCreditCardRequestAsync(message).ConfigureAwait(false);
        });

        return Task.CompletedTask;
    }

    private async Task ProcessCreditCardRequestAsync(ProposalCreditCardEvent message)
    {
        try
        {
            await _requestCreditCardUseCase.ProcessCreditCardRequestAsync(message).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar solicitação de cartão de crédito.");
        }
    }
}

