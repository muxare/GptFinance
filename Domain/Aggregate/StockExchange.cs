using GptFinance.Domain.Entity;

namespace GptFinance.Domain.Aggregate
{
    public class StockExchangeAggregate
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Marketplace { get; set; }

        public int Ranking { get; set; }

        public string TimeZone { get; set; }

        public TradingHours TradingHours { get; set; }
        public LunchBreak LunchBreak { get; set; }
    }
}
