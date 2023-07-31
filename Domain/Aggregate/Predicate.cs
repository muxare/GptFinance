namespace GptFinance.Domain.Aggregate
{
    public class Predicate
    {
        public FinancialData FinancialData { get; set; }

        public Predicate()
        {
            FinancialData = new FinancialData();
        }
    }
}
