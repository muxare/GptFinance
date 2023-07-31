namespace GptFinance.Domain.Aggregate
{
    /// <summary>
    /// A BackTest is the testing of a strategy on historical financial data to see how it would have performed.
    /// </summary>
    public class BackTest
    {
        public Guid Id { get; set; }
    }
}
