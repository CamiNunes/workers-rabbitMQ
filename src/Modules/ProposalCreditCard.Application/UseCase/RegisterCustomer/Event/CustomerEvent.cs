namespace ProposalCreditCard.Application.UseCase.RegisterCustomer.Event
{
	public class CustomerEvent
	{
		public string Name { get; set; }
		public decimal Salary { get; set; }
        public decimal CreditValue { get; set; }
		public string Decision { get; set; }
		public ProposalSimulationEvent Simulation { get; set; }
	}

	public class ProposalSimulationEvent
	{
		public Guid CustomerId { get; set; }
		public string Status { get; set; }
		public string Message { get; set; }
		public string CardNumber { get; set; }
	}

    public class ProposalEvent
    {
		public Guid CustomerId { get; set; }
		public string Name { get; set; }
        public decimal Salary { get; set; }
		public decimal CreditValue { get; set; }
	}
}

