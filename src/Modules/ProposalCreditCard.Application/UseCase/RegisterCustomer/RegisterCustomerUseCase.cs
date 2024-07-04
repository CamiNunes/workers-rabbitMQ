using Marraia.Queue.Interfaces;
using ProposalCreditCard.Application.UseCase.RegisterCustomer.Event;
using ProposalCreditCard.Application.UseCase.RegisterCustomer.Interfaces;
using ProposalCreditCard.Domain;
using ProposalCreditCard.Domain.Repositories.Interfaces;

namespace ProposalCreditCard.Application.UseCase.RegisterCustomer
{
	public class RegisterCustomerUseCase : IRegisterCustomerUseCase
	{
        private readonly ICustomerRepository _customerRepository;
        private readonly IEventBus _eventBus;

        public RegisterCustomerUseCase(ICustomerRepository customerRepository,
                                        IEventBus eventBus)
		{
            _customerRepository = customerRepository;
            _eventBus = eventBus;
        }

        public async Task ProcessEventCustomerAsync(CustomerEvent eventCustomer)
        {
            if (eventCustomer.Decision == "REGISTER")
            {
                await RegisterCustomerAsync(eventCustomer);
                return;
            }

            await SimulationProposalAsync(eventCustomer)
                    .ConfigureAwait(false);
        }

        private async Task RegisterCustomerAsync(CustomerEvent eventCustomer)
        {
            var customer = new Customer()
            {
                Name = eventCustomer.Name,
                Salary = eventCustomer.Salary
            };

            //TODO: Pode validar se os dados vieram com valores...

            await _customerRepository
                     .InsertAsync(customer)
                     .ConfigureAwait(false);

            var eventProposal = new ProposalEvent()
            {
                CustomerId = customer.Id,
                Name = customer.Name,
                Salary = customer.Salary,
                CreditValue = eventCustomer.CreditValue
            };

            await _eventBus
                    .PublishAsync(eventProposal, "messagebus.proposal.eventhandler")
                    .ConfigureAwait(false);
        }

        private async Task SimulationProposalAsync(CustomerEvent eventCustomer)
        {
            var customer = await _customerRepository
                                    .GetByIdAsync(eventCustomer.Simulation.CustomerId);

            customer.AddHistoryProposal(eventCustomer.Simulation.Message);

            if (eventCustomer.Simulation.Status == "OK")
                customer.AddCreditCard(eventCustomer.Simulation.CardNumber, eventCustomer.CreditValue);

            await _customerRepository
                    .UpdateAsync(customer);
        }
    }
}

