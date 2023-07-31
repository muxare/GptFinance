using GptFinance.Domain.Entity;

namespace GptFinance.Domain.Aggregate
{
    public class FinancialData
    {
        public Guid Id { get; set; }
        public ICollection<Eod> EodData { get; set; } = new List<Eod>();
        public ICollection<Ema> EmaData { get; set; } = new List<Ema>();
        public ICollection<Macd> MacdData { get; set; } = new List<Macd>();
    }
}
