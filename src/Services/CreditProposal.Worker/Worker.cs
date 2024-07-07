using Marraia.Queue;
using ProposalCreditCard.Application.UseCase.ProposalValidate.Interfaces;
using ProposalCreditCard.Application.UseCase.RegisterCustomer.Event;

namespace CreditProposal.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly Consumer _consumer;
    private readonly IProposalValidateUseCase _proposalValidateUseCase;
    private readonly IConfiguration _configuration;
    private IDisposable _disposable;

    public Worker(ILogger<Worker> logger,
                      IConfiguration configuration,
                      IProposalValidateUseCase proposalValidateUseCase,
                      Consumer consumer)
    {
        _logger = logger;
        _configuration = configuration;
        _proposalValidateUseCase = proposalValidateUseCase;
        _consumer = consumer;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _disposable = _consumer.Start<ProposalEvent>(_configuration.GetSection("RabbitMq:QueueName").Value, async (message) =>
        {
            await ProcessProposalAsync(message).ConfigureAwait(false);
        });

        return Task.CompletedTask;
    }

    private async Task ProcessProposalAsync(ProposalEvent message)
    {
        try
        {
            await _proposalValidateUseCase.ProposalValidateAsync(message).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar proposta.");
        }
    }
}

