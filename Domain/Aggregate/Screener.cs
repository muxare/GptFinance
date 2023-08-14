using System.ComponentModel.DataAnnotations;

namespace GptFinance.Domain.Aggregate
{
    /// <summary>
    /// Should be a query with financial data predicates that results in a list of companies that match the criteria.
    /// </summary>
    public class Screener
    {
        public Guid Id { get; set; }

        public Screener()
        {
            Id = Guid.NewGuid();
        }
    }
}
