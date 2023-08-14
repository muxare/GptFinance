namespace GptFinance.Domain.Aggregate
{
    public class Predicate
    {
        public FinancialDataAggregate FinancialData { get; set; }

        public Predicate()
        {
            FinancialData = new FinancialDataAggregate();
        }
    }
}
